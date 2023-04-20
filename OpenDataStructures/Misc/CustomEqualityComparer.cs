using System;
using System.Collections.Generic;

namespace OpenDataStructures.Misc;

public class CustomEqualityComparer<T> : IEqualityComparer<T>
{
    public readonly Func<T, T, bool> EqualsFunc;
    public readonly Func<T?, int> GetHashCodeFunc;

    public CustomEqualityComparer(Func<T?, T?, bool> equalsFunc, Func<T?, int> getHashCodeFunc)
    {
        EqualsFunc = equalsFunc;
        GetHashCodeFunc = getHashCodeFunc;
    }

    public bool Equals(T? x, T? y)
    {
        return GetHashCodeFunc.Invoke(x) == GetHashCodeFunc.Invoke(y) &&
               EqualsFunc.Invoke(x, y);
    }

    public int GetHashCode(T obj) => GetHashCodeFunc.Invoke(obj);
}