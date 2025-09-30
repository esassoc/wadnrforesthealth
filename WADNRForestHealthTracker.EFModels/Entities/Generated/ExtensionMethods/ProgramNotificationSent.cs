//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProgramNotificationSent]
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
    // Table [dbo].[ProgramNotificationSent] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[ProgramNotificationSent]")]
    public partial class ProgramNotificationSent : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected ProgramNotificationSent()
        {
            this.ProgramNotificationSentProjects = new HashSet<ProgramNotificationSentProject>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProgramNotificationSent(int programNotificationSentID, int programNotificationConfigurationID, int sentToPersonID, DateTime programNotificationSentDate) : this()
        {
            this.ProgramNotificationSentID = programNotificationSentID;
            this.ProgramNotificationConfigurationID = programNotificationConfigurationID;
            this.SentToPersonID = sentToPersonID;
            this.ProgramNotificationSentDate = programNotificationSentDate;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public ProgramNotificationSent(int programNotificationConfigurationID, int sentToPersonID, DateTime programNotificationSentDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProgramNotificationSentID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.ProgramNotificationConfigurationID = programNotificationConfigurationID;
            this.SentToPersonID = sentToPersonID;
            this.ProgramNotificationSentDate = programNotificationSentDate;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields, using objects whenever possible
        /// </summary>
        public ProgramNotificationSent(ProgramNotificationConfiguration programNotificationConfiguration, Person sentToPerson, DateTime programNotificationSentDate) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.ProgramNotificationSentID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            this.ProgramNotificationConfigurationID = programNotificationConfiguration.ProgramNotificationConfigurationID;
            this.ProgramNotificationConfiguration = programNotificationConfiguration;
            programNotificationConfiguration.ProgramNotificationSents.Add(this);
            this.SentToPersonID = sentToPerson.PersonID;
            this.SentToPerson = sentToPerson;
            sentToPerson.ProgramNotificationSentsWhereYouAreTheSentToPerson.Add(this);
            this.ProgramNotificationSentDate = programNotificationSentDate;
        }

        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static ProgramNotificationSent CreateNewBlank(ProgramNotificationConfiguration programNotificationConfiguration, Person sentToPerson)
        {
            return new ProgramNotificationSent(programNotificationConfiguration, sentToPerson, default(DateTime));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return ProgramNotificationSentProjects.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(ProgramNotificationSentProjects.Any())
            {
                dependentObjects.Add(typeof(ProgramNotificationSentProject).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(ProgramNotificationSent).Name, typeof(ProgramNotificationSentProject).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.ProgramNotificationSents.Remove(this);
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

            foreach(var x in ProgramNotificationSentProjects.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int ProgramNotificationSentID { get; set; }
        public int ProgramNotificationConfigurationID { get; set; }
        public int SentToPersonID { get; set; }
        public DateTime ProgramNotificationSentDate { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return ProgramNotificationSentID; } set { ProgramNotificationSentID = value; } }

        public virtual ICollection<ProgramNotificationSentProject> ProgramNotificationSentProjects { get; set; }
        public virtual ProgramNotificationConfiguration ProgramNotificationConfiguration { get; set; }
        public virtual Person SentToPerson { get; set; }

        public static class FieldLengths
        {

        }
    }
}