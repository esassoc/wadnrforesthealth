//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.CostTypeDatamartMapping
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class CostTypeDatamartMappingPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<CostTypeDatamartMapping>
    {
        public CostTypeDatamartMappingPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public CostTypeDatamartMappingPrimaryKey(CostTypeDatamartMapping costTypeDatamartMapping) : base(costTypeDatamartMapping){}

        public static implicit operator CostTypeDatamartMappingPrimaryKey(int primaryKeyValue)
        {
            return new CostTypeDatamartMappingPrimaryKey(primaryKeyValue);
        }

        public static implicit operator CostTypeDatamartMappingPrimaryKey(CostTypeDatamartMapping costTypeDatamartMapping)
        {
            return new CostTypeDatamartMappingPrimaryKey(costTypeDatamartMapping);
        }
    }
}