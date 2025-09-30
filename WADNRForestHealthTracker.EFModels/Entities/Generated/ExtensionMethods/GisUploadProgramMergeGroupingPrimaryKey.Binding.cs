//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisUploadProgramMergeGrouping
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisUploadProgramMergeGroupingPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisUploadProgramMergeGrouping>
    {
        public GisUploadProgramMergeGroupingPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisUploadProgramMergeGroupingPrimaryKey(GisUploadProgramMergeGrouping gisUploadProgramMergeGrouping) : base(gisUploadProgramMergeGrouping){}

        public static implicit operator GisUploadProgramMergeGroupingPrimaryKey(int primaryKeyValue)
        {
            return new GisUploadProgramMergeGroupingPrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisUploadProgramMergeGroupingPrimaryKey(GisUploadProgramMergeGrouping gisUploadProgramMergeGrouping)
        {
            return new GisUploadProgramMergeGroupingPrimaryKey(gisUploadProgramMergeGrouping);
        }
    }
}