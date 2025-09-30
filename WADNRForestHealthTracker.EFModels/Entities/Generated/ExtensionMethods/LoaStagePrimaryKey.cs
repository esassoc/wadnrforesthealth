//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.LoaStage
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class LoaStagePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<LoaStage>
    {
        public LoaStagePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public LoaStagePrimaryKey(LoaStage loaStage) : base(loaStage){}

        public static implicit operator LoaStagePrimaryKey(int primaryKeyValue)
        {
            return new LoaStagePrimaryKey(primaryKeyValue);
        }

        public static implicit operator LoaStagePrimaryKey(LoaStage loaStage)
        {
            return new LoaStagePrimaryKey(loaStage);
        }
    }
}