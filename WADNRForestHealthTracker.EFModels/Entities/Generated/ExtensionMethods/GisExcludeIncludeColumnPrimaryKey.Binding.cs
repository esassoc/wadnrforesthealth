//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisExcludeIncludeColumn
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisExcludeIncludeColumnPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisExcludeIncludeColumn>
    {
        public GisExcludeIncludeColumnPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisExcludeIncludeColumnPrimaryKey(GisExcludeIncludeColumn gisExcludeIncludeColumn) : base(gisExcludeIncludeColumn){}

        public static implicit operator GisExcludeIncludeColumnPrimaryKey(int primaryKeyValue)
        {
            return new GisExcludeIncludeColumnPrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisExcludeIncludeColumnPrimaryKey(GisExcludeIncludeColumn gisExcludeIncludeColumn)
        {
            return new GisExcludeIncludeColumnPrimaryKey(gisExcludeIncludeColumn);
        }
    }
}