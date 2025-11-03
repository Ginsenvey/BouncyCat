using System.Collections.Generic;
using System.Text.Json;

namespace BouncyCat.Helpers;

public static class TypeExtensions
{
    public static double ToIntSafe(this string input, int defaultValue = 0)
    {
        if (string.IsNullOrWhiteSpace(input))
            return defaultValue;

        return int.TryParse(input, out int result) ? result : defaultValue;
    }
    public static double ToDoubleSafe(this string input, double defaultValue = 5.0)
    {
        if (string.IsNullOrWhiteSpace(input))
            return defaultValue;

        return double.TryParse(input, out double result) ? result : defaultValue;
    }
    public static List<T> DeepCopy<T>(this List<T> source)
    {
        if (source == null) return null;

        return JsonSerializer.Deserialize<List<T>>(
            JsonSerializer.Serialize(source));
    }
    public static double StringToSize(this string input,double defaultValue=0)
    {
        if (string.IsNullOrWhiteSpace(input))
            return defaultValue;
        if (input.Contains("MB"))
        {
            return input.Replace("MB","").ToDoubleSafe();
        }
        if (input.Contains("GB"))
        {
            return 1024*input.Replace("GB", "").ToDoubleSafe();
        }
        return defaultValue;
    }
}