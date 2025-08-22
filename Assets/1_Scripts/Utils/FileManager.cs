using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class FileManager : MonoBehaviour 
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SaveImageToIndexedDB(string fileName, string base64Data, int dataLength);

    [DllImport("__Internal")]
    private static extern void LoadImageFromIndexedDB(string fileName, string callbackObjectName, string callbackMethodName);
#endif

    public static string ReadFile(string fileName)
    {
        var path = GetFilePath(fileName);
        if (File.Exists(path))
        {
            using StreamReader reader = new StreamReader(path);
            var json = reader.ReadToEnd();
            return json;
        }
        return "Error: File not found";
    }

    public static void WriteToFile(string fileName, string content)
    {
        var path = GetFilePath(fileName);
        var fileStream = new FileStream(path, FileMode.Create);

        using var writer = new StreamWriter(fileStream);
        writer.Write(content);
        Logger.Log(path);
    }

    public static bool FileExist(string fileName)
    {
        return File.Exists(GetFilePath(fileName));
    }

    public static string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }

    public static string SaveImage(string data, bool isBase64 = false)
    {
        try
        {
            string fileName = $"catch_{DateTime.Now.Ticks}.jpg";
            string newPath = GetFilePath(fileName);
            Debug.Log($"Saving image to: {newPath}");

#if UNITY_WEBGL && !UNITY_EDITOR
            string base64 = isBase64 ? data : Convert.ToBase64String(File.ReadAllBytes(data));
            Debug.Log($"Saving base64 data (length: {base64.Length}) to IndexedDB");
            if (string.IsNullOrEmpty(base64))
            {
                Debug.LogError("Base64 data is empty");
                return null;
            }
            SaveImageToIndexedDB(fileName, base64, base64.Length);
#else
            byte[] imageBytes = isBase64 ? Convert.FromBase64String(data) : File.ReadAllBytes(data);
            Debug.Log($"Writing {imageBytes.Length} bytes to: {newPath}");
            File.WriteAllBytes(newPath, imageBytes);
#endif

            return newPath;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save image: {ex.Message}");
            return null;
        }
    }

    public static string SaveTexture(Texture2D texture, string fileNamePrefix = "qr")
    {
        try
        {
            string fileName = $"{fileNamePrefix}_{DateTime.Now.Second}_{DateTime.Now.Ticks}.png";
            string path = GetFilePath(fileName);
            byte[] textureBytes = texture.EncodeToPNG();

#if UNITY_WEBGL && !UNITY_EDITOR
            string base64 = Convert.ToBase64String(textureBytes);
            SaveImageToIndexedDB(fileName, base64, base64.Length);
#else
            File.WriteAllBytes(path, textureBytes);
#endif

            Debug.Log($"Texture saved to: {path}");
            return path;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save texture: {ex.Message}");
            return null;
        }
    }

    public static async UniTask<Texture2D> LoadImage(string fileName)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            var tcs = new UniTaskCompletionSource<string>();
            var callbackObject = new GameObject("ImageLoadCallback");
            var callbackComponent = callbackObject.AddComponent<ImageLoadCallback>();
            callbackComponent.SetCallback((data) =>
            {
                if (data != null && data != "Error: File not found")
                {
                    tcs.TrySetResult(data);
                }
                else
                {
                    tcs.TrySetException(new Exception("Failed to load image from IndexedDB: File not found"));
                }
            });

            LoadImageFromIndexedDB(fileName, callbackObject.name, nameof(ImageLoadCallback.OnImageLoaded));
            string base64Data = await tcs.Task;

            byte[] imageBytes = Convert.FromBase64String(base64Data);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageBytes))
            {
                Debug.Log($"Image loaded successfully: {fileName}");
                return texture;
            }
            else
            {
                Debug.LogError($"Failed to load image data: {fileName}");
                Destroy(texture);
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load image from IndexedDB: {ex.Message}");
            return null;
        }
        finally
        {
            // ”ничтожаем callbackObject, если он еще существует
            var obj = GameObject.Find("ImageLoadCallback");
            if (obj != null)
            {
                Destroy(obj);
            }
        }
#else
        try
        {
            string path = GetFilePath(fileName);
            if (File.Exists(path))
            {
                byte[] imageBytes = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(imageBytes))
                {
                    Debug.Log($"Image loaded successfully: {path}");
                    return texture;
                }
                else
                {
                    Debug.LogError($"Failed to load image data: {path}");
                    Destroy(texture);
                    return null;
                }
            }
            else
            {
                Debug.LogError($"File not found: {path}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load image: {ex.Message}");
            return null;
        }
#endif
    }

    private class ImageLoadCallback : MonoBehaviour
    {
        private Action<string> callback;

        public void SetCallback(Action<string> cb)
        {
            callback = cb;
        }

        public void OnImageLoaded(string base64Data)
        {
            Debug.Log($"Received base64 data from IndexedDB (length: {base64Data?.Length}): {(base64Data != null && base64Data.Length > 50 ? base64Data.Substring(0, 50) + "..." : base64Data ?? "null")}");
            callback?.Invoke(base64Data);
        }
    }
}