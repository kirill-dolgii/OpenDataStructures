using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDataStructures.Collections.Hashing;
public class HashSet<T> : Map<T>, ISet<T>
{
	private  HashSet(IEnumerable<T> data, 
				   IEqualityComparer<T>? comparer, 
				   int initialCapacity) : base(data, comparer, initialCapacity, 0.75) { }
	
	public HashSet() : this(Enumerable.Empty<T>(), EqualityComparer<T>.Default, 32) { }

	public HashSet(IEnumerable<T> data) : this(data, EqualityComparer<T>.Default, 32) { }

	public HashSet(IEnumerable<T> data, 
				   IEqualityComparer<T> comparer) : this(data, comparer, 32) {}

	public HashSet(IEqualityComparer<T> comparer) : this(Enumerable.Empty<T>(), comparer, 32) { }

	public HashSet(int initialCapacity) : this(Enumerable.Empty<T>(), 
											   EqualityComparer<T>.Default, 
											   initialCapacity) { }

	public HashSet(int initialCapacity, 
				   IEqualityComparer<T> comparer) : this(Enumerable.Empty<T>(), comparer, initialCapacity) { }

	protected override int Probe(int index, int step) => (index + 1) % base.Capacity;

	public new bool Add(T item)
	{
		if (item == null) throw new ArgumentNullException();
		if (Contains(item)) return false;
		base.Add(item);
		return true;
	}
	public void ExceptWith(IEnumerable<T> other)
	{
		if (other == null) throw new ArgumentNullException();
		if (!other.Any()) return;
		foreach (var otherItem in other) Remove(otherItem);
	}

	public void IntersectWith(IEnumerable<T> other)
	{
		if (other == null) throw new ArgumentNullException();
		if (!other.Any()) return;
		var contained = other.Distinct(base.Comparer).Where(Contains);
		foreach (var absent in (this as Map<T>).Except(contained, base.Comparer)) Remove(absent);
	}

	public bool IsProperSubsetOf(IEnumerable<T> other) 
		=> this.IsSubsetOf(other) && other.Count() > this.Count;

	public bool IsProperSupersetOf(IEnumerable<T> other)
		=> this.IsSupersetOf(other) && other.Count() < this.Count;

	public bool IsSubsetOf(IEnumerable<T> other)
	{
		if (other == null) throw new ArgumentNullException();
		if (!other.Any()) return false;
		var otherMap = new HashSet<T>(other, base.Comparer, 32);
		if (otherMap.Count < this.Count) return false;
		foreach (T item in this)
			if (!otherMap.Contains(item)) return false;
		return true;
	}

	public bool IsSupersetOf(IEnumerable<T> other)
	{
		if (other == null) throw new ArgumentNullException();
		if (other.Count() > this.Count) return false;
		foreach (var otherItem in other)
			if (!Contains(otherItem)) return false;
		return true;
	}

	public bool Overlaps(IEnumerable<T> other)
	{
		if (other == null) throw new ArgumentNullException();
		if (!other.Any()) return this.Count == 0;
		foreach (var otherItem in other)
			if (Contains(otherItem)) return true;
		return false;
	}

	public bool SetEquals(IEnumerable<T> other)
	{
		if (other == null) throw new ArgumentNullException();
		if (!other.Any()) return this.Count == 0;

		int otherSize = 0;
		foreach (var otherItem in other)
		{
			if (!Contains(otherItem)) return false;
			otherSize++;
		}
		return otherSize == this.Count;
	}

	public void SymmetricExceptWith(IEnumerable<T> other)
	{
		if (other == null) throw new ArgumentNullException();
		if (!other.Any()) return;
		var tmp = new HashSet<T>(other, Comparer);
		tmp.IntersectWith(this);
		foreach (var intersItem in tmp) Remove(intersItem);
		foreach (var otherItem in other.Except(tmp)) Add(otherItem);
	}

	public void UnionWith(IEnumerable<T> other)
	{
		if (other == null) throw new ArgumentNullException();
		if (!other.Any()) return;
		foreach (var otherItem in other) Add(otherItem);
	}
}

