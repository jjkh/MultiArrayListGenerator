using System.Collections;

public sealed class EquatableDictionary<TKey, TValue>
    : IDictionary<TKey, TValue>, IEquatable<EquatableDictionary<TKey, TValue>>
{
    private readonly Dictionary<TKey, TValue> _dictionary;

    public EquatableDictionary()
    {
        _dictionary = [];
    }

    public EquatableDictionary(Dictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
    }

    public override bool Equals(object other) => Equals(other as EquatableDictionary<TKey, TValue>);

    public bool Equals(EquatableDictionary<TKey, TValue>? other)
    {
        if (other is null || Count != other.Count)
            return false;
        
        foreach (var pair in this)
        {
            if (!other.TryGetValue(pair.Key, out var otherValue))
                return false;

            if (!EqualityComparer<TValue>.Default.Equals(pair.Value, otherValue))
                return false;
        }
        return true;
    }

    public override int GetHashCode()
    {
        int hash = 0;
        foreach (var pair in this)
        {
            int miniHash = 17;
            miniHash = miniHash * 31 +
                   EqualityComparer<TKey>.Default.GetHashCode(pair.Key);
            miniHash = miniHash * 31 +
                   EqualityComparer<TValue>.Default.GetHashCode(pair.Value);
            hash ^= miniHash;
        }
        return hash;
    }

    #region Implementation delegated to _dictionary
    public TValue this[TKey key] { get => ((IDictionary<TKey, TValue>)_dictionary)[key]; set => ((IDictionary<TKey, TValue>)_dictionary)[key] = value; }

    public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_dictionary).Keys;

    public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_dictionary).Values;

    public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;

    public void Add(TKey key, TValue value)
    {
        ((IDictionary<TKey, TValue>)_dictionary).Add(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
    }

    public void Clear()
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
        return ((IDictionary<TKey, TValue>)_dictionary).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        return ((IDictionary<TKey, TValue>)_dictionary).Remove(key);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return ((IDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }

#endregion
}
