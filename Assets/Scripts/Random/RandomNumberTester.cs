using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class is used to check if the RandomNumberGenerator Class actually produces random numbers. Run RandomNumberTester.Test() to test.
/// </summary>
public static class RandomNumberTester
{

    public static void Test()
    {
        long Seed;
        Dictionary<int, int> dic = new Dictionary<int, int>();

        for (int r = 0; r < 100; r++)
        {
            Seed = Random.Range(0, 10000000);
            RandomNumberGenerator RNG = new RandomNumberGenerator(Seed, (long)System.Math.Pow(2, 32), 1664525, 1013904223);
            for (int i = 0; i < 100000; i++)
            {
                int x = (int)(RNG.Next() * 1000);

                if (dic.ContainsKey(x)) dic[x]++;
                else dic.Add(x, 1);
            }
        }

        Debug.Log("-------------------------------RESULTS------------------------------------");
        var ordered = dic.OrderByDescending(x => x.Value);
        foreach(KeyValuePair<int, int> kvp in ordered)
        {
            Debug.Log(kvp.Key + ": " + kvp.Value);
        }
    }
}
