using System;

namespace OpenDataStructures.Queue;

public class ArrayQueue<T> : IQueue<T>
{
	public ArrayQueue(int capacity)
	{
		this._storage = new T[capacity];
		this._capacity = capacity;
		this._size = 0;
	}

	private          int _size;
	private readonly int _capacity;

	private readonly T[] _storage;

	private int _front = 0;
	private int _back = 0;

	// if elements have exceeded the right bound of the container update the index 
	// (just like in roulette - the ball makes several rounds and stops at the position that is between the bounds)
	private int AdjustIndex(int index)
	{
		var ret = index >= this._capacity ? index % this._capacity : index;
		return ret;
	}

	public void Enqueue(T item)
	{
		if (this._capacity == this._size) throw new InvalidOperationException("Queue is full");
		this._back = this.AdjustIndex(this._back);
		this._storage[this._back++] = item;
		this._size++;
	}

	public T Dequeue()
	{
		if (this._storage.Length == 0) throw new InvalidOperationException("Queue is empty");
		this._front = this.AdjustIndex(this._front++);
		T ret =  this._storage[this._front++];
		this._size--;
		return ret;
	}

	public T Peek() => this._storage[this._front];

	public bool IsEmpty() => this._size == 0;

	public int Count() => this._size;
}