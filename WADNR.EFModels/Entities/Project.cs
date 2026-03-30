using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public partial class Project
{
    public bool IsActiveProject()
    {
        return ProjectApprovalStatus == ProjectApprovalStatus.Approved;
    }

    public int? GetImplementationStartYear()
    {
        return PlannedDate?.Year;
    }

    public int? GetCompletionYear()
    {
        return CompletionDate?.Year;
    }

    public string Duration
    {
        get
        {
            if (GetImplementationStartYear() == GetCompletionYear() && GetImplementationStartYear().HasValue)
            {
                return GetImplementationStartYear().Value.ToString(CultureInfo.InvariantCulture);
            }

            return
                $"{GetImplementationStartYear()?.ToString(CultureInfo.InvariantCulture) ?? "?"} - {GetCompletionYear()?.ToString(CultureInfo.InvariantCulture) ?? "?"}";
        }
    }

    public OrganizationLookupItem GetLeadImplementerOrganization()
    {
       return ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => new OrganizationLookupItem
            {
                OrganizationID = po.Organization.OrganizationID,
                OrganizationName = po.Organization.DisplayName
            })
            .SingleOrDefault();
    }
}