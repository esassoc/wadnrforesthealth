//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisUploadAttemptWorkflowSectionGrouping
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisUploadAttemptWorkflowSectionGroupingPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisUploadAttemptWorkflowSectionGrouping>
    {
        public GisUploadAttemptWorkflowSectionGroupingPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisUploadAttemptWorkflowSectionGroupingPrimaryKey(GisUploadAttemptWorkflowSectionGrouping gisUploadAttemptWorkflowSectionGrouping) : base(gisUploadAttemptWorkflowSectionGrouping){}

        public static implicit operator GisUploadAttemptWorkflowSectionGroupingPrimaryKey(int primaryKeyValue)
        {
            return new GisUploadAttemptWorkflowSectionGroupingPrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisUploadAttemptWorkflowSectionGroupingPrimaryKey(GisUploadAttemptWorkflowSectionGrouping gisUploadAttemptWorkflowSectionGrouping)
        {
            return new GisUploadAttemptWorkflowSectionGroupingPrimaryKey(gisUploadAttemptWorkflowSectionGrouping);
        }
    }
}