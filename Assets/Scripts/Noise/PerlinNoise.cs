using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise
{

    public static float[,] GenerateNoiseMap(int mapChunkSize, RandomNumberGenerator rng, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        // Create a Perlin Noise 2D Field


        // Init values
        if (scale <= 0) scale = 0.0001f;
        float[,] noiseMap = new float[mapChunkSize, mapChunkSize];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfSize = mapChunkSize / 2f;

        // Create random octave offsets
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-100000, 100000) + offset.x;
            float offsetY = rng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // Create noise map
        for(int y = 0; y < mapChunkSize; y++)
        {
            for(int x = 0; x < mapChunkSize; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // Generate value for each pixel going through n octaves with x persistance and y lacunarity
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (float)(x - halfSize) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (float)(y - halfSize) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize noise map to [0,1]
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    /*
    public float[,] PerlinField(int mapWidth, int mapHeight)
    {

    }*/
}
