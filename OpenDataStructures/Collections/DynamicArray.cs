using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenDataStructures.Collections;

public class DynamicArray<T> : IList<T>
{
    private int _size;
    private int _capacity;

    private double _resizeScale = 2;

    private const int DefaultCapacity = 8;

    private T[] _storage;

    public DynamicArray()
    {
        _size = 0;
        _capacity = DefaultCapacity;
        _storage = new T[_capacity];
    }

    public DynamicArray(int capacity)
    {
        if (capacity < 0) throw new ArgumentOutOfRangeException($"Illegal capacity {nameof(capacity)}");
        _size = 0;
        _capacity = _size > DefaultCapacity ? capacity : DefaultCapacity;
        _storage = new T[_capacity];
    }

    public DynamicArray(T[] data)
    {
        _size = data.Length;
        _capacity = (int)Math.Pow(2, (int)Math.Ceiling(Math.Log(_size)));

        _storage = new T[_capacity];
        data.CopyTo(_storage, 0);
    }

    private class Enumerator<T> : IEnumerator<T>
    {
        public Enumerator(DynamicArray<T> array) => _array = array;

        private readonly DynamicArray<T> _array;

        public bool MoveNext() => ++_idx < _array._size;

        public void Reset() => _idx = -1;

        private int _idx = -1;

        public T Current => _array[_idx];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }

    public IEnumerator<T> GetEnumerator() => new Enumerator<T>(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator<T>(this);

    private void Resize()
    {
        if (_size == _capacity)
        {
            _capacity *= 2;

            var tmp = new T[_capacity];
            _storage.CopyTo(tmp, 0);
            _storage = tmp;
        }
        else if (_size <= 0.5 * _capacity)
        {
            _capacity = (int)(_capacity / 1.5);

            var tmp = new T[_capacity];
            for (int i = 0; i < _size; i++) tmp[i] = _storage[i];
            _storage = tmp;
        }
    }

    public void Add(T item)
    {
        if (_size == _capacity)
            Capacity = (int)(_capacity * _resizeScale);
        _storage[_size++] = item;
    }

    public void Clear()
    {
        Array.Clear(_storage, 0, _storage.GetUpperBound(0));
        _size = 0;
    }

    public bool Contains(T item)
    {
        if (_size == 0) throw new ArgumentException("Array is empty");
        return _storage.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex) => _storage.CopyTo(array, arrayIndex);

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index == -1) return false;
        RemoveAt(index);
        return true;
    }

    public int Capacity
    {
        get => _capacity;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException($"{nameof(value)} is below zero.");
            if (value < _size)
                throw new ArgumentException($"{nameof(value)} is smaller than " +
                                            $"the size of the array.");
            if (value == _capacity) return;
            if (value < 0)
                throw new ArgumentOutOfRangeException($"{nameof(value)} is below zero.");
            if (value > 0)
            {
                T[] newStorage = new T[_capacity];
                Array.Copy(_storage, newStorage, 0);
                _storage = newStorage;
            }
        }
    }

    public int Count => _size;
    public bool IsReadOnly { get; }

    public int IndexOf(T item)
    {
        for (var i = 0; i < _size; i++)
        {
            if (ReferenceEquals(_storage[i], null)) continue;
            if (_storage[i]!.Equals(item)) return i;
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index < 0 || index > _capacity - 1) throw new IndexOutOfRangeException();
        if (_capacity - _size == 0) Capacity = (int)(Capacity * _resizeScale);
        for (var i = _size - 1; i >= index; i--) _storage[i + 1] = _storage[i];
        _storage[index] = item;
        _size++;
    }

    public void RemoveAt(int index)
    {
        if (index >= _size || index < 0) throw new IndexOutOfRangeException("Index is out of bounds");
        for (var i = index; i < _size - 1; i++) _storage[i] = _storage[i + 1];
        _storage[_size - 1] = default;
        _size--;
        if (_size < Capacity / _resizeScale) Capacity = (int)(Capacity * _resizeScale);
    }

    public T this[int index]
    {
        get
        {
            if (index >= _size || index < 0) throw new IndexOutOfRangeException(nameof(index));
            return _storage[index];
        }
        set
        {
            if (index >= _size || index < 0) throw new IndexOutOfRangeException(nameof(index));
            _storage[index] = value;
        }
    }
}
