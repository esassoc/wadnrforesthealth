//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Country]
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public static partial class DatabaseContextExtensions
    {
        public static Country GetCountry(this IQueryable<Country> countries, int countryID)
        {
            var country = countries.SingleOrDefault(x => x.CountryID == countryID);
            Check.RequireNotNullThrowNotFound(country, "Country", countryID);
            return country;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteCountry(this IQueryable<Country> countries, List<int> countryIDList)
        {
            if(countryIDList.Any())
            {
                var countriesInSourceCollectionToDelete = countries.Where(x => countryIDList.Contains(x.CountryID));
                foreach (var countryToDelete in countriesInSourceCollectionToDelete.ToList())
                {
                    countryToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteCountry(this IQueryable<Country> countries, ICollection<Country> countriesToDelete)
        {
            if(countriesToDelete.Any())
            {
                var countryIDList = countriesToDelete.Select(x => x.CountryID).ToList();
                var countriesToDeleteFromSourceList = countries.Where(x => countryIDList.Contains(x.CountryID)).ToList();

                foreach (var countryToDelete in countriesToDeleteFromSourceList)
                {
                    countryToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteCountry(this IQueryable<Country> countries, int countryID)
        {
            DeleteCountry(countries, new List<int> { countryID });
        }

        public static void DeleteCountry(this IQueryable<Country> countries, Country countryToDelete)
        {
            DeleteCountry(countries, new List<Country> { countryToDelete });
        }
    }
}