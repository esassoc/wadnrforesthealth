//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ForesterWorkUnit
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ForesterWorkUnitPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ForesterWorkUnit>
    {
        public ForesterWorkUnitPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ForesterWorkUnitPrimaryKey(ForesterWorkUnit foresterWorkUnit) : base(foresterWorkUnit){}

        public static implicit operator ForesterWorkUnitPrimaryKey(int primaryKeyValue)
        {
            return new ForesterWorkUnitPrimaryKey(primaryKeyValue);
        }

        public static implicit operator ForesterWorkUnitPrimaryKey(ForesterWorkUnit foresterWorkUnit)
        {
            return new ForesterWorkUnitPrimaryKey(foresterWorkUnit);
        }
    }
}