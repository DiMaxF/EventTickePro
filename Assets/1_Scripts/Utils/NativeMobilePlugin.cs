using System.Runtime.InteropServices;
using UnityEngine;


public interface INativePlugin
{
    void RateApp();
    void ShareText(string text, string subject = "");
    void ShareImage(string imagePath, string text = "", string subject = "");
    void ShareFile(string filePath, string text = "", string subject = ""); // Новый метод
    void ShowToast(string message, bool isLongDuration = false);
    void OpenEmail(string emailAddress, string subject = "", string body = "");
    void OpenEmailMultiple(string[] emailAddresses, string subject = "", string body = "");
}
public class NativeMobilePlugin : INativePlugin
{
    private static NativeMobilePlugin _instance;
    public static NativeMobilePlugin Instance => _instance ??= new NativeMobilePlugin();

    private NativeMobilePlugin() { }

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RateAppiOS();

    [DllImport("__Internal")]
    private static extern void ShareTextiOS(string text, string subject);

    [DllImport("__Internal")]
    private static extern void ShowToastiOS(string message, float duration);

    [DllImport("__Internal")]
    private static extern void OpenEmailiOS(string emailAddress, string subject, string body);
#endif

    public void RateApp()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass plugin = new AndroidJavaClass("com.example.nativeplugin.NativePlugin");
            plugin.CallStatic("rateApp", activity);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NativePlugin] Failed to rate app on Android: {ex.Message}");
        }
#elif UNITY_IOS && !UNITY_EDITOR
        try
        {
            RateAppiOS();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NativePlugin] Failed to rate app on iOS: {ex.Message}");
        }
#else
        Debug.LogWarning("[NativePlugin] RateApp is not supported in the Unity Editor.");
#endif
    }

    public void ShareText(string text, string subject = "")
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass plugin = new AndroidJavaClass("com.example.nativeplugin.NativePlugin");
            plugin.CallStatic("shareText", activity, text, subject);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NativePlugin] Failed to share text on Android: {ex.Message}");
        }
#elif UNITY_IOS && !UNITY_EDITOR
        try
        {
            ShareTextiOS(text, subject);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NativePlugin] Failed to share text on iOS: {ex.Message}");
        }
#else
        Debug.LogWarning("[NativePlugin] ShareText is not supported in the Unity Editor.");
#endif
    }

    public void ShowToast(string message, bool isLongDuration = false)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    try
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        if (activity == null)
        {
            Debug.LogError("[NativePlugin] Current activity is null");
            return;
        }

        AndroidJavaClass plugin = new AndroidJavaClass("com.example.nativeplugin.NativePlugin");
        if (plugin == null)
        {
            Debug.LogError("[NativePlugin] NativePlugin class not found");
            return;
        }

        Debug.Log($"[NativePlugin] Showing toast: {message}, isLongDuration: {isLongDuration}");
        plugin.CallStatic("showToast", activity, message, isLongDuration);
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"[NativePlugin] Failed to show toast on Android: {ex.Message}");
    }
#elif UNITY_IOS && !UNITY_EDITOR
    try
    {
        ShowToastiOS(message, isLongDuration ? 3.5f : 2.0f);
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"[NativePlugin] Failed to show toast on iOS: {ex.Message}");
    }
#else
        Debug.LogWarning($"[NativePlugin] Toast: {message} (not supported in Unity Editor)");
#endif
    }

    public void OpenEmail(string emailAddress, string subject = "", string body = "")
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass plugin = new AndroidJavaClass("com.example.nativeplugin.NativePlugin");
            plugin.CallStatic("openEmail", activity, emailAddress, subject, body);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NativePlugin] Failed to open email on Android: {ex.Message}");
        }
#elif UNITY_IOS && !UNITY_EDITOR
        try
        {
            OpenEmailiOS(emailAddress, subject, body);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NativePlugin] Failed to open email on iOS: {ex.Message}");
        }
#else
        Debug.LogWarning($"[NativePlugin] OpenEmail to {emailAddress} is not supported in the Unity Editor.");
#endif
    }

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void ShareImageiOS(string imagePath, string text, string subject);

    [DllImport("__Internal")]
    private static extern void OpenEmailMultipleiOS(string emailAddresses, string subject, string body);
#endif

    public void ShareImage(string imagePath, string text = "", string subject = "")
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass plugin = new AndroidJavaClass("com.example.nativeplugin.NativePlugin");
            plugin.CallStatic("shareImage", activity, imagePath, text, subject);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NativePlugin] Failed to share image on Android: {ex.Message}");
        }
#elif UNITY_IOS && !UNITY_EDITOR
        try
        {
            ShareImageiOS(imagePath, text, subject);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NativePlugin] Failed to share image on iOS: {ex.Message}");
        }
#else
        Debug.LogWarning("[NativePlugin] ShareImage is not supported in the Unity Editor.");
#endif
    }

    public void OpenEmailMultiple(string[] emailAddresses, string subject = "", string body = "")
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass plugin = new AndroidJavaClass("com.example.nativeplugin.NativePlugin");
            AndroidJavaObject arrayList = new AndroidJavaObject("java.util.ArrayList");
            foreach (string email in emailAddresses)
            {
                arrayList.Call<bool>("add", email);
            }
            plugin.CallStatic("openEmailMultiple", activity, arrayList, subject, body);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NativePlugin] Failed to open email with multiple recipients on Android: {ex.Message}");
        }
#elif UNITY_IOS && !UNITY_EDITOR
        try
        {
            string emailList = string.Join(",", emailAddresses);
            OpenEmailMultipleiOS(emailList, subject, body);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NativePlugin] Failed to open email with multiple recipients on iOS: {ex.Message}");
        }
#else
        Debug.LogWarning($"[NativePlugin] OpenEmailMultiple to {string.Join(",", emailAddresses)} is not supported in the Unity Editor.");
#endif
    }

    public void ShareFile(string filePath, string text = "", string subject = "")
    {
#if UNITY_ANDROID && !UNITY_EDITOR
 try
 {
 AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
 AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
 if (activity == null)
 {
 Debug.LogError("[NativePlugin] Текущая активность равна null");
 return;
 }

 AndroidJavaClass plugin = new AndroidJavaClass("com.example.nativeplugin.NativePlugin");
 if (plugin == null)
 {
 Debug.LogError("[NativePlugin] Класс NativePlugin не найден");
 return;
 }

 Debug.Log($"[NativePlugin] Поделиться файлом: {filePath}, текст: {text}, тема: {subject}");
 plugin.CallStatic("shareFile", activity, filePath, text, subject);
 }
 catch (System.Exception ex)
 {
 Debug.LogError($"[NativePlugin] Не удалось поделиться файлом на Android: {ex.Message}");
 }
#elif UNITY_IOS && !UNITY_EDITOR
 try
 {
 ShareFileiOS(filePath, text, subject);
 }
 catch (System.Exception ex)
 {
 Debug.LogError($"[NativePlugin] Не удалось поделиться файлом на iOS: {ex.Message}");
 }
#else
        Debug.LogWarning($"[NativePlugin] Поделиться файлом {filePath} не поддерживается в редакторе Unity.");
#endif
    }

}