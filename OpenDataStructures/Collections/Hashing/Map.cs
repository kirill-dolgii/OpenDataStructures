using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenDataStructures.Collections.Hashing;
public abstract class Map<T> : ICollection<T>
{
    private const int DEFAULT_CAPACITY = 32;
    private const double DEFAULT_LOAD_FACTOR = 0.75;
    private const double RESIZE_SCALE = 0.5;

    private readonly double _loadFactor;

    private int _size;
    private int _capacity;

    protected Entity?[] Storage;
    private List<Entity> _added;

    protected readonly IEqualityComparer<T> Comparer;

    protected Map(IEnumerable<T> data,
                  IEqualityComparer<T>? comparer,
                  int initialCapacity,
                  double loadFactor)
    {
        if (loadFactor < 0 || _loadFactor > 1)
            throw new ArgumentException($"{nameof(loadFactor)} must be between 0 and 1.");
        _loadFactor = loadFactor;

        Comparer = comparer ?? EqualityComparer<T>.Default;

        _capacity = new[] { initialCapacity, (int)(data.Count() / _loadFactor), DEFAULT_CAPACITY }.Max();

        Storage = new Entity?[_capacity];
        _added = new List<Entity?>(_capacity);

        foreach (var item in data) Add(item);
    }

    private int GetHash(T item)
    {
        if (item == null) throw new ArgumentNullException($"{nameof(item)} was null.");
        return Math.Abs(Comparer?.GetHashCode(item) ?? item.GetHashCode());
    }

    protected abstract int Probe(int index, int step);

    protected int FindItemPos(T item, out int hashCode)
    {
        if (item == null) throw new ArgumentNullException($"{nameof(item)} was null.");
        hashCode = GetHash(item);
        int index = hashCode % Capacity;
        return index;
    }

    public void Add(T item)
    {
        if (item == null) throw new ArgumentNullException($"{nameof(item)} was null.");
        if (_size >= (int)(Capacity * _loadFactor)) Capacity = (int)(Capacity / RESIZE_SCALE);

        int index = FindItemPos(item, out var hashCode);
        for (int i = 0;
             Storage[index] != null && !Storage[index]!.IsThumbStone && i < Capacity;
             index = Probe(index, i))
        {
            if (hashCode == Storage[index]!.HashCode &&
                Comparer.Equals(Storage[index]!.Value, item))
                throw new NotSupportedException($"{nameof(item)} key is already " +
                                                $"presented in the dictionary.");
        }

        Entity entity = new(item, hashCode);
        Storage[index] = entity;
        _added.Add(entity);

        _size++;
    }

    public void Clear()
    {
        Storage = new Entity?[Capacity];
        _added = new List<Entity?>(Capacity);
        _size = 0;
    }

    public bool Contains(T item)
    {
        if (item == null) throw new ArgumentNullException($"{nameof(item)} was null.");
        int index = FindItemPos(item, out int hashCode);
        for (int i = 0; Storage[index] != null && i < Capacity; index = Probe(index, i))
        {
            if (hashCode == Storage[index]!.HashCode &&
                Comparer.Equals(Storage[index]!.Value, item))
                return !Storage[index]!.IsThumbStone;
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException($"{nameof(array)} was null.");
        if (arrayIndex < 0 || arrayIndex > Capacity - 1)
            throw new ArgumentOutOfRangeException(
                $"{nameof(arrayIndex)} must be positive and less than the number of elements.");
        if (Count == 0) return;

        _added.Where(entity => entity != null && !entity!.IsThumbStone)
              .Select(entity => entity!.Value).ToArray()
              .CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        if (item == null) throw new ArgumentNullException($"{nameof(item)} was null.");

        int index = FindItemPos(item, out var hashCode);
        for (int i = 0; Storage[index] != null && i < Capacity; index = Probe(index, i))
        {
            if (hashCode == Storage[index]!.HashCode &&
                Comparer.Equals(Storage[index]!.Value, item))
            {
                Storage[index]!.IsThumbStone = true;

                _size--;

                if (_capacity > DEFAULT_CAPACITY && _size <= (int)(Capacity * _loadFactor * RESIZE_SCALE))
                    Capacity = (int)(_size / _loadFactor * 1.25);

                return true;
            }
        }

        return false;
    }

    public int Count => _size;
    public bool IsReadOnly => false;

    public int Capacity
    {
        get => _capacity;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            if (value == _capacity) return;

            _capacity = value;
            _size = 0;

            var items = _added.Where(entity => !entity!.IsThumbStone)
                              .Select(entity => entity!.Value).ToArray();
            Storage = new Entity?[_capacity];
            _added = new List<Entity?>(_capacity);

            foreach (var item in items) Add(item);
        }
    }

    protected class Entity
    {
        public readonly T Value;
        public bool IsThumbStone = false;
        public readonly int HashCode;

        public Entity(T value, int hashCode)
        {
            Value = value;
            HashCode = hashCode;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        var ret = _added.Where(entity => !entity!.IsThumbStone).Select(entity => entity!.Value);
        return ret.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}


