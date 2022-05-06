using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public static class DictionaryExtensions
    {
        public static T SubtractRandom<T>(this Dictionary<T, int> dictionary, Random random)
        {
            var keyValue = dictionary.ElementAt(random.Next(0, dictionary.Count));

            var key = keyValue.Key;
            var value = keyValue.Value;

            value--;

            if (value > 0)
                dictionary[key] = value;
            else
                dictionary.Remove(key);

            return key;
        }
    }
}