using System;
using System.Linq;

using System.Collections.Generic;
using MyHashSet = OpenDataStructures.Collections.Hashing.HashSet<int>;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDataStructures.Tests.Helpers;

namespace OpenDataStructures.Tests;

[TestClass]
public class HashSetTests
{
	private MyHashSet    _hashSet = null!;
	private HashSet<int> _set     = null!;

	private ICollection<int> _testData;

	[TestInitialize]
	public void Initialize()
	{
		_set     = new HashSet<int>(Enumerable.Range(0, 10));
		_hashSet = new MyHashSet(Enumerable.Range(0, 10));

		Random rand = new(2);
		_testData = Enumerable.Range(0, 100).Select(i => rand.Next()).ToArray();
	}

    #region ICollection member's tests

	[TestMethod]
	public void CONSTRUCTION_FROM_ENUMERABLE() 
		=> TestHelperICollection.CONSTRUCTION_FROM_ENUMERABLE(new MyHashSet(_testData), 
															  _testData.Distinct());

	[TestMethod]
	public void CONSTRUCTION_BY_ADDING() 
		=> TestHelperICollection.CONSTRUCTION_BY_ADDING(new MyHashSet(), _testData);

	[TestMethod]
	public void CONSTRUCTION_NULL_ENUMERABLE() 
		=> TestHelperICollection.CONSTRUCTION_NULL_ENUMERABLE<int>(data => new MyHashSet(data));

	[TestMethod]
	public void ADD_SUCCESSFUL() 
		=> TestHelperICollection.ADD_SUCCESSFUL(new MyHashSet(), 3);

	[TestMethod]
	public void ADD_NULL() 
		=> TestHelperICollection.ADD_NULL(new Collections.Hashing.HashSet<string>());

	[TestMethod]
	public void REMOVE_SUCCESSFUL() 
		=> TestHelperICollection.REMOVE_SUCCESSFUL(_hashSet, _testData);

	[TestMethod]
	public void REMOVE_SUBSEQUENCE_FROM_MIDDLE() 
		=> TestHelperICollection.REMOVE_SUBSEQUENCE_FROM_MIDDLE(new MyHashSet(_testData), _testData);

	[TestMethod]
	public void REMOVE_NULL() 
		=> TestHelperICollection.REMOVE_NULL(new Collections.Hashing.HashSet<string>());

	[TestMethod]
	public void COPY_TO_FROM_0() 
		=> TestHelperICollection.COPY_TO_FROM_0(_hashSet);

	[TestMethod]
	public void COPY_TO_FROM_MIDDLE() 
		=> TestHelperICollection.COPY_TO_FROM_MIDDLE(_hashSet);

	[TestMethod]
	public void REMOVE_NOT_EXISTING() 
		=> TestHelperICollection.REMOVE_NOT_EXISTING(_hashSet, 322);

	#endregion

	#region ISet member's tests

	[TestMethod]
    public void IS_PROPER_SUBSET_OF_SUCCESSFUL()
    {
		Assert.AreEqual(_hashSet.IsProperSubsetOf(Enumerable.Range(0, 100)),
						_set.IsProperSubsetOf(Enumerable.Range(0, 100)));

        Assert.AreEqual(_hashSet.IsProperSubsetOf(Enumerable.Range(0, 5)),
						_set.IsProperSubsetOf(Enumerable.Range(0, 5)));

        Assert.AreEqual(_hashSet.IsProperSubsetOf(Enumerable.Empty<int>()),
						_set.IsProperSubsetOf(Enumerable.Empty<int>()));
    }

	[TestMethod]
	public void IS_PROPER_SUBSET_OF_UNSUCCESSFUL() 
		=> Assert.ThrowsException<ArgumentNullException>(() => _hashSet.IsProperSubsetOf(null!));

	[TestMethod]
    public void INTERSECT_WITH_UNSUCCESSFUL() 
		=> Assert.ThrowsException<ArgumentNullException>(() => _hashSet.IntersectWith(null!));

	[TestMethod]
    public void EXCEPT_WITH_SUCCESSFUL()
    {
        _set.ExceptWith(Enumerable.Range(0, 5));
		_hashSet.ExceptWith(Enumerable.Range(0, 5));

        Assert.IsTrue(_set.SequenceEqual(_hashSet));

        _set.ExceptWith(Enumerable.Empty<int>());
		_hashSet.ExceptWith(Enumerable.Empty<int>());

        Assert.IsTrue(_set.SequenceEqual(_hashSet));
    }

