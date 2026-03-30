using System;
using System.Collections.Generic;
using System.Linq;
using WADNR.EFModels.Entities;

namespace WADNR.API.ReportTemplates.Models
{
    public class ReportTemplateProjectModel : ReportTemplateBaseModel
    {
        private Project Project { get; set; }
        private List<ProjectPerson> ProjectPersons { get; set; }
        private List<ProjectOrganization> ProjectOrganizations { get; set; }
        private List<ProjectImage> ProjectImages { get; set; }
        private List<ProjectRegion> ProjectRegions { get; set; }
        private List<ProjectCounty> ProjectCounties { get; set; }
        private List<Treatment> ProjectTreatments { get; set; }

        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectUrl { get; set; }
        public ReportTemplateOrganizationModel PrimaryContactOrganization { get; set; }
        public string ProjectStage { get; set; }
        public ReportTemplatePersonModel ProjectPrimaryContact { get; set; }
        public string PlanningDesignStartYear { get; set; }
        public string ImplementationStartYear { get; set; }
        public string CompletionYear { get; set; }
        public string PrimaryTaxonomyLeaf { get; set; }
        public string FundingType { get; set; }
        public string EstimatedTotalCost { get; set; }
        public int? PercentageMatch { get; set; }
        public string PercentageMatchDisplay => PercentageMatch.HasValue ? $"{PercentageMatch.Value}%" : string.Empty;

