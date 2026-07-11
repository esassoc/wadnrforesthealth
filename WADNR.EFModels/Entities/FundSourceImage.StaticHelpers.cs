using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class FundSourceImages
{
    public static async Task<List<FundSourceImageGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext, int fundSourceID)
    {
        return await dbContext.FundSourceImages
            .AsNoTracking()
            .Where(x => x.FundSourceID == fundSourceID)
            .OrderByDescending(x => x.IsKeyPhoto)
            .ThenBy(x => x.Caption)
            .Select(FundSourceImageProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<FundSourceImageDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int fundSourceImageID)
    {
        return await dbContext.FundSourceImages
            .AsNoTracking()
            .Where(x => x.FundSourceImageID == fundSourceImageID)
            .Select(FundSourceImageProjections.AsDetail)
            .SingleOrDefaultAsync();
    }

    public static async Task<FundSourceImage?> GetByIDWithTrackingAsync(WADNRDbContext dbContext, int fundSourceImageID)
    {
        return await dbContext.FundSourceImages.FindAsync(fundSourceImageID);
    }

    public static async Task<FundSourceImage?> GetByIDWithFileResourceAsync(WADNRDbContext dbContext, int fundSourceImageID)
    {
        return await dbContext.FundSourceImages
            .Include(fsi => fsi.FileResource)
            .FirstOrDefaultAsync(fsi => fsi.FundSourceImageID == fundSourceImageID);
    }

    public static async Task<FundSourceImage> CreateAsync(
        WADNRDbContext dbContext,
        int fundSourceID,
        int fileResourceID,
        string caption,
        string credit)
    {
        // First photo for the fund source becomes the key photo
        var hasExistingPhotos = await dbContext.FundSourceImages.AnyAsync(x => x.FundSourceID == fundSourceID);

        var fundSourceImage = new FundSourceImage
        {
            FundSourceID = fundSourceID,
            FileResourceID = fileResourceID,
            Caption = caption,
            Credit = credit,
            IsKeyPhoto = !hasExistingPhotos
        };

        dbContext.FundSourceImages.Add(fundSourceImage);
        await dbContext.SaveChangesAsync();
        return fundSourceImage;
    }

    public static async Task UpdateAsync(
        WADNRDbContext dbContext,
        FundSourceImage fundSourceImage,
        FundSourceImageUpsertRequest request)
    {
        fundSourceImage.Caption = request.Caption;
        fundSourceImage.Credit = request.Credit;

        await dbContext.SaveChangesAsync();
    }

    public static async Task<Guid> DeleteAsync(WADNRDbContext dbContext, FundSourceImage fundSourceImage)
    {
        var fundSourceID = fundSourceImage.FundSourceID;
        var wasKeyPhoto = fundSourceImage.IsKeyPhoto;
        var fileResourceGuid = fundSourceImage.FileResource.FileResourceGUID;

        dbContext.FundSourceImages.Remove(fundSourceImage);
        dbContext.FileResources.Remove(fundSourceImage.FileResource);
        await dbContext.SaveChangesAsync();

        // If the deleted photo was the key photo, promote another to key photo
        if (wasKeyPhoto)
        {
            var nextPhoto = await dbContext.FundSourceImages
                .Where(x => x.FundSourceID == fundSourceID)
                .OrderBy(x => x.Caption)
                .FirstOrDefaultAsync();

            if (nextPhoto != null)
            {
                nextPhoto.IsKeyPhoto = true;
                await dbContext.SaveChangesAsync();
            }
        }

        return fileResourceGuid;
    }

    public static async Task SetKeyPhotoAsync(WADNRDbContext dbContext, int fundSourceImageID)
    {
        var fundSourceImage = await dbContext.FundSourceImages
            .FirstOrDefaultAsync(x => x.FundSourceImageID == fundSourceImageID);

        if (fundSourceImage == null) return;

        var otherImages = await dbContext.FundSourceImages
            .Where(x => x.FundSourceID == fundSourceImage.FundSourceID && x.FundSourceImageID != fundSourceImageID)
            .ToListAsync();

        foreach (var img in otherImages)
        {
            img.IsKeyPhoto = false;
        }

        fundSourceImage.IsKeyPhoto = true;

        await dbContext.SaveChangesAsync();
    }
}
