using Cysharp.Threading.Tasks;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CrossplatformUtilsManager 
{
    [DllImport("__Internal")]
    private static extern void RequestFile(string callbackObjectName, string callbackMethodName, string extensions);

    public static void PickFile(Action<string> callback, string extensions = "")
    {
#if UNITY_WEBGL && !UNITY_EDITOR
       string callbackMethod = "OnFilePicked";
        fileCallback = callback;
        RequestFile("CrossplatformUtilsManager", callbackMethod, extensions);
#else
        Debug.LogWarning("File picker is only supported in WebGL builds");
#endif
    }

    private static Action<string> fileCallback;

    public static void OnFilePicked(string fileUrl)
    {
        Debug.Log("Selected file: " + fileUrl);
        if (fileCallback != null)
        {
            fileCallback(fileUrl);
        }
    }

    public async UniTask LoadTextureFromFileAsync(string fileUrl, Image targetImage)
    {
        await LoadTextureAsync(fileUrl, targetImage);
    }

    private async UniTask LoadTextureAsync(string fileUrl, Image targetImage)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(fileUrl))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                targetImage.sprite = sprite;
            }
            else
            {
                Debug.LogError("Error loading texture: " + request.error);
            }
        }
    }
}