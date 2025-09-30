//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FindYourForesterQuestion]
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
        public static FindYourForesterQuestion GetFindYourForesterQuestion(this IQueryable<FindYourForesterQuestion> findYourForesterQuestions, int findYourForesterQuestionID)
        {
            var findYourForesterQuestion = findYourForesterQuestions.SingleOrDefault(x => x.FindYourForesterQuestionID == findYourForesterQuestionID);
            Check.RequireNotNullThrowNotFound(findYourForesterQuestion, "FindYourForesterQuestion", findYourForesterQuestionID);
            return findYourForesterQuestion;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFindYourForesterQuestion(this IQueryable<FindYourForesterQuestion> findYourForesterQuestions, List<int> findYourForesterQuestionIDList)
        {
            if(findYourForesterQuestionIDList.Any())
            {
                var findYourForesterQuestionsInSourceCollectionToDelete = findYourForesterQuestions.Where(x => findYourForesterQuestionIDList.Contains(x.FindYourForesterQuestionID));
                foreach (var findYourForesterQuestionToDelete in findYourForesterQuestionsInSourceCollectionToDelete.ToList())
                {
                    findYourForesterQuestionToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFindYourForesterQuestion(this IQueryable<FindYourForesterQuestion> findYourForesterQuestions, ICollection<FindYourForesterQuestion> findYourForesterQuestionsToDelete)
        {
            if(findYourForesterQuestionsToDelete.Any())
            {
                var findYourForesterQuestionIDList = findYourForesterQuestionsToDelete.Select(x => x.FindYourForesterQuestionID).ToList();
                var findYourForesterQuestionsToDeleteFromSourceList = findYourForesterQuestions.Where(x => findYourForesterQuestionIDList.Contains(x.FindYourForesterQuestionID)).ToList();

                foreach (var findYourForesterQuestionToDelete in findYourForesterQuestionsToDeleteFromSourceList)
                {
                    findYourForesterQuestionToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFindYourForesterQuestion(this IQueryable<FindYourForesterQuestion> findYourForesterQuestions, int findYourForesterQuestionID)
        {
            DeleteFindYourForesterQuestion(findYourForesterQuestions, new List<int> { findYourForesterQuestionID });
        }

        public static void DeleteFindYourForesterQuestion(this IQueryable<FindYourForesterQuestion> findYourForesterQuestions, FindYourForesterQuestion findYourForesterQuestionToDelete)
        {
            DeleteFindYourForesterQuestion(findYourForesterQuestions, new List<FindYourForesterQuestion> { findYourForesterQuestionToDelete });
        }
    }
}