//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: dbo.GisUploadSourceOrganization
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public class GisUploadSourceOrganizationPrimaryKey : LtInfo.Common.EntityModelBinding.LtInfoEntityPrimaryKey<GisUploadSourceOrganization>
    {
        public GisUploadSourceOrganizationPrimaryKey(int primaryKeyValue) : base(primaryKeyValue){}
        public GisUploadSourceOrganizationPrimaryKey(GisUploadSourceOrganization gisUploadSourceOrganization) : base(gisUploadSourceOrganization){}

        public static implicit operator GisUploadSourceOrganizationPrimaryKey(int primaryKeyValue)
        {
            return new GisUploadSourceOrganizationPrimaryKey(primaryKeyValue);
        }

        public static implicit operator GisUploadSourceOrganizationPrimaryKey(GisUploadSourceOrganization gisUploadSourceOrganization)
        {
            return new GisUploadSourceOrganizationPrimaryKey(gisUploadSourceOrganization);
        }
    }
}