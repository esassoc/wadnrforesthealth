//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.Division
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class DivisionPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<Division>
    {
        public DivisionPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public DivisionPrimaryKey(Division division) : base(division){}

        public static implicit operator DivisionPrimaryKey(int primaryKeyValue)
        {
            return new DivisionPrimaryKey(primaryKeyValue);
        }

        public static implicit operator DivisionPrimaryKey(Division division)
        {
            return new DivisionPrimaryKey(division);
        }
    }
}