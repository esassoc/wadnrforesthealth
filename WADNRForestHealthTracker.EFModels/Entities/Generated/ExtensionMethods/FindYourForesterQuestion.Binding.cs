//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FindYourForesterQuestion]
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    // Table [dbo].[FindYourForesterQuestion] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FindYourForesterQuestion]")]
    public partial class FindYourForesterQuestion : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FindYourForesterQuestion()
        {
            this.FindYourForesterQuestionsWhereYouAreTheParentQuestion = new HashSet<FindYourForesterQuestion>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FindYourForesterQuestion(int findYourForesterQuestionID, string questionText, int? parentQuestionID, int? foresterRoleID, string resultsBonusContent) : this()
        {
            this.FindYourForesterQuestionID = findYourForesterQuestionID;
            this.QuestionText = questionText;
            this.ParentQuestionID = parentQuestionID;
            this.ForesterRoleID = foresterRoleID;
            this.ResultsBonusContent = resultsBonusContent;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public FindYourForesterQuestion(string questionText) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.FindYourForesterQuestionID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.QuestionText = questionText;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FindYourForesterQuestion CreateNewBlank()
        {
            return new FindYourForesterQuestion(default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return FindYourForesterQuestionsWhereYouAreTheParentQuestion.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(FindYourForesterQuestionsWhereYouAreTheParentQuestion.Any())
            {
                dependentObjects.Add(typeof(FindYourForesterQuestion).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FindYourForesterQuestion).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FindYourForesterQuestions.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            DeleteChildren(dbContext);
            Delete(dbContext);
        }
        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public void DeleteChildren(DatabaseEntities dbContext)
        {

            foreach(var x in FindYourForesterQuestionsWhereYouAreTheParentQuestion.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int FindYourForesterQuestionID { get; set; }
        public string QuestionText { get; set; }
        public int? ParentQuestionID { get; set; }
        public int? ForesterRoleID { get; set; }
        public string ResultsBonusContent { get; set; }
        [NotMapped]
        public HtmlString ResultsBonusContentHtmlString
        { 
            get { return ResultsBonusContent == null ? null : new HtmlString(ResultsBonusContent); }
            set { ResultsBonusContent = value?.ToString(); }
        }
        [NotMapped]
        public int PrimaryKey { get { return FindYourForesterQuestionID; } set { FindYourForesterQuestionID = value; } }

        public virtual ICollection<FindYourForesterQuestion> FindYourForesterQuestionsWhereYouAreTheParentQuestion { get; set; }
        public virtual FindYourForesterQuestion ParentQuestion { get; set; }
        public ForesterRole ForesterRole { get { return ForesterRoleID.HasValue ? ForesterRole.AllLookupDictionary[ForesterRoleID.Value] : null; } }

        public static class FieldLengths
        {
            public const int QuestionText = 500;
        }
    }
}