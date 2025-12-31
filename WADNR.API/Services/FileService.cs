using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using WADNR.Common;
using WADNR.EFModels.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WADNR.API.Services
{
    public class FileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly IAzureStorage _azureStorage;

        public const string FileContainerName = "wadnr-file-resource";

        public FileService(ILogger<FileService> logger, IAzureStorage azureStorage)
        {
            _logger = logger;
            _azureStorage = azureStorage;
        }

        public string MakePrettyFileName(string filename)
        {
            var replacedFileName = filename
                .Replace("(", "")
                .Replace(")", "")
                .Replace(" ", "")
                .Replace(",", "")
                .Replace(":", "")
                .Replace(";", "")
                .Replace("\"", "")
                .Replace("&", "")
                .Replace("#", "")
                .Replace("'", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace(" ", "");

            return replacedFileName;
        }

        public async Task<Stream> GetFileStreamFromBlobStorage(string canonicalName)
        {
            try
            {
                var blobDto = await _azureStorage.DownloadAsync(FileContainerName, canonicalName);
                return blobDto.Content;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return null;
        }

        public async Task SaveFileStreamToAzureBlobStorage(string canonicalName, Stream stream)
        {
            _logger.LogInformation($"Saving file stream {canonicalName} to {FileContainerName}");
            stream.Seek(0, SeekOrigin.Begin);
            await _azureStorage.UploadAsync(FileContainerName, canonicalName, stream);
        }

        public async Task<FileResource> CreateFileResource(WADNRDbContext dbContext, IFormFile file, int createUserID)
        {
            _logger.LogInformation($"Creating new File Resource from IFormFile {file.Name}");
            await using var stream = file.OpenReadStream();
            return await CreateFileResource(dbContext, createUserID, stream, file.FileName);
        }

        private async Task<FileResource> CreateFileResource(WADNRDbContext dbContext, int createUserID,
            Stream stream, string fileName)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var canonicalName = Guid.NewGuid();
            var uploadedFile = await _azureStorage.UploadAsync(FileContainerName, canonicalName.ToString(), stream);

            if (uploadedFile.Error)
            {
                throw new Exception(
                    $"There was an error uploading the FormFile \"{fileName}\" to blob storage with the canonical name \"{canonicalName}\". Error Details: {uploadedFile.Status}");
            }

            var fileNameSegments = fileName.Split(".");
            var newFileResource = new FileResource()
            {
                CreateDate = DateTime.UtcNow,
                CreatePersonID = createUserID,
                //FileResourceCanonicalName = canonicalName.ToString(),
                FileResourceGUID = canonicalName,
                OriginalFileExtension = fileNameSegments.Last(),
                OriginalBaseFilename = String.Join(".", fileNameSegments.Take(fileNameSegments.Length - 1)),
                InBlobStorage = true,
                ContentLength = stream.Length
            };

            dbContext.FileResources.Add(newFileResource);
            await dbContext.SaveChangesAsync();
            await dbContext.Entry(newFileResource).ReloadAsync();
            return newFileResource;
        }

        public async Task<FileResource> CreateFileResource(WADNRDbContext dbContext, Stream stream, string fullFileName, int createUserID)
        {
            return await CreateFileResource(dbContext, createUserID, stream, fullFileName);
        }

        public async Task<FileStream> CreateZipFileFromFileResources(List<FileResource> fileResources)
        {
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileResourceInfo in fileResources)
                {
                    var fileInZip = archive.CreateEntry($"{fileResourceInfo.OriginalBaseFilename}.{fileResourceInfo.OriginalFileExtension}");
                    var blobStream = await GetFileStreamFromBlobStorage(fileResourceInfo.FileResourceGUID.ToString());

                    await using var s = fileInZip.Open();
                    await blobStream.CopyToAsync(s);
                }
            }

            var disposableTempFile = DisposableTempFile.MakeDisposableTempFileEndingIn(".zip");
            var fileStream = new FileStream(disposableTempFile.FileInfo.FullName, FileMode.Create);
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(fileStream);
            fileStream.Seek(0, SeekOrigin.Begin);
            return fileStream;
        }

        public async Task DeleteFileStreamFromBlobStorageAsync(string canonicalName)
        {
            try
            {
                await _azureStorage.DeleteAsync(FileContainerName, canonicalName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting blob {CanonicalName} from {Container}", canonicalName, FileContainerName);
            }
        }
    }
}