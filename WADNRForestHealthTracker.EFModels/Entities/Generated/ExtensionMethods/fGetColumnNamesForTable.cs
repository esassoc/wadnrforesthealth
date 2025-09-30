//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[fGetColumnNamesForTable_Result]
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
    public partial class fGetColumnNamesForTable_Result
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public fGetColumnNamesForTable_Result()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public fGetColumnNamesForTable_Result(int primaryKey, string columnName) : this()
        {
            this.PrimaryKey = primaryKey;
            this.ColumnName = columnName;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public fGetColumnNamesForTable_Result(fGetColumnNamesForTable_Result fGetColumnNamesForTable_Result) : this()
        {
            this.PrimaryKey = fGetColumnNamesForTable_Result.PrimaryKey;
            this.ColumnName = fGetColumnNamesForTable_Result.ColumnName;
            CallAfterConstructor(fGetColumnNamesForTable_Result);
        }

        partial void CallAfterConstructor(fGetColumnNamesForTable_Result fGetColumnNamesForTable_Result);

        [Key]
        public int PrimaryKey { get; set; }
        public string ColumnName { get; set; }
    }
}