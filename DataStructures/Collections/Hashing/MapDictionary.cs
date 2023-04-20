using DataStructures.Collections.Hashing;

namespace DataStructures;

public class MapDictionary<TKey, TValue> : Map<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
{
	protected override int Probe(int index, int step) => (index + 1) % base.Capacity;

    #region Constructors
	public MapDictionary() : base(Enumerable.Empty<KeyValuePair<TKey, TValue>>(), 
								  new KeyValuePairComparer(EqualityComparer<TKey>.Default), 
								  0, 0.75) {}

	public MapDictionary(IEnumerable<KeyValuePair<TKey, TValue>> data) : base(
		data, new KeyValuePairComparer(EqualityComparer<TKey>.Default), 0, 0.75) {}

	public MapDictionary(IEnumerable<KeyValuePair<TKey, TValue>> data, 
						 IEqualityComparer<TKey> keyComparer) : base(
		data, new KeyValuePairComparer(keyComparer), -1 , 0.75) { }

    public MapDictionary(IEqualityComparer<TKey> keyComparer, int initialCapacity) : base(
		Enumerable.Empty<KeyValuePair<TKey, TValue>>(), 
		new KeyValuePairComparer(keyComparer), 
		initialCapacity, 0.75) {}

	public MapDictionary(IEqualityComparer<TKey> keyComparer) : this(keyComparer, 0) { }

	public MapDictionary(int initialCapacity) : this (EqualityComparer<TKey>.Default, initialCapacity) {}

    #endregion

    /// <summary>
    /// Special IEqualityComparer class that ignores value of the KeyValuePair during
    /// equality check.
    /// </summary>
    private class KeyValuePairComparer : IEqualityComparer<KeyValuePair<TKey, TValue>>
	{
		private readonly IEqualityComparer<TKey> _keyComparer;

		public KeyValuePairComparer(IEqualityComparer<TKey> keyComparer) 
			=> _keyComparer = keyComparer;

		public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) 
			=> _keyComparer.Equals(x.Key, y.Key);

		public int GetHashCode(KeyValuePair<TKey, TValue> obj)
		{
			if (obj.Key == null) throw new ArgumentNullException($"{nameof(obj.Key)} was null.");
			return Math.Abs(_keyComparer.GetHashCode(obj.Key!));
		}
	}

	public void Add(TKey key, TValue value)
	{
		if (key == null) throw new ArgumentNullException($"{nameof(key)} was null.");
		Add(new KeyValuePair<TKey, TValue>(key, value));
	}
    #region Dictionary Members
	public bool ContainsKey(TKey key) => Contains(new KeyValuePair<TKey, TValue>(key, default(TValue)!));

	public bool Remove(TKey key) => Remove(new KeyValuePair<TKey, TValue>(key, default(TValue)!));

	public bool TryGetValue(TKey key, out TValue value) => throw new NotImplementedException();

	public TValue this[TKey key]
	{
		get
		{
			if (key == null) throw new ArgumentNullException($"{nameof(key)} was null");
			var kv = new KeyValuePair<TKey, TValue>(key, default(TValue)!);
			int index = FindItemPos(kv, out int hashCode);
			for (int i = 0;
				 Storage[index] != null && !Storage[index]!.IsThumbStone && i < Capacity;
				 index = Probe(index, i++))
			{
				if (hashCode == Storage[index]!.HashCode &&
					Comparer.Equals(kv, Storage[index]!.Value))
					return Storage[index]!.Value.Value;
			}

			throw new KeyNotFoundException($"{nameof(key)} is not presented in the hash table.");
		}
		set
		{
			if (key == null) throw new ArgumentNullException($"{nameof(key)} was null");
			var kv    = new KeyValuePair<TKey, TValue>(key, default(TValue)!);
			int index = FindItemPos(kv, out int hashCode);
			for (int i = 0; i < Capacity;
				 index = Probe(index, i++))
			{
				if (Storage[index] == null)
				{
					Add(new KeyValuePair<TKey, TValue>(key, value));
					return;
				}
				if (hashCode == Storage[index]!.HashCode &&
					Comparer.Equals(kv, Storage[index]!.Value))
				{
					Storage[index] = new Entity(new KeyValuePair<TKey, TValue>(key, value), hashCode);
					return;
				}
			}
			throw new KeyNotFoundException($"{nameof(key)} is not presented in the hash table.");
		}
	}

	public ICollection<TKey>   Keys   => this.Select(kv => kv.Key).ToArray();
	public ICollection<TValue> Values => this.Select(kv => kv.Value).ToArray();

    #endregion
}

