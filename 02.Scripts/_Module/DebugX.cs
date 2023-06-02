using System.Diagnostics;
using Debug = UnityEngine.Debug;

//using UnityEngine;

public class DebugX
{
    [Conditional("UnityEditor")]
    public static void Log(object msg)
    {
        Debug.Log(msg);
    }

    [Conditional("UnityEditor")]
    public static void LogError(object msg)
    {
        Debug.LogError(msg);
    }
}