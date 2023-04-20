using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenDataStructures.Tests.Helpers;

public class TestHelperIDictionary
{
    public static void CONSTRUCTION_FROM_ENUMERABLE<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> testData,
                                                                  IDictionary<TKey, TValue> testingDict,
                                                                  IDictionary<TKey, TValue> sampleDict)
    {
        foreach (var kv in sampleDict)
            Assert.IsTrue(testingDict.Contains(kv) && testingDict.ContainsKey(kv.Key));
        Assert.AreEqual(testingDict.Count, sampleDict.Count);

        Assert.IsTrue(testingDict.SequenceEqual(sampleDict));
    }

    public static void CONSTRUCTION_BY_ADDING<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> testData,
                                                            IDictionary<TKey, TValue> testingDict,
                                                            IDictionary<TKey, TValue> sampleDict)
    {
        foreach (var kv in testData)
        {
            testingDict.Add(kv.Key, kv.Value);
            sampleDict.Add(kv.Key, kv.Value);
        }

        foreach (var kv in testData) Assert.IsTrue(testingDict.ContainsKey(kv.Key) && testingDict.Contains(kv));
        Assert.AreEqual(testingDict.Count, sampleDict.Count);
        Assert.IsTrue(testingDict.SequenceEqual(sampleDict));
    }

    public static void ADD_DUPLICATE_KEY<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> testData,
                                                       IDictionary<TKey, TValue> testingDict)
    {
        testingDict.Add(testData.First());
        Assert.ThrowsException<NotSupportedException>(() => testingDict.Add(testData.First()));
    }

    public static void ADD_NULL<TKey, TValue>(IDictionary<TKey, TValue> testingDict)
    where TKey : class
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
                testingDict.Add(new KeyValuePair<TKey, TValue>(null, default)));
    }

    public static void REMOVE_NULL<TKey, TValue>(IDictionary<TKey, TValue> testingDict)
    where TKey : class
    {
        Assert.ThrowsException<ArgumentNullException>(() => testingDict.Remove(new KeyValuePair<TKey, TValue>(null, default)));
    }

    public static void REMOVE_ALL<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> testData,
                                                IDictionary<TKey, TValue> testingDict,
                                                IDictionary<TKey, TValue> sampleDict)
    {

        foreach (var kv in testData)
        {
            testingDict.Add(kv);
            sampleDict.Add(kv);
        }

        foreach (var kv in testData)
        {
            Assert.AreEqual(testingDict.Remove(kv), sampleDict.Remove(kv));
            Assert.IsFalse(testingDict.Contains(kv));
        }

        Assert.AreEqual(testingDict.Count, 0);
        Assert.IsTrue(Enumerable.Empty<KeyValuePair<TKey, TValue>>().SequenceEqual(testingDict));
    }

    public static void ADD_NULL_KV<TKey, TValue>(IDictionary<TKey, TValue> testingDict)
    where TKey : class =>
        Assert.ThrowsException<ArgumentNullException>(() => testingDict.Add(null, default));

    public static void ADD_KV_SUCCESSFUL<TKey, TValue>(KeyValuePair<TKey, TValue> kv,
                                                       IDictionary<TKey, TValue> testingDict)
    {
        testingDict.Add(kv);
        Assert.IsTrue(testingDict.Contains(kv));
        Assert.IsTrue(testingDict.Remove(kv));
    }

    public static void CONTAINS_KEY_SUCCESSFUL<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> testData,
                                                             IDictionary<TKey, TValue> testingDict,
                                                             IDictionary<TKey, TValue> sampleDict)
    {
        foreach (var kv in testData)
        {
            testingDict.Add(kv);
            sampleDict.Add(kv);
            Assert.AreEqual(testingDict.ContainsKey(kv.Key), sampleDict.ContainsKey(kv.Key));
        }
        foreach (var kv in testData)
            Assert.AreEqual(testingDict.ContainsKey(kv.Key), sampleDict.ContainsKey(kv.Key));

        Assert.AreEqual(testingDict.Count, sampleDict.Count);
    }

    public static void CONTAINS_KEY_NULL<TKey, TValue>(IDictionary<TKey, TValue> testingDict)
    where TKey : class
        => Assert.ThrowsException<ArgumentNullException>(() => testingDict.ContainsKey(null));

    public static void CONTAINS_KEY_NOT_EXISTING<TKey, TValue>(IDictionary<TKey, TValue> testingDict,
                                                               KeyValuePair<TKey, TValue> abcentKv)
        => Assert.IsFalse(testingDict.ContainsKey(abcentKv.Key));

    public static void REMOVE_KEY_SUCCESSFUL<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> testData,
                                                           IDictionary<TKey, TValue> testingDict)
    {
        foreach (var td in testData.Take(5)) testingDict.Add(td);
        foreach (var td in testData.Take(5)) Assert.IsTrue(testingDict.Remove(td.Key));
        foreach (KeyValuePair<TKey, TValue> td in testData) Assert.IsFalse(testingDict.ContainsKey(td.Key));
        Assert.AreEqual(0, testingDict.Count);
    }

    public static void REMOVE_KEY_NULL<TKey, TValue>(IDictionary<TKey, TValue> testingDict)
    where TKey : class
        => Assert.ThrowsException<ArgumentNullException>(() => testingDict.Remove(null));

    public static void REMOVE_KEY_NOT_EXISTING<TKey, TValue>(IDictionary<TKey, TValue> testingDict,
                                                             KeyValuePair<TKey, TValue> nonExistingKv)
        => Assert.IsFalse(testingDict.Remove(nonExistingKv.Key));

    public static void GET_VALUE_OPERATOR_SUCCESSFUL<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> testData,
                                                                   IDictionary<TKey, TValue> testingDict)
    {
        foreach (var kv in testData)
        {
            var valFromDict = testingDict[kv.Key];
            Assert.IsTrue(EqualityComparer<TValue>.Default.Equals(valFromDict, kv.Value));
        }
    }

    public static void GET_VALUE_OPERATOR_NOT_EXISTING<TKey, TValue>(IDictionary<TKey, TValue> testingDict,
                                                              KeyValuePair<TKey, TValue> nonExistingKv)
        => Assert.ThrowsException<KeyNotFoundException>(() => testingDict[nonExistingKv.Key]);

    public static void SET_VALUE_SUCCESSFUL<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> testData,
                                                          IDictionary<TKey, TValue> testingDict)
    {
        foreach (var kv in testData) testingDict[kv.Key] = kv.Value;
        foreach (var kv in testData) Assert.AreEqual(testingDict[kv.Key], kv.Value);
    }

    public static void SET_VALUE_NULL<TKey, TValue>(IDictionary<TKey, TValue> testingDict)
    where TKey : class =>
        Assert.ThrowsException<ArgumentNullException>(() => testingDict[null] = default);

    public static void COPY_TO_SUCCESSFUL<TKey, TValue>(IDictionary<TKey, TValue> testingDict)
    {
        var emptyArray = new KeyValuePair<TKey, TValue>[testingDict.Count];
        testingDict.CopyTo(emptyArray, 0);
        foreach (var kv in emptyArray) Assert.AreEqual(testingDict[kv.Key], kv.Value);
    }

    public static void ENUMERATION_SUCCESSFUL<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> testData,
                                                            IDictionary<TKey, TValue> testingDict)
    {
        foreach (var kv in testData) testingDict.Add(kv);
        Assert.IsTrue(testData.SequenceEqual(testingDict));
    }

    public static void ENUMERATION_AFTER_REMOVAL<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> testData,
                                                        IDictionary<TKey, TValue> testingDict)
    {
        var rand = new Random(22);
        int size = testData.Count() / 2;

        var init = testData.ToList();
        var removed = init.Skip(30).Take(30).ToList();

        init = init.Except(removed).ToList();

        foreach (var kv in testData) testingDict.Add(kv);
        foreach (var kv in removed) Assert.IsTrue(testingDict.Remove(kv));

        Assert.IsTrue(init.SequenceEqual(testingDict));
    }
}