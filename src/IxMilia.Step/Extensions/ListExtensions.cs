using System.Collections.Generic;

namespace IxMilia.Step.Extensions
{
    public static class ListExtensions
    {
        public static T GetValueAtIndexOrDefault<T>(this IList<T> list, int index, T defaultValue = default(T))
        {
            return index < list.Count ? list[index] : defaultValue;
        }

        public static void SetValueAtIndexAndEnsureCount<T>(this IList<T> list, int index, T value, T fillerValue = default(T))
        {
            while (list.Count <= index)
            {
                list.Add(fillerValue);
            }

            list[index] = value;
        }
    }
}
