using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertUtil
{
    public static List<T> ToList<T>(List<object> objList)
    {
        List<T> TList = new List<T>();
        foreach (object obj in objList)
        {
            TList.Add((T)obj);
        }
        return TList;
    }
}
