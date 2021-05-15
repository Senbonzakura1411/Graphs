using System;
using UnityEngine;
using static UnityEngine.Mathf;
using Random = UnityEngine.Random;

public static class FunctionLibrary
{
    public delegate Vector3 Function(float u, float v, float t);
    public static int FunctionCount => _functions.Length;
    public enum FunctionName
    {
        Wave,
        DoubleWave,
        TripleWave,
        Ripple,
        Sphere,
        TwistedSphere,
        Torus,
        TwistedTorus,
    }
    
    // Remember to synchronize the enum above with this method's switch expression below. 
    public static Function GetFunction(FunctionName name)
    {
        return name switch
        {
            FunctionName.Wave => Wave,
            FunctionName.DoubleWave => DoubleWave,
            FunctionName.TripleWave => TripleWave,
            FunctionName.Ripple => Ripple,
            FunctionName.Sphere => Sphere,
            FunctionName.TwistedSphere => TwistedSphere,
            FunctionName.Torus => Torus,
            FunctionName.TwistedTorus => TwistedTorus,
            _ => null
        };
    }
    
    private static readonly FunctionName[] _functions = Enum.GetValues(typeof(FunctionName)) as FunctionName[];
    
    

    public static FunctionName GetNextFunctionName(FunctionName name)
    {
       return name < _functions[FunctionCount - 1] ?  name + 1 :  0;
    }
    public static FunctionName GetRandomFunctionNameOtherThan (FunctionName name) {
        var choice = (FunctionName)Random.Range(1, FunctionCount - 1);
        return choice == name ? 0 : choice;
    }

    public static Vector3 Morph(float u, float v, float t, Function from, Function to, float progress)
    {
        return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));
    }
    
   
    static Vector3 Wave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;
        return p;
    }

    static Vector3 DoubleWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y *= (2f / 3f);
        p.z = v;
        return p;
    }

    static Vector3 TripleWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + t * 0.25f));
        p.y *= 0.4f;
        p.z = v;
        return p;
    }

    static Vector3 Ripple(float u, float v, float t)
    {
        float d = Sqrt(u * u + v * v);
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d - t));
        p.y /= (1f + 10f * d);
        p.z = v;
        return p;
    }

    static Vector3 Sphere(float u, float v, float t)
    {
        float r = 0.5f + 0.5f * Sin(PI * t);
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    static Vector3 TwistedSphere(float u, float v, float t)
    {
        float r = 0.9f + 0.1f * Sin(PI * (12f * u + 8f * v + t));
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    static Vector3 Torus(float u, float v, float t)
    {
        float r1 = 0.5f + 0.25f * Sin(PI * t);
        float r2 = 0.25f;
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2* Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    
    static Vector3 TwistedTorus(float u, float v, float t)
    {
        float r1 = 0.7f + 0.1f * Sin(PI * (8f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (16f * u + 8f * v + 3f * t));
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2* Sin(PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
}