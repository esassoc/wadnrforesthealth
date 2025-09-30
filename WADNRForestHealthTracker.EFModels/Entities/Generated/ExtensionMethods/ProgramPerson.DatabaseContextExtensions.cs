//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ProgramPerson]
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
        public static ProgramPerson GetProgramPerson(this IQueryable<ProgramPerson> programPeople, int programPersonID)
        {
            var programPerson = programPeople.SingleOrDefault(x => x.ProgramPersonID == programPersonID);
            Check.RequireNotNullThrowNotFound(programPerson, "ProgramPerson", programPersonID);
            return programPerson;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProgramPerson(this IQueryable<ProgramPerson> programPeople, List<int> programPersonIDList)
        {
            if(programPersonIDList.Any())
            {
                var programPeopleInSourceCollectionToDelete = programPeople.Where(x => programPersonIDList.Contains(x.ProgramPersonID));
                foreach (var programPersonToDelete in programPeopleInSourceCollectionToDelete.ToList())
                {
                    programPersonToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProgramPerson(this IQueryable<ProgramPerson> programPeople, ICollection<ProgramPerson> programPeopleToDelete)
        {
            if(programPeopleToDelete.Any())
            {
                var programPersonIDList = programPeopleToDelete.Select(x => x.ProgramPersonID).ToList();
                var programPeopleToDeleteFromSourceList = programPeople.Where(x => programPersonIDList.Contains(x.ProgramPersonID)).ToList();

                foreach (var programPersonToDelete in programPeopleToDeleteFromSourceList)
                {
                    programPersonToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProgramPerson(this IQueryable<ProgramPerson> programPeople, int programPersonID)
        {
            DeleteProgramPerson(programPeople, new List<int> { programPersonID });
        }

        public static void DeleteProgramPerson(this IQueryable<ProgramPerson> programPeople, ProgramPerson programPersonToDelete)
        {
            DeleteProgramPerson(programPeople, new List<ProgramPerson> { programPersonToDelete });
        }
    }
}