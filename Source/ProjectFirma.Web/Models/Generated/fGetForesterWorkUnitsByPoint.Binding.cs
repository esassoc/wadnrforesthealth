//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[fGetForesterWorkUnitsByPoint_Result]
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
    public partial class fGetForesterWorkUnitsByPoint_Result
    {
        /// <summary>
        /// Needed by ModelBinder
        /// </summary>
        public fGetForesterWorkUnitsByPoint_Result()
        {
        }

        /// <summary>
        /// Constructor for building a new object with MaximalConstructor required fields in preparation for insert into database
        /// </summary>
        public fGetForesterWorkUnitsByPoint_Result(int foresterWorkUnitID, int foresterRoleID, string foresterRoleDisplayName, int? personID, string firstName, string lastName, string email, string phone, string foresterWorkUnitName, int sortOrder) : this()
        {
            this.ForesterWorkUnitID = foresterWorkUnitID;
            this.ForesterRoleID = foresterRoleID;
            this.ForesterRoleDisplayName = foresterRoleDisplayName;
            this.PersonID = personID;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Phone = phone;
            this.ForesterWorkUnitName = foresterWorkUnitName;
            this.SortOrder = sortOrder;
        }

        /// <summary>
        /// Constructor for building a new simple object with the POCO class
        /// </summary>
        public fGetForesterWorkUnitsByPoint_Result(fGetForesterWorkUnitsByPoint_Result fGetForesterWorkUnitsByPoint_Result) : this()
        {
            this.ForesterWorkUnitID = fGetForesterWorkUnitsByPoint_Result.ForesterWorkUnitID;
            this.ForesterRoleID = fGetForesterWorkUnitsByPoint_Result.ForesterRoleID;
            this.ForesterRoleDisplayName = fGetForesterWorkUnitsByPoint_Result.ForesterRoleDisplayName;
            this.PersonID = fGetForesterWorkUnitsByPoint_Result.PersonID;
            this.FirstName = fGetForesterWorkUnitsByPoint_Result.FirstName;
            this.LastName = fGetForesterWorkUnitsByPoint_Result.LastName;
            this.Email = fGetForesterWorkUnitsByPoint_Result.Email;
            this.Phone = fGetForesterWorkUnitsByPoint_Result.Phone;
            this.ForesterWorkUnitName = fGetForesterWorkUnitsByPoint_Result.ForesterWorkUnitName;
            this.SortOrder = fGetForesterWorkUnitsByPoint_Result.SortOrder;
            CallAfterConstructor(fGetForesterWorkUnitsByPoint_Result);
        }

        partial void CallAfterConstructor(fGetForesterWorkUnitsByPoint_Result fGetForesterWorkUnitsByPoint_Result);

        public int ForesterWorkUnitID { get; set; }
        public int ForesterRoleID { get; set; }
        public string ForesterRoleDisplayName { get; set; }
        public int? PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ForesterWorkUnitName { get; set; }
        public int SortOrder { get; set; }
    }
}