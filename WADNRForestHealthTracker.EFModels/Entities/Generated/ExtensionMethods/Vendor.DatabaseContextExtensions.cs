//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Vendor]
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public static partial class DatabaseContextExtensions
    {
        public static Vendor GetVendor(this IQueryable<Vendor> vendors, int vendorID)
        {
            var vendor = vendors.SingleOrDefault(x => x.VendorID == vendorID);
            Check.RequireNotNullThrowNotFound(vendor, "Vendor", vendorID);
            return vendor;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteVendor(this IQueryable<Vendor> vendors, List<int> vendorIDList)
        {
            if(vendorIDList.Any())
            {
                var vendorsInSourceCollectionToDelete = vendors.Where(x => vendorIDList.Contains(x.VendorID));
                foreach (var vendorToDelete in vendorsInSourceCollectionToDelete.ToList())
                {
                    vendorToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteVendor(this IQueryable<Vendor> vendors, ICollection<Vendor> vendorsToDelete)
        {
            if(vendorsToDelete.Any())
            {
                var vendorIDList = vendorsToDelete.Select(x => x.VendorID).ToList();
                var vendorsToDeleteFromSourceList = vendors.Where(x => vendorIDList.Contains(x.VendorID)).ToList();

                foreach (var vendorToDelete in vendorsToDeleteFromSourceList)
                {
                    vendorToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteVendor(this IQueryable<Vendor> vendors, int vendorID)
        {
            DeleteVendor(vendors, new List<int> { vendorID });
        }

        public static void DeleteVendor(this IQueryable<Vendor> vendors, Vendor vendorToDelete)
        {
            DeleteVendor(vendors, new List<Vendor> { vendorToDelete });
        }
    }
}