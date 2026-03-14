/*-----------------------------------------------------------------------
<copyright file="FindYourForesterViewData.cs" company="Tahoe Regional Planning Agency and Environmental Science Associates">
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
using System.Linq;
using System.Web;
using ProjectFirma.Web.Common;
using ProjectFirma.Web.Controllers;
using ProjectFirma.Web.Models;

namespace ProjectFirma.Web.Views.FindYourForester
{
    public class FindYourForesterViewData : FirmaViewData
    {
        public readonly MapInitJson MapInitJson;

        public List<FindYourForesterQuestion> RootQuestions { get; }
        public string GeocodeAddressUrl { get; set; }
        public Dictionary<int, string> ForesterRoleDefinitions { get; }

        public FindYourForesterViewData(Person currentPerson, MapInitJson mapInitJson, Models.FirmaPage firmaPage,
            List<FindYourForesterQuestion> findYourForesterQuestions, Dictionary<int, string> foresterRoleDefinitions) : base(currentPerson, firmaPage)
        {
            PageTitle = "Find Your Forester";
            MapInitJson = mapInitJson;
            RootQuestions = findYourForesterQuestions;
            GeocodeAddressUrl = SitkaRoute<ResultsController>.BuildUrlFromExpression(t => t.GeocodeAddress(null));
            ForesterRoleDefinitions = foresterRoleDefinitions;
        }

    }
}
