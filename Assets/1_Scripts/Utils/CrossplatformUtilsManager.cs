using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class CrossplatformUtilsManager
{
#if UNITY_WEBGL && !UNITY_EDITOR

#endif
    [DllImport("__Internal")]
    private static extern void RequestFile(string callbackObjectName, string callbackMethodName, string extensions);

    public static void PickFile(Action<string> callback, string extensions = "image/*")
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        var callbackObject = new GameObject("FilePickerCallback");
        var callbackComponent = callbackObject.AddComponent<FilePickerCallback>();
        callbackComponent.SetCallback(callback);
        RequestFile(callbackObject.name, nameof(FilePickerCallback.OnFilePicked), extensions);
#else
#endif
    }

    private class FilePickerCallback : MonoBehaviour
    {
        private Action<string> callback;

        public void SetCallback(Action<string> cb)
        {
            callback = cb;
        }

        public void OnFilePicked(string base64Data)
        {
            Debug.Log($"Received base64 data: {(base64Data.Length > 50 ? base64Data.Substring(0, 50) + "..." : base64Data)}");
            callback?.Invoke(base64Data);
            Destroy(gameObject); 
        }
    }
}