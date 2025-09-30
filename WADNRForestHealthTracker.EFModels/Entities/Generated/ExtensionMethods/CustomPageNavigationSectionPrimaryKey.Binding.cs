//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.CustomPageNavigationSection
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class CustomPageNavigationSectionPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<CustomPageNavigationSection>
    {
        public CustomPageNavigationSectionPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public CustomPageNavigationSectionPrimaryKey(CustomPageNavigationSection customPageNavigationSection) : base(customPageNavigationSection){}

        public static implicit operator CustomPageNavigationSectionPrimaryKey(int primaryKeyValue)
        {
            return new CustomPageNavigationSectionPrimaryKey(primaryKeyValue);
        }

        public static implicit operator CustomPageNavigationSectionPrimaryKey(CustomPageNavigationSection customPageNavigationSection)
        {
            return new CustomPageNavigationSectionPrimaryKey(customPageNavigationSection);
        }
    }
}