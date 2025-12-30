using Microsoft.EntityFrameworkCore;
using WADNRForestHealthTracker.Models.DataTransferObjects;

namespace WADNRForestHealthTracker.EFModels.Entities;

public static class People
{
    public static async Task<PersonDetail?> GetByIDAsDetailAsync(WADNRForestHealthTrackerDbContext dbContext, int personID)
    {
        var person = await dbContext.People
            .AsNoTracking()
            .Where(x => x.PersonID == personID)
            .Select(PersonProjections.AsDetail)
            .SingleOrDefaultAsync();
        return person;
    }

    public static PersonDetail? GetByEmailAsDetail(WADNRForestHealthTrackerDbContext dbContext, string email)
    {
        var person = dbContext.People
            .AsNoTracking()
            .Where(x => x.Email == email)
            .Select(PersonProjections.AsDetail)
            .SingleOrDefault();
        return person;
    }
}