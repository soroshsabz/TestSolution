using System;
using System.Collections.Generic;
using System.Resources;

namespace BSN.Resa.Core.Commons
{
    [Flags]
    public enum DaysOfWeek
    {
        Sunday = 1,

        Monday = 2,

        Tuesday = 4,

        Wednesday = 8,

        Thursday = 16,

        Friday = 32,

        Saturday = 64
    }

    public static class DaysOfWeekExtensions
    {
        public static bool Contains(this DaysOfWeek daysOfWeek, DayOfWeek value)
        {
            return (daysOfWeek & (DaysOfWeek)Math.Pow(2, (int)value)) != 0;
        }

        public static bool Contains(this DaysOfWeek daysOfWeek, DaysOfWeek value)
        {
            return (daysOfWeek & value) != 0;
        }

        public static DaysOfWeek ShiftDays(this DaysOfWeek daysOfWeek, int value)
        {
            value %= 7;
            value = value < 0 ? value + 7 : value;

            var result = (int)daysOfWeek;
            while (value > 0)
            {
                result *= 2;

                if (result > 127)
                    result -= 127;

                value -= 1;
            }

            return (DaysOfWeek)result;
        }

        public static System.DayOfWeek ToStandardDayOfWeek(this DaysOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DaysOfWeek.Sunday:
                    return DayOfWeek.Sunday;

                case DaysOfWeek.Monday:
                    return DayOfWeek.Monday;

                case DaysOfWeek.Tuesday:
                    return DayOfWeek.Tuesday;

                case DaysOfWeek.Wednesday:
                    return DayOfWeek.Wednesday;

                case DaysOfWeek.Thursday:
                    return DayOfWeek.Thursday;

                case DaysOfWeek.Friday:
                    return DayOfWeek.Friday;

                case DaysOfWeek.Saturday:
                    return DayOfWeek.Saturday;

                default:
                    throw new ArgumentOutOfRangeException("dayOfWeek value is not in range");
            }
        }

        public static List<string> GetDayNames(this DaysOfWeek daysOfWeek)
        {
            var result = new List<string>();

            if (daysOfWeek.HasFlag(DaysOfWeek.Sunday))
                result.Add("Su");

            if (daysOfWeek.HasFlag(DaysOfWeek.Monday))
                result.Add("Mo");

            if (daysOfWeek.HasFlag(DaysOfWeek.Tuesday))
                result.Add("Tu");

            if (daysOfWeek.HasFlag(DaysOfWeek.Wednesday))
                result.Add("We");

            if (daysOfWeek.HasFlag(DaysOfWeek.Thursday))
                result.Add("Th");

            if (daysOfWeek.HasFlag(DaysOfWeek.Friday))
                result.Add("Fr");

            if (daysOfWeek.HasFlag(DaysOfWeek.Saturday))
                result.Add("Sa");

            return result;
        }

        public static string ToStringWithLocale(this DaysOfWeek daysOfWeek, ResourceManager resourceManager)
        {
            var result = string.Empty;
            foreach (DaysOfWeek x in Enum.GetValues(typeof(DaysOfWeek)))
            {
                if ((daysOfWeek & x) == x)
                    result += (string.IsNullOrEmpty(result) ? "" : " | ") + resourceManager.GetString(x.ToString());
            }
            return result;
        }

        public static DaysOfWeek MapToDaysOfWeek(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Friday:
                    return DaysOfWeek.Friday;
                case DayOfWeek.Monday:
                    return DaysOfWeek.Monday;
                case DayOfWeek.Saturday:
                    return DaysOfWeek.Saturday;
                case DayOfWeek.Sunday:
                    return DaysOfWeek.Sunday;
                case DayOfWeek.Thursday:
                    return DaysOfWeek.Thursday;
                case DayOfWeek.Tuesday:
                    return DaysOfWeek.Tuesday;
                case DayOfWeek.Wednesday:
                    return DaysOfWeek.Wednesday;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null);
            }
        }
    }
}