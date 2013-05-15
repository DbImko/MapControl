using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapControl.WPF.Core
{
    internal static class DictionaryExt
    {
        internal static void RemoveElementsByKeys<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
             where TKey : class
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }
            foreach (var key in keys)
            {
                dictionary.Remove(key);
            }
        }
    }
}
