using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services;

public class HttpContextAuditUserProvider(
    IHttpContextAccessor httpContextAccessor,
    IServiceProvider serviceProvider) : IAuditUserProvider
{
    private int? _cachedPersonID;

    public int GetCurrentPersonID()
    {
        if (_cachedPersonID.HasValue) return _cachedPersonID.Value;

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _cachedPersonID = Person.AnonymousPersonID;
            return _cachedPersonID.Value;
        }

        var personDetail = serviceProvider.GetService<PersonDetail>();
        _cachedPersonID = personDetail?.PersonID ?? Person.AnonymousPersonID;
        return _cachedPersonID.Value;
    }
}
