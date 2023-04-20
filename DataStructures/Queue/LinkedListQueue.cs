namespace DataStructures.Queue;

public class LinkedListQueue<T> : IQueue<T>
{
	private class Node
	{
		public Node(T value)
		{
			this.Value = value;
			this.Next = null;
		}

		public readonly T    Value;
		public          Node? Next;
	}

    public LinkedListQueue()
    {
        this._head = null;
        this._tail = this._head;
    }

    public LinkedListQueue(T first)
	{
		this._head = new(first);
		this._tail = this._head;
		this._size = 1;
	}

	private Node? _head;
	private Node? _tail;

	private int _size;

	public void Enqueue(T item)
	{
		this._size++;
		if (this._head == null)
		{
			this._head = new(item);
			this._tail = this._head;
			return;
		}

		this._tail!.Next = new(item);
		this._tail = this._tail.Next;
	}

	public T Dequeue()
	{
		if(this._head == null) throw new InvalidOperationException("the queue is empty");

		this._size--;
		T ret = this._head.Value;
		this._head = this._head.Next;
		return ret;
	}

	public T Peek()
	{
		if (this._head == null) throw new InvalidOperationException("the queue is empty");
		return this._head.Value;
	}

	public bool IsEmpty() => this._head == null;

	public int Count() => this._size;
}