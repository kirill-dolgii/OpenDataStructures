using System.Collections;

namespace DataStructures.Collections.Hashing.HashTable;

public abstract class HashTableOpenAddressingBase<TKey, TValue> : IDictionary<TKey, TValue>
{
    private class KeyValuePairEntity
    {
        public bool IsDeleted;
        public readonly KeyValuePair<TKey, TValue> Kv;
        public readonly int HashCode;

        public KeyValuePairEntity(KeyValuePair<TKey, TValue> kv, int hashCode, bool isDeleted)
        {
            Kv = kv;
            IsDeleted = isDeleted;
            HashCode = hashCode;
        }
    }

    private const int DEFAILT_CAPACITY = 4;
    private const double DEFAULT_LOAD_FACTOR = 0.75;
    private const double RESIZE_SCALE = 0.65;

    private readonly IEqualityComparer<TKey> _keyCmp = EqualityComparer<TKey>.Default;
    private readonly double _loadFactor;

    private KeyValuePairEntity?[] _entities;

    private int _size;
    protected int Capacity;
    private List<KeyValuePairEntity> _added;

    private protected HashTableOpenAddressingBase(IEnumerable<KeyValuePair<TKey, TValue>> data,
                                        int initialCapacity,
                                        double loadFactor)
    {
        if (loadFactor <= 0 || loadFactor > 1) throw new ArgumentException(nameof(loadFactor));
        if (data == null) throw new ArgumentNullException(nameof(data));

        _loadFactor = loadFactor;
        Capacity = new int[] { DEFAILT_CAPACITY, initialCapacity,
                                (int)(data.Count() / _loadFactor) }.Max();

        _entities = new KeyValuePairEntity?[Capacity];
        _added = new List<KeyValuePairEntity>(Capacity);

        foreach (var kv in data) Add(kv);
    }

    private protected HashTableOpenAddressingBase() : this(Enumerable.Empty<KeyValuePair<TKey, TValue>>(),
                                                           DEFAILT_CAPACITY,
                                                           DEFAULT_LOAD_FACTOR)
    { }

    private protected HashTableOpenAddressingBase(IEnumerable<KeyValuePair<TKey, TValue>> data) : this(data,
                                                                                                       DEFAILT_CAPACITY,
                                                                                                       DEFAULT_LOAD_FACTOR)
    { }

    private int AdjustedHash(TKey key, out int hashCode)
    {
        if (key == null) throw new NullReferenceException();
        hashCode = Math.Abs(_keyCmp?.GetHashCode(key) ?? key.GetHashCode());
        return hashCode % Capacity;
    }

    protected abstract int Probe(int index, int i);

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        if (item.Key == null) throw new ArgumentNullException($"{nameof(item.Key)} was null.");
        if (ContainsKey(item.Key))
            throw new NotSupportedException($"An element with {nameof(item.Key)} key already exists");

        if (_size >= Capacity * _loadFactor) Resize((int)(Capacity / RESIZE_SCALE));

        int index = AdjustedHash(item.Key, out int hashCode);

        for (var i = 0; _entities[index] != null && !_entities[index]!.IsDeleted; index = Probe(index, i++)) { }

        KeyValuePairEntity entity = new(item, hashCode, false);

        _entities[index] = entity;
        _added.Add(entity);
        _size++;
    }

    private void Resize(int newCapacity)
    {
        var existing = _added.Where(ent => ent is { IsDeleted: false }).ToArray();

        Capacity = newCapacity;

        _entities = new KeyValuePairEntity?[Capacity];
        _added = new List<KeyValuePairEntity>(Capacity);
        _size = 0;

        foreach (var ex in existing) Add(ex.Kv);
    }

    public void Clear()
    {
        _entities = new KeyValuePairEntity[Capacity];
        _added = new List<KeyValuePairEntity>(Capacity);
        _size = 0;
    }

    private int FindEntityIndex(TKey key, out int hc)
    {
        int index = AdjustedHash(key, out int hashCode);
        hc = hashCode;
        if (_entities[index] == null) return -1;

        for (int i = 0; _entities[index] != null && i <= Capacity; index = Probe(index, i++))
            if (hashCode == _entities[index]!.HashCode && _keyCmp.Equals(_entities[index]!.Kv.Key, key))
            {
                if (!_entities[index]!.IsDeleted) return index;
                return -1;
            }

        return -1;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException("array");
        if (arrayIndex < 0) throw new ArgumentException("arrayIndex");
        _added.Where(ent => !ent.IsDeleted).Select(ent => ent.Kv).ToArray().CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

    public int Count { get => _size; }
    public bool IsReadOnly { get => false; }
    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key))
            throw new NotSupportedException($"{nameof(key)} is already presented in the hash table");

        Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null) throw new ArgumentNullException($"{nameof(key)} was null.");
        int index = FindEntityIndex(key, out _);
        return index != -1;
    }

    public bool Remove(TKey key)
    {
        if (key == null) throw new ArgumentNullException($"{nameof(key)} was null.");
        int index = FindEntityIndex(key, out _);

        if (index == -1) return false;

        _entities[index]!.IsDeleted = true;

        if (--_size <= Capacity * _loadFactor * RESIZE_SCALE && _size > DEFAILT_CAPACITY)
            Resize((int)(Capacity * RESIZE_SCALE));

        return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        throw new NotImplementedException();
    }

    public TValue this[TKey key]
    {
        get
        {
            if (key == null) throw new ArgumentNullException($"{nameof(key)} was null");
            int index = FindEntityIndex(key, out _);
            if (index == -1) throw new KeyNotFoundException($"{nameof(key)} is not presented in the hash table.");

            return _entities[index]!.Kv.Value;
        }
        set
        {
            if (key == null) throw new ArgumentNullException($"{nameof(key)} was null");
            int index = FindEntityIndex(key, out int hashCode);
            if (index == -1) throw new KeyNotFoundException($"{nameof(key)} is not presented in the hash table.");
            _entities[index] = new(new KeyValuePair<TKey, TValue>(key, value), hashCode, false);
        }
    }

    public ICollection<TKey> Keys => _added.Where(ent => !ent.IsDeleted).Select(ent => ent.Kv.Key).ToList();
    public ICollection<TValue> Values => _added.Where(ent => !ent.IsDeleted).Select(ent => ent.Kv.Value).ToList();

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => _added.Where(ent => !ent.IsDeleted).Select(ent => ent.Kv).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

