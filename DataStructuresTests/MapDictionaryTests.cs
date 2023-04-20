using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDataStructures.Collections.Hashing;
using OpenDataStructures.Tests.Helpers;

namespace OpenDataStructures.Tests;

[TestClass]
public class MapDictionaryTests
{
	private record Person(string Name, int Age);

	private record City(string Name, int Population);

	private IEnumerable<KeyValuePair<Person, City>> _testData;
	private PersonComparer                          _cmp;
	private MapDictionary<Person, City>             _mapDict;

	private readonly Random                                  _rand = new Random(222);

	private string RandomString(int length, Random random)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		return new string(Enumerable.Repeat(chars, length)
									.Select(s => s[random.Next(s.Length)]).ToArray());
	}

	private IEnumerable<KeyValuePair<Person, City>> GenerateTestData(Random rand, int size)
	{
		return Enumerable.Range(0, size).Select(i => new KeyValuePair<Person, City>(
													new Person(RandomString(rand.Next(8, 10), rand), rand.Next(50)),
													new City(RandomString(rand.Next(8, 10), rand), rand.Next(50))));
	}

	private class PersonComparer : IEqualityComparer<Person>
	{
		public bool Equals(Person? x, Person? y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			if (x.GetType() != y.GetType()) return false;
			return x.Name == y.Name && x.Age == y.Age;
		}

		public int GetHashCode(Person obj)
		{
			return (obj.Name.Length * obj.Age) * 333;
		}
	}

	[TestInitialize]
	public void Initialize()
	{
		_testData = GenerateTestData(_rand, 100).ToArray();
		_cmp = new PersonComparer();
		_mapDict = new(_testData, _cmp);
	}

	[TestMethod]
	public void CONSTRUCTION_FROM_ENUMERABLE()
	{
		TestHelperIDictionary.CONSTRUCTION_FROM_ENUMERABLE(Enumerable.Empty<KeyValuePair<Person, City>>(),
														   _mapDict,
														   new Dictionary<Person, City>(_testData));
	}

	[TestMethod]
	public void CONSTRUCTION_BY_ADDING()
	{
		_mapDict = new MapDictionary<Person, City>();

		TestHelperIDictionary.CONSTRUCTION_BY_ADDING(_testData,
													 _mapDict,
													 new Dictionary<Person, City>());
	}

	[TestMethod]
	public void ADD_DUPLICATE_KEY()
	{
		_mapDict = new MapDictionary<Person, City>();
		TestHelperIDictionary.ADD_DUPLICATE_KEY(_testData, _mapDict);
	}

	[TestMethod]
	public void ADD_NULL() => TestHelperIDictionary.ADD_NULL(_mapDict);

	[TestMethod]
	public void REMOVE_NULL() => TestHelperIDictionary.REMOVE_NULL(_mapDict);

	[TestMethod]
	public void REMOVE_ALL()
	{
		_mapDict = new();
		TestHelperIDictionary.REMOVE_ALL(_testData, _mapDict, new Dictionary<Person, City>());
	}

	[TestMethod]
	public void ADD_NULL_KV() => TestHelperIDictionary.ADD_NULL_KV(_mapDict);

	[TestMethod]
	public void ADD_KV_SUCCESSFUL()
	{
		var kv = GenerateTestData(_rand, 1).First();

		TestHelperIDictionary.ADD_KV_SUCCESSFUL(kv, _mapDict);
	}

	[TestMethod]
	public void CONTAINS_KEY_SUCCESSFUL()
	{
		_mapDict = new();
		TestHelperIDictionary.CONTAINS_KEY_SUCCESSFUL(_testData, _mapDict, new Dictionary<Person, City>());
	}

	[TestMethod]
	public void CONTAINS_NULL_KEY() => TestHelperIDictionary.CONTAINS_KEY_NULL(_mapDict);

	[TestMethod]
	public void CONTAINS_NOT_EXISTING_KV()
	{
		_mapDict = new();
		TestHelperIDictionary.
			CONTAINS_KEY_NOT_EXISTING(_mapDict, new KeyValuePair<Person, City>(new Person("ds", 23), null));
	}

	[TestMethod]
	public void REMOVE_KEY_SUCCESSFUL()
	{
		_mapDict = new();
		TestHelperIDictionary.REMOVE_KEY_SUCCESSFUL(_testData, _mapDict);
	}

	[TestMethod]
	public void REMOVE_KEY_NULL() => TestHelperIDictionary.REMOVE_KEY_NULL(_mapDict);

	[TestMethod]
	public void REMOVE_KEY_NOT_EXISTING()
	{
		var nonExisting = new KeyValuePair<Person, City>(new Person("asd", 223), new City("qq1", 22));

		TestHelperIDictionary.REMOVE_KEY_NOT_EXISTING(_mapDict, nonExisting);
	}

	[TestMethod]
	public void GET_VALUE_OPERATOR_SUCCESSFUL() => TestHelperIDictionary.GET_VALUE_OPERATOR_SUCCESSFUL(_testData, _mapDict);

	[TestMethod]
	public void GET_VALUE_OPERATOR_NOT_EXISTING()
	{
		var nonExistingKv = new KeyValuePair<Person, City>(new Person("asd", 2321), new City("11228", 72727));
		TestHelperIDictionary.GET_VALUE_OPERATOR_NOT_EXISTING(_mapDict, nonExistingKv);
	}

	[TestMethod]
	public void SET_VALUE_SUCCESSFUL()
	{
		_mapDict = new();
		TestHelperIDictionary.SET_VALUE_SUCCESSFUL(_testData, _mapDict);
	}

	[TestMethod]
	public void SET_VALUE_NULL() => TestHelperIDictionary.SET_VALUE_NULL(_mapDict);

	[TestMethod]
	public void COPY_TO_SUCCESSFUL() => TestHelperIDictionary.COPY_TO_SUCCESSFUL(_mapDict);

	[TestMethod]
	public void ENUMERATION_SUCCESSFUL()
	{
		_mapDict = new();
		TestHelperIDictionary.ENUMERATION_SUCCESSFUL(_testData, _mapDict);
	}

	[TestMethod]
	public void ENUMERATION_AFTER_REMOVAL()
	{
		_mapDict = new();
		TestHelperIDictionary.ENUMERATION_AFTER_REMOVAL(_testData, _mapDict);
	}

	[TestMethod]
	public void FILL_REMOVE()
	{
		_mapDict = new();
		foreach (var item in _testData) _mapDict.Add(item);
		foreach (var item in _testData) _mapDict.Remove(item);
		foreach (var item in _testData) _mapDict.Add(item);
		Assert.IsTrue(Enumerable.SequenceEqual(_testData, _mapDict));
	}
}
