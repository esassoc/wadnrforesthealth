//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.DNRUplandRegionContentImage
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class DNRUplandRegionContentImagePrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<DNRUplandRegionContentImage>
    {
        public DNRUplandRegionContentImagePrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public DNRUplandRegionContentImagePrimaryKey(DNRUplandRegionContentImage dNRUplandRegionContentImage) : base(dNRUplandRegionContentImage){}

        public static implicit operator DNRUplandRegionContentImagePrimaryKey(int primaryKeyValue)
        {
            return new DNRUplandRegionContentImagePrimaryKey(primaryKeyValue);
        }

        public static implicit operator DNRUplandRegionContentImagePrimaryKey(DNRUplandRegionContentImage dNRUplandRegionContentImage)
        {
            return new DNRUplandRegionContentImagePrimaryKey(dNRUplandRegionContentImage);
        }
    }
}