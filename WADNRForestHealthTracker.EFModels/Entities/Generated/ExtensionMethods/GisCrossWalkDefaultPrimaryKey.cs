//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisCrossWalkDefault
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisCrossWalkDefaultPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisCrossWalkDefault>
    {
        public GisCrossWalkDefaultPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisCrossWalkDefaultPrimaryKey(GisCrossWalkDefault gisCrossWalkDefault) : base(gisCrossWalkDefault){}

        public static implicit operator GisCrossWalkDefaultPrimaryKey(int primaryKeyValue)
        {
            return new GisCrossWalkDefaultPrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisCrossWalkDefaultPrimaryKey(GisCrossWalkDefault gisCrossWalkDefault)
        {
            return new GisCrossWalkDefaultPrimaryKey(gisCrossWalkDefault);
        }
    }
}