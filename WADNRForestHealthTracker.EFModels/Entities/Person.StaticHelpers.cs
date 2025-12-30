using Microsoft.EntityFrameworkCore;
using WADNRForestHealthTracker.Models.DataTransferObjects;

namespace WADNRForestHealthTracker.EFModels.Entities;

public static class People
{
    public static async Task<PersonSimpleDto?> GetByIDAsSimpleDto(WADNRForestHealthTrackerDbContext dbContext, int personID)
    {
        var person = await dbContext.People
            .AsNoTracking()
            .Where(x => x.PersonID == personID)
            .Select(PersonProjections.AsSimpleDto)
            .SingleOrDefaultAsync();
        return person;
    }

    public static PersonSimpleDto? GetByEmailAsSimpleDto(WADNRForestHealthTrackerDbContext dbContext, string email)
    {
        var person = dbContext.People
            .AsNoTracking()
            .Where(x => x.Email == email)
            .Select(PersonProjections.AsSimpleDto)
            .SingleOrDefault();
        return person;
    }
}