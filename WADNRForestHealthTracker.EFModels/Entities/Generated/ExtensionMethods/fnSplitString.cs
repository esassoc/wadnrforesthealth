//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[fnSplitString_Result]
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
    public partial class fnSplitString_Result
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public fnSplitString_Result()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public fnSplitString_Result(string splitdata) : this()
        {
            this.splitdata = splitdata;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public fnSplitString_Result(fnSplitString_Result fnSplitString_Result) : this()
        {
            this.splitdata = fnSplitString_Result.splitdata;
            CallAfterConstructor(fnSplitString_Result);
        }

        partial void CallAfterConstructor(fnSplitString_Result fnSplitString_Result);

        public string splitdata { get; set; }
    }
}