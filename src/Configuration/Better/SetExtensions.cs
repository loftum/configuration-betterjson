﻿namespace Configuration.Better;

internal static class SetExtensions
{
    public static void AddRange<T>(this ISet<T> set, IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            set.Add(value);
        }
    }
}