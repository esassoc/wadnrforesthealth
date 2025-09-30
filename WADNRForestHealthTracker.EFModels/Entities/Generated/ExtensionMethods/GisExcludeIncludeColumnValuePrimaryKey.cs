//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisExcludeIncludeColumnValue
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisExcludeIncludeColumnValuePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisExcludeIncludeColumnValue>
    {
        public GisExcludeIncludeColumnValuePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisExcludeIncludeColumnValuePrimaryKey(GisExcludeIncludeColumnValue gisExcludeIncludeColumnValue) : base(gisExcludeIncludeColumnValue){}

        public static implicit operator GisExcludeIncludeColumnValuePrimaryKey(int primaryKeyValue)
        {
            return new GisExcludeIncludeColumnValuePrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisExcludeIncludeColumnValuePrimaryKey(GisExcludeIncludeColumnValue gisExcludeIncludeColumnValue)
        {
            return new GisExcludeIncludeColumnValuePrimaryKey(gisExcludeIncludeColumnValue);
        }
    }
}