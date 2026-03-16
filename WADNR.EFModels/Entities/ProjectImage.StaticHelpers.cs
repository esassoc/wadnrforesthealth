using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectImages
{
    public static async Task<List<ProjectImageGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var images = await dbContext.ProjectImages
            .AsNoTracking()
            .Where(x => x.ProjectID == projectID)
            .OrderByDescending(x => x.IsKeyPhoto)
            .ThenBy(x => x.Caption)
            .Select(ProjectImageProjections.AsGridRow)
            .ToListAsync();

        // Resolve timing display names client-side
        foreach (var image in images)
        {
            if (image.ProjectImageTimingID.HasValue &&
                ProjectImageTiming.AllLookupDictionary.TryGetValue(image.ProjectImageTimingID.Value, out var timing))
            {
                image.ProjectImageTimingDisplayName = timing.ProjectImageTimingDisplayName;
            }
        }

        return images;
    }

    public static async Task<ProjectImageDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int projectImageID)
    {
        var detail = await dbContext.ProjectImages
            .AsNoTracking()
            .Where(x => x.ProjectImageID == projectImageID)
            .Select(ProjectImageProjections.AsDetail)
            .SingleOrDefaultAsync();

        // Resolve timing display name client-side
        if (detail?.ProjectImageTimingID != null &&
            ProjectImageTiming.AllLookupDictionary.TryGetValue(detail.ProjectImageTimingID.Value, out var timing))
        {
            detail.ProjectImageTimingDisplayName = timing.ProjectImageTimingDisplayName;
        }

        return detail;
    }

    public static async Task<ProjectImage?> GetByIDWithTrackingAsync(WADNRDbContext dbContext, int projectImageID)
    {
        return await dbContext.ProjectImages.FindAsync(projectImageID);
    }

    public static async Task<ProjectImage?> GetByIDWithFileResourceAsync(WADNRDbContext dbContext, int projectImageID)
    {
        return await dbContext.ProjectImages
            .Include(pi => pi.FileResource)
            .FirstOrDefaultAsync(pi => pi.ProjectImageID == projectImageID);
    }

    public static async Task<ProjectImage> CreateAsync(
        WADNRDbContext dbContext,
        int projectID,
        int fileResourceID,
        string caption,
        string credit,
        int? projectImageTimingID,
        bool excludeFromFactSheet)
    {
        // Check if this should be the key photo (first photo for the project)
        var hasExistingPhotos = await dbContext.ProjectImages.AnyAsync(x => x.ProjectID == projectID);

        var projectImage = new ProjectImage
        {
            ProjectID = projectID,
            FileResourceID = fileResourceID,
            Caption = caption,
            Credit = credit,
            ProjectImageTimingID = projectImageTimingID,
            ExcludeFromFactSheet = excludeFromFactSheet,
            IsKeyPhoto = !hasExistingPhotos // First photo becomes key photo
        };

        dbContext.ProjectImages.Add(projectImage);
        await dbContext.SaveChangesAsync();
        return projectImage;
    }

    public static async Task UpdateAsync(
        WADNRDbContext dbContext,
        ProjectImage projectImage,
        ProjectImageUpsertRequest request)
    {
        projectImage.Caption = request.Caption;
        projectImage.Credit = request.Credit;
        projectImage.ProjectImageTimingID = request.ProjectImageTimingID;
        projectImage.ExcludeFromFactSheet = request.ExcludeFromFactSheet;

        await dbContext.SaveChangesAsync();
    }

    public static async Task<Guid> DeleteAsync(WADNRDbContext dbContext, ProjectImage projectImage)
    {
        var projectID = projectImage.ProjectID;
        var wasKeyPhoto = projectImage.IsKeyPhoto;
        var fileResourceGuid = projectImage.FileResource.FileResourceGUID;

        // Remove the project image
        dbContext.ProjectImages.Remove(projectImage);

        // Also remove the file resource
        dbContext.FileResources.Remove(projectImage.FileResource);

        await dbContext.SaveChangesAsync();

        // If deleted photo was key photo, set another photo as key
        if (wasKeyPhoto)
        {
            var nextPhoto = await dbContext.ProjectImages
                .Where(x => x.ProjectID == projectID)
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

    public static async Task SetKeyPhotoAsync(WADNRDbContext dbContext, int projectImageID)
    {
        var projectImage = await dbContext.ProjectImages
            .FirstOrDefaultAsync(x => x.ProjectImageID == projectImageID);

        if (projectImage == null) return;

        // Clear key photo flag from all other images for this project
        var otherImages = await dbContext.ProjectImages
            .Where(x => x.ProjectID == projectImage.ProjectID && x.ProjectImageID != projectImageID)
            .ToListAsync();

        foreach (var img in otherImages)
        {
            img.IsKeyPhoto = false;
        }

        // Set this image as key photo
        projectImage.IsKeyPhoto = true;

        await dbContext.SaveChangesAsync();
    }

    public static List<ProjectImageTimingLookupItem> ListTimingAsLookupItem()
    {
        return ProjectImageTiming.All
            .OrderBy(x => x.SortOrder)
            .Select(x => new ProjectImageTimingLookupItem
            {
                ProjectImageTimingID = x.ProjectImageTimingID,
                DisplayName = x.ProjectImageTimingDisplayName
            })
            .ToList();
    }
}
