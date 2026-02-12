using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharpDocx;
using SkiaSharp;
using WADNR.API.ReportTemplates.Models;
using WADNR.API.Services;
using WADNR.EFModels.Entities;

namespace WADNR.API.ReportTemplates
{
    public class ReportTemplateGenerator
    {
        public const string TemplateTempDirectoryName = "WADNRReportTemplates";
        public const string TemplateTempImageDirectoryName = "Images";
        public const int TemplateTempDirectoryFileLifespanInDays = 2;
        protected ReportTemplate ReportTemplate { get; set; }
        protected ReportTemplateModelEnum ReportTemplateModelEnum { get; set; }
        protected ReportTemplateModelTypeEnum ReportTemplateModelTypeEnum { get; set; }
        protected List<int> SelectedModelIDs { get; set; }
        protected string FullTemplateTempDirectory { get; set; }
        protected string FullTemplateTempImageDirectory { get; set; }

        protected Guid ReportTemplateUniqueIdentifier { get; set; }

        public ReportTemplateGenerator(ReportTemplate reportTemplate, List<int> selectedModelIDs)
        {
            ReportTemplate = reportTemplate;
            ReportTemplateModelEnum = (ReportTemplateModelEnum)reportTemplate.ReportTemplateModelID;
            ReportTemplateModelTypeEnum = (ReportTemplateModelTypeEnum)reportTemplate.ReportTemplateModelTypeID;
            SelectedModelIDs = selectedModelIDs;
            ReportTemplateUniqueIdentifier = Guid.NewGuid();
            InitializeTempFolders(new DirectoryInfo(Path.GetTempPath()));
        }

        private void InitializeTempFolders(DirectoryInfo directoryInfo)
        {
            var baseTempDirectory = new DirectoryInfo(Path.Combine(directoryInfo.FullName, TemplateTempDirectoryName));
            baseTempDirectory.Create();
            FullTemplateTempDirectory = baseTempDirectory.FullName;
            FullTemplateTempImageDirectory = baseTempDirectory.CreateSubdirectory(TemplateTempImageDirectoryName).FullName;
        }

