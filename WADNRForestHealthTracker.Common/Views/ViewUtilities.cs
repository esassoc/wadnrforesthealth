/*-----------------------------------------------------------------------
<copyright file="ViewUtilities.cs" company="Sitka Technology Group">
Copyright (c) Sitka Technology Group. All rights reserved.
<author>Sitka Technology Group</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/

using System.Web;

namespace WADNRForestHealthTracker.Common.Views
{
    public static class ViewUtilities
    {
        public const string NoneString = "None";
        public const string NoAnswerProvided = "<No answer provided>";
        public const string NoCommentString = "<no comment>";
        public const string NaString = "n/a";
        public const string NotFoundString = "(not found)";
        public const string NotAvailableString = "Not available";
        public const string NotProvidedString = "not provided";
        public const string NoChangesRecommended = "No changes recommended";
        public const string NotApplicableIndicatorIsInAttainment = "Not applicable.  Indicator is in attainment.";
        public const string UnknownString = "Unknown";

        public static string CheckedIfEqual(int? value, int testValue)
        {
            return (value.HasValue && testValue == value.Value).ToCheckedOrEmpty();
        }

        public static string CheckedIfEqual(bool? value, bool testValue)
        {
            return (value.HasValue && testValue == value.Value).ToCheckedOrEmpty();
        }

        public static string Prune(this string value, int totalLength)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            if (value.Length < totalLength)
                return value;

            return $"{value.Substring(0, totalLength - 3)}...";
        }

        public static string Flatten(this string value, string replacement)
        {
            return String.IsNullOrEmpty(value) ? value : value.Replace("\r\n", replacement).Replace("\n", replacement).Replace("\r", replacement);
        }

        public static string Flatten(this string value)
        {
            return Flatten(value, " ");
        }

        public static string HtmlEncode(this string value)
        {
            return String.IsNullOrEmpty(value) ? value : HttpUtility.HtmlEncode(value);
        }

        public static string HtmlEncodeWithBreaks(this string value)
        {
            var ret = value.HtmlEncode();
            return String.IsNullOrEmpty(ret) ? ret : ret.Replace("\r\n","\n").Replace("\r","\n").Replace("\n", "<br/>\r\n");
        }

        public static string DisplayDataLine(Func<object> value)
        {
            return DisplayDataLine(true, value);
        }

        public static string DisplayDataLineDefaultString(string defaultString)
        {
            var result = "<p style=\"color:grey\">" + defaultString + "</p>";
            return result;
        }

        public static string DisplayDataLine(bool predicate, Func<object> stringFuncIfTrue, string stringIfFalse)
        {
            var result = stringIfFalse;

            if (predicate)
            {
                var o = stringFuncIfTrue();
                if (o != null)
                    result = o.ToString();
            }
            return result.HtmlEncode().Flatten("<br/>");
        }

        public static string DisplayDataLine(bool predicate, Func<object> stringFuncIfTrue)
        {
            return DisplayDataLine(predicate, stringFuncIfTrue, NoneString);
        }

        public static string DisplayValue(this int value, string stringIfNullOrDefault)
        {
            return new int?(value).DisplayValue(stringIfNullOrDefault);
        }

        public static string DisplayValue(this int? value, string stringIfNullOrDefault)
        {
            return value == null || value == default(int) ? stringIfNullOrDefault : value.ToString();
        }

        public static string DisplayValue(this int? value)
        {
            return DisplayValue(value, String.Empty);
        }

        public static string DisplayValue(this int value)
        {
            return DisplayValue(value, String.Empty);
        }

        public static string DisplayValue(this DateTime? value, string format)
        {
            return value == null ? String.Empty : value.Value.ToString(format);
        }

        public static string DisplayValue(this Boolean? value)
        {
            return value == null ? String.Empty : value.Value.ToString();
        }
    }
}
