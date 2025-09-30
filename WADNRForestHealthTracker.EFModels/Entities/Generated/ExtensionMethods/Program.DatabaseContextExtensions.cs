//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Program]
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
        public static Program GetProgram(this IQueryable<Program> programs, int programID)
        {
            var program = programs.SingleOrDefault(x => x.ProgramID == programID);
            Check.RequireNotNullThrowNotFound(program, "Program", programID);
            return program;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteProgram(this IQueryable<Program> programs, List<int> programIDList)
        {
            if(programIDList.Any())
            {
                var programsInSourceCollectionToDelete = programs.Where(x => programIDList.Contains(x.ProgramID));
                foreach (var programToDelete in programsInSourceCollectionToDelete.ToList())
                {
                    programToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteProgram(this IQueryable<Program> programs, ICollection<Program> programsToDelete)
        {
            if(programsToDelete.Any())
            {
                var programIDList = programsToDelete.Select(x => x.ProgramID).ToList();
                var programsToDeleteFromSourceList = programs.Where(x => programIDList.Contains(x.ProgramID)).ToList();

                foreach (var programToDelete in programsToDeleteFromSourceList)
                {
                    programToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteProgram(this IQueryable<Program> programs, int programID)
        {
            DeleteProgram(programs, new List<int> { programID });
        }

        public static void DeleteProgram(this IQueryable<Program> programs, Program programToDelete)
        {
            DeleteProgram(programs, new List<Program> { programToDelete });
        }
    }
}