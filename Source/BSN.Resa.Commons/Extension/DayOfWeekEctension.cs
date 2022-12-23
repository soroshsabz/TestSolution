using System;

namespace BSN.Resa.Commons.Extension
{
    public static class DayOfWeekEctension
    {
        public static string GetName(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek.ToString().Substring(0,2);
        }
    }
}
