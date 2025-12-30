using System;

namespace WADNRForestHealthTracker.API.Services.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class LogIgnoreAttribute : Attribute
{
    public LogIgnoreAttribute() {}
}