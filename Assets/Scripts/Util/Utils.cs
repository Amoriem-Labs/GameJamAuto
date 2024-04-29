using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utils
{

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1); 
            T temp = list[i];        
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
