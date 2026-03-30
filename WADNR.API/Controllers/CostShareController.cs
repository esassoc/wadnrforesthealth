using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;

namespace WADNR.API.Controllers;

[ApiController]
[Route("cost-share")]
public class CostShareController(
    WADNRDbContext dbContext,
    ILogger<CostShareController> logger,
    IOptions<WADNRConfiguration> configuration,
    IWebHostEnvironment env)
    : SitkaController<CostShareController>(dbContext, logger, configuration)
{
    private string GetTemplatePath() =>
        Path.Combine(env.ContentRootPath, "Content", "CostShareBlankFormFields.pdf");

    [HttpGet("generate-pdf")]
    [AllowAnonymous]
    public IActionResult BlankCostShareAgreementPdf()
    {
        var templatePath = GetTemplatePath();
        if (!System.IO.File.Exists(templatePath))
        {
            return NotFound("Cost share agreement template not found.");
        }

        var bytes = System.IO.File.ReadAllBytes(templatePath);
        return File(bytes, "application/pdf", "CostShareAgreementBlank.pdf");
    }

    [HttpGet("generate-pdf/{projectPersonID}")]
    [CostShareViewFeature]
    [EntityNotFound(typeof(ProjectPerson), "projectPersonID")]
    public async Task<IActionResult> FilledCostShareAgreementPdf([FromRoute] int projectPersonID)
    {
        var projectPerson = await DbContext.ProjectPeople
            .AsNoTracking()
            .Include(pp => pp.Person)
            .FirstOrDefaultAsync(pp => pp.ProjectPersonID == projectPersonID);

        if (projectPerson == null)
        {
            return NotFound($"ProjectPerson with ID {projectPersonID} does not exist.");
        }

        if (projectPerson.ProjectPersonRelationshipTypeID != (int)ProjectPersonRelationshipTypeEnum.PrivateLandowner)
        {
            return NotFound("Cost share agreements are only available for Private Landowner contacts.");
        }

        var templatePath = GetTemplatePath();
        if (!System.IO.File.Exists(templatePath))
        {
            return NotFound("Cost share agreement template not found.");
        }

        var person = projectPerson.Person;
        var fullName = $"{person.FirstName} {person.LastName}".Trim();

        using var templateStream = System.IO.File.OpenRead(templatePath);
        var doc = PdfReader.Open(templateStream, PdfDocumentOpenMode.Modify);

        // Work at the raw PDF dictionary level to avoid PDFsharp's AcroForm field API,
        // which triggers font resolution (and fails on .NET Core) inside PdfTextField's constructor.
        var acroFormDict = doc.AcroForm;
        if (acroFormDict != null)
        {
            acroFormDict.Elements.SetBoolean("/NeedAppearances", true);

            var fieldsArray = acroFormDict.Elements.GetArray("/Fields");
            if (fieldsArray != null)
            {
                SetRawFieldValue(fieldsArray, "Names", fullName);
                SetRawFieldValue(fieldsArray, "Address1", person.PersonAddress ?? "");
                SetRawFieldValue(fieldsArray, "Address2", "");
                SetRawFieldValue(fieldsArray, "PhoneNumber", person.Phone ?? "");
                SetRawFieldValue(fieldsArray, "Email", person.Email ?? "");
            }
        }

        using var outputStream = new MemoryStream();
        doc.Save(outputStream);
        var outputBytes = outputStream.ToArray();

        var safeFileName = fullName.Replace(" ", "");
        return File(outputBytes, "application/pdf", $"CostShareAgreement-{safeFileName}.pdf");
    }

    /// <summary>
    /// Sets a form field value by walking the /Fields array at the raw PDF dictionary level.
    /// This avoids PDFsharp's PdfTextField constructor, which triggers font resolution
    /// and throws on .NET Core without platform-specific font packages.
    /// </summary>
    private static void SetRawFieldValue(PdfArray fieldsArray, string fieldName, string value)
    {
        for (var i = 0; i < fieldsArray.Elements.Count; i++)
        {
            var fieldRef = fieldsArray.Elements.GetDictionary(i);
            if (fieldRef == null) continue;

            var nameElement = fieldRef.Elements.GetString("/T");
            if (nameElement == fieldName)
            {
                fieldRef.Elements.SetString("/V", value);
                // Remove existing appearance stream so the PDF viewer regenerates it
                fieldRef.Elements.Remove("/AP");
                return;
            }
        }
    }
}
