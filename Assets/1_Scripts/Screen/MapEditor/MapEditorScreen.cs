using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapEditorScreen : AppScreen
{
    [SerializeField] private CameraController cam;
    [SerializeField] private Transform area;
    [SerializeField] Button clickDetector;
    [Header("Prefabs")]
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private GameObject figurePrefab;
    [SerializeField] private GameObject seatPrefab;
    [SerializeField] private GameObject seatsPrefab;
    [Header("Setting Panel")]
    [SerializeField] BaseView settingsPanel;
    [SerializeField] ListView colorsPicker;
    [SerializeField] SeatsSettingsView seatsPicker;
    [SerializeField] ListView formsPicker;
    [SerializeField] Sprite[] forms;
    [Header("Tools")]
    [SerializeField] Button center;
    [SerializeField] Button moveCamera;
    [SerializeField] Button box;
    [SerializeField] Button text;
    [SerializeField] Button seat;
    [SerializeField] Button seats;
    [Header("Actions")]
    [SerializeField] Button layoutUp;
    [SerializeField] Button layoutDown;
    [SerializeField] Button rotate;
    [SerializeField] Button duplicate;
    [SerializeField] Button delete;
    [SerializeField] Button save;
    [SerializeField] Button exit;
    private List<EditorView> editorViews = new List<EditorView>();
    private EditorView selectedView = null;

    private List<Color> textColors;
    private Color textColor;
    private Color viewColor;
    private Color seatColor;
    private Sprite form;

    private List<Color> figureColors;
    private EditorSeatView.Data seatSettings;

    protected override void OnStart()
    {
        InitColors();
        base.OnStart();
        ui.RegisterView(seatsPicker);
        OpenTextSettings(false);
        OpenFigureSettings(false);
        var map = data.GetByEvent(data.selectedEvent);
        if (map != null)
        {
            LoadMap(map);
        }

    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        foreach (var view in editorViews)
        {
            view.UpdateUI();
        }
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        ui.SubscribeToComponent<Button, object>(text, _ => AddText());
        ui.SubscribeToComponent<Button, object>(box, _ => AddFigure());
        ui.SubscribeToComponent<Button, object>(seat, _ => AddSeat());
        ui.SubscribeToComponent<Button, object>(seats, _ => AddSeats());
        ui.SubscribeToComponent<Button, object>(layoutUp, _ => OnButtonLayoutUp());
        ui.SubscribeToComponent<Button, object>(layoutDown, _ => OnButtonLayoutDown());

        ui.SubscribeToComponent<Button, object>(duplicate, _ => OnButtonDuplicate());
        ui.SubscribeToComponent<Button, object>(delete, _ => OnButtonDelete());
        ui.SubscribeToComponent<Button, object>(rotate, _ => OnButtonRotate());
        ui.SubscribeToComponent<Button, object>(center, _ => OnButtonCenter());
        ui.SubscribeToComponent<Button, object>(moveCamera, _ => OnButtonCamera());
        ui.SubscribeToComponent<Button, object>(save, _ => OnButtonSave());
        ui.SubscribeToComponent<Button, object>(exit, _ => OnButtonExit());

        ui.SubscribeToComponent<Button, object>(clickDetector, _ => OnClickOutside());

        foreach (var view in editorViews)
        {
            ui.SubscribeToView(view as View, (EditorView v) => OnViewSelected(v));
        }
        ui.SubscribeToView<ListView, Color>(colorsPicker, val =>
        {
            ApplyColor(val);
            if (selectedView != null && selectedView is EditorTextView)
                ui.InitView(colorsPicker, GetTextColorsList());
            else if (selectedView != null && selectedView is EditorFigureView)
                ui.InitView(colorsPicker, GetViewColorsList());
            else if (selectedView != null && selectedView is EditorSeatView seatView)
            {
                ui.InitView(colorsPicker, GetSeatsColorsList());
                ui.InitView(seatView, seatSettings);
            }

        });

        ui.SubscribeToView<ListView, Sprite>(formsPicker, val =>
        {
            ApplyForm(val);
            ui.InitView(formsPicker, GetFormsList());
        });

        ui.SubscribeToView<SeatsSettingsView, EditorSeatView.Data>(seatsPicker, val =>
        {
            if (selectedView is EditorSeatView seatView)
                ui.InitView(seatView, seatSettings);
        });
    }

    private void InitColors()
    {
        figureColors = new List<Color>
            {
                ColorUtility.TryParseHtmlString("#CDA6F8", out Color color1) ? color1 : Color.white,
                ColorUtility.TryParseHtmlString("#A6DEF8", out Color color2) ? color2 : Color.white,
                ColorUtility.TryParseHtmlString("#A6F8C7", out Color color3) ? color3 : Color.white,
                ColorUtility.TryParseHtmlString("#F8A6A7", out Color color4) ? color4 : Color.white,
                ColorUtility.TryParseHtmlString("#F8D4A6", out Color color5) ? color5 : Color.white,
                ColorUtility.TryParseHtmlString("#F8A6DE", out Color color6) ? color6 : Color.white
            };

        textColors = new List<Color>
            {
                ColorUtility.TryParseHtmlString("#8C5AB0", out Color color7) ? color7 : Color.white,
                ColorUtility.TryParseHtmlString("#6198AF", out Color color8) ? color8 : Color.white,
                ColorUtility.TryParseHtmlString("#62B085", out Color color9) ? color9 : Color.white,
                ColorUtility.TryParseHtmlString("#000000", out Color color10) ? color10 : Color.white,
                ColorUtility.TryParseHtmlString("#AF8F5A", out Color color11) ? color11 : Color.white,
                ColorUtility.TryParseHtmlString("#AF5A9A", out Color color12) ? color12 : Color.white
            };
        textColor = textColors[0];
        viewColor = figureColors[0];
        seatColor = figureColors[0];

        if (forms.Length > 0) form = forms[0];
    }

    private void OnViewSelected(EditorView view)
    {
        selectedView = view;
        DeselectedViews();
        if (view is EditorTextView) OpenTextSettings(true);
        if (view is EditorFigureView) OpenFigureSettings(true);
        if (view is EditorSeatView seatView)
        {
            seatSettings = seatView.data;
            OpenSeatsSettings(true);

        }

    }

    private void OnClickOutside()
    {
        if (selectedView != null)
        {
            selectedView.Deselect();
            OpenTextSettings(false);
            OpenFigureSettings(false);
            OpenSeatsSettings(false);
            selectedView = null;
        }
    }
    private void OnButtonCamera() 
    {
        if (cam.ToggleCameraActive()) 
        {
            moveCamera.image.color = new Color(159f / 255f, 0, 7f / 255f);
        }
        else
        {
            moveCamera.image.color = new Color(47f / 255f, 34f / 255f, 98f / 255f);

        }

    }
    private void OnButtonCenter()
    {
        if (editorViews.Count == 0)
            return;

        Vector3 center = Vector3.zero;
        foreach (var view in editorViews)
        {
            center += view.transform.position;
        }
        center /= editorViews.Count;

        cam.MoveToPosition(center);
    }

    private void OnButtonExit()
    {
        SceneManager.LoadScene("Home");
    }

    private void OnButtonSave()
    {
        MapData mapData = new MapData();

        foreach (var view in editorViews.OfType<EditorTextView>())
        {
            var rect = view.GetComponent<RectTransform>();
            mapData.texts.Add(new TextViewData
            {
                position = view.transform.position,
                rotation = view.transform.rotation.eulerAngles,
                sizeDelta = rect != null ? rect.sizeDelta : Vector2.zero,
                color = textColor,
                siblingIndex = view.transform.GetSiblingIndex(),
                text = view.Text
            });
        }

        foreach (var view in editorViews.OfType<EditorFigureView>())
        {
            var rect = view.GetComponent<RectTransform>();
            int formIndex = Array.IndexOf(forms, form);
            mapData.figures.Add(new FigureViewData
            {
                position = view.transform.position,
                rotation = view.transform.rotation.eulerAngles,
                sizeDelta = rect != null ? rect.sizeDelta : Vector2.zero,
                color = viewColor,
                formIndex = formIndex >= 0 ? formIndex : 0,
                siblingIndex = view.transform.GetSiblingIndex()
            });
        }

        foreach (var view in editorViews.OfType<EditorSeatView>())
        {
            var rect = view.GetComponent<RectTransform>();
            mapData.seats.Add(new SeatViewData
            {
                position = view.transform.position,
                rotation = view.transform.rotation.eulerAngles,
                sizeDelta = rect != null ? rect.sizeDelta : Vector2.zero,
                seatSettings = view.data,
                siblingIndex = view.transform.GetSiblingIndex()
            });
        }

        List<SeatModel> existingSeat = new List<SeatModel>();
        List<SeatModel> allSeats = new List<SeatModel>();
        foreach (var seatView in editorViews.OfType<EditorSeatView>())
        {
            List<SeatModel> seats = seatView.GenerateSeatModels();
            foreach (var seat in seats)
            {
                if (!existingSeat.Contains(seat))
                {

                    allSeats.Add(seat);
                }
                existingSeat.Add(seat);
            }
        }

        mapData.Event = data.selectedEvent;
        data.selectedEvent.seats = allSeats;
        mapData.pathPreview = SavePreview();
        var existingMap = data.GetByEvent(data.selectedEvent);
        if (existingMap != null)
        {
            data.maps.Remove(existingMap);
        }
        data.maps.Add(mapData);
        core.SaveData();
        
        NativeMobilePlugin.Instance.ShowToast("Map saved successfully!");
    }

    private void LoadMap(MapData mapData)
    {
        foreach (var view in editorViews)
        {
            Destroy(view.gameObject);
        }
        editorViews.Clear();
        DeselectedViews();

        var viewsWithIndices = new List<(EditorView view, int siblingIndex)>();

        foreach (var textData in mapData.texts)
        {
            var view = Instantiate(textPrefab, textData.position, Quaternion.Euler(textData.rotation), area).GetComponent<EditorTextView>();
            if (view.GetComponent<RectTransform>() is RectTransform rect) rect.sizeDelta = textData.sizeDelta;
            ui.InitView(view, textData.text);
            editorViews.Add(view);
            viewsWithIndices.Add((view, textData.siblingIndex));
            ui.RegisterView(view);
            ui.SubscribeToView<EditorView, EditorView>(view, OnViewSelected);
        }

        foreach (var figureData in mapData.figures)
        {
            var view = Instantiate(figurePrefab, figureData.position, Quaternion.Euler(figureData.rotation), area).GetComponent<EditorFigureView>();
            if (view.GetComponent<RectTransform>() is RectTransform rect) rect.sizeDelta = figureData.sizeDelta;
            view.UpdateColor(figureData.color);
            view.UpdateForm(figureData.formIndex < forms.Length ? forms[figureData.formIndex] : forms[0]);
            editorViews.Add(view);
            viewsWithIndices.Add((view, figureData.siblingIndex));
            ui.RegisterView(view);
            ui.SubscribeToView<EditorView, EditorView>(view, OnViewSelected);
        }

        foreach (var seatData in mapData.seats)
        {
            var view = Instantiate(seatPrefab, seatData.position, Quaternion.Euler(seatData.rotation), area).GetComponent<EditorSeatView>();
            if (view.GetComponent<RectTransform>() is RectTransform rect) rect.sizeDelta = seatData.sizeDelta;
            ui.InitView(view, seatData.seatSettings);
            editorViews.Add(view);
            viewsWithIndices.Add((view, seatData.siblingIndex));
            ui.RegisterView(view);
            ui.SubscribeToView<EditorView, EditorView>(view, OnViewSelected);
        }

        viewsWithIndices.Sort((a, b) => a.siblingIndex.CompareTo(b.siblingIndex));
        for (int i = 0; i < viewsWithIndices.Count; i++)
        {
            viewsWithIndices[i].view.transform.SetSiblingIndex(i);
        }
    }

    public void AddText()
    {
        var textView = Instantiate(textPrefab, DisplayManager.Center, Quaternion.identity, area).GetComponent<EditorTextView>();
        editorViews.Add(textView);
        textView.Select();
        textView.UpdateColor(textColor);
        ui.RegisterView(textView);
        ui.SubscribeToView(textView, (EditorView v) => OnViewSelected(v));
        OnViewSelected(textView);
    }

    public void AddFigure()
    {
        var figureView = Instantiate(figurePrefab, DisplayManager.Center, Quaternion.identity, area).GetComponent<EditorFigureView>();
        editorViews.Add(figureView);
        figureView.Select();
        figureView.UpdateColor(viewColor);
        figureView.UpdateForm(form);
        ui.RegisterView(figureView);
        ui.SubscribeToView(figureView, (EditorView v) => OnViewSelected(v));
        OnViewSelected(figureView);
    }

    public void AddSeat()
    {
        if (seatSettings == null || seatSettings.countRow != 1)
        {
            seatSettings = new EditorSeatView.Data("1", 5, 1, seatColor);
        }
        var seatView = Instantiate(seatPrefab, DisplayManager.Center, Quaternion.identity, area).GetComponent<EditorSeatView>();
        editorViews.Add(seatView);
        ui.RegisterView(seatView);
        ui.InitView(seatView, seatSettings);
        ui.SubscribeToView(seatView, (EditorView v) => OnViewSelected(v));
        OnViewSelected(seatView);
    }

    public void AddSeats()
    {
        if (seatSettings == null || seatSettings.countRow == 1)
        {
            seatSettings = new EditorSeatView.Data("Numbers", 5, 3, seatColor);
        }
        var seatView = Instantiate(seatsPrefab, DisplayManager.Center, Quaternion.identity, area).GetComponent<EditorSeatView>();
        editorViews.Add(seatView);
        ui.RegisterView(seatView);
        ui.InitView(seatView, seatSettings);
        ui.SubscribeToView(seatView, (EditorView v) => OnViewSelected(v));
        OnViewSelected(seatView);
    }

    private void OnButtonDelete()
    {
        if (selectedView != null)
        {
            editorViews.Remove(selectedView);
            Destroy(selectedView.gameObject);
            selectedView = null;
            DeselectedViews();
        }
    }

    private void OnButtonDuplicate()
    {
        if (selectedView == null) return;
        var offset = new Vector3(0.5f, 0.5f, 0);
        EditorView newView = null;

        if (selectedView is EditorTextView)
        {
            newView = Instantiate(textPrefab, selectedView.transform.position + offset, selectedView.transform.rotation, area).GetComponent<EditorTextView>();
            newView.UpdateColor(textColor);
        }
        else if (selectedView is EditorFigureView)
        {
            newView = Instantiate(figurePrefab, selectedView.transform.position + offset, selectedView.transform.rotation, area).GetComponent<EditorFigureView>();
            newView.UpdateColor(viewColor);
            ((EditorFigureView)newView).UpdateForm(form);
        }
        else if (selectedView is EditorSeatView seatView)
        {
            newView = Instantiate(seatPrefab, seatView.transform.position + offset, seatView.transform.rotation, area).GetComponent<EditorSeatView>();
            newView.UpdateColor(seatColor);
            EditorSeatView.Data seatData = seatView.data; 
            if (seatData != null)
            {
                ui.InitView(newView, seatData); 
            }
            else
            {
                Debug.LogWarning("SeatView data is null, using default settings");
                seatData = new EditorSeatView.Data("1", 5, 1, seatColor);
                ui.InitView(newView, seatData);
            }
        }
        if (newView != null)
        {
            newView.transform.localScale = selectedView.transform.localScale;
            var rectS = selectedView.GetComponent<RectTransform>();
            var rectN = newView.GetComponent<RectTransform>();
            if (rectS != null && rectN != null)
            {
                rectN.sizeDelta = rectS.sizeDelta;
            }

            editorViews.Add(newView);
            ui.RegisterView(newView);
            ui.SubscribeToView(newView, (EditorView v) => OnViewSelected(v));

            newView.Select();
            OnViewSelected(newView);
        }
    }

    private void OnButtonRotate()
    {
        if (selectedView != null)
        {
            selectedView.Rotate(30);
        }
    }

    private void OnButtonLayoutUp()
    {
        if (selectedView != null)
        {
            selectedView.transform.SetSiblingIndex(selectedView.transform.GetSiblingIndex() + 1 < selectedView.transform.parent.childCount ? selectedView.transform.GetSiblingIndex() + 1 : selectedView.transform.parent.childCount);
        }
    }

    private void OnButtonLayoutDown()
    {
        if (selectedView != null)
        {
            selectedView.transform.SetSiblingIndex(selectedView.transform.GetSiblingIndex() - 1 > 0 ? selectedView.transform.GetSiblingIndex() - 1 : 0);
        }
    }

    private void DeselectedViews()
    {
        foreach (var v in editorViews)
        {
            if (selectedView == null)
            {
                v.Deselect();
            }
            else if (selectedView != v)
            {
                v.Deselect();
            }
        }
    }

    private void OpenTextSettings(bool show)
    {
        if (show)
        {
            settingsPanel.Show();
            colorsPicker.Show();
            formsPicker.Hide();
            seatsPicker.Hide();
            ui.InitView(colorsPicker, GetTextColorsList());
        }
        else
        {
            settingsPanel.Hide();
        }
    }

    private void OpenFigureSettings(bool show)
    {
        if (show)
        {
            settingsPanel.Show();
            colorsPicker.Show();
            formsPicker.Show();
            seatsPicker.Hide();
            ui.InitView(colorsPicker, GetViewColorsList());
            ui.InitView(formsPicker, GetFormsList());
        }
        else
        {
            settingsPanel.Hide();
        }
    }

    private void OpenSeatsSettings(bool show)
    {

        if (show)
        {
            ui.InitView(colorsPicker, GetSeatsColorsList());
            ui.InitView(seatsPicker, seatSettings);

            seatsPicker.Show();
            settingsPanel.Show();
            colorsPicker.Show();
            formsPicker.Hide();

        }
        else
        {
            /*if(selectedView is EditorSeatView seatView) 
                ui.InitView(seatView, seatSettings);*/
            settingsPanel.Hide();
        }
    }

    private void ApplyColor(Color newColor)
    {
        if (selectedView != null)
        {
            selectedView.UpdateColor(newColor);
            if (selectedView is EditorTextView)
            {
                textColor = newColor;
            }
            else if (selectedView is EditorFigureView)
            {
                viewColor = newColor;
            }
            else if (selectedView is EditorSeatView)
            {
                seatColor = newColor;
                seatSettings.color = newColor;
            }
        }
    }

    private List<ColorButtonView.Data> GetTextColorsList()
    {
        var list = new List<ColorButtonView.Data>();
        foreach (var c in textColors)
        {
            list.Add(new ColorButtonView.Data(c, textColor == c));
        }
        return list;
    }

    private List<ColorButtonView.Data> GetViewColorsList()
    {
        var list = new List<ColorButtonView.Data>();
        foreach (var c in figureColors)
        {
            list.Add(new ColorButtonView.Data(c, viewColor == c));
        }
        return list;
    }

    private List<ColorButtonView.Data> GetSeatsColorsList()
    {
        var list = new List<ColorButtonView.Data>();
        foreach (var c in figureColors)
        {
            list.Add(new ColorButtonView.Data(c, seatColor == c));
        }
        return list;
    }

    private void ApplyForm(Sprite newForm)
    {
        if (selectedView != null && selectedView is EditorFigureView figure)
        {
            figure.UpdateForm(newForm);
            form = newForm;
        }
    }

    private List<FormButtonView.Data> GetFormsList()
    {
        var list = new List<FormButtonView.Data>();
        foreach (var c in forms)
        {
            list.Add(new FormButtonView.Data(c, form == c));
        }
        return list;
    }

    private string SavePreview()
    {
        bool wasSettingsPanelActive = settingsPanel.gameObject.activeSelf;
        bool wasColorsPickerActive = colorsPicker.gameObject.activeSelf;
        bool wasFormsPickerActive = formsPicker.gameObject.activeSelf;
        bool wasSeatsPickerActive = seatsPicker.gameObject.activeSelf;
        var prev = selectedView;
        selectedView = null;
        DeselectedViews();
        selectedView = prev;
        settingsPanel.gameObject.SetActive(false);
        colorsPicker.gameObject.SetActive(false);
        formsPicker.gameObject.SetActive(false);
        seatsPicker.gameObject.SetActive(false);

        Bounds bounds = CalculateBounds();
        float orthographicSize = CalculateOrthographicSize(bounds, Camera.main.aspect) * 0.7f;

        GameObject tempCameraObj = new GameObject("TempPreviewCamera");
        Camera tempCamera = tempCameraObj.AddComponent<Camera>();
        tempCamera.orthographic = true;
        tempCamera.orthographicSize = orthographicSize;
        tempCamera.transform.position = bounds.center + new Vector3(0, 0, -10); 
        tempCamera.clearFlags = CameraClearFlags.SolidColor;
        tempCamera.backgroundColor = Color.clear; 
        tempCamera.cullingMask = 1 << area.gameObject.layer; 

        int width = 256; 
        int height = 256;
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        tempCamera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;

        tempCamera.Render();

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        string path = FileManager.SaveTexture(texture, $"preview_map_{data.selectedEvent.date}");

        settingsPanel.gameObject.SetActive(wasSettingsPanelActive);
        colorsPicker.gameObject.SetActive(wasColorsPickerActive);
        formsPicker.gameObject.SetActive(wasFormsPickerActive);
        seatsPicker.gameObject.SetActive(wasSeatsPickerActive);

        tempCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(texture);
        Destroy(tempCameraObj);

        return path;
    }

    private Bounds CalculateBounds()
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
                {
                    bounds.Encapsulate(corner);
                }
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