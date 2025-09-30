//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vArcOnlineRawJsonImportIndex]
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public partial class vArcOnlineRawJsonImportIndex
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vArcOnlineRawJsonImportIndex()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vArcOnlineRawJsonImportIndex(int arcOnlineFinanceApiRawJsonImportID, DateTime createDate, int arcOnlineFinanceApiRawJsonImportTableTypeID, string arcOnlineFinanceApiRawJsonImportTableTypeName, int? bienniumFiscalYear, DateTime? financeApiLastLoadDate, int jsonImportStatusTypeID, string jsonImportStatusTypeName, DateTime? jsonImportDate, long? rawJsonStringLength) : this()
        {
            this.ArcOnlineFinanceApiRawJsonImportID = arcOnlineFinanceApiRawJsonImportID;
            this.CreateDate = createDate;
            this.ArcOnlineFinanceApiRawJsonImportTableTypeID = arcOnlineFinanceApiRawJsonImportTableTypeID;
            this.ArcOnlineFinanceApiRawJsonImportTableTypeName = arcOnlineFinanceApiRawJsonImportTableTypeName;
            this.BienniumFiscalYear = bienniumFiscalYear;
            this.FinanceApiLastLoadDate = financeApiLastLoadDate;
            this.JsonImportStatusTypeID = jsonImportStatusTypeID;
            this.JsonImportStatusTypeName = jsonImportStatusTypeName;
            this.JsonImportDate = jsonImportDate;
            this.RawJsonStringLength = rawJsonStringLength;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public vArcOnlineRawJsonImportIndex(vArcOnlineRawJsonImportIndex vArcOnlineRawJsonImportIndex) : this()
        {
            this.ArcOnlineFinanceApiRawJsonImportID = vArcOnlineRawJsonImportIndex.ArcOnlineFinanceApiRawJsonImportID;
            this.CreateDate = vArcOnlineRawJsonImportIndex.CreateDate;
            this.ArcOnlineFinanceApiRawJsonImportTableTypeID = vArcOnlineRawJsonImportIndex.ArcOnlineFinanceApiRawJsonImportTableTypeID;
            this.ArcOnlineFinanceApiRawJsonImportTableTypeName = vArcOnlineRawJsonImportIndex.ArcOnlineFinanceApiRawJsonImportTableTypeName;
            this.BienniumFiscalYear = vArcOnlineRawJsonImportIndex.BienniumFiscalYear;
            this.FinanceApiLastLoadDate = vArcOnlineRawJsonImportIndex.FinanceApiLastLoadDate;
            this.JsonImportStatusTypeID = vArcOnlineRawJsonImportIndex.JsonImportStatusTypeID;
            this.JsonImportStatusTypeName = vArcOnlineRawJsonImportIndex.JsonImportStatusTypeName;
            this.JsonImportDate = vArcOnlineRawJsonImportIndex.JsonImportDate;
            this.RawJsonStringLength = vArcOnlineRawJsonImportIndex.RawJsonStringLength;
            CallAfterConstructor(vArcOnlineRawJsonImportIndex);
        }

        partial void CallAfterConstructor(vArcOnlineRawJsonImportIndex vArcOnlineRawJsonImportIndex);

        public int ArcOnlineFinanceApiRawJsonImportID { get; set; }
        public DateTime CreateDate { get; set; }
        public int ArcOnlineFinanceApiRawJsonImportTableTypeID { get; set; }
        public string ArcOnlineFinanceApiRawJsonImportTableTypeName { get; set; }
        public int? BienniumFiscalYear { get; set; }
        public DateTime? FinanceApiLastLoadDate { get; set; }
        public int JsonImportStatusTypeID { get; set; }
        public string JsonImportStatusTypeName { get; set; }
        public DateTime? JsonImportDate { get; set; }
        public long? RawJsonStringLength { get; set; }
    }
}