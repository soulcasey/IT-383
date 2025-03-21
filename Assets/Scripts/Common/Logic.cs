using System;
using System.Collections.Generic;
using System.Linq;

public static class Logic
{
    private static readonly Random random = new Random();

    public static T GetRandomEnum<T>(params T[] exceptions) where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();

        if (exceptions.Length > 0)
        {
            values = values.Where(value => exceptions.Contains(value) == false).ToList();
        }

        if (values.Count == 0)
        {
            throw new InvalidOperationException("No valid enum values available.");
        }

        return values[random.Next(values.Count)];
    }
}
