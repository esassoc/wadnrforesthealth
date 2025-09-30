//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TrainingVideo]
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
        public static TrainingVideo GetTrainingVideo(this IQueryable<TrainingVideo> trainingVideos, int trainingVideoID)
        {
            var trainingVideo = trainingVideos.SingleOrDefault(x => x.TrainingVideoID == trainingVideoID);
            Check.RequireNotNullThrowNotFound(trainingVideo, "TrainingVideo", trainingVideoID);
            return trainingVideo;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteTrainingVideo(this IQueryable<TrainingVideo> trainingVideos, List<int> trainingVideoIDList)
        {
            if(trainingVideoIDList.Any())
            {
                var trainingVideosInSourceCollectionToDelete = trainingVideos.Where(x => trainingVideoIDList.Contains(x.TrainingVideoID));
                foreach (var trainingVideoToDelete in trainingVideosInSourceCollectionToDelete.ToList())
                {
                    trainingVideoToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteTrainingVideo(this IQueryable<TrainingVideo> trainingVideos, ICollection<TrainingVideo> trainingVideosToDelete)
        {
            if(trainingVideosToDelete.Any())
            {
                var trainingVideoIDList = trainingVideosToDelete.Select(x => x.TrainingVideoID).ToList();
                var trainingVideosToDeleteFromSourceList = trainingVideos.Where(x => trainingVideoIDList.Contains(x.TrainingVideoID)).ToList();

                foreach (var trainingVideoToDelete in trainingVideosToDeleteFromSourceList)
                {
                    trainingVideoToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteTrainingVideo(this IQueryable<TrainingVideo> trainingVideos, int trainingVideoID)
        {
            DeleteTrainingVideo(trainingVideos, new List<int> { trainingVideoID });
        }

        public static void DeleteTrainingVideo(this IQueryable<TrainingVideo> trainingVideos, TrainingVideo trainingVideoToDelete)
        {
            DeleteTrainingVideo(trainingVideos, new List<TrainingVideo> { trainingVideoToDelete });
        }
    }
}