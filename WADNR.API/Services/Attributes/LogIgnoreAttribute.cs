using System;

namespace WADNR.API.Services.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class LogIgnoreAttribute : Attribute
{
    public LogIgnoreAttribute() {}
}