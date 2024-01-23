//HintName: MultiArrayList.TestNamespace.TestClass.Xxx.g.cs
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestNamespace
{
    public partial class TestClass
    {
        public interface IXxxMultiArray: IList<Xxx>
        {
            ReadOnlyCollection<String> X { get; }
            ReadOnlyCollection<Boolean> Y { get; }
        }

        public class XxxMultiArray : IXxxMultiArray
        {
            readonly List<String> _X;
            public ReadOnlyCollection<String> X { get; }
            readonly List<Boolean> _Y;
            public ReadOnlyCollection<Boolean> Y { get; }

            public int Count => X.Count;
            public bool IsReadOnly => false;

            public Xxx this[int index]
            {
                get => new {
                    X = X[index],
                    Y = Y[index],
                };
                set
                {
                    _X[index] = value.X;
                    _Y[index] = value.Y;
                }
            }

            public XxxMultiArray(IEnumerable<Xxx> data)
            {
                int count = (data as ICollection<Xxx>)?.Count ?? 0;
                _X = new(count);
                _Y = new(count);

                foreach (var datum in data)
                {
                    _X.Add(datum.X);
                    _Y.Add(datum.Y);
                }

                X = _X.AsReadOnly();
                Y = _Y.AsReadOnly();
            }

            public int IndexOf(Xxx item)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (X[i] == item.X
                        && Y[i] == item.Y)
                        return i;
                }
                return -1;
            }

            public void Insert(int index, Xxx item)
            {
                _X.Insert(index, item.X);
                _Y.Insert(index, item.Y);
            }

            public void RemoveAt(int index)
            {
                _X.RemoveAt(index);
                _Y.RemoveAt(index);
            }

            public void Add(Xxx item)
            {
                _X.Add(item.X);
                _Y.Add(item.Y);
            }

            public void Clear()
            {
                _X.Clear();
                _Y.Clear();
            }

            public bool Contains(Xxx item)
            {
                return IndexOf(item) != -1;
            }

            public void CopyTo(Xxx[] array, int arrayIndex)
            {
                for (int i = 0; i < Count; i++)
                    array[arrayIndex+i] = this[i];
            }

            public bool Remove(Xxx item)
            {
                int index = IndexOf(item);
                if (index == -1)
                    return false;

                RemoveAt(index);
                return true;
            }

            public IEnumerator<Xxx> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
