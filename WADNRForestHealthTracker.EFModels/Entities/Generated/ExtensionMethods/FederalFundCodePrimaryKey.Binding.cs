//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.FederalFundCode
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class FederalFundCodePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<FederalFundCode>
    {
        public FederalFundCodePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public FederalFundCodePrimaryKey(FederalFundCode federalFundCode) : base(federalFundCode){}

        public static implicit operator FederalFundCodePrimaryKey(int primaryKeyValue)
        {
            return new FederalFundCodePrimaryKey(primaryKeyValue);
        }

        public static implicit operator FederalFundCodePrimaryKey(FederalFundCode federalFundCode)
        {
            return new FederalFundCodePrimaryKey(federalFundCode);
        }
    }
}