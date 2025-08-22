using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AsyncImageView : View
{
    [SerializeField] private RawImage _image;
    [SerializeField] private GameObject _loading;
    private string _imagePath;
    private AspectRatioFitter aspectRatioFitter;
    public bool widthFill; 

    public override void Init<T>(T data)
    {
        if (data is string path)
        {
            _imagePath = path;
        }

        aspectRatioFitter = _image.GetComponent<AspectRatioFitter>();
        if (aspectRatioFitter == null)
        {
            aspectRatioFitter = _image.gameObject.AddComponent<AspectRatioFitter>();
        }
        UpdateUI();
    }

    public override void UpdateUI()
    {
        if (string.IsNullOrEmpty(_imagePath))
        {
            _image.texture = null;
            TriggerAction<string>(null);
            _image.color = Color.clear;
            return;
        }
        _image.color = Color.white;
        if (_loading != null) _loading.SetActive(true); 

        if (IsLocalFilePath(_imagePath))
        {
            LoadLocalFile(_imagePath);
        }
        else if (IsResourcesPath(_imagePath))
        {
            LoadResourcesPath(_imagePath);
        }
    }

    private bool IsLocalFilePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;

        var hasFileExtension = path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase);

        var isAbsolutePath = Path.IsPathRooted(path);
        var isPersistentPath = path.StartsWith(Application.persistentDataPath, StringComparison.OrdinalIgnoreCase);

        return hasFileExtension && (isAbsolutePath || isPersistentPath);
    }

    private bool IsResourcesPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;

        return char.IsLetter(path[0]) &&
               !path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
               !path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
               !path.StartsWith("file://", StringComparison.OrdinalIgnoreCase) &&
               !Path.IsPathRooted(path) &&
               !path.StartsWith(Application.persistentDataPath, StringComparison.OrdinalIgnoreCase);
    }

    private async UniTask LoadLocalFile(string path)
    {
        try
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        Texture2D texture = await FileManager.LoadImage(path);
        if (texture != null)
        {
            _image.color = Color.white;
            _image.texture = texture;
            UpdateAspectRatio(texture);
            if (_loading != null) _loading.SetActive(false);
            TriggerAction(path);
        }
        else
        {
            _image.texture = null;
            if (_loading != null) _loading.SetActive(false);
            TriggerAction<string>(null);
        }
#else
            if (!File.Exists(path))
            {
                _image.texture = null;
                if (_loading != null) _loading.SetActive(false);
                TriggerAction<string>(null);
                return;
            }

            byte[] imageBytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageBytes))
            {
                _image.color = Color.white;
                _image.texture = texture;
                UpdateAspectRatio(texture);
                if (_loading != null) _loading.SetActive(false);
                TriggerAction(path);
            }
            else
            {
                _image.texture = null;
                if (_loading != null) _loading.SetActive(false);
                TriggerAction<string>(null);
                Destroy(texture);
            }
#endif
        }
        catch (Exception ex) 
        {
        
        }

    }

    private void LoadResourcesPath(string path)
    {
        Logger.TryCatch(() =>
        {
            string resourcePath = Path.ChangeExtension(path, null);
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture != null)
            {
                _image.color = Color.white;
                _image.texture = texture;
                UpdateAspectRatio(texture);
                if (_loading != null) _loading.SetActive(false);
                TriggerAction(_imagePath);
            }
            else
            {
                _image.texture = null;
                if (_loading != null) _loading.SetActive(false);
                TriggerAction<string>(null);
            }
        });
    }

    private void UpdateAspectRatio(Texture2D texture)
    {
        if (texture == null || aspectRatioFitter == null) return;

        float aspectRatio = (float)texture.width / texture.height;
        aspectRatioFitter.aspectRatio = aspectRatio;

        aspectRatioFitter.aspectMode = widthFill
            ? AspectRatioFitter.AspectMode.WidthControlsHeight
            : (texture.width > texture.height
                ? AspectRatioFitter.AspectMode.WidthControlsHeight
                : AspectRatioFitter.AspectMode.HeightControlsWidth);
    }
}