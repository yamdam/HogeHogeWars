using UnityEngine;

public static class MyDebug
{
    public static void Log(object message)
    {
        #if UNITY_EDITOR
        Debug.Log(message);
        #endif
    }

    public static void LogError(object message)
    {
        #if UNITY_EDITOR
        Debug.LogError(message);
        #endif
    }

    public static void LogFormat(string format, params object[] args)
    {
        #if UNITY_EDITOR
        Debug.LogFormat(format, args);
        #endif
    }
}
