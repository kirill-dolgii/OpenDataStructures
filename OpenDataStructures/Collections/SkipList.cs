using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenDataStructures.Collections;

public class SkipList<T> : ICollection<T>
{
	private class SkipNode
	{
		public SkipNode(int level)
		{
			this.Value = default;
			this.Forward = new SkipNode[level + 1];
		}

		public SkipNode(T item, int level) : this(level) => this.Value = item;

		public T?          Value;
		public readonly SkipNode?[] Forward;

	}
	private SkipList(SkipNode head, 
					 IEnumerable<T> data, 
					 int initLevel, 
					 Func<T, T, int>? compFunc,
					 IComparer<T>? comparer)
	{
		if (data == null) throw new ArgumentNullException($"{nameof(data)} was null.");

		if (compFunc != null) _compFunc = compFunc;
		else if (comparer != null) _compFunc = comparer.Compare;
		else if (typeof(IComparable).IsAssignableFrom(typeof(T)))
			_compFunc = (item1, item2) => ((IComparable)item1!).CompareTo((IComparable)item2!);
		else _compFunc = Comparer<T>.Default.Compare;

		this._head = head;
		this._maxLevel = initLevel;
		foreach (var item in data) this.Add(item);
	}

	/// <summary>
	/// Creates an instance of the SkipList with default compare function of the provided type parameter.
	/// Initial level of the list is 1.
	/// </summary>
	public SkipList() : this(new(0), Enumerable.Empty<T>(), 0, null, null) {}

	public SkipList(IEnumerable<IComparable> data) : this(new(0), data.Cast<T>(), 0, null, null) {}

	public SkipList(IEnumerable<T> data, Func<T, T, int> compFunc) : this(new(0), data, 0, compFunc, null) {}

	public SkipList(IEnumerable<T> data, IComparer<T> comparer) : this(new(0), data, 0, null, comparer) { }

	private readonly Func<T, T, int> _compFunc;

	private int      _size;
	private SkipNode _head;

	private int _maxLevel;

	public int Count => _size;

	public bool IsReadOnly => false;
	
    private SkipNode[] FindPrecedingNodes(T value, SkipNode startNode, int level)
	{
		if (startNode == null) throw new NullReferenceException();
		var predecessors = new SkipNode[level + 1];

		for (int lvl = level; lvl >= 0; lvl--)
		{
			while (startNode!.Forward[lvl] != null && 
				   this.Compare(startNode.Forward[lvl]!.Value!, value) < 0)
				startNode = startNode.Forward[lvl]!;
			predecessors[lvl] = startNode;
		}
		return predecessors;
	}

	private void AdjustHead(int newMaxLevel)
	{
		_maxLevel = newMaxLevel;
		SkipNode newHead = new SkipNode(_head.Value!, _maxLevel);
		for (int lvl = _head.Forward.Length - 1; lvl >= 0; lvl--)
			newHead.Forward[lvl] = _head.Forward[lvl];
		_head = newHead;
	}

	public bool Remove(T value)
	{
		if (value == null) throw new ArgumentNullException($"{nameof(value)} was null");
		var predecessors = FindPrecedingNodes(value, _head, _maxLevel);
		if (predecessors[0].Forward[0] == null || Compare(predecessors[0].Forward[0]!.Value!, value) != 0) return false;

		for (int i = _maxLevel; i >= 0; i--)
			if (predecessors[i].Forward[i] != null && Compare(predecessors[i].Forward[i]!.Value!, value) == 0)
				predecessors[i].Forward[i] = predecessors[i]!.Forward[i]!.Forward[i];

		_size--;
		return true;
	}

	public void Print()
	{
		var trav = _head;
		for (int i = 0; i <= _maxLevel; i++)
		{
			var sb = new StringBuilder();
			while (trav!.Forward[0] != null)
			{
				var app = i < trav.Forward.Length && trav.Forward[0] != null ? 
							sb.Append($"---{trav.Forward[0]!.Value!.ToString()}") : 
							sb.Append("---**");
				trav = trav.Forward[0];
			}
			Console.WriteLine(sb.ToString());
			trav = _head;
		}
	}

	private int Compare(T value1, T value2) => _compFunc!.Invoke(value1, value2);

	public void Add(T item)
    {
        if (item == null) throw new ArgumentNullException($"{nameof(item)} was null.");
        Random random = new Random();

        int newLevel = 0;
        for (; random.Next() % 2 == 0; newLevel++) { }

        if (newLevel > _maxLevel) AdjustHead(newLevel);

        SkipNode newNode = new(item, newLevel);

        var predecessors = FindPrecedingNodes(item, this._head, _maxLevel);
		for (int lvl = newLevel; lvl >= 0; lvl--)
        {
            newNode.Forward[lvl] = predecessors[lvl]!.Forward[lvl];
            predecessors[lvl]!.Forward[lvl] = newNode;
        }

        _size++;
	}

	public void Clear()
    {
		for (int i = 0; i < _maxLevel; i++) _head.Forward[i] = null;
		_size = 0;
	}

    public bool Contains(T item)
	{
		var predecessors = FindPrecedingNodes(item, _head, _maxLevel);
		if (predecessors[0].Forward[0] == null) return false;
		return Compare(predecessors[0].Forward[0]!.Value!, item) == 0;
	}

    public void CopyTo(T[] array, int arrayIndex)
	{
		if (array == null) throw new ArgumentNullException($"{nameof(array)} was null.");
		if (arrayIndex < 0) throw new ArgumentOutOfRangeException($"{nameof(array)} is below zero.");
		if (arrayIndex > _size)
			throw new ArgumentOutOfRangeException($"{nameof(arrayIndex)} is bigger than the size of the list.");
		if (array.Length < this.Count)
			throw new AggregateException("array size must be greater or equal to the skip list size.");

		var trav = _head.Forward[0];
		//for (int i = 0; i++ <= arrayIndex && trav.Forward[0] != null; trav = trav.Forward[0]) {}
		for (int i = arrayIndex; trav != null; trav = trav.Forward[0]) array[i++] = trav.Value!;
	}

	public IEnumerator<T> GetEnumerator() => new SkipListEnumerator(this);

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private class SkipListEnumerator : IEnumerator<T>
	{
		private readonly SkipList<T> _sl;
		private SkipNode _trav;

		public SkipListEnumerator(SkipList<T> sl) { _sl = sl; _trav = sl._head; }

		public bool MoveNext()
		{
			if (_trav.Forward[0] != null) {_trav = _trav.Forward[0]!; return true; }
			return false;
		}

		public void Reset() => _trav = _sl._head;

		public T Current => _trav!.Value!;

		object IEnumerator.Current => Current!;

		public void Dispose() { }
	}
}