using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementUI
{
    public static class ArrayExtensions
    {
        public static bool Exists<T>(this T[] array, Predicate<T> predicate)
        {
            if (null == array || array.Length <= 0)
                return false;

            return Array.Exists(array, predicate);
        }
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            Array.ForEach(array, action);
        }
        public static TOutput[] ToArray<TInput, TOutput>(this TInput[] inputArray, Func<TInput, TOutput> conversion)
        {
            TOutput[] tArr = new TOutput[inputArray.Length];
            for (int i = 0; i < inputArray.Length; i++)
            {
                tArr[i] = conversion(inputArray[i]);
            }

            return tArr;
        }
        public static T[] Get<T>(this IList selectedItems)
        {
            T[] tArr = new T[selectedItems.Count];
            for (int i = 0; i < selectedItems.Count; i++)
            {
                tArr[i] = (T)selectedItems[i];
            }

            return tArr;
        }
    }
}
