using System;
using System.Linq;
using System.Xml.Linq;

namespace RKD_TD.Helpers;

internal static class ParseHelper
{
    public static float[] ParseToFloatArr(XElement element, string attributeName, char separator)
    {
        return ParseToFloatArr(element.Attribute(attributeName)!.Value, separator);
    }
    
    public static float[] ParseToFloatArr(string value, char separator)
    {
        return value
            .Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(float.Parse)
            .ToArray();
    }
}