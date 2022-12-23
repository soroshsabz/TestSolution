using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace BSN.Resa.Commons.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// Get and return DisplayAttribute name.
        /// </summary>
        /// <returns>The DisplayAttribute name</returns>
        public static string GetEnumDisplayName(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>().GetName();
        }
    }
}
