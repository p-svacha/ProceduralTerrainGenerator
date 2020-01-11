using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationModel : MonoBehaviour
{
    private RandomNumberGenerator RNG;
    public long Seed;

    void Start()
    {
        Seed = (long)Random.Range(1, ((long)System.Math.Pow(2, 32)) - 1);
        RNG = new RandomNumberGenerator(Seed, (long)System.Math.Pow(2, 32), 1664525, 1013904223);
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Space))
        {
            RandomNumberTester.Test();
        }
    }
}
