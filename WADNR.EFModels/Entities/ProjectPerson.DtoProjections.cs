using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectPersonProjections
{
    public static ProjectPersonItem ToItem(ProjectPerson pp)
    {
        var relType = ProjectPersonRelationshipType.AllLookupDictionary.TryGetValue(pp.ProjectPersonRelationshipTypeID, out var rt)
            ? rt
            : null;

        return new ProjectPersonItem
        {
            ProjectPersonID = pp.ProjectPersonID,
            PersonID = pp.PersonID,
            PersonFullName = pp.Person.FirstName + " " + pp.Person.LastName,
            RelationshipTypeID = pp.ProjectPersonRelationshipTypeID,
            RelationshipTypeName = relType?.ProjectPersonRelationshipTypeDisplayName ?? "(unknown)",
            SortOrder = relType?.SortOrder ?? 0
        };
    }
}
