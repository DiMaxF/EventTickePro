using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Logger 
{
    public enum TypeMessage 
    {
        Message,
        Error,
        Warning
    }
    const bool active = true;
    const string TAG = "EventTicketPro";

    public static void TryCatch(Action action)
    {
        try
        {
            action?.Invoke();
        }
        catch (Exception ex)
        {
            LogError($"{ex.Message}\nStackTrace: {ex.StackTrace}", $"[TryCatch] {action.Method.Name}");
        }
    }

    public static void LogError(string message, string tag = "")
    {
        DebugLog(tag, message, TypeMessage.Error);
    }

    public static void Log(string message, string tag = "")
    {
        DebugLog(tag, message);
    }

    public static void LogWarning(string message, string tag = "") 
    {
        DebugLog(tag, message, TypeMessage.Warning);
    }

    private static void DebugLog(string tag, string message, TypeMessage type = TypeMessage.Message) 
    {
        if (!active) return;
        var log = $"[{type}] [{(tag == "" ? TAG : tag)}] " + message;
        switch (type) 
        {
            case TypeMessage.Message:
                Debug.Log(log);
                break;
            case TypeMessage.Error:
                Debug.LogError(log);
                break;
            case TypeMessage.Warning:
                Debug.LogWarning(log);
                break;
        }
    }

}
