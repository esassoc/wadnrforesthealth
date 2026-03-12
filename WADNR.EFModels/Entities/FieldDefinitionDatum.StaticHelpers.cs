using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FieldDefinitionData
{
    public static async Task<FieldDefinitionDatumDetail?> GetByFieldDefinitionAsDetailAsync(WADNRDbContext dbContext, int fieldDefinitionID)
    {
        return await dbContext.FieldDefinitionData.AsNoTracking().Where(x => x.FieldDefinitionID == fieldDefinitionID).Select(FieldDefinitionDatumProjections.AsSimpleDto).SingleOrDefaultAsync();
    }

    public static async Task<List<FieldDefinitionDatumDetail>> ListAsDetailAsync(WADNRDbContext dbContext)
    {
        return await dbContext.FieldDefinitionData.AsNoTracking().Select(FieldDefinitionDatumProjections.AsSimpleDto).ToListAsync();
    }

    public static async Task<FieldDefinitionDatumDetail?> Update(WADNRDbContext dbContext, int fieldDefinitionID,
        FieldDefinitionDatumUpsertRequest upsertRequest)
    {
        var fieldDefinitionDatum = await dbContext.FieldDefinitionData
            .SingleAsync(x => x.FieldDefinitionID == fieldDefinitionID);

        fieldDefinitionDatum.FieldDefinitionDatumValue = upsertRequest.FieldDefinitionDatumValue;
        fieldDefinitionDatum.FieldDefinitionLabel = upsertRequest.FieldDefinitionLabel;

        await dbContext.SaveChangesAsync();

        return await GetByFieldDefinitionAsDetailAsync(dbContext, fieldDefinitionID);
    }

    public static async Task<FieldDefinitionDatum> CreateIfNotExists(WADNRDbContext dbContext, int fieldDefinitionID)
    {
        var fieldDefinitionDatum = new FieldDefinitionDatum()
        {
            FieldDefinitionID = fieldDefinitionID,
            FieldDefinitionDatumValue = $"<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</p>"
        };

        dbContext.FieldDefinitionData.Add(fieldDefinitionDatum);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(fieldDefinitionDatum).ReloadAsync();
        return fieldDefinitionDatum;
    }
}