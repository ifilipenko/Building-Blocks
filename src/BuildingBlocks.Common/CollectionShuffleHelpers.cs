using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace BuildingBlocks.Common
{
    public static class CollectionShuffleHelpers
    {
        /// <summary>
        /// Shuffle elements in very good random order. Very slow method. To get a random on the order used RNGCryptoServiceProvider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            list.ShuffleThis();
            return list;
        }

        /// <summary>
        /// Shuffle elements of list in very good random order. Very slow method. To get a random on the order used RNGCryptoServiceProvider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void ShuffleThis<T>(this IList<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            var n = list.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do
                {
                    provider.GetBytes(box);
                }
                while (!(box[0] < n * (Byte.MaxValue / n)));
                var k = (box[0] % n);

                n--;

                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Shuffle elements in random order. To get a random on the order used Random class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> FastShuffle<T>(this IEnumerable<T> source)
        {
            var rnd = new Random();
            return source.OrderBy(item => rnd.Next());
        }

        /// <summary>
        /// Shuffle elements of list in random order. To get a random on the order used Random class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void FastShuffleThis<T>(this IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}