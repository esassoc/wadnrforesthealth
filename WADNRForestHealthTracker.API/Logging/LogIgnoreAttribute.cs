using System;

namespace WADNRForestHealthTracker.API.Logging
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LogIgnoreAttribute : Attribute
    {
        public LogIgnoreAttribute() { }
    }
}
