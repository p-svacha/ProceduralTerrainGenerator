using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{

    public static float[,] GenerateNoiseMapWithUnityPerlin(int mapChunkSize, int worldMapWidth, int worldMapHeight, RandomNumberGenerator rng, 
        float scale = 500f, 
        int octaves = 5, 
        float persistance = 0.5f, 
        float lacunarity = 2f, 
        float seaLevel = 0.4f, 
        HeightCurveOperation heightCurveOperation = HeightCurveOperation.Exponential,
        bool useDynamicHeightExtremes = true, // if true, map will always have a very deep ocean and a very high mountain
        float minNoiseHeight = -1.5f,
        float maxNoiseHeight = 1.5f)
    {

        // Init values
        int worldWidth = ((mapChunkSize - 1) * worldMapWidth) + 1;
        int worldHeight = ((mapChunkSize - 1) * worldMapHeight) + 1;
        if (scale <= 0) scale = 0.0001f;
        float[,] noiseMap = new float[worldWidth, worldHeight];

        if (useDynamicHeightExtremes)
        {
            maxNoiseHeight = float.MinValue;
            minNoiseHeight = float.MaxValue;
        }

        float halfWidth = worldWidth / 2f;
        float halfHeight = worldHeight / 2f;

        // Create random octave offsets
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-100000, 100000);
            float offsetY = rng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // Create noise map
        for(int y = 0; y < worldHeight; y++)
        {
            for(int x = 0; x < worldWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                // Generate value for each pixel going through n octaves with x persistance and y lacunarity
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (float)(x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (float)(y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (useDynamicHeightExtremes)
                {
                    if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                    if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }
        
        // Normalize noise map to [0,1]
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        Debug.Log("Min Value: " + minNoiseHeight + ", Max Value: " + maxNoiseHeight);

        // Terrain stuff
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                // Water curve
                if (noiseMap[x, y] <= seaLevel) noiseMap[x,y] = 0;
                else
                {
                    noiseMap[x, y] = HeightCurveOperations.InterpolationByOperation(heightCurveOperation, seaLevel, 1, noiseMap[x, y]);
                }
            }
        }


        return noiseMap;
    }


    
    public static float[,] GenerateHeightMapWithOwnAlgorithm(int mapChunkSize, WorldMap wm, RandomNumberGenerator rng)
    {
        // Init values
        int worldWidth = ((mapChunkSize - 1) * wm.WorldMapWidth) + 1;
        int worldHeight = ((mapChunkSize - 1) * wm.WorldMapHeight) + 1;
        float[,] noiseMap = new float[worldWidth, worldHeight];

        // fill random noise seed with [0,1]
        float[,] noiseSeed = new float[worldWidth*2,worldHeight*2];
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                noiseSeed[x,y] = rng.Next();
            }
        }

        // create perlin noise
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                float value = 0f;

                float frequency = 1;
                float amplitude = 1;
                float amplitudeSum = 0f;

                for (int o = 0; o < wm.Octaves; o++)
                {
                    int segmentLengthX = (int)(worldWidth / frequency);
                    int segmentLengthY = (int)(worldHeight / frequency);
                    if (segmentLengthX < 1) segmentLengthX = 1;
                    if (segmentLengthY < 1) segmentLengthY = 1;

                    int nSampleX1 = (x / segmentLengthX) * segmentLengthX;
                    int nSampleY1 = (y / segmentLengthY) * segmentLengthY;

                    int nSampleX2 = (nSampleX1 + segmentLengthX); // add % worldWith to tessalate
                    int nSampleY2 = (nSampleY1 + segmentLengthY); // add % worldWith to tessalate

                    float fBlendX = (float)(x - nSampleX1) / (float)segmentLengthX; // position within pitch [0,1]
                    float fBlendY = (float)(y - nSampleY1) / (float)segmentLengthY; // position within pitch [0,1]

                    float fSampleT = (1f - fBlendX) * noiseSeed[nSampleX1, nSampleY1] + fBlendX * noiseSeed[nSampleX2, nSampleY1];
                    float fSampleB = (1f - fBlendX) * noiseSeed[nSampleX1, nSampleY2] + fBlendX * noiseSeed[nSampleX2, nSampleY2];

                    float octaveValue = (fBlendY * (fSampleB - fSampleT) + fSampleT) * amplitude;

                    value += octaveValue;
                    amplitudeSum += amplitude;

                    frequency *= wm.Lacunarity;
                    amplitude *= wm.Persistance;
                }

                noiseMap[x,y] = value / amplitudeSum;
            }
        }

        return noiseMap;
    }
}
