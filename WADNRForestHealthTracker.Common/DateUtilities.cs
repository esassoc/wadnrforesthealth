/*-----------------------------------------------------------------------
<copyright file="DateUtilities.cs" company="Sitka Technology Group">
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

namespace WADNRForestHealthTracker.Common
{
    public static class DateUtilities
    {
        public const int MonthsInFiscalYear = 12;

        public enum FiscalQuarter
        {
            First = 1,
            Second = 2,
            Third = 3,
            Fourth = 4
        }

        public enum Month
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }

        public static string ShortMonthName(Month month)
        {
            // "Jan", "Feb", etc.
            return month.ToString().Substring(0, 3);
        }

        public static DateTime GetEpoch()
        {
            return DateTime.Parse("1754-01-01 00:00:00.000");
        }

        // ------------------------- Last/First day of month -------------------------
        public static DateOnly GetLastDateInMonthDateOnly(this DateOnly dateInMonth)
        {
            return new DateOnly(dateInMonth.Year, dateInMonth.Month, DateTime.DaysInMonth(dateInMonth.Year, dateInMonth.Month));
        }

        public static DateOnly GetLastDateInMonthDateOnly(this DateTime dateInMonth)
        {
            return new DateOnly(dateInMonth.Year, dateInMonth.Month, DateTime.DaysInMonth(dateInMonth.Year, dateInMonth.Month));
        }

        public static DateTime GetLastDateInMonth(this DateTime dateInMonth)
        {
            return GetLastDateInMonthDateOnly(dateInMonth).ToDateTime(TimeOnly.MinValue);
        }

        public static DateTime GetLastDateInMonth(this DateOnly dateInMonth)
        {
            return dateInMonth.GetLastDateInMonthDateOnly().ToDateTime(TimeOnly.MinValue);
        }

        public static DateOnly GetFirstDateInMonthDateOnly(this DateOnly dateInMonth)
        {
            return new DateOnly(dateInMonth.Year, dateInMonth.Month, 1);
        }

        public static DateOnly GetFirstDateInMonthDateOnly(this DateTime dateInMonth)
        {
            return new DateOnly(dateInMonth.Year, dateInMonth.Month, 1);
        }

        public static DateTime GetFirstDateInMonth(this DateTime dateInMonth)
        {
            return GetFirstDateInMonthDateOnly(dateInMonth).ToDateTime(TimeOnly.MinValue);
        }

        public static DateTime GetFirstDateInMonth(this DateOnly dateInMonth)
        {
            return GetFirstDateInMonthDateOnly(dateInMonth).ToDateTime(TimeOnly.MinValue);
        }

        // ------------------------- Fiscal quarter helpers -------------------------
        public static DateOnly GetFirstDateInFiscalQuarterDateOnly(FiscalQuarter qtr, int calendarYear)
        {
            if (qtr == FiscalQuarter.First) // 1st FiscalQuarter = October 1 to December 31
            {
                return new DateOnly(calendarYear, 10, 1);
            }
            if (qtr == FiscalQuarter.Second) // 2nd FiscalQuarter = January 1 to  March 31
            {
                return new DateOnly(calendarYear, 1, 1);
            }
            if (qtr == FiscalQuarter.Third) // 3rd FiscalQuarter = April 1 to June 30
            {
                return new DateOnly(calendarYear, 4, 1);
            }
            // 4th FiscalQuarter = July 1 to Sept 30
            return new DateOnly(calendarYear, 7, 1);
        }

        public static DateTime GetFirstDateInFiscalQuarter(FiscalQuarter qtr, int calendarYear)
        {
            return GetFirstDateInFiscalQuarterDateOnly(qtr, calendarYear).ToDateTime(TimeOnly.MinValue);
        }

        public static DateOnly GetLastDateInFiscalQuarterDateOnly(FiscalQuarter qtr, int calendarYear)
        {
            if (qtr == FiscalQuarter.First) // 1st FiscalQuarter = October 1 to December 31
            {
                return new DateOnly(calendarYear, 12, DateTime.DaysInMonth(calendarYear, 12));
            }
            if (qtr == FiscalQuarter.Second) // 2nd FiscalQuarter = January 1 to  March 31
            {
                return new DateOnly(calendarYear, 3, DateTime.DaysInMonth(calendarYear, 3));
            }
            if (qtr == FiscalQuarter.Third) // 3rd FiscalQuarter = April 1 to June 30
            {
                return new DateOnly(calendarYear, 6, DateTime.DaysInMonth(calendarYear, 6));
            }
            // 4th FiscalQuarter = July 1 to Sept 30
            return new DateOnly(calendarYear, 9, DateTime.DaysInMonth(calendarYear, 9));
        }

        public static DateTime GetLastDateInFiscalQuarter(FiscalQuarter qtr, int calendarYear)
        {
            return GetLastDateInFiscalQuarterDateOnly(qtr, calendarYear).ToDateTime(TimeOnly.MinValue);
        }

        public static DateOnly GetFirstDateInFiscalQuarterDateOnly(this DateOnly dateInQuarter)
        {
            FiscalQuarter qtr = ((Month)dateInQuarter.Month).GetFiscalQuarter();
            return GetFirstDateInFiscalQuarterDateOnly(qtr, dateInQuarter.Year);
        }

        public static DateOnly GetFirstDateInFiscalQuarterDateOnly(this DateTime dateInQuarter)
        {
            FiscalQuarter qtr = ((Month)dateInQuarter.Month).GetFiscalQuarter();
            return GetFirstDateInFiscalQuarterDateOnly(qtr, dateInQuarter.Year);
        }

        public static DateTime GetFirstDateInFiscalQuarter(this DateTime dateInQuarter)
        {
            return GetFirstDateInFiscalQuarterDateOnly(dateInQuarter).ToDateTime(TimeOnly.MinValue);
        }

        public static DateTime GetFirstDateInFiscalQuarter(this DateOnly dateInQuarter)
        {
            return GetFirstDateInFiscalQuarterDateOnly(dateInQuarter).ToDateTime(TimeOnly.MinValue);
        }

        public static DateOnly GetLastDateInFiscalQuarterDateOnly(this DateOnly dateInQuarter)
        {
            FiscalQuarter qtr = ((Month)dateInQuarter.Month).GetFiscalQuarter();
            return GetLastDateInFiscalQuarterDateOnly(qtr, dateInQuarter.Year);
        }

        public static DateOnly GetLastDateInFiscalQuarterDateOnly(this DateTime dateInQuarter)
        {
            FiscalQuarter qtr = ((Month)dateInQuarter.Month).GetFiscalQuarter();
            return GetLastDateInFiscalQuarterDateOnly(qtr, dateInQuarter.Year);
        }

        public static DateTime GetLastDateInFiscalQuarter(this DateTime dateInQuarter)
        {
            return GetLastDateInFiscalQuarterDateOnly(dateInQuarter).ToDateTime(TimeOnly.MinValue);
        }

        public static DateTime GetLastDateInFiscalQuarter(this DateOnly dateInQuarter)
        {
            return GetLastDateInFiscalQuarterDateOnly(dateInQuarter).ToDateTime(TimeOnly.MinValue);
        }

        // ------------------------- Fiscal quarter navigation -------------------------
        public static FiscalQuarter GetFiscalQuarter(this Month month)
        {
            if (month >= Month.October)
            {
                return FiscalQuarter.First; // 1st Fiscal FiscalQuarter = October 1 to December 31
            }
            if (month <= Month.March)
            {
                return FiscalQuarter.Second; // 2nd Fiscal FiscalQuarter = January 1 to  March 31
            }
            if (month >= Month.April && month <= Month.June)
            {
                return FiscalQuarter.Third; // 3rd Fiscal FiscalQuarter = April 1 to June 30
            }
            //else if (month >= Month.July && month <= Month.September)
            return FiscalQuarter.Fourth; // 4th Fiscal FiscalQuarter = July 1 to September 30
        }

        public static FiscalQuarter GetCurrentFiscalQuarter()
        {
            return ((Month)DateTime.UtcNow.Month).GetFiscalQuarter();
        }

        public static FiscalQuarter GetPreviousFiscalQuarter()
        {
            return GetFiscalQuarterPreviousToFiscalQuarter(GetCurrentFiscalQuarter());
        }

        public static DateOnly GetLastDateOfPreviousFiscalQuarterDateOnly(DateOnly fromDate)
        {
            // What quarter are we in?
            FiscalQuarter currentQuarter = GetFiscalQuarter((Month)fromDate.Month);
            // What's the previous quarter?
            FiscalQuarter previousQuarter = GetFiscalQuarterPreviousToFiscalQuarter(currentQuarter);
            // If we wrapped our FY  (i.e. went from quarter 2 => quarter 1), decrease the calendar year to the previous year.
            int calendarYear = fromDate.Year;
            if (previousQuarter == FiscalQuarter.First && currentQuarter == FiscalQuarter.Second)
            {
                calendarYear -= 1;
            }

            // Use the previous Quarter / Calendar Year to get the last date in the fiscal quarter
            return GetLastDateInFiscalQuarterDateOnly(previousQuarter, calendarYear);
        }

        public static DateOnly GetLastDateOfPreviousFiscalQuarterDateOnly(DateTime fromDate)
        {
            return GetLastDateOfPreviousFiscalQuarterDateOnly(new DateOnly(fromDate.Year, fromDate.Month, fromDate.Day));
        }

        public static DateTime GetLastDateOfPreviousFiscalQuarter(DateOnly fromDate)
        {
            return GetLastDateOfPreviousFiscalQuarterDateOnly(fromDate).ToDateTime(TimeOnly.MinValue);
        }

        public static DateTime GetLastDateOfPreviousFiscalQuarter(DateTime fromDate)
        {
            return GetLastDateOfPreviousFiscalQuarterDateOnly(fromDate).ToDateTime(TimeOnly.MinValue);
        }

        public static FiscalQuarter GetFiscalQuarterPreviousToFiscalQuarter(this FiscalQuarter givenFiscalQuarter)
        {
            int fiscalQuarterNumber = (int)givenFiscalQuarter - 1;
            if (fiscalQuarterNumber == 0)
            {
                // Wrap back to prior year
                fiscalQuarterNumber = 4;
            }
            return (FiscalQuarter)fiscalQuarterNumber;
        }

        // ------------------------- Date range helpers -------------------------
        public static bool DateRangesOverlap(DateTime startDate1, DateTime endDate1, DateTime startDate2, DateTime endDate2)
        {
            return (
                (startDate2 >= startDate1 && startDate2 < endDate1)
                ||
                (endDate2 <= endDate1 && endDate2 > startDate1)
                ||
                (startDate2 <= startDate1 && endDate2 >= endDate1)
            );
        }

        public static bool DateRangesOverlap(DateOnly startDate1, DateOnly endDate1, DateOnly startDate2, DateOnly endDate2)
        {
            return (
                (startDate2 >= startDate1 && startDate2 < endDate1)
                ||
                (endDate2 <= endDate1 && endDate2 > startDate1)
                ||
                (startDate2 <= startDate1 && endDate2 >= endDate1)
            );
        }

        // Mixed overloads that convert to DateOnly
        public static bool DateRangesOverlap(DateOnly startDate1, DateOnly endDate1, DateTime startDate2, DateTime endDate2)
        {
            return DateRangesOverlap(startDate1, endDate1, new DateOnly(startDate2.Year, startDate2.Month, startDate2.Day), new DateOnly(endDate2.Year, endDate2.Month, endDate2.Day));
        }

        public static bool DateRangesOverlap(DateTime startDate1, DateTime endDate1, DateOnly startDate2, DateOnly endDate2)
        {
            return DateRangesOverlap(new DateOnly(startDate1.Year, startDate1.Month, startDate1.Day), new DateOnly(endDate1.Year, endDate1.Month, endDate1.Day), startDate2, endDate2);
        }

        // ------------------------- Difference / comparison helpers -------------------------
        public static string GetDifferenceInEnglish(DateTime firstDate, DateTime secondDate, bool showMinutes)
        {
            var age = firstDate.Subtract(secondDate);

            var days = Convert.ToInt32(Math.Floor(age.TotalDays));
            var hours = Convert.ToInt32(Math.Floor(age.TotalHours - (days * 24)));
            var minutes = Convert.ToInt32(Math.Floor(age.TotalMinutes - (days * 24 * 60) - (hours * 60)));

            if (showMinutes)
            {
                return $"{days} day{(days == 1 ? "" : "s")}, {hours} hour{(hours == 1 ? "" : "s")}, {minutes} minute{(minutes == 1 ? "" : "s")}";
            }

            // No minutes
            return $"{days} day{(days == 1 ? "" : "s")}, {hours} hour{(hours == 1 ? "" : "s")}";
        }

        /// <summary>
        /// Gets a simplified version that removes two things:
        /// - Values with zero values that aren't significant digits: (0 days, 0 hours, 36 minutes) => 36 minutes
        /// - Less significant values as net time increases. 36 minutes => 3 hours => 1 day
        /// </summary>
        /// <param name="firstDate"></param>
        /// <param name="secondDate"></param>
        /// <returns></returns>
        public static string GetDifferenceInEnglishSimplified(DateTime firstDate, DateTime secondDate)
        {
            var age = firstDate.Subtract(secondDate);

            var days = Convert.ToInt32(Math.Floor(age.TotalDays));
            var hours = Convert.ToInt32(Math.Floor(age.TotalHours - (days * 24)));
            var minutes = Convert.ToInt32(Math.Floor(age.TotalMinutes - (days * 24 * 60) - (hours * 60)));
            var seconds = Convert.ToInt32(Math.Floor(age.TotalSeconds - (days * 24 * 60) - (hours * 60) - (minutes * 60)));

            if (days > 0) return $"{days} day{(days == 1 ? "" : "s")}";
            if (hours > 0) return $"{hours} hour{(hours == 1 ? "" : "s")}";
            if (minutes > 0) return $"{minutes} minute{(minutes == 1 ? "" : "s")}";
            return $"{seconds} second{(seconds == 1 ? "" : "s")}";
        }

        public static int GetDifferenceInDays(DateTime firstDate, DateTime secondDate)
        {
            return firstDate.Date.Subtract(secondDate.Date).Days;
        }

        public static int GetDaysSinceToday(this DateTime dateToCheck)
        {
            return GetDifferenceInDays(DateTime.Today, dateToCheck);
        }

        public static int GetDaysFromToday(this DateTime dateToCheck)
        {
            return GetDifferenceInDays(dateToCheck, DateTime.Today);
        }

        // ------------------------- Fiscal year helpers -------------------------
        public static int GetCurrentFiscalYear()
        {
            return DateTime.UtcNow.GetFiscalYear();
        }

        public static int GetPreviousFiscalYear()
        {
            return DateTime.UtcNow.GetPreviousFiscalYear();
        }

        public static int GetPreviousFiscalYear(this DateTime dateToCheck)
        {
            return dateToCheck.GetFiscalYear() - 1;
        }

        public static int GetNextFiscalYear()
        {
            return DateTime.UtcNow.GetNextFiscalYear();
        }

        public static int GetNextFiscalYear(this DateTime dateToCheck)
        {
            return DateTime.UtcNow.GetFiscalYear() + 1;
        }

        public static int GetFiscalYear(this DateTime dateToCheck)
        {
            if (((Month)dateToCheck.Month).GetFiscalQuarter() == FiscalQuarter.First)
                return dateToCheck.Year + 1;
            return dateToCheck.Year;
        }

        public static int GetFiscalMonth(this DateTime dateToCheck)
        {
            var month = (Month)dateToCheck.Month;
            switch (month)
            {
                case Month.October:
                    return 1;
                case Month.November:
                    return 2;
                case Month.December:
                    return 3;
                case Month.January:
                    return 4;
                case Month.February:
                    return 5;
                case Month.March:
                    return 6;
                case Month.April:
                    return 7;
                case Month.May:
                    return 8;
                case Month.June:
                    return 9;
                case Month.July:
                    return 10;
                case Month.August:
                    return 11;
                case Month.September:
                    return 12;
                default:
                    return 0;
            }
        }

        // DateOnly variants
        public static int GetCurrentFiscalYearDateOnly()
        {
            return DateOnly.FromDateTime(DateTime.Now).GetFiscalYear();
        }

        public static int GetPreviousFiscalYear(this DateOnly dateToCheck)
        {
            return dateToCheck.GetFiscalYear() - 1;
        }

        public static int GetNextFiscalYear(this DateOnly dateToCheck)
        {
            return dateToCheck.GetFiscalYear() + 1;
        }

        public static int GetFiscalYear(this DateOnly dateToCheck)
        {
            if (((Month)dateToCheck.Month).GetFiscalQuarter() == FiscalQuarter.First)
                return dateToCheck.Year + 1;
            return dateToCheck.Year;
        }

        public static int? GetFiscalYear(this DateOnly? dateToCheck)
        {
            if (dateToCheck == null)
            {
                return null;
            }

            return dateToCheck.Value.GetFiscalYear();
        }

        public static int GetFiscalMonth(this DateOnly dateToCheck)
        {
            var month = (Month)dateToCheck.Month;
            switch (month)
            {
                case Month.October:
                    return 1;
                case Month.November:
                    return 2;
                case Month.December:
                    return 3;
                case Month.January:
                    return 4;
                case Month.February:
                    return 5;
                case Month.March:
                    return 6;
                case Month.April:
                    return 7;
                case Month.May:
                    return 8;
                case Month.June:
                    return 9;
                case Month.July:
                    return 10;
                case Month.August:
                    return 11;
                case Month.September:
                    return 12;
                default:
                    return 0;
            }
        }

        // ------------------------- Fiscal year start/end -------------------------
        public static DateOnly GetFirstDateInFiscalYearDateOnly(this DateOnly dateInFiscalYear)
        {
            int fiscalYear = dateInFiscalYear.GetFiscalYear();
            return GetFirstDateInFiscalYearDateOnly(fiscalYear);
        }

        public static DateTime GetFirstDateInFiscalYear(this DateTime dateInFiscalYear)
        {
            int fiscalYear = dateInFiscalYear.GetFiscalYear();
            return GetFirstDateInFiscalYearDateOnly(fiscalYear).ToDateTime(TimeOnly.MinValue);
        }

        public static DateOnly GetFirstDateInFiscalYearDateOnly(int fiscalYear)
        {
            const int firstDayInMonth = 1;
            const int firstFiscalMonth = (int)Month.October;
            return new DateOnly(fiscalYear - 1, firstFiscalMonth, firstDayInMonth);
        }

        public static DateTime GetFirstDateInFiscalYear(int fiscalYear)
        {
            return GetFirstDateInFiscalYearDateOnly(fiscalYear).ToDateTime(TimeOnly.MinValue);
        }

        public static DateOnly GetLastDateInFiscalYearDateOnly(this DateOnly dateInFiscalYear)
        {
            int fiscalYear = dateInFiscalYear.GetFiscalYear();
            return GetLastDateInFiscalYearDateOnly(fiscalYear);
        }

        public static DateTime GetLastDateInFiscalYear(this DateTime dateInFiscalYear)
        {
            int fiscalYear = dateInFiscalYear.GetFiscalYear();
            return GetLastDateInFiscalYearDateOnly(fiscalYear).ToDateTime(TimeOnly.MinValue);
        }

        public static DateOnly GetLastDateInFiscalYearDateOnly(int fiscalYear)
        {
            const int lastDayInMonth = 30;
            const int lastFiscalMonth = (int)Month.September;
            return new DateOnly(fiscalYear, lastFiscalMonth, lastDayInMonth);
        }

        public static DateTime GetLastDateInFiscalYear(int fiscalYear)
        {
            return GetLastDateInFiscalYearDateOnly(fiscalYear).ToDateTime(TimeOnly.MinValue);
        }

        // Calendar year helpers
        public static DateOnly GetFirstDateInYearDateOnly(int calendarYear)
        {
            const int firstDayInMonth = 1;
            const int firstCalendarMonth = (int)Month.January;
            return new DateOnly(calendarYear, firstCalendarMonth, firstDayInMonth);
        }

        public static DateTime GetFirstDateInYear(int calendarYear)
        {
            return GetFirstDateInYearDateOnly(calendarYear).ToDateTime(TimeOnly.MinValue);
        }

        public static DateTime GetFirstDateInYear(this DateTime date)
        {
            return GetFirstDateInYear(date.Year);
        }

        public static DateOnly GetLastDateInYearDateOnly(int calendarYear)
        {
            const int lastDayInMonth = 31;
            const int lastCalendarMonth = (int)Month.December;
            return new DateOnly(calendarYear, lastCalendarMonth, lastDayInMonth);
        }

        public static DateTime GetLastDateInYear(int calendarYear)
        {
            return GetLastDateInYearDateOnly(calendarYear).ToDateTime(TimeOnly.MinValue);
        }

        public static DateTime GetLastDateInYear(this DateTime date)
        {
            return GetLastDateInYear(date.Year);
        }

        // ------------------------- Parsing helpers -------------------------
        /// <summary>
        /// Indicates if string would parse as a date
        /// </summary>
        public static bool IsValidDateFormat(this string stringToCheck)
        {
            DateTime dummy;
            return DateTime.TryParse(stringToCheck, out dummy);
        }

        // ------------------------- Fiscal month -> calendar conversions -------------------------
        public static int GetCalendarYearForFiscalYearFiscalMonth(int fiscalYear, int fiscalMonth)
        {
            return GetCalendarDateForFiscalYearFiscalMonthDateOnly(fiscalYear, fiscalMonth).Year;
        }

        public static int GetCalendarMonthForFiscalYearFiscalMonth(int fiscalYear, int fiscalMonth)
        {
            return GetCalendarDateForFiscalYearFiscalMonthDateOnly(fiscalYear, fiscalMonth).Month;
        }

        public static DateOnly GetCalendarDateForFiscalYearFiscalMonthDateOnly(int fiscalYear, int fiscalMonth)
        {
            var startFyDate = GetFirstDateInFiscalYearDateOnly(fiscalYear);
            return startFyDate.AddMonths(fiscalMonth - 1);
        }

        public static DateTime GetCalendarDateForFiscalYearFiscalMonth(int fiscalYear, int fiscalMonth)
        {
            return GetCalendarDateForFiscalYearFiscalMonthDateOnly(fiscalYear, fiscalMonth).ToDateTime(TimeOnly.MinValue);
        }

        // ------------------------- Misc -------------------------
        public static List<int> GetRangeOfYears(int startYear, int endYear)
        {
            return Enumerable.Range(startYear, (endYear - startYear) + 1).ToList();
        }

        public static DateTime TruncateToMinute(DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks - dateTime.Ticks % TimeSpan.TicksPerMinute, dateTime.Kind);
        }

        // Restore DateTime comparison extension methods (preserve original behavior)
        public static bool IsTodayOnOrBeforeDate(this DateTime dateToCheck)
        {
            return DateTime.Today.IsDateOnOrBefore(dateToCheck);
        }

        public static bool IsDateOnOrBefore(this DateTime dateToCheck, DateTime dateToCheckAgainst)
        {
            return dateToCheck.Date.CompareTo(dateToCheckAgainst.Date) < 1;
        }

        public static bool IsDateOnOrAfter(this DateTime dateToCheck, DateTime dateToCheckAgainst)
        {
            return dateToCheck.Date.CompareTo(dateToCheckAgainst.Date) > -1;
        }

        public static bool IsDateBefore(this DateTime dateToCheck, DateTime dateToCheckAgainst)
        {
            return dateToCheck.Date.CompareTo(dateToCheckAgainst.Date) < 0;
        }

        public static bool IsDateAfter(this DateTime dateToCheck, DateTime dateToCheckAgainst)
        {
            return dateToCheck.Date.CompareTo(dateToCheckAgainst.Date) > 0;
        }

        public static bool IsDateInRange(this DateTime dateToCheck, DateTime startOfRange, DateTime endOfRange)
        {
            return dateToCheck.IsDateOnOrAfter(startOfRange) && dateToCheck.IsDateOnOrBefore(endOfRange);
        }

        // DateOnly equivalents for comparison
        public static bool IsDateOnOrBefore(this DateOnly dateToCheck, DateOnly dateToCheckAgainst)
        {
            return dateToCheck.CompareTo(dateToCheckAgainst) <= 0;
        }

        public static bool IsDateOnOrAfter(this DateOnly dateToCheck, DateOnly dateToCheckAgainst)
        {
            return dateToCheck.CompareTo(dateToCheckAgainst) >= 0;
        }

        public static bool IsDateBefore(this DateOnly dateToCheck, DateOnly dateToCheckAgainst)
        {
            return dateToCheck < dateToCheckAgainst;
        }

        public static bool IsDateAfter(this DateOnly dateToCheck, DateOnly dateToCheckAgainst)
        {
            return dateToCheck > dateToCheckAgainst;
        }

        public static bool IsDateInRange(this DateOnly dateToCheck, DateOnly startOfRange, DateOnly endOfRange)
        {
            return dateToCheck >= startOfRange && dateToCheck <= endOfRange;
        }

        // GetEarliest/GetLatest overloads
        public static DateTime GetEarliestDate(DateTime firstDate, DateTime secondDate)
        {
            return Convert.ToDateTime((firstDate.IsDateBefore(secondDate)) ? firstDate : secondDate);
        }

        public static DateOnly GetEarliestDate(DateOnly firstDate, DateOnly secondDate)
        {
            return (firstDate < secondDate) ? firstDate : secondDate;
        }

        public static DateTime GetLatestDate(DateTime firstDate, DateTime secondDate)
        {
            return Convert.ToDateTime((firstDate.IsDateAfter(secondDate)) ? firstDate : secondDate);
        }

        public static DateOnly GetLatestDate(DateOnly firstDate, DateOnly secondDate)
        {
            return (firstDate > secondDate) ? firstDate : secondDate;
        }
    }
}