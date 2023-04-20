using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenDataStructures.Tests.Helpers;
public static class TestHelperICollection
{
    public static void CONSTRUCTION_FROM_ENUMERABLE<T>(ICollection<T> enumConstructedCollection,
                                                       IEnumerable<T> orderedData)
    {
        Assert.AreEqual(orderedData.Count(), enumConstructedCollection.Count);
        Assert.IsTrue(orderedData.SequenceEqual(enumConstructedCollection));
        foreach (var item in orderedData)
            Assert.IsTrue(enumConstructedCollection.Contains(item));
    }

    public static void CONSTRUCTION_BY_ADDING<T>(ICollection<T> emptyCollection,
                                                 IEnumerable<T> orderedData)
    {
        foreach (var item in orderedData) emptyCollection.Add(item);
        Assert.IsTrue(orderedData.SequenceEqual(emptyCollection));
        foreach (var item in orderedData) Assert.IsTrue(emptyCollection.Contains(item));
        Assert.AreEqual(orderedData.Count(), emptyCollection.Count);
    }

    public static void CONSTRUCTION_NULL_ENUMERABLE<T>(Func<IEnumerable<T>,
                                                       ICollection<T>> creator)
        => Assert.ThrowsException<ArgumentNullException>(() => creator.Invoke(null!));

    public static void ADD_SUCCESSFUL<T>(ICollection<T> testingCollection, T sampleAddition)
    {
        Assert.IsFalse(testingCollection.Contains(sampleAddition));
        testingCollection.Add(sampleAddition);
        Assert.IsTrue(testingCollection.Contains(sampleAddition));
    }

    public static void ADD_NULL<T>(ICollection<T> testingCollection)
    where T : class
        => Assert.ThrowsException<ArgumentNullException>(() => testingCollection.Add(null!));

    public static void REMOVE_SUCCESSFUL<T>(ICollection<T> testingCollection,
                                            IEnumerable<T> testData)
    {
        Assert.IsFalse(testingCollection.Contains(testData.First()));
        testingCollection.Add(testData.First());
        Assert.IsTrue(testingCollection.Contains(testData.First()));
        Assert.IsTrue(testingCollection.Remove(testData.First()));
    }

    public static void REMOVE_SUBSEQUENCE_FROM_MIDDLE<T>(ICollection<T> testingCollection,
                                                         IEnumerable<T> testData)
    {
        var count = testData.Count();
        Assert.IsTrue(testingCollection.SequenceEqual(testData));
        var toRemove = testData.Skip((int)(count * 0.25)).Take((int)(count * 0.5)).ToArray();
        foreach (var item in toRemove)
            Assert.IsTrue(testingCollection.Remove(item));
        Assert.IsTrue(testingCollection.SequenceEqual(testData.Except(toRemove)));
    }

    public static void REMOVE_NULL<T>(ICollection<T> testingCollection)
    where T : class
        => Assert.ThrowsException<ArgumentNullException>(() => testingCollection.Remove(null!));

    public static void COPY_TO_FROM_0<T>(ICollection<T> testCollection)
    {
        var testArray = new T[testCollection.Count];
        testCollection.CopyTo(testArray, 0);
        Assert.IsTrue(testCollection.SequenceEqual(testArray));
    }
    public static void COPY_TO_FROM_MIDDLE<T>(ICollection<T> testCollection)
    {
        int index = testCollection.Count / 2;
        var testArray = new T[testCollection.Count + index];
        testCollection.CopyTo(testArray, index);
        Assert.IsTrue(testCollection.SequenceEqual(testArray.Skip(index)));
    }

    public static void REMOVE_NOT_EXISTING<T>(ICollection<T> testCollection,
                                              T notExisting)
    {
        Assert.IsFalse(testCollection.Contains(notExisting));
        Assert.IsFalse(testCollection.Remove(notExisting));
    }
}
