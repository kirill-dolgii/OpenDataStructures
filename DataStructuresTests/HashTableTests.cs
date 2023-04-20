using System;
using System.Collections.Generic;
using System.Linq;
using OpenDataStructures.Collections.Hashing.HashTable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDataStructures.Tests.Helpers;

namespace OpenDataStructures.Tests;

[TestClass]
public class HashTableTests
{
	private record Person(string Name, int Age);

	private record City(string Name, int Population);

	private IEnumerable<KeyValuePair<Person, City>> _testData;

	//private HashFunction<Person> _hf   = new((pers) => pers.Age * pers.Name.Length * 300000);
	private Random               _rand = new Random(222);

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

	private HashTableSeparateChaining<Person, City>              _htSepCh;
	private HashTableLinearProbing<Person, City> _htLinPr;


	[TestInitialize]
	public void Initialize()
	{
		_testData = GenerateTestData(_rand, 100).ToArray();
		_htSepCh = new(_testData);
		_htLinPr = new(_testData);
	}

	[TestMethod]
	public void CONSTRUCTION_FROM_ENUMERABLE()
	{
		TestHelperIDictionary.CONSTRUCTION_FROM_ENUMERABLE(Enumerable.Empty<KeyValuePair<Person, City>>(),
														   _htLinPr,
														   new Dictionary<Person, City>(_testData));

		TestHelperIDictionary.CONSTRUCTION_FROM_ENUMERABLE(Enumerable.Empty<KeyValuePair<Person, City>>(),
														   _htSepCh,
														   new Dictionary<Person, City>(_testData));
	}

	[TestMethod]
	public void CONSTRUCTION_BY_ADDING()
	{
		_htLinPr  = new HashTableLinearProbing<Person, City>();
		_htSepCh  = new HashTableSeparateChaining<Person, City>();

		TestHelperIDictionary.CONSTRUCTION_BY_ADDING(_testData,
													 _htSepCh,
													 new Dictionary<Person, City>());

		TestHelperIDictionary.CONSTRUCTION_BY_ADDING(_testData,
													 _htLinPr,
													 new Dictionary<Person, City>());
	}

	[TestMethod]
	public void ADD_DUPLICATE_KEY()
	{
        var sepCh = new HashTableSeparateChaining<Person, City>();
        var linPr = new HashTableLinearProbing<Person, City>();

        TestHelperIDictionary.ADD_DUPLICATE_KEY(_testData, sepCh);
		TestHelperIDictionary.ADD_DUPLICATE_KEY(_testData, linPr);
	}

	[TestMethod]
	public void ADD_NULL()
	{
		TestHelperIDictionary.ADD_NULL(_htSepCh);
		TestHelperIDictionary.ADD_NULL(_htLinPr);
	}

	[TestMethod]
	public void REMOVE_NULL()
	{
		TestHelperIDictionary.REMOVE_NULL(_htSepCh);
		TestHelperIDictionary.REMOVE_NULL(_htLinPr);
	}

	[TestMethod]
	public void REMOVE_ALL()
	{
		var sepCh    = new HashTableSeparateChaining<Person, City>();
		var linPr    = new HashTableLinearProbing<Person, City>();

		TestHelperIDictionary.REMOVE_ALL(_testData, linPr, new Dictionary<Person, City>());
		TestHelperIDictionary.REMOVE_ALL(_testData, sepCh, new Dictionary<Person, City>());
	}

	[TestMethod]
	public void ADD_NULL_KV()
	{
		TestHelperIDictionary.ADD_NULL_KV(_htLinPr);
		TestHelperIDictionary.ADD_NULL_KV(_htSepCh);
	}

	[TestMethod]
	public void ADD_KV_SUCCESSFUL()
	{
		var sepCh = new HashTableSeparateChaining<Person, City>();
		var linPr = new HashTableLinearProbing<Person, City>();

		var kv = GenerateTestData(_rand, 1).First();

		TestHelperIDictionary.ADD_KV_SUCCESSFUL(kv, linPr);
		TestHelperIDictionary.ADD_KV_SUCCESSFUL(kv, sepCh);
	}

	[TestMethod]
	public void CONTAINS_KEY_SUCCESSFUL()
	{
		var sepCh = new HashTableSeparateChaining<Person, City>();
		var linPr = new HashTableLinearProbing<Person, City>();
		
		TestHelperIDictionary.CONTAINS_KEY_SUCCESSFUL(_testData, linPr, new Dictionary<Person, City>());
		TestHelperIDictionary.CONTAINS_KEY_SUCCESSFUL(_testData, sepCh, new Dictionary<Person, City>());
	}

