using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Generic.Utility
{
    public static class EnumerableUtils
    {
        /// <summary>
        /// Performs the <paramref name="action"/> to each element in the <paramref name="source"/> enumerable.
        /// </summary>
        /// <param name="source">The source enumerable.</param>
        /// <param name="action">The action that is performed to each element of the <paramref name="source"/></param>
        /// <returns>An enumerable of type <typeparamref name="T"/> after the <paramref name="action"/> has been performed to each of the elements.</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
                action(element);

            return source;
        }

        /// <summary>
        /// Randomizes the order of the elements in the <paramref name="source"/> list.
        /// </summary>
        /// <typeparam name="T">The generic type of the elements in the source list</typeparam>
        /// <param name="source">The source list that is to be shuffled</param>
        /// <returns>The source list after the elements have been randomized</returns>
        public static IList<T> Shuffle<T>(this IList<T> source)
        {
            var sourceSize = source.Count();

            for (int i = 0; i < sourceSize; i++)
            {
                var rand = NumberUtils.GenerateRandomNumber(0, sourceSize-1);

                var elementToSwap = source[i];
                source[i] = source[rand];
                source[rand] = elementToSwap;
            }

            return source;
        }

        /// <summary>
        /// Disposes of each element in the enumerable.
        /// </summary>
        /// <param name="disposableObjs">The objects to be disposed.</param>
        public static void Dispose(this IEnumerable<IDisposable> disposableObjs)
        {
            foreach (var obj in disposableObjs)
                obj.Dispose();
        }
    }
}