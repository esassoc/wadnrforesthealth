using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using WADNR.API.Services.Attributes;
using WADNR.EFModels.Entities;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace WADNR.API.Services.Middleware;

/// <summary>
/// Middleware that checks each <see cref="EntityNotFoundAttribute"/> on route actions
/// and adjusts the response status code to <see cref="HttpStatusCode.NotFound"/> when
/// the entity is not found
/// </summary>
public class EntityNotFoundMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, WADNRDbContext dbContext)
    {
        var endpoint = context.GetEndpoint();
        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        var notFoundAttributes = actionDescriptor?.MethodInfo.GetCustomAttributes<EntityNotFoundAttribute>().ToList();

        if (notFoundAttributes != null && notFoundAttributes.Any())
        {
            foreach (var entityNotFoundAttribute in notFoundAttributes)
            {
                // Get the entity ID from the route parameters
                if (context.Request.RouteValues.TryGetValue(entityNotFoundAttribute.PKStringInRoute, out var idObj)
                    && int.TryParse(idObj.ToString(), out var id))
                {
                    // Check if the entity exists in the database
                    var entityType = entityNotFoundAttribute.EntityType;
                    var entity = await dbContext.FindAsync(entityType, id);
                    if (entity == null)
                    {
                        // Return a 404 response if the entity doesn't exist
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        return;
                    }
                }
            }
        }

        // Call the next middleware in the pipeline
        await next(context);
    }

}