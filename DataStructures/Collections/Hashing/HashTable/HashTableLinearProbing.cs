namespace DataStructures.Collections.Hashing.HashTable;
public class HashTableLinearProbing<TKey, TValue> : HashTableOpenAddressingBase<TKey, TValue>
{
    public HashTableLinearProbing() : base() { }

    public HashTableLinearProbing(IEnumerable<KeyValuePair<TKey, TValue>> data) : base(data) { }

    protected override int Probe(int index, int i) => (index + 1) % Capacity;
}

