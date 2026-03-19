/*-----------------------------------------------------------------------
<copyright file="FindYourForester.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
Copyright (c) Tahoe Regional Planning Agency and Environmental Science Associates. All rights reserved.
<author>Environmental Science Associates</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/

using System.Collections.Generic;

namespace ProjectFirma.Web.Views.FindYourForester
{
    public abstract class FindYourForester : LtInfo.Common.Mvc.TypedWebViewPage<FindYourForesterViewData>
    {
    }

    public class FindYourForesterSimple
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<ForesterWorkUnitSimple> ForesterWorkUnits { get; set; }

        public FindYourForesterSimple(double latitude, double longitude, List<ForesterWorkUnitSimple> foresterWorkUnits)
        {
            Latitude = latitude;
            Longitude = longitude;
            ForesterWorkUnits = foresterWorkUnits;
        }
    }

    public class ForesterWorkUnitSimple
    {
        public int ForesterWorkUnitID { get; set; }
        public int ForesterRoleID { get; set; }
        public string ForesterRoleDisplayName { get; set; }
        public int? PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ForesterWorkUnitName { get; set; }

        public ForesterWorkUnitSimple(int foresterWorkUnitID, int foresterRoleID, string foresterRoleDisplayName,
            int? personID, string firstName, string lastName, string email, string phone, string foresterWorkUnitName)
        {
            ForesterWorkUnitID = foresterWorkUnitID;
            ForesterRoleID = foresterRoleID;
            ForesterRoleDisplayName = foresterRoleDisplayName;
            PersonID = personID;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            ForesterWorkUnitName = foresterWorkUnitName;
        }
    }
}
