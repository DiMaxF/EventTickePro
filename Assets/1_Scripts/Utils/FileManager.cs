using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class FileManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SaveImageToIndexedDB(string fileName, string base64Data, int dataLength, string callbackObjectName, string callbackMethodName);

    [DllImport("__Internal")]
    private static extern void LoadImageFromIndexedDB(string fileName, string callbackObjectName, string callbackMethodName);
#endif

    public static string ReadFile(string fileName)
    {
        try
        {
            string path = GetFilePath(fileName);
            if (File.Exists(path))
            {
                using StreamReader reader = new StreamReader(path);
                return reader.ReadToEnd();
            }
            Debug.LogWarning($"File not found: {path}");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to read file {fileName}: {ex.Message}");
            return null;
        }
    }

    public static async UniTask WriteToFile(string fileName, string content)
    {
        try
        {
            string path = GetFilePath(fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            using (var writer = new StreamWriter(fileStream))
            {
                await writer.WriteAsync(content);
                Debug.Log($"File written to: {path}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to write file {fileName}: {ex.Message}");
        }
    }

    public static bool FileExists(string fileName)
    {
        try
        {
            return File.Exists(GetFilePath(fileName));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error checking file existence {fileName}: {ex.Message}");
            return false;
        }
    }

    public static string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public static async UniTask<string> SaveImage(string data, bool isBase64 = false)
    {
        string fileName = $"catch_{DateTime.Now.Ticks}.jpg";
#if UNITY_WEBGL && !UNITY_EDITOR
        string savePath = fileName; // Для IndexedDB используем только имя файла
#else
        string savePath = GetFilePath(fileName); // Для non-WebGL полный путь
#endif
        Debug.Log($"Saving image to: {savePath}");

        try
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            string base64 = isBase64 ? data : Convert.ToBase64String(File.ReadAllBytes(data));
            if (string.IsNullOrEmpty(base64))
            {
                Debug.LogError("Base64 data is empty");
                return null;
            }
            Debug.Log($"Saving base64 data (length: {base64.Length}) to IndexedDB");

            var tcs = new UniTaskCompletionSource<string>();
            var callbackObject = new GameObject("ImageSaveCallback");
            var callbackComponent = callbackObject.AddComponent<ImageSaveCallback>();
            callbackComponent.SetCallback((result) =>
            {
                if (result == "success")
                {
                    tcs.TrySetResult(savePath);
                }
                else
                {
                    tcs.TrySetException(new Exception($"Save error: {result}"));
                }
            });

            SaveImageToIndexedDB(fileName, base64, base64.Length, callbackObject.name, nameof(ImageSaveCallback.OnImageSaved));
            return await tcs.Task;
#else
            byte[] imageBytes = isBase64 ? Convert.FromBase64String(data) : File.ReadAllBytes(data);
            Debug.Log($"Writing {imageBytes.Length} bytes to: {savePath}");
            await File.WriteAllBytesAsync(savePath, imageBytes);
            return savePath;
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save image {fileName}: {ex.Message}");
            return null;
        }
        finally
        {
            var obj = GameObject.Find("ImageSaveCallback");
            if (obj != null) Destroy(obj);
        }
    }

    public static async UniTask<string> SaveTexture(Texture2D texture, string fileNamePrefix = "qr")
    {
        string fileName = $"{fileNamePrefix}_{DateTime.Now.Second}_{DateTime.Now.Ticks}.png";
#if UNITY_WEBGL && !UNITY_EDITOR
        string savePath = fileName; // Для IndexedDB только имя файла
#else
        string savePath = GetFilePath(fileName); // Для non-WebGL полный путь
#endif
        Debug.Log($"Saving texture to: {savePath}");

        try
        {
            byte[] textureBytes = texture.EncodeToPNG();
            if (textureBytes == null || textureBytes.Length == 0)
            {
                Debug.LogError("Texture encoding failed: Empty or null bytes");
                return null;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            string base64 = Convert.ToBase64String(textureBytes);
            if (string.IsNullOrEmpty(base64))
            {
                Debug.LogError("Base64 conversion failed for texture");
                return null;
            }
            Debug.Log($"Saving texture base64 data (length: {base64.Length}) to IndexedDB");

            var tcs = new UniTaskCompletionSource<string>();
            var callbackObject = new GameObject("ImageSaveCallback");
            var callbackComponent = callbackObject.AddComponent<ImageSaveCallback>();
            callbackComponent.SetCallback((result) =>
            {
                if (result == "success")
                {
                    tcs.TrySetResult(savePath);
                }
                else
                {
                    tcs.TrySetException(new Exception($"Save texture error: {result}"));
                }
            });

            SaveImageToIndexedDB(fileName, base64, base64.Length, callbackObject.name, nameof(ImageSaveCallback.OnImageSaved));
            return await tcs.Task;
#else
            await File.WriteAllBytesAsync(savePath, textureBytes);
            Debug.Log($"Texture saved to: {savePath}");
            return savePath;
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save texture {fileName}: {ex.Message}");
            return null;
        }
        finally
        {
            var obj = GameObject.Find("ImageSaveCallback");
            if (obj != null) Destroy(obj);
        }
    }

    public static async UniTask<Texture2D> LoadImage(string fileName)
    {
        Debug.Log($"Loading image: {fileName}");

        try
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Для WebGL используем только имя файла (без пути)
            string loadFileName = Path.GetFileName(fileName);
            var tcs = new UniTaskCompletionSource<string>();
            var callbackObject = new GameObject("ImageLoadCallback");
            var callbackComponent = callbackObject.AddComponent<ImageLoadCallback>();
            callbackComponent.SetCallback((result) =>
            {
                if (string.IsNullOrEmpty(result) || result.StartsWith("Error:") || result.StartsWith("IndexedDB"))
                {
                    tcs.TrySetException(new Exception($"Load error: {result ?? "Null data"}"));
                }
                else
                {
                    tcs.TrySetResult(result);
                }
            });

            LoadImageFromIndexedDB(loadFileName, callbackObject.name, nameof(ImageLoadCallback.OnImageLoaded));
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
#else
            string path = fileName.StartsWith(Application.persistentDataPath) ? fileName : GetFilePath(fileName);
            if (!File.Exists(path))
            {
                Debug.LogError($"File not found: {path}");
                return null;
            }

            byte[] imageBytes = await File.ReadAllBytesAsync(path);
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
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load image {fileName}: {ex.Message}");
            return null;
        }
        finally
        {
            var obj = GameObject.Find("ImageLoadCallback");
            if (obj != null) Destroy(obj);
        }
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
            Debug.Log($"Received base64 data from IndexedDB (length: {base64Data?.Length ?? 0}): {(base64Data != null && base64Data.Length > 50 ? base64Data.Substring(0, 50) + "..." : base64Data ?? "null")}");
            callback?.Invoke(base64Data);
            Destroy(gameObject);
        }
    }

    private class ImageSaveCallback : MonoBehaviour
    {
        private Action<string> callback;

        public void SetCallback(Action<string> cb)
        {
            callback = cb;
        }

        public void OnImageSaved(string result)
        {
            Debug.Log($"Save result: {result}");
            callback?.Invoke(result);
            Destroy(gameObject);
        }
    }
}