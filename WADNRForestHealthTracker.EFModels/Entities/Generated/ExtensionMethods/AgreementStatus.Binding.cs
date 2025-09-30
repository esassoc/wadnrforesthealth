//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AgreementStatus]
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
    // Table [dbo].[AgreementStatus] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[AgreementStatus]")]
    public partial class AgreementStatus : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected AgreementStatus()
        {
            this.Agreements = new HashSet<Agreement>();
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public AgreementStatus(int agreementStatusID, string agreementStatusName) : this()
        {
            this.AgreementStatusID = agreementStatusID;
            this.AgreementStatusName = agreementStatusName;
        }

        /// <summary>
        /// Constructor for building a new object with MinimalConstructor required fields in preparation for insert into database
        /// </summary>
        public AgreementStatus(string agreementStatusName) : this()
        {
            // Mark this as a new object by setting primary key with special value
            this.AgreementStatusID = ModelObjectHelpers.MakeNextUnsavedPrimaryKeyValue();
            
            this.AgreementStatusName = agreementStatusName;
        }


        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static AgreementStatus CreateNewBlank()
        {
            return new AgreementStatus(default(string));
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return Agreements.Any();
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            if(Agreements.Any())
            {
                dependentObjects.Add(typeof(Agreement).Name);
            }
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(AgreementStatus).Name, typeof(Agreement).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.AgreementStatuses.Remove(this);
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

            foreach(var x in Agreements.ToList())
            {
                x.DeleteFull(dbContext);
            }
        }

        [Key]
        public int AgreementStatusID { get; set; }
        public string AgreementStatusName { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return AgreementStatusID; } set { AgreementStatusID = value; } }

        public virtual ICollection<Agreement> Agreements { get; set; }

        public static class FieldLengths
        {
            public const int AgreementStatusName = 100;
        }
    }
}