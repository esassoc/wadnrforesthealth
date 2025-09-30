//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FundSourceAllocationExpenditureJsonStage]
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
    // Table [dbo].[FundSourceAllocationExpenditureJsonStage] is NOT multi-tenant, so is attributed as ICanDeleteFull
    [Table("[dbo].[FundSourceAllocationExpenditureJsonStage]")]
    public partial class FundSourceAllocationExpenditureJsonStage : IHavePrimaryKey, ICanDeleteFull
    {
        /// <summary>
        /// Default Constructor; only used by EF
        /// </summary>
        protected FundSourceAllocationExpenditureJsonStage()
        {

        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public FundSourceAllocationExpenditureJsonStage(int fundSourceAllocationExpenditureJsonStageID, int? biennium, int? fiscalMo, int? fiscalAdjMo, int? calYr, string moString, string sourceSystem, string docNo, string docSuffix, DateTime? docDate, string invoiceDesc, DateTime? invoiceDate, int? glAcctNo, string objCd, string objName, string subObjCd, string subObjName, string subSubObjCd, string subSubObjName, string apprnCd, string apprnName, string fundCd, string fundName, string orgCd, string orgName, string progIdxCd, string progIdxName, string progCd, string progName, string subProgCd, string subProgName, string activityCd, string activityName, string subActivityCd, string subActivityName, string projectCd, string projectName, string vendorNo, string vendorName, decimal? expendAccrued) : this()
        {
            this.FundSourceAllocationExpenditureJsonStageID = fundSourceAllocationExpenditureJsonStageID;
            this.Biennium = biennium;
            this.FiscalMo = fiscalMo;
            this.FiscalAdjMo = fiscalAdjMo;
            this.CalYr = calYr;
            this.MoString = moString;
            this.SourceSystem = sourceSystem;
            this.DocNo = docNo;
            this.DocSuffix = docSuffix;
            this.DocDate = docDate;
            this.InvoiceDesc = invoiceDesc;
            this.InvoiceDate = invoiceDate;
            this.GlAcctNo = glAcctNo;
            this.ObjCd = objCd;
            this.ObjName = objName;
            this.SubObjCd = subObjCd;
            this.SubObjName = subObjName;
            this.SubSubObjCd = subSubObjCd;
            this.SubSubObjName = subSubObjName;
            this.ApprnCd = apprnCd;
            this.ApprnName = apprnName;
            this.FundCd = fundCd;
            this.FundName = fundName;
            this.OrgCd = orgCd;
            this.OrgName = orgName;
            this.ProgIdxCd = progIdxCd;
            this.ProgIdxName = progIdxName;
            this.ProgCd = progCd;
            this.ProgName = progName;
            this.SubProgCd = subProgCd;
            this.SubProgName = subProgName;
            this.ActivityCd = activityCd;
            this.ActivityName = activityName;
            this.SubActivityCd = subActivityCd;
            this.SubActivityName = subActivityName;
            this.ProjectCd = projectCd;
            this.ProjectName = projectName;
            this.VendorNo = vendorNo;
            this.VendorName = vendorName;
            this.ExpendAccrued = expendAccrued;
        }



        /// <summary>
        /// Creates a "blank" object of this type and populates primitives with defaults
        /// </summary>
        public static FundSourceAllocationExpenditureJsonStage CreateNewBlank()
        {
            return new FundSourceAllocationExpenditureJsonStage();
        }

        /// <summary>
        /// Does this object have any dependent objects? (If it does have dependent objects, these would need to be deleted before this object could be deleted.)
        /// </summary>
        /// <returns></returns>
        public bool HasDependentObjects()
        {
            return false;
        }

        /// <summary>
        /// Active Dependent type names of this object
        /// </summary>
        public List<string> DependentObjectNames() 
        {
            var dependentObjects = new List<string>();
            
            return dependentObjects.Distinct().ToList();
        }

        /// <summary>
        /// Dependent type names of this entity
        /// </summary>
        public static readonly List<string> DependentEntityTypeNames = new List<string> {typeof(FundSourceAllocationExpenditureJsonStage).Name};


        /// <summary>
        /// Delete just the entity 
        /// </summary>
        public void Delete(DatabaseEntities dbContext)
        {
            dbContext.FundSourceAllocationExpenditureJsonStages.Remove(this);
        }
        
        /// <summary>
        /// Delete entity plus all children
        /// </summary>
        public void DeleteFull(DatabaseEntities dbContext)
        {
            
            Delete(dbContext);
        }

        [Key]
        public int FundSourceAllocationExpenditureJsonStageID { get; set; }
        public int? Biennium { get; set; }
        public int? FiscalMo { get; set; }
        public int? FiscalAdjMo { get; set; }
        public int? CalYr { get; set; }
        public string MoString { get; set; }
        public string SourceSystem { get; set; }
        public string DocNo { get; set; }
        public string DocSuffix { get; set; }
        public DateTime? DocDate { get; set; }
        public string InvoiceDesc { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? GlAcctNo { get; set; }
        public string ObjCd { get; set; }
        public string ObjName { get; set; }
        public string SubObjCd { get; set; }
        public string SubObjName { get; set; }
        public string SubSubObjCd { get; set; }
        public string SubSubObjName { get; set; }
        public string ApprnCd { get; set; }
        public string ApprnName { get; set; }
        public string FundCd { get; set; }
        public string FundName { get; set; }
        public string OrgCd { get; set; }
        public string OrgName { get; set; }
        public string ProgIdxCd { get; set; }
        public string ProgIdxName { get; set; }
        public string ProgCd { get; set; }
        public string ProgName { get; set; }
        public string SubProgCd { get; set; }
        public string SubProgName { get; set; }
        public string ActivityCd { get; set; }
        public string ActivityName { get; set; }
        public string SubActivityCd { get; set; }
        public string SubActivityName { get; set; }
        public string ProjectCd { get; set; }
        public string ProjectName { get; set; }
        public string VendorNo { get; set; }
        public string VendorName { get; set; }
        public decimal? ExpendAccrued { get; set; }
        [NotMapped]
        public int PrimaryKey { get { return FundSourceAllocationExpenditureJsonStageID; } set { FundSourceAllocationExpenditureJsonStageID = value; } }



        public static class FieldLengths
        {
            public const int MoString = 20;
            public const int SourceSystem = 50;
            public const int DocNo = 200;
            public const int DocSuffix = 10;
            public const int ObjCd = 20;
            public const int SubObjCd = 20;
            public const int SubSubObjCd = 20;
            public const int ApprnCd = 20;
            public const int FundCd = 20;
            public const int OrgCd = 20;
            public const int ProgIdxCd = 20;
            public const int ProgCd = 20;
            public const int SubProgCd = 20;
            public const int ActivityCd = 20;
            public const int SubActivityCd = 20;
            public const int ProjectCd = 20;
            public const int VendorNo = 100;
        }
    }
}