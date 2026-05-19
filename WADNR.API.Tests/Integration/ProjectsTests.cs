using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;

namespace WADNR.API.Tests.Integration;

[TestClass]
public class ProjectsTests
{
    private static WADNRDbContext NewInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new WADNRDbContext(options);
    }

    private static Project SeedProject(string fhtProjectNumber) => new()
    {
        ProjectName = $"Seed {fhtProjectNumber}",
        FhtProjectNumber = fhtProjectNumber,
    };

    [TestMethod]
    public async Task GenerateFhtProjectNumber_EmptyDb_ReturnsYearDashOne()
    {
        await using var dbContext = NewInMemoryContext();
        var year = DateTime.Now.Year;

        var result = await Projects.GenerateFhtProjectNumberAsync(dbContext);

        Assert.AreEqual($"FHT-{year}-00001", result);
    }

    [TestMethod]
    public async Task GenerateFhtProjectNumber_ExistingCurrentYear_IncrementsCounter()
    {
        await using var dbContext = NewInMemoryContext();
        var year = DateTime.Now.Year;
        dbContext.Projects.Add(SeedProject($"FHT-{year}-00005"));
        await dbContext.SaveChangesAsync();

        var result = await Projects.GenerateFhtProjectNumberAsync(dbContext);

        Assert.AreEqual($"FHT-{year}-00006", result);
    }

    [TestMethod]
    public async Task GenerateFhtProjectNumber_PriorYearOnly_RestartsAtOne()
    {
        await using var dbContext = NewInMemoryContext();
        var year = DateTime.Now.Year;
        dbContext.Projects.Add(SeedProject("FHT-2019-00099"));
        await dbContext.SaveChangesAsync();

        var result = await Projects.GenerateFhtProjectNumberAsync(dbContext);

        Assert.AreEqual($"FHT-{year}-00001", result);
    }

    [TestMethod]
    public async Task GenerateFhtProjectNumber_BugFormatPresent_RestartsAtOne()
    {
        await using var dbContext = NewInMemoryContext();
        var year = DateTime.Now.Year;
        dbContext.Projects.Add(SeedProject("FHT-00001"));
        dbContext.Projects.Add(SeedProject("FHT-00002"));
        await dbContext.SaveChangesAsync();

        var result = await Projects.GenerateFhtProjectNumberAsync(dbContext);

        Assert.AreEqual($"FHT-{year}-00001", result);
    }

    [TestMethod]
    public async Task GenerateFhtProjectNumber_MixedYears_OnlyConsidersCurrentYear()
    {
        await using var dbContext = NewInMemoryContext();
        var year = DateTime.Now.Year;
        dbContext.Projects.Add(SeedProject($"FHT-{year - 1}-00050"));
        dbContext.Projects.Add(SeedProject($"FHT-{year}-00003"));
        await dbContext.SaveChangesAsync();

        var result = await Projects.GenerateFhtProjectNumberAsync(dbContext);

        Assert.AreEqual($"FHT-{year}-00004", result);
    }

    [TestMethod]
    public async Task GenerateFhtProjectNumber_GisIdentifierFormat_Ignored()
    {
        await using var dbContext = NewInMemoryContext();
        var year = DateTime.Now.Year;
        dbContext.Projects.Add(SeedProject("Chesaw RX fire"));
        await dbContext.SaveChangesAsync();

        var result = await Projects.GenerateFhtProjectNumberAsync(dbContext);

        Assert.AreEqual($"FHT-{year}-00001", result);
    }
}