	[TestMethod]
	public void EXCEPT_WITH_UNSUCCESSFUL() 
		=> Assert.ThrowsException<ArgumentNullException>(() => _hashSet.ExceptWith(null!));

	[TestMethod]
	public void IS_PROPER_SUPERSET_OF_SUCCESSFUL()
	{
		var tmp = new System.Collections.Generic.HashSet<int>(Enumerable.Range(0, 10));

		Assert.AreEqual(_hashSet.IsProperSupersetOf(Enumerable.Empty<int>()), 
						tmp.IsProperSupersetOf(Enumerable.Empty<int>()));

		Assert.AreEqual(_hashSet.IsProperSupersetOf(_set.SkipLast(3)), 
						tmp.IsProperSupersetOf(_set.SkipLast(3)));

	}

	[TestMethod]
	public void IS_PROPER_SUPERSET_OF_UNSUCCESSFUL() 
		=> Assert.ThrowsException<ArgumentNullException>(() => _hashSet.IsProperSubsetOf(null!));

	[TestMethod]
	public void OVERLAPS_SUCCESSFUL()
	{
		var tmp = Enumerable.Range(0, 5);
		Assert.AreEqual(_set.Overlaps(tmp), _hashSet.Overlaps(tmp));
		Assert.AreEqual(_set.Overlaps(Enumerable.Empty<int>()), _hashSet.Overlaps(Enumerable.Empty<int>()));
	}

	[TestMethod]
	public void OVERLAPS_UNSUCCESSFUL() 
		=> Assert.ThrowsException<ArgumentNullException>(() => _hashSet.Overlaps(null!));

	[TestMethod]
	public void SET_EQUALS_SUCCESSFUL()
	{
		var tmp = Enumerable.Range(0, 10);
		var tmp1 = Enumerable.Range(0, 5);
		Assert.AreEqual(_set.SetEquals(tmp), _hashSet.SetEquals(tmp));
		Assert.AreEqual(_set.SetEquals(tmp1), _hashSet.SetEquals(tmp1));
		Assert.AreEqual(_set.SetEquals(Enumerable.Empty<int>()), _hashSet.SetEquals(Enumerable.Empty<int>()));
	}

	[TestMethod]
	public void SET_EQUALS_UNSUCCESSFUL() 
		=> Assert.ThrowsException<ArgumentNullException>(() => _hashSet.SetEquals(null!));

	[TestMethod]
	public void SYMMETRIC_EXCEPT_WITH_SUCCESSFUL()
	{
		var tmp = Enumerable.Range(5, 15);
		var tmp1 = Enumerable.Range(6, 3);

		_hashSet.SymmetricExceptWith(tmp);
		_set.SymmetricExceptWith(tmp);

		Assert.IsTrue(_set.SequenceEqual(_hashSet));

		_hashSet.SymmetricExceptWith(tmp1);
		_set.SymmetricExceptWith(tmp1);

		Assert.IsTrue(_set.OrderBy(i => i).SequenceEqual(_hashSet.OrderBy(i => i)));
	}

	[TestMethod]
	public void SYMMETRIC_EXCEPT_WITH_UNSUCCESSFUL() 
		=> Assert.ThrowsException<ArgumentNullException>(() => _hashSet.SymmetricExceptWith(null!));

	[TestMethod]
	public void UNION_WITH_SUCCESSFUL()
	{
		var tmp = Enumerable.Range(9, 10);
		
		_hashSet.UnionWith(tmp);
		_set.UnionWith(tmp);

		Assert.IsTrue(_set.SequenceEqual(_hashSet));

		_hashSet.UnionWith(Enumerable.Empty<int>());
		_set.UnionWith(Enumerable.Empty<int>());

		Assert.IsTrue(_set.SequenceEqual(_hashSet));
	}

	[TestMethod]
	public void UNION_WITH_UNSUCCESSFUL() 
		=> Assert.ThrowsException<ArgumentNullException>(() => _hashSet.UnionWith(null!));

    #endregion
}

