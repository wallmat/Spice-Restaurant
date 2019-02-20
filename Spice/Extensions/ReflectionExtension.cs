using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Extensions
{
    public static class ReflectionExtension
    {
        public static string GetPropertyValue<T>(this T item, string propertyName)
        {
            //get the property and then get the value of that property from item and convert it to a string
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        }
    }
}
