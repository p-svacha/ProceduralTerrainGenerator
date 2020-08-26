using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightCurveOperations
{

    public static float InterpolationByOperation(HeightCurveOperation operation, float min, float max, float value)
    {
        switch(operation)
        {
            case HeightCurveOperation.Linear:
                return Lerp(min, max, value);
            case HeightCurveOperation.Sin:
                return Sinerp(min, max, value);
            case HeightCurveOperation.Cos:
                return Coserp(min, max, value);
            case HeightCurveOperation.Exponential:
                return Exponential(min, max, value);
            case HeightCurveOperation.Exponential3:
                return Exponential3(min, max, value);
            case HeightCurveOperation.SmoothStep:
                return SmoothStep(min, max, value);
            case HeightCurveOperation.SmootherStep:
                return SmootherStep(min, max, value);
            default:
                throw new System.Exception("Height Curve Operation not handled.");
        }
    }

    // Linear
    public static float Lerp(float min, float max, float value)
    {
        return (value - min) / (max - min);
    }

    // First steep that getting flatter
    public static float Sinerp(float min, float max, float value)
    {
        float linearValue = Lerp(min, max, value);
        return Mathf.Sin(linearValue * Mathf.PI * 0.5f);
    }

    // First flat that getting steeper
    public static float Coserp(float min, float max, float value)
    {
        float linearValue = Lerp(min, max, value);
        return 1f - Mathf.Cos(linearValue * Mathf.PI * 0.5f);
    }

    // First flat that getting steeper
    public static float Exponential(float min, float max, float value)
    {
        float linearValue = Lerp(min, max, value);
        return linearValue * linearValue;
    }

    // First very flat that getting very steep
    public static float Exponential3(float min, float max, float value)
    {
        float linearValue = Lerp(min, max, value);
        return linearValue * linearValue * linearValue;
    }

    // Bezier-like
    public static float SmoothStep(float min, float max, float value)
    {
        float linearValue = Lerp(min, max, value);
        return linearValue * linearValue * (3f - 2f*linearValue);
    }

    // Bezier-like
    public static float SmootherStep(float min, float max, float value)
    {
        float linearValue = Lerp(min, max, value);
        return linearValue * linearValue * linearValue * (linearValue * (6f * linearValue - 15f) + 10f);
    }
}
