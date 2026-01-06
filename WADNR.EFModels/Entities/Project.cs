using System;
using System.Collections.Generic;
using System.Text;
namespace WADNR.EFModels.Entities;

public partial class Project
{
    public bool IsActiveProject()
    {
        return ProjectApprovalStatus == ProjectApprovalStatus.Approved;
    }
}