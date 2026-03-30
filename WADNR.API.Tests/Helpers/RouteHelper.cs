using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace WADNR.API.Tests.Helpers;

/// <summary>
/// Generates type-safe route URLs from controller method expressions.
/// </summary>
public static class RouteHelper
{
    public static string GetRouteTemplateFor(Type controllerType, MethodInfo methodInfo)
    {
        // Extract controller route
        var controllerRouteAttr = controllerType.GetCustomAttribute<RouteAttribute>();
        var controllerRoute = controllerRouteAttr?.Template ?? "";

        // Replace [controller] placeholder with actual controller name
        if (controllerRoute.Contains("[controller]"))
        {
            var controllerName = controllerType.Name;
            if (controllerName.EndsWith("Controller"))
            {
                controllerName = controllerName[..^10]; // Remove "Controller" suffix
            }
            controllerRoute = controllerRoute.Replace("[controller]", controllerName);
        }

        // Extract action route from HTTP method attribute
        var actionRouteAttr = methodInfo.GetCustomAttributes().FirstOrDefault(attr =>
            attr is RouteAttribute ||
            attr is HttpGetAttribute ||
            attr is HttpPostAttribute ||
            attr is HttpPutAttribute ||
            attr is HttpDeleteAttribute ||
            attr is HttpPatchAttribute);

        var actionRoute = actionRouteAttr switch
        {
            RouteAttribute routeAttr => routeAttr.Template,
            HttpGetAttribute getAttr => getAttr.Template,
            HttpPostAttribute postAttr => postAttr.Template,
            HttpPutAttribute putAttr => putAttr.Template,
            HttpDeleteAttribute deleteAttr => deleteAttr.Template,
            HttpPatchAttribute patchAttr => patchAttr.Template,
            _ => null
        };

        // Combine routes
        var fullRouteTemplate = string.IsNullOrEmpty(actionRoute)
            ? controllerRoute
            : $"{controllerRoute}/{actionRoute}".Trim('/');

        return fullRouteTemplate;
    }

    public static string GetRouteFor<TController>(Expression<Func<TController, object>> action)
    {
        var methodCall = action.Body as MethodCallExpression
            ?? (action.Body as UnaryExpression)?.Operand as MethodCallExpression
            ?? throw new ArgumentException("Expression must be a method call", nameof(action));

        var methodInfo = methodCall.Method;
        var fullRoute = GetRouteTemplateFor(typeof(TController), methodInfo);

        // Extract and replace parameters dynamically
        var parameters = methodCall.Method.GetParameters();
        var argumentValues = methodCall.Arguments.Select(arg => Expression.Lambda(arg).Compile().DynamicInvoke()).ToArray();

        if (parameters.Length != argumentValues.Length)
        {
            throw new InvalidOperationException("Mismatch between method parameters and extracted values.");
        }

        var queryParameters = new List<string>();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            var value = argumentValues[i];

            // Check if the parameter has a [FromQuery] attribute
            var fromQueryAttr = parameter.GetCustomAttribute<FromQueryAttribute>();
            if (fromQueryAttr != null)
            {
                if (value is IEnumerable enumerable and not string)
                {
                    foreach (var item in enumerable)
                    {
                        queryParameters.Add($"{parameter.Name}={Uri.EscapeDataString(item?.ToString() ?? string.Empty)}");
                    }
                }
                else
                {
                    queryParameters.Add($"{parameter.Name}={Uri.EscapeDataString(value?.ToString() ?? string.Empty)}");
                }
            }
            else
            {
                var placeholder = $"{{{parameter.Name}}}";
                fullRoute = fullRoute.Replace(placeholder, value?.ToString());
            }
        }

        // Append query parameters to the route if any exist
        if (queryParameters.Count > 0)
        {
            fullRoute += "?" + string.Join("&", queryParameters);
        }

        return fullRoute;
    }
}
