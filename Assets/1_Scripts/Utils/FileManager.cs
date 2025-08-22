using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public static class FileManager
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SaveImageToIndexedDB(string fileName, string base64Data, int dataLength);

    [DllImport("__Internal")]
    private static extern void LoadImageFromIndexedDB(string fileName, Action<string> callback);
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

    public static string SaveImage(string path)
    {
        try
        {
            string fileName = $"catch_{DateTime.Now.Ticks}.jpg";
            string newPath = GetFilePath(fileName);
            byte[] imageBytes = File.ReadAllBytes(path);

#if UNITY_WEBGL && !UNITY_EDITOR
            string base64 = Convert.ToBase64String(imageBytes);
            SaveImageToIndexedDB(fileName, base64, base64.Length);
#else
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

            LoadImageFromIndexedDB(fileName, (data) =>
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

            string base64Data = await tcs.Task;

            byte[] imageBytes = Convert.FromBase64String(base64Data);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            return texture;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load image from IndexedDB: {ex.Message}");
            return null;
        }
#else
        try
        {
            string path = GetFilePath(fileName);
            if (File.Exists(path))
            {
                byte[] imageBytes = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);
                return texture;
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
}