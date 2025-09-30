//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FocusAreaStatus
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FocusAreaStatusPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FocusAreaStatus>
    {
        public FocusAreaStatusPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FocusAreaStatusPrimaryKey(FocusAreaStatus focusAreaStatus) : base(focusAreaStatus){}

        public static implicit operator FocusAreaStatusPrimaryKey(int primaryKeyValue)
        {
            return new FocusAreaStatusPrimaryKey(primaryKeyValue);
        }

        public static implicit operator FocusAreaStatusPrimaryKey(FocusAreaStatus focusAreaStatus)
        {
            return new FocusAreaStatusPrimaryKey(focusAreaStatus);
        }
    }
}