	[TestMethod]
	public void CONTAINS_NULL_KEY()
	{
		TestHelperIDictionary.CONTAINS_KEY_NULL(_htSepCh);
		TestHelperIDictionary.CONTAINS_KEY_NULL(_htLinPr);
	}

	[TestMethod]
	public void CONTAINS_NOT_EXISTING_KV()
	{
		var sepCh = new HashTableSeparateChaining<Person, City>();
		var linPr = new HashTableLinearProbing<Person, City>();

		TestHelperIDictionary.
			CONTAINS_KEY_NOT_EXISTING(sepCh, new KeyValuePair<Person, City>(new Person("ds", 23), null));
		TestHelperIDictionary.
			CONTAINS_KEY_NOT_EXISTING(linPr, new KeyValuePair<Person, City>(new Person("ds", 23), null));
	}

	[TestMethod]
	public void REMOVE_KEY_SUCCESSFUL()
	{
		var sepCh = new HashTableSeparateChaining<Person, City>();
		var linPr = new HashTableLinearProbing<Person, City>();

		TestHelperIDictionary.REMOVE_KEY_SUCCESSFUL(_testData, linPr);
		TestHelperIDictionary.REMOVE_KEY_SUCCESSFUL(_testData, sepCh);
	}

	[TestMethod]
	public void REMOVE_KEY_NULL()
	{
		var sepCh = new HashTableSeparateChaining<Person, City>();
		var linPr = new HashTableLinearProbing<Person, City>();

		TestHelperIDictionary.REMOVE_KEY_NULL(sepCh);
		TestHelperIDictionary.REMOVE_KEY_NULL(linPr);

	}

	[TestMethod]
	public void REMOVE_KEY_NOT_EXISTING()
	{
		var nonExisting = new KeyValuePair<Person, City>(new Person("asd", 223), new City("qq1", 22));

		TestHelperIDictionary.REMOVE_KEY_NOT_EXISTING(_htLinPr, nonExisting);
		TestHelperIDictionary.REMOVE_KEY_NOT_EXISTING(_htSepCh, nonExisting);
	}

	[TestMethod]
	public void GET_VALUE_OPERATO_SUCCESSFUL()
	{
		TestHelperIDictionary.GET_VALUE_OPERATOR_SUCCESSFUL(_testData, _htLinPr);
		TestHelperIDictionary.GET_VALUE_OPERATOR_SUCCESSFUL(_testData, _htSepCh);
	}

	[TestMethod]
	public void GET_VALUE_OPERATOR_SUCCESSFUL()
	{
		var nonExistingKv = new KeyValuePair<Person, City>(new Person("asd", 2321), new City("11228", 72727));

		TestHelperIDictionary.GET_VALUE_OPERATOR_NOT_EXISTING(_htLinPr, nonExistingKv);
		TestHelperIDictionary.GET_VALUE_OPERATOR_NOT_EXISTING(_htSepCh, nonExistingKv);
	}

	[TestMethod]
	public void SET_VALUE_SUCCESSFUL()
	{
		TestHelperIDictionary.SET_VALUE_SUCCESSFUL(_testData, _htLinPr);
		TestHelperIDictionary.SET_VALUE_SUCCESSFUL(_testData, _htSepCh);
	}

	[TestMethod]
	public void SET_VALUE_NULL()
	{
		TestHelperIDictionary.SET_VALUE_NULL(_htLinPr);
		TestHelperIDictionary.SET_VALUE_NULL(_htSepCh);
	}

	[TestMethod]
	public void COPY_TO_SUCCESSFUL()
	{
		TestHelperIDictionary.COPY_TO_SUCCESSFUL(_htLinPr);
		TestHelperIDictionary.COPY_TO_SUCCESSFUL(_htSepCh);
	}

	[TestMethod]
	public void ENUMERATION_SUCCESSFUL()
	{
		var htLinPr = new HashTableLinearProbing<Person, City>();
		var htSepCh = new HashTableSeparateChaining<Person, City>();

		TestHelperIDictionary.ENUMERATION_SUCCESSFUL(_testData, htLinPr);
		TestHelperIDictionary.ENUMERATION_SUCCESSFUL(_testData, htSepCh);
	}

	[TestMethod]
	public void ENUMERATION_AFTER_REMOVAL()
	{
		var htLinPr = new HashTableLinearProbing<Person, City>();
		var htSepCh = new HashTableSeparateChaining<Person, City>();

		TestHelperIDictionary.ENUMERATION_AFTER_REMOVAL(_testData, htLinPr);
		TestHelperIDictionary.ENUMERATION_AFTER_REMOVAL(_testData, htSepCh);
	}

}
