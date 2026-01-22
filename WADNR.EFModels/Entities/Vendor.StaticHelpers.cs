using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Vendors
{
    public static async Task<List<VendorGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        return await dbContext.Vendors
            .AsNoTracking()
            .OrderBy(x => x.VendorName)
            .Select(VendorProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<VendorDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int vendorID)
    {
        return await dbContext.Vendors
            .AsNoTracking()
            .Where(x => x.VendorID == vendorID)
            .Select(VendorProjections.AsDetail)
            .SingleOrDefaultAsync();
    }

    public static async Task<List<VendorLookupItem>> SearchAsync(WADNRDbContext dbContext, string searchTerm, int maxResults = 20)
    {
        var term = searchTerm.ToLower();
        return await dbContext.Vendors
            .AsNoTracking()
            .Where(x => x.VendorName.ToLower().Contains(term) ||
                        x.StatewideVendorNumber.ToLower().Contains(term))
            .OrderBy(x => x.VendorName)
            .Take(maxResults)
            .Select(VendorProjections.AsLookupItem)
            .ToListAsync();
    }

    public static async Task<List<VendorPersonGridRow>> ListPeopleAsync(WADNRDbContext dbContext, int vendorID)
    {
        return await dbContext.People
            .AsNoTracking()
            .Where(x => x.VendorID == vendorID && x.IsActive)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new VendorPersonGridRow
            {
                PersonID = x.PersonID,
                FullName = x.FirstName + " " + x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public static async Task<List<VendorOrganizationGridRow>> ListOrganizationsAsync(WADNRDbContext dbContext, int vendorID)
    {
        return await dbContext.Organizations
            .AsNoTracking()
            .Where(x => x.VendorID == vendorID && x.IsActive)
            .OrderBy(x => x.OrganizationName)
            .Select(x => new VendorOrganizationGridRow
            {
                OrganizationID = x.OrganizationID,
                OrganizationName = x.OrganizationName,
                OrganizationShortName = x.OrganizationShortName,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }
}
