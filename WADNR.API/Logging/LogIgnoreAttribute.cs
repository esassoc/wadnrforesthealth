using System;

namespace WADNR.API.Logging
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LogIgnoreAttribute : Attribute
    {
        public LogIgnoreAttribute() { }
    }
}