        public async Task Generate(WADNRDbContext dbContext, FileService fileService)
        {
            var templatePath = GetTemplatePath();
            DocxDocument document;
            await SaveTemplateFileToTempDirectoryAsync(fileService);

            if (TemplateHasImages(templatePath))
            {
                await SaveImageFilesToTempDirectoryAsync(dbContext, fileService);
            }

            RemoveBookmarks(templatePath);

            switch (ReportTemplateModelEnum)
            {
                case ReportTemplateModelEnum.Project:
                    var baseViewModel = new ReportTemplateProjectBaseViewModel
                    {
                        ReportTitle = ReportTemplate.DisplayName,
                        ReportModel = await GetListOfProjectModelsAsync(dbContext)
                    };
                    document = DocumentFactory.Create<DocxDocument>(templatePath, baseViewModel);
                    break;
                case ReportTemplateModelEnum.InvoicePaymentRequest:
                    var iprBaseViewModel = new ReportTemplateInvoicePaymentRequestBaseViewModel
                    {
                        ReportTitle = ReportTemplate.DisplayName,
                        ReportModel = await GetListOfInvoicePaymentRequestModelsAsync(dbContext)
                    };
                    document = DocumentFactory.Create<DocxDocument>(templatePath, iprBaseViewModel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var compilePath = GetCompilePath();
            document.ImageDirectory = FullTemplateTempImageDirectory;

            // SharpDocx compiles and executes inline C# from the .docx template.
            // On Linux containers the default culture is invariant, which renders
            // currency as ¤ instead of $. Set en-US for the duration of Generate().
            var previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            try
            {
                document.Generate(compilePath);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = previousCulture;
            }

            CleanTempDirectoryOfOldFiles(FullTemplateTempDirectory);
        }

        private void RemoveBookmarks(string templatePath)
        {
            using var wordDoc = WordprocessingDocument.Open(templatePath, true);
            var bs = wordDoc.MainDocumentPart.Document
                .Descendants<BookmarkStart>()
                .ToList();
            foreach (var s in bs)
                s.Remove();

            var be = wordDoc.MainDocumentPart.Document
                .Descendants<BookmarkEnd>()
                .ToList();
            foreach (var e in be)
                e.Remove();
        }

        private static bool TemplateHasImages(string templatePath)
        {
            using var wordDoc = WordprocessingDocument.Open(templatePath, true);
            using var sr = new StreamReader(wordDoc.MainDocumentPart.GetStream());
            var docText = sr.ReadToEnd();
            var regexForImage = @"Image\(";
            var match = Regex.Match(docText, regexForImage);
            return match.Success;
        }

        private async Task SaveImageFilesToTempDirectoryAsync(WADNRDbContext dbContext, FileService fileService)
        {
            switch (ReportTemplateModelEnum)
            {
                case ReportTemplateModelEnum.Project:
                    var projectImages = await dbContext.ProjectImages
                        .AsNoTracking()
                        .Include(x => x.FileResource)
                        .Where(x => SelectedModelIDs.Contains(x.ProjectID))
                        .ToListAsync();

                    foreach (var projectImage in projectImages)
                    {
                        var fileName = $"{projectImage.FileResource.FileResourceGUID}.{projectImage.FileResource.OriginalFileExtension}";
                        var imagePath = Path.Combine(FullTemplateTempImageDirectory, fileName);
                        var stream = await fileService.GetFileStreamFromBlobStorage(projectImage.FileResource.FileResourceGUID.ToString());
                        if (stream != null)
                        {
                            using var ms = new MemoryStream();
                            await stream.CopyToAsync(ms);
                            CorrectImageAndSaveToDisk(ms.ToArray(), imagePath);
                        }
                    }
                    break;
                case ReportTemplateModelEnum.InvoicePaymentRequest:
                    // IPR does not have images
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void CorrectImageAndSaveToDisk(byte[] imageBytes, string imagePath)
        {
            if (File.Exists(imagePath)) return;
            using var skBitmap = SKBitmap.Decode(imageBytes);
            if (skBitmap == null) return;
            using var image = SKImage.FromBitmap(skBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 85);
            using var stream = File.OpenWrite(imagePath);
            data.SaveTo(stream);
        }

        private async Task SaveTemplateFileToTempDirectoryAsync(FileService fileService)
        {
            var filePath = GetTemplatePath();
            var stream = await fileService.GetFileStreamFromBlobStorage(ReportTemplate.FileResource.FileResourceGUID.ToString());
            if (stream != null)
            {
                await using var fileStream = File.Create(filePath);
                await stream.CopyToAsync(fileStream);
            }
        }

        private string FileExtensionWithDot => ReportTemplate.FileResource.OriginalFileExtension.StartsWith(".")
            ? ReportTemplate.FileResource.OriginalFileExtension
            : $".{ReportTemplate.FileResource.OriginalFileExtension}";

        private string GetTemplatePath()
        {
            var fileName = new FileInfo(Path.Combine(FullTemplateTempDirectory, $"{ReportTemplateUniqueIdentifier}-{ReportTemplate.FileResource.OriginalBaseFilename}{FileExtensionWithDot}"));
            fileName.Directory.Create();
            return fileName.FullName;
        }

        public string GetCompilePath()
        {
            var fileName = new FileInfo(Path.Combine(FullTemplateTempDirectory, $"{ReportTemplateUniqueIdentifier}-generated-{ReportTemplate.FileResource.OriginalBaseFilename}{FileExtensionWithDot}"));
            fileName.Directory.Create();
            return fileName.FullName;
        }

        private async Task<List<ReportTemplateProjectModel>> GetListOfProjectModelsAsync(WADNRDbContext dbContext)
        {
            // 1. Load base projects with reference (non-collection) navigations only
            var projects = await dbContext.Projects
                .AsNoTracking()
                .Include(x => x.ProjectType).ThenInclude(x => x.TaxonomyBranch)
                .Where(x => SelectedModelIDs.Contains(x.ProjectID))
                .ToListAsync();

            var projectIds = projects.Select(p => p.ProjectID).ToList();

            // 2. Load each collection separately to avoid cartesian explosion
            var projectPeople = await dbContext.ProjectPeople
                .AsNoTracking()
                .Include(x => x.Person).ThenInclude(x => x.Organization).ThenInclude(x => x.OrganizationType)
                .Where(x => projectIds.Contains(x.ProjectID))
                .ToListAsync();

            var projectOrganizations = await dbContext.ProjectOrganizations
                .AsNoTracking()
                .Include(x => x.Organization).ThenInclude(x => x.People)
                .Include(x => x.Organization).ThenInclude(x => x.OrganizationType)
                .Include(x => x.Organization).ThenInclude(x => x.PrimaryContactPerson)
                .Include(x => x.RelationshipType)
                .Where(x => projectIds.Contains(x.ProjectID))
                .ToListAsync();

            var projectImages = await dbContext.ProjectImages
                .AsNoTracking()
                .Include(x => x.FileResource)
                .Where(x => projectIds.Contains(x.ProjectID))
                .ToListAsync();

            var projectRegions = await dbContext.ProjectRegions
                .AsNoTracking()
                .Include(x => x.DNRUplandRegion)
                .Where(x => projectIds.Contains(x.ProjectID))
                .ToListAsync();

            var projectCounties = await dbContext.ProjectCounties
                .AsNoTracking()
                .Include(x => x.County)
                .Where(x => projectIds.Contains(x.ProjectID))
                .ToListAsync();

            var treatments = await dbContext.Treatments
                .AsNoTracking()
                .Where(x => projectIds.Contains(x.ProjectID))
                .ToListAsync();

            var projectFundingSources = await dbContext.ProjectFundingSources
                .AsNoTracking()
                .Where(x => projectIds.Contains(x.ProjectID))
                .ToListAsync();

            var invoicePaymentRequests = await dbContext.InvoicePaymentRequests
                .AsNoTracking()
                .Include(x => x.Vendor)
                .Include(x => x.PreparedByPerson)
                .Include(x => x.Invoices).ThenInclude(x => x.FundSource)
                .Include(x => x.Invoices).ThenInclude(x => x.ProgramIndex)
                .Include(x => x.Invoices).ThenInclude(x => x.ProjectCode)
                .Where(x => projectIds.Contains(x.ProjectID))
                .ToListAsync();

            // 3. Assign collections back to project entities
            foreach (var project in projects)
            {
                project.ProjectPeople = projectPeople.Where(x => x.ProjectID == project.ProjectID).ToList();
                project.ProjectOrganizations = projectOrganizations.Where(x => x.ProjectID == project.ProjectID).ToList();
                project.ProjectImages = projectImages.Where(x => x.ProjectID == project.ProjectID).ToList();
                project.ProjectRegions = projectRegions.Where(x => x.ProjectID == project.ProjectID).ToList();
                project.ProjectCounties = projectCounties.Where(x => x.ProjectID == project.ProjectID).ToList();
                project.Treatments = treatments.Where(x => x.ProjectID == project.ProjectID).ToList();
                project.ProjectFundingSources = projectFundingSources.Where(x => x.ProjectID == project.ProjectID).ToList();
                project.InvoicePaymentRequests = invoicePaymentRequests.Where(x => x.ProjectID == project.ProjectID).ToList();
            }

            return projects
                .OrderBy(p => SelectedModelIDs.IndexOf(p.ProjectID))
                .Select(x => new ReportTemplateProjectModel(x))
                .ToList();
        }

        private async Task<List<ReportTemplateInvoicePaymentRequestModel>> GetListOfInvoicePaymentRequestModelsAsync(WADNRDbContext dbContext)
        {
            var iprModels = await dbContext.InvoicePaymentRequests
                .AsNoTracking()
                .Include(x => x.Vendor)
                .Include(x => x.PreparedByPerson).ThenInclude(x => x.Organization)
                .Where(x => SelectedModelIDs.Contains(x.InvoicePaymentRequestID))
                .ToListAsync();

            var iprIds = iprModels.Select(x => x.InvoicePaymentRequestID).ToList();

            var invoices = await dbContext.Invoices
                .AsNoTracking()
                .Include(x => x.FundSource)
                .Include(x => x.ProgramIndex)
                .Include(x => x.ProjectCode)
                .Where(x => iprIds.Contains(x.InvoicePaymentRequestID))
                .ToListAsync();

            foreach (var ipr in iprModels)
            {
                ipr.Invoices = invoices.Where(x => x.InvoicePaymentRequestID == ipr.InvoicePaymentRequestID).ToList();
            }

            return iprModels
                .OrderBy(p => SelectedModelIDs.IndexOf(p.InvoicePaymentRequestID))
                .Select(x => new ReportTemplateInvoicePaymentRequestModel(x))
                .ToList();
        }

        public static async Task<ReportTemplateValidationResult> ValidateReportTemplateAsync(
            ReportTemplate reportTemplate, WADNRDbContext dbContext, ILogger logger, FileService fileService)
        {
            var reportTemplateModel = (ReportTemplateModelEnum)reportTemplate.ReportTemplateModelID;
            List<int> selectedModelIDs;
            switch (reportTemplateModel)
            {
                case ReportTemplateModelEnum.Project:
                    selectedModelIDs = await dbContext.Projects.AsNoTracking()
                        .Select(x => x.ProjectID).Take(10).ToListAsync();
                    break;
                case ReportTemplateModelEnum.InvoicePaymentRequest:
                    selectedModelIDs = await dbContext.InvoicePaymentRequests.AsNoTracking()
                        .Select(x => x.InvoicePaymentRequestID).Take(10).ToListAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return await ValidateReportTemplateForSelectedModelIDsAsync(reportTemplate, selectedModelIDs, logger, dbContext, fileService);
        }

        public static async Task<ReportTemplateValidationResult> ValidateReportTemplateForSelectedModelIDsAsync(
            ReportTemplate reportTemplate, List<int> selectedModelIDs, ILogger logger,
            WADNRDbContext dbContext, FileService fileService)
        {
            var validationResult = new ReportTemplateValidationResult();
            var reportTemplateGenerator = new ReportTemplateGenerator(reportTemplate, selectedModelIDs);
            var tempDirectory = reportTemplateGenerator.GetCompilePath();
            try
            {
                await reportTemplateGenerator.Generate(dbContext, fileService);
                validationResult.IsValid = true;
            }
            catch (SharpDocxCompilationException exception)
            {
                validationResult.ErrorMessage = exception.Errors;
                validationResult.SourceCode = exception.SourceCode;
                validationResult.IsValid = false;
                logger.LogError(
                    "There was a SharpDocxCompilationException validating a report template. Temporary template file location:\"{TempDirectory}\" Error Message: \"{ErrorMessage}\". Source Code: \"{SourceCode}\"",
                    tempDirectory, validationResult.ErrorMessage, validationResult.SourceCode);
            }
            catch (Exception exception)
            {
                validationResult.IsValid = false;
                switch (exception.Message)
                {
                    case "No end tag found for code.":
                        validationResult.ErrorMessage =
                            $"CodeBlockBuilder exception: \"{exception.Message}\". Could not find a matching closing tag \"%>\" for an opening tag.";
                        break;
                    case "TextBlock is not terminated with '<% } %>'.":
                        validationResult.ErrorMessage = $"CodeBlockBuilder exception: \"{exception.Message}\".";
                        break;
                    default:
                        validationResult.ErrorMessage = exception.Message;
                        break;
                }

                validationResult.SourceCode = exception.StackTrace;
                logger.LogError(exception,
                    "There was an exception validating a report template. Temporary template file location:\"{TempDirectory}\". Error Message: \"{ErrorMessage}\".",
                    tempDirectory, validationResult.ErrorMessage);
            }

            return validationResult;
        }

        private void CleanTempDirectoryOfOldFiles(string targetDirectory)
        {
            if (Directory.Exists(targetDirectory))
            {
                var fileEntries = Directory.GetFiles(targetDirectory);
                foreach (var fileName in fileEntries)
                    DeleteFileIfOlderThanLifespan(fileName);
                var directories = Directory.GetDirectories(targetDirectory);
                foreach (var directory in directories)
                    CleanTempDirectoryOfOldFiles(directory);
            }
        }

        private void DeleteFileIfOlderThanLifespan(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.LastAccessTime < DateTime.Now.AddDays(-TemplateTempDirectoryFileLifespanInDays))
                fileInfo.Delete();
        }
    }

    public class ReportTemplateValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public string SourceCode { get; set; }

        public ReportTemplateValidationResult()
        {
            IsValid = true;
            ErrorMessage = "";
            SourceCode = "";
        }
    }
}
