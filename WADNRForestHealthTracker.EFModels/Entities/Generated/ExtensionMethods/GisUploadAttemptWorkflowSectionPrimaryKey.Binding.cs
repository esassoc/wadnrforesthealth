//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisUploadAttemptWorkflowSection
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisUploadAttemptWorkflowSectionPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisUploadAttemptWorkflowSection>
    {
        public GisUploadAttemptWorkflowSectionPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisUploadAttemptWorkflowSectionPrimaryKey(GisUploadAttemptWorkflowSection gisUploadAttemptWorkflowSection) : base(gisUploadAttemptWorkflowSection){}

        public static implicit operator GisUploadAttemptWorkflowSectionPrimaryKey(int primaryKeyValue)
        {
            return new GisUploadAttemptWorkflowSectionPrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisUploadAttemptWorkflowSectionPrimaryKey(GisUploadAttemptWorkflowSection gisUploadAttemptWorkflowSection)
        {
            return new GisUploadAttemptWorkflowSectionPrimaryKey(gisUploadAttemptWorkflowSection);
        }
    }
}