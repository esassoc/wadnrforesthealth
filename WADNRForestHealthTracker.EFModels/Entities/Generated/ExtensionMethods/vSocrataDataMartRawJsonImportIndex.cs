//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[vSocrataDataMartRawJsonImportIndex]
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
    public partial class vSocrataDataMartRawJsonImportIndex
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public vSocrataDataMartRawJsonImportIndex()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public vSocrataDataMartRawJsonImportIndex(int socrataDataMartRawJsonImportID, DateTime createDate, int socrataDataMartRawJsonImportTableTypeID, string socrataDataMartRawJsonImportTableTypeName, int? bienniumFiscalYear, DateTime? financeApiLastLoadDate, int jsonImportStatusTypeID, string jsonImportStatusTypeName, DateTime? jsonImportDate, long? rawJsonStringLength) : this()
        {
            this.SocrataDataMartRawJsonImportID = socrataDataMartRawJsonImportID;
            this.CreateDate = createDate;
            this.SocrataDataMartRawJsonImportTableTypeID = socrataDataMartRawJsonImportTableTypeID;
            this.SocrataDataMartRawJsonImportTableTypeName = socrataDataMartRawJsonImportTableTypeName;
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
        public vSocrataDataMartRawJsonImportIndex(vSocrataDataMartRawJsonImportIndex vSocrataDataMartRawJsonImportIndex) : this()
        {
            this.SocrataDataMartRawJsonImportID = vSocrataDataMartRawJsonImportIndex.SocrataDataMartRawJsonImportID;
            this.CreateDate = vSocrataDataMartRawJsonImportIndex.CreateDate;
            this.SocrataDataMartRawJsonImportTableTypeID = vSocrataDataMartRawJsonImportIndex.SocrataDataMartRawJsonImportTableTypeID;
            this.SocrataDataMartRawJsonImportTableTypeName = vSocrataDataMartRawJsonImportIndex.SocrataDataMartRawJsonImportTableTypeName;
            this.BienniumFiscalYear = vSocrataDataMartRawJsonImportIndex.BienniumFiscalYear;
            this.FinanceApiLastLoadDate = vSocrataDataMartRawJsonImportIndex.FinanceApiLastLoadDate;
            this.JsonImportStatusTypeID = vSocrataDataMartRawJsonImportIndex.JsonImportStatusTypeID;
            this.JsonImportStatusTypeName = vSocrataDataMartRawJsonImportIndex.JsonImportStatusTypeName;
            this.JsonImportDate = vSocrataDataMartRawJsonImportIndex.JsonImportDate;
            this.RawJsonStringLength = vSocrataDataMartRawJsonImportIndex.RawJsonStringLength;
            CallAfterConstructor(vSocrataDataMartRawJsonImportIndex);
        }

        partial void CallAfterConstructor(vSocrataDataMartRawJsonImportIndex vSocrataDataMartRawJsonImportIndex);

        public int SocrataDataMartRawJsonImportID { get; set; }
        public DateTime CreateDate { get; set; }
        public int SocrataDataMartRawJsonImportTableTypeID { get; set; }
        public string SocrataDataMartRawJsonImportTableTypeName { get; set; }
        public int? BienniumFiscalYear { get; set; }
        public DateTime? FinanceApiLastLoadDate { get; set; }
        public int JsonImportStatusTypeID { get; set; }
        public string JsonImportStatusTypeName { get; set; }
        public DateTime? JsonImportDate { get; set; }
        public long? RawJsonStringLength { get; set; }
    }
}