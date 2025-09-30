//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FocusAreaLocationStaging
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FocusAreaLocationStagingPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FocusAreaLocationStaging>
    {
        public FocusAreaLocationStagingPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FocusAreaLocationStagingPrimaryKey(FocusAreaLocationStaging focusAreaLocationStaging) : base(focusAreaLocationStaging){}

        public static implicit operator FocusAreaLocationStagingPrimaryKey(int primaryKeyValue)
        {
            return new FocusAreaLocationStagingPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FocusAreaLocationStagingPrimaryKey(FocusAreaLocationStaging focusAreaLocationStaging)
        {
            return new FocusAreaLocationStagingPrimaryKey(focusAreaLocationStaging);
        }
    }
}