using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
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
            File.WriteAllBytes(newPath, imageBytes);

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
            string fileName = $"{fileNamePrefix}_{DateTime.Now.Ticks}.png";
            string path = GetFilePath(fileName);
            byte[] textureBytes = texture.EncodeToPNG(); 
            File.WriteAllBytes(path, textureBytes);
            Debug.Log($"Texture saved to: {path}");
            return path;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save texture: {ex.Message}");
            return null;
        }
    }
}
