using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class People
{
    public static async Task<List<PersonLookupItem>> ListAsLookupItemAsync(WADNRDbContext dbContext)
    {
        var items = await dbContext.People
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(PersonProjections.AsLookupItem)
            .ToListAsync();
        return items;
    }

    public static async Task<PersonDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int personID)
    {
        var person = await dbContext.People
            .AsNoTracking()
            .Where(x => x.PersonID == personID)
            .Select(PersonProjections.AsDetail)
            .SingleOrDefaultAsync();
        return person;
    }

    public static PersonDetail? GetByEmailAsDetail(WADNRDbContext dbContext, string email)
    {
        var person = dbContext.People
            .AsNoTracking()
            .Where(x => x.Email == email)
            .Select(PersonProjections.AsDetail)
            .SingleOrDefault();
        return person;
    }
}