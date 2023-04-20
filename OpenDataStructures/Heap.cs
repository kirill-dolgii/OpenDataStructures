// Author : Dolghi Chirill, 07.03.2022
//-------------------------------------------------------------------------------------------
// this data structure implements priority queue -> heap (data is stored inside of an array).
// both min and max queues are supported.
// priority queue operations: insert O(log(n) and extractHead O(log(N)) are supported.
// both O(n) and O(log(N)) constructors are implemented.

using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDataStructures;

public class Heap<T>
	where T : IComparable<T>
{
	private T[] _container;
	private uint _capacity;

	private uint _size = 0;

	private const uint DefaultCapacity = 10;

	private readonly bool _isMinHeap;

	public readonly IComparer<T>? Comparer = null;

	public Heap() : this(DefaultCapacity) { }

	public Heap(uint capacity, bool isMinHeap = false)
	{
		this._isMinHeap = isMinHeap;

		this._capacity = capacity;
		_container = new T[capacity];
	}

	/// <summary>
	/// O(n * logN) heap constructor.
	/// </summary>
	/// <param name="data">Initial sequence to construct a heap of.</param>
	/// <param name="isMinHeap">Specify the priority of a heap. Min heap means that small items have higher priority</param>
	public Heap(T[] data, bool isMinHeap)
	{
		this._isMinHeap = isMinHeap;

		_container = new T[data.Length];
		_capacity = (uint)data.Length;

		foreach (var item in data) Insert(item);
	}

	// O(n) heap constructor (bubble down all nodes from the half untill the beginning)
	public Heap(IEnumerable<T> data, bool isMinHeap = false, IComparer<T>? comparer = null)
	{
		if (data == null) throw new ArgumentNullException(nameof(data));

		if (comparer != null) this.Comparer = comparer;

		this._isMinHeap = isMinHeap;
		var size = data.Count();

		_container = new T[size];

		var enumerator = data.GetEnumerator();
		for (int i = 0; i < size; i++)
		{
			enumerator.MoveNext();
			_container[i] = enumerator.Current;
		}

		enumerator.Dispose();

		_capacity = (uint)size;
		_size = (uint)size;

		for (var i = (int)_size / 2 - 1; i >= 0; i--)
			BubbleDown((uint)i);
	}
	
	public bool IsEmpty()
	{
		return _size == 0;
	}

	public void Clear()
	{
		_container = new T[_capacity < DefaultCapacity ? DefaultCapacity : _capacity];
		_size = 0;
	}

	public uint Size()
	{
		return _size;
	}

	public T Peek()
	{
		return _container[0];
	}

	public bool Contains(T elem)
	{
		return _container.Contains(elem);
	}

	// O(log(N)) 
	public void Insert(T item)
	{
		if (_size >= _capacity)
			throw new IndexOutOfRangeException("not enough space");

		_container[_size++] = item;
		BubbleUp(_size - 1);
	}

	public bool Remove(T elem)
	{
		var elemPos = -1;

		foreach (var item in _container)
		{
			elemPos++;
			if (item.CompareTo(elem) == 0)
				break;
		}

		if (elemPos >= _size - 1)
			return false;

		Swap((uint)elemPos, _size - 1);
		Swap((uint)_size - 1, _capacity-- - 1);
        BubbleDown((uint)elemPos);

		return true;
	}

	public T ExtractHead()
	{
		var ret = _container[0];
		Remove(ret);
		_size--;
		return ret;
	}

	private void BubbleDown(uint index)
	{
		if (2 * index + 1 >= _size)
			return;

		var minIdx = index;

		for (var i = 1; i <= 2 && index * 2 + i < _size - 1; i++)
			if (Compare(_container[minIdx], _container[index * 2 + i]))
				minIdx = (uint)(index * 2 + i);
		if (minIdx != index)
		{
			Swap(index, minIdx);
			BubbleDown(minIdx);
		}
	}

	private void BubbleUp(uint index)
	{
		if (index == 0 || index >= _size)
			return;

		var parent = (index - 1) / 2;

		if (Compare(_container[parent], _container[index]))
		{
			Swap(index, parent);
			BubbleUp(parent);
		}
	}

	private void Swap(uint item1, uint item2)
	{
		if (item1 == item2) return;
		(_container[item1], _container[item2]) = (_container[item2], _container[item1]);
	}

	private bool Compare(T item1, T item2)
	{
		if (this.Comparer == null)
			return item1.CompareTo(item2) * (_isMinHeap ? 1 : -1) > 0;

		return Comparer.Compare(item1, item2) * (_isMinHeap ? 1 : -1) > 0;
	}
}