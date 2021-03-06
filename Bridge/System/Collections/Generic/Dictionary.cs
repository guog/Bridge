using Bridge;

namespace System.Collections.Generic
{
    [External]
    public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IBridgeClass
    {
        public extern Dictionary();

        [Template("new (System.Collections.Generic.Dictionary$2({TKey}, {TValue}))()")]
        public extern Dictionary(int capacity);

        [Template("new (System.Collections.Generic.Dictionary$2({TKey}, {TValue}))(null, {comparer})")]
        public extern Dictionary(int capacity, IEqualityComparer<TKey> comparer);

        public extern Dictionary(object obj);

        public extern Dictionary(object obj, IEqualityComparer<TKey> comparer);

        [Template("new (System.Collections.Generic.Dictionary$2({TKey}, {TValue}))(null, {comparer})")]
        public extern Dictionary(IEqualityComparer<TKey> comparer);

        public extern Dictionary(IDictionary<TKey, TValue> dictionary);

        public extern Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);

        public extern IEqualityComparer<TKey> Comparer
        {
            get;
        }

        public extern int Count
        {
            get;
        }

        public extern new ICollection<TKey> Keys
        {
            get;
        }

        public extern ICollection<TValue> Values
        {
            get;
        }

        [AccessorsIndexer]
        public extern TValue this[TKey key]
        {
            [Name("get")]
            get;
            [Name("set")]
            set;
        }

        public extern void Set(TKey key, TValue value);

        public extern void Add(TKey key, TValue value);

        public extern TValue Get(TKey key);

        private extern TValue Items(TKey key);

        public extern void Clear();

        public extern bool ContainsKey(TKey key);

        public extern bool ContainsValue(TValue value);

        public extern IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

        extern IEnumerator IEnumerable.GetEnumerator();

        public extern bool Remove(TKey key);

        public extern bool TryGetValue(TKey key, out TValue value);
    }
}