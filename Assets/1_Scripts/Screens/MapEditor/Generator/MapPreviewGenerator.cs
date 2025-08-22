using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class MapPreviewGenerator
{
    private readonly Transform _area;
    private readonly CameraController _cam;

    public MapPreviewGenerator(Transform area, CameraController cam)
    {
        _area = area;
        _cam = cam;
    }

    public async UniTask<string> GeneratePreview(IReadOnlyList<EditorView> editorViews, MapEditorUIManager uiManager, string name = "")
    {
        var panelStates = uiManager.SavePanelStates();
        uiManager.HideAllPanels();

        Bounds bounds = CalculateBounds(editorViews);
        float orthographicSize = CalculateOrthographicSize(bounds, Camera.main.aspect) * 0.7f;

        GameObject tempCameraObj = new GameObject("TempPreviewCamera");
        Camera tempCamera = tempCameraObj.AddComponent<Camera>();
        tempCamera.orthographic = true;
        tempCamera.orthographicSize = orthographicSize;
        tempCamera.transform.position = bounds.center + new Vector3(0, 0, -10);
        tempCamera.clearFlags = CameraClearFlags.SolidColor;
        tempCamera.backgroundColor = Color.clear;
        tempCamera.cullingMask = 1 << _area.gameObject.layer;

        int width = 256;
        int height = 256;
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        tempCamera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;

        tempCamera.Render();

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        string path = await FileManager.SaveTexture(texture, $"preview_map_{name}");

        uiManager.RestorePanelStates(panelStates);
        tempCamera.targetTexture = null;
        RenderTexture.active = null;
        Object.Destroy(renderTexture);
        Object.Destroy(texture);
        Object.Destroy(tempCameraObj);

        return path;
    }

    private Bounds CalculateBounds(IReadOnlyList<EditorView> editorViews)
    {
        if (editorViews.Count == 0)
            return new Bounds(Vector3.zero, Vector3.one);

        Bounds bounds = new Bounds(editorViews[0].transform.position, Vector3.zero);
        foreach (var view in editorViews)
        {
            RectTransform rect = view.GetComponent<RectTransform>();
            if (rect != null)
            {
                Vector3[] corners = new Vector3[4];
                rect.GetWorldCorners(corners);
                foreach (var corner in corners)
                    bounds.Encapsulate(corner);
            }
            else
            {
                bounds.Encapsulate(view.transform.position);
            }
        }
        return bounds;
    }

    private float CalculateOrthographicSize(Bounds bounds, float aspect)
    {
        float width = bounds.size.x;
        float height = bounds.size.y;
        float size = Mathf.Max(width / aspect, height) * 0.5f;
        return Mathf.Max(size, 1f);
    }
}