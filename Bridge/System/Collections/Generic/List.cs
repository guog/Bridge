using Bridge;

namespace System.Collections.Generic
{
    [External]
    public class List<T> : IList<T>, IBridgeClass
    {
        public extern List();

        public extern List(int capacity);

        public extern List(IEnumerable<T> items);

        [AccessorsIndexer]
        public extern T this[int index]
        {
            get;
            set;
        }

        public extern int Count
        {
            get;
        }

        private extern T Items(int index);

        public extern T Get(int index);

        public extern void Set(int index, T value);

        public extern void Add(T item);

        public extern void AddRange(IEnumerable<T> items);

        public extern void Clear();

        public extern bool Contains(T item);

        [Template("{this}.convertAll({TOutput}, {converter})")]
        public extern List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter);

        extern IEnumerator IEnumerable.GetEnumerator();

        public extern IEnumerator<T> GetEnumerator();

        public extern List<T> GetRange(int index);

        public extern List<T> GetRange(int index, int count);

        public extern int IndexOf(T item);

        public extern int IndexOf(T item, int startIndex);

        public extern void Insert(int index, T item);

        public extern void InsertRange(int index, IEnumerable<T> items);

        public extern string Join();

        public extern string Join(string delimiter);

        public extern int LastIndexOf(object item);

        public extern int LastIndexOf(object item, int fromIndex);

        public extern bool Remove(T item);

        public extern void RemoveAt(int index);

        public extern List<T> RemoveRange(int index, int count);

        public extern void Reverse();

        public extern List<T> Slice(int start);

        public extern List<T> Slice(int start, int end);

        public extern void Sort();

        public extern void Sort(Func<T, T, int> comparison);

        [Template("{this}.sort(Bridge.fn.bind({comparer}, {comparer}.compare))")]
        public extern void Sort(IComparer<T> comparer);

        public extern void Splice(int start, int deleteCount);

        public extern void Splice(int start, int deleteCount, IEnumerable<T> itemsToInsert);

        public extern void Unshift(params T[] items);

        public extern T[] ToArray();

        public extern int BinarySearch(T value);

        public extern int BinarySearch(int index, int length, T value);

        public extern int BinarySearch(T value, IComparer<T> comparer);

        public extern int BinarySearch(int index, int length, T value, IComparer<T> comparer);
    }
}