        public string TotalFunding { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime ProjectLastUpdated { get; set; }
        public string ProjectLastUpdatedDisplay => ProjectLastUpdated.ToShortDateString();
        public DateTime? ApprovalDate { get; set; }
        public string ApprovalDateDisplay => ApprovalDate.HasValue ? ApprovalDate.Value.ToShortDateString() : string.Empty;
        public DateOnly? ExpirationDate { get; set; }
        public string ExpirationDateDisplay => ExpirationDate.HasValue ? ExpirationDate.Value.ToString("d") : string.Empty;
        public string ProjectRegionsDisplay => string.Join(", ", GetProjectRegions().OrderBy(x => x.Name).Select(x => x.Name));
        public string ProjectCountiesDisplay => string.Join(", ", GetProjectCounties().OrderBy(x => x.Name).Select(x => x.Name));

        public List<ReportTemplateInvoicePaymentRequestModel> InvoicePaymentRequests { get; set; }

        public ReportTemplateProjectModel(Project project)
        {
            // Private properties
            Project = project;
            ProjectPersons = project.ProjectPeople.ToList();
            ProjectOrganizations = project.ProjectOrganizations.ToList();
            ProjectImages = project.ProjectImages.ToList();
            ProjectRegions = project.ProjectRegions.ToList();
            ProjectCounties = project.ProjectCounties.ToList();
            ProjectTreatments = project.Treatments.ToList();

            // Public properties
            ProjectID = Project.ProjectID;
            ProjectName = Project.ProjectName;
            ProjectUrl = string.Empty; // URL generation is not available in API context

            // Primary contact organization: the org linked via a RelationshipType where IsPrimaryContact = true
            var primaryContactOrg = ProjectOrganizations.FirstOrDefault(x => x.RelationshipType?.IsPrimaryContact == true)?.Organization;
            PrimaryContactOrganization = primaryContactOrg != null ? new ReportTemplateOrganizationModel(primaryContactOrg) : null;

            // Project stage from lookup enum
            ProjectStage = EFModels.Entities.ProjectStage.AllLookupDictionary.TryGetValue(Project.ProjectStageID, out var stage)
                ? stage.ProjectStageDisplayName
                : string.Empty;

            // Primary contact: first check ProjectPeople for PrimaryContact relationship, then fall back to org's primary contact
            var primaryContactPerson = ProjectPersons.FirstOrDefault(pp =>
                pp.ProjectPersonRelationshipTypeID == ProjectPersonRelationshipType.PrimaryContact.ProjectPersonRelationshipTypeID)?.Person;
            if (primaryContactPerson == null)
            {
                primaryContactPerson = primaryContactOrg?.PrimaryContactPerson;
            }
            ProjectPrimaryContact = primaryContactPerson != null ? new ReportTemplatePersonModel(primaryContactPerson) : null;

            PlanningDesignStartYear = Project.PlannedDate.HasValue ? Project.PlannedDate.Value.Year.ToString() : string.Empty;
            ImplementationStartYear = Project.PlannedDate.HasValue ? Project.PlannedDate.Value.Year.ToString() : string.Empty;
            CompletionYear = Project.CompletionDate.HasValue ? Project.CompletionDate.Value.Year.ToString() : string.Empty;

            // Taxonomy branch from ProjectType
            PrimaryTaxonomyLeaf = Project.ProjectType?.TaxonomyBranch?.TaxonomyBranchName;

            // Funding type from ProjectFundingSources
            FundingType = string.Join(", ", Project.ProjectFundingSources.Select(x => x.FundingSourceID.ToString()));
            EstimatedTotalCost = Project.EstimatedTotalCost.HasValue ? Project.EstimatedTotalCost.Value.ToString("C", UsCulture) : string.Empty;
            PercentageMatch = Project.PercentageMatch;

            // Total funding from ProjectFundSourceAllocationRequests
            TotalFunding = string.Empty; // Not directly available without ProjectFundSourceAllocationRequests include
            ProjectDescription = Project.ProjectDescription;
            ProjectLastUpdated = DateTime.UtcNow; // Audit log-based LastUpdateDate not available in new EF model
            ApprovalDate = Project.ApprovalDate;
            ExpirationDate = Project.ExpirationDate;

            InvoicePaymentRequests = Project.InvoicePaymentRequests
                .Select(x => new ReportTemplateInvoicePaymentRequestModel(x)).ToList();
        }

        public List<ReportTemplateProjectContactModel> GetProjectContacts()
        {
            return ProjectPersons.Select(x => new ReportTemplateProjectContactModel(x)).ToList();
        }

        public List<ReportTemplateProjectContactModel> GetProjectContactsByType(string contactTypeName)
        {
            var projectContactsInType = ProjectPersons.Where(x =>
                ProjectPersonRelationshipType.AllLookupDictionary.TryGetValue(x.ProjectPersonRelationshipTypeID, out var relType)
                && relType.ProjectPersonRelationshipTypeDisplayName == contactTypeName).ToList();
            return projectContactsInType.Select(x => new ReportTemplateProjectContactModel(x)).ToList();
        }

        public List<ReportTemplateProjectOrganizationModel> GetProjectOrganizations()
        {
            return ProjectOrganizations.Select(x => new ReportTemplateProjectOrganizationModel(x)).ToList();
        }

        public List<ReportTemplateProjectOrganizationModel> GetProjectOrganizationsByType(string organizationTypeName)
        {
            var organizationsInType = ProjectOrganizations.Where(x => x.RelationshipType?.RelationshipTypeName == organizationTypeName).ToList();
            return organizationsInType.Select(x => new ReportTemplateProjectOrganizationModel(x)).ToList();
        }

        public string GetProjectContactNamesByType(string contactTypeName)
        {
            var projectContactsInType = ProjectPersons.Where(x =>
                ProjectPersonRelationshipType.AllLookupDictionary.TryGetValue(x.ProjectPersonRelationshipTypeID, out var relType)
                && relType.ProjectPersonRelationshipTypeDisplayName == contactTypeName).ToList();
            var projectContactNames = projectContactsInType.Select(x => $"{x.Person.FirstName} {x.Person.LastName}").ToList();
            return string.Join(", ", projectContactNames);
        }

        public string GetProjectOrganizationNamesByType(string organizationTypeName)
        {
            var organizationsInType = ProjectOrganizations.Where(x => x.RelationshipType?.RelationshipTypeName == organizationTypeName).ToList();
            var organizationNames = organizationsInType.Select(x => x.Organization?.OrganizationName).ToList();
            return string.Join(", ", organizationNames);
        }

        public List<ReportTemplateProjectImageModel> GetProjectImages()
        {
            return ProjectImages.Select(x => new ReportTemplateProjectImageModel(x)).ToList();
        }

        public List<ReportTemplateProjectImageModel> GetProjectImagesByTiming(string timingName)
        {
            return ProjectImages.Where(x =>
                x.ProjectImageTimingID.HasValue
                && ProjectImageTiming.AllLookupDictionary.TryGetValue(x.ProjectImageTimingID.Value, out var timing)
                && timing.ProjectImageTimingName == timingName)
                .Select(x => new ReportTemplateProjectImageModel(x)).ToList();
        }

        public List<ReportTemplateProjectRegionModel> GetProjectRegions()
        {
            return ProjectRegions.Select(x => new ReportTemplateProjectRegionModel(x)).ToList();
        }

        public List<ReportTemplateProjectCountyModel> GetProjectCounties()
        {
            return ProjectCounties.Select(x => new ReportTemplateProjectCountyModel(x)).ToList();
        }

        public List<ReportTemplateProjectTreatmentModel> GetProjectTreatments()
        {
            return ProjectTreatments.Select(x => new ReportTemplateProjectTreatmentModel(x))
                .OrderBy(x => x.StartDate)
                .ThenBy(x => x.EndDate)
                .ThenBy(x => x.Name)
                .ToList();
        }

        public ReportTemplateProjectImageModel GetProjectKeyPhoto()
        {
            var projectKeyPhoto = ProjectImages.FirstOrDefault(x => x.IsKeyPhoto);
            return projectKeyPhoto != null ? new ReportTemplateProjectImageModel(projectKeyPhoto) : null;
        }

        public List<ReportTemplateInvoicePaymentRequestModel> GetInvoicePaymentRequests()
        {
            return InvoicePaymentRequests;
        }
    }
}
