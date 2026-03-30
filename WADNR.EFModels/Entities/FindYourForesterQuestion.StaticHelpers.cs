using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects.FindYourForester;

namespace WADNR.EFModels.Entities;

public static class FindYourForesterQuestions
{
    public static async Task<List<FindYourForesterQuestionTreeNode>> ListAsTreeAsync(WADNRDbContext dbContext)
    {
        // Load flat questions using projection
        var allNodes = await dbContext.FindYourForesterQuestions
            .AsNoTracking()
            .Select(FindYourForesterQuestionProjections.AsTreeNode)
            .ToListAsync();

        // Load parent mappings separately (ParentQuestionID is not on the DTO)
        var parentMap = await dbContext.FindYourForesterQuestions
            .AsNoTracking()
            .Where(x => x.ParentQuestionID.HasValue)
            .Select(x => new { x.FindYourForesterQuestionID, x.ParentQuestionID })
            .ToDictionaryAsync(x => x.FindYourForesterQuestionID, x => x.ParentQuestionID!.Value);

        // Resolve ForesterRole display names client-side
        foreach (var node in allNodes)
        {
            if (node.ForesterRoleID.HasValue && ForesterRole.AllLookupDictionary.TryGetValue(node.ForesterRoleID.Value, out var role))
            {
                node.ForesterRoleDisplayName = role.ForesterRoleDisplayName;
                node.ForesterRoleName = role.ForesterRoleName;
            }
        }

        // Build tree in memory
        var nodeMap = allNodes.ToDictionary(n => n.FindYourForesterQuestionID);

        foreach (var (childID, parentID) in parentMap)
        {
            if (nodeMap.TryGetValue(parentID, out var parent))
            {
                parent.Children.Add(nodeMap[childID]);
            }
        }

        // Return only root nodes (those not in parentMap)
        return allNodes
            .Where(n => !parentMap.ContainsKey(n.FindYourForesterQuestionID))
            .ToList();
    }
}
