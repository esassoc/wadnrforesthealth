//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.ForesterRole
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class ForesterRolePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<ForesterRole>
    {
        public ForesterRolePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public ForesterRolePrimaryKey(ForesterRole foresterRole) : base(foresterRole){}

        public static implicit operator ForesterRolePrimaryKey(int primaryKeyValue)
        {
            return new ForesterRolePrimaryKey(primaryKeyValue);
        }

        public static implicit operator ForesterRolePrimaryKey(ForesterRole foresterRole)
        {
            return new ForesterRolePrimaryKey(foresterRole);
        }
    }
}