using System;
using System.Collections.Generic;

namespace AdventOfCode2021
{
    internal static class Extensions
    {
        public static string Join(this IEnumerable<string> value, string separator)
        {
            return string.Join(separator, value);
        }

        public static IEnumerable<string[]> SplitIntoBatch(this IEnumerable<string> input, Func<string, bool> dividerFunc)
        {
            var buffer = new List<string>();
            foreach (var item in input)
            {
                if (dividerFunc(item))
                {
                    yield return buffer.ToArray();
                    buffer.Clear();
                    continue;
                }
                
                buffer.Add(item);
            }

            if (buffer.Count > 0)
                yield return buffer.ToArray();
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TKey, TValue> valueFactory)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;

            value = valueFactory(key);
            dictionary[key] = value;

            return value;
        }

        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> values)
        {
            foreach (var value in values) 
                hashSet.Add(value);
        }

    }
}
