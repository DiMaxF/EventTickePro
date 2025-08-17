using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapEditorScreen : AppScreen
{
    [Header("Game Area")]
    [SerializeField] private CameraController cam;
    [SerializeField] private Transform area;
    [SerializeField] private ButtonView clickDetector;

    [Header("Prefabs")]
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private GameObject figurePrefab;
    [SerializeField] private GameObject seatPrefab;
    [SerializeField] private GameObject seatsPrefab;

    [Header("Setting Panel")]
    [SerializeField] private BaseView settingsPanel;
    [SerializeField] private ListView colorsPicker;
    [SerializeField] private SeatsSettingsView seatsPicker;
    [SerializeField] private ListView formsPicker;
    [SerializeField] private Sprite[] forms;

    [Header("Tools")]
    [SerializeField] private ButtonView center;
    [SerializeField] private ButtonView moveCamera;
    [SerializeField] private ButtonView box;
    [SerializeField] private ButtonView text;
    [SerializeField] private ButtonView seat;
    [SerializeField] private ButtonView seats;

    [Header("Actions")]
    [SerializeField] private ButtonView layoutUp;
    [SerializeField] private ButtonView layoutDown;
    [SerializeField] private ButtonView rotate;
    [SerializeField] private ButtonView duplicate;
    [SerializeField] private ButtonView delete;
    [SerializeField] private ButtonView save;
    [SerializeField] private ButtonView exit;

    private MapObjectManager _objectManager;
    private MapEditorUIManager _uiManager;
    private MapDataManager _dataManager;
    private MapPreviewGenerator _previewGenerator;

    protected override void OnStart()
    {
        _objectManager = new MapObjectManager(area, this);
        //_uiManager = new MapEditorUIManager(settingsPanel, colorsPicker, formsPicker, seatsPicker, forms);
        _dataManager = new MapDataManager(data, core);
        _previewGenerator = new MapPreviewGenerator(area, cam);

        _uiManager.InitializeColors();
        base.OnStart();
        UIContainer.RegisterView(seatsPicker);

        var map = data.GetByEvent(data.selectedEvent);
        if (map != null)
        {
            _dataManager.LoadMap(map, _objectManager, textPrefab, figurePrefab, seatPrefab, forms);
        }
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        _objectManager.UpdateViews();
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(text, _ => _objectManager.AddText(textPrefab, _uiManager.TextColor));
        UIContainer.SubscribeToView<ButtonView, object>(box, _ => _objectManager.AddFigure(figurePrefab, _uiManager.ViewColor, _uiManager.CurrentForm));
        UIContainer.SubscribeToView<ButtonView, object>(seat, _ => _objectManager.AddSeat(seatPrefab, _uiManager.SeatSettings));
        UIContainer.SubscribeToView<ButtonView, object>(seats, _ => _objectManager.AddSeats(seatsPrefab, _uiManager.SeatSettings));
        UIContainer.SubscribeToView<ButtonView, object>(layoutUp, _ => _objectManager.MoveSelectedUp());
        UIContainer.SubscribeToView<ButtonView, object>(layoutDown, _ => _objectManager.MoveSelectedDown());
        UIContainer.SubscribeToView<ButtonView, object>(duplicate, _ => _objectManager.DuplicateSelected(textPrefab, figurePrefab, seatPrefab, _uiManager));
        UIContainer.SubscribeToView<ButtonView, object>(delete, _ => _objectManager.DeleteSelected());
        UIContainer.SubscribeToView<ButtonView, object>(rotate, _ => _objectManager.RotateSelected(30f));
        UIContainer.SubscribeToView<ButtonView, object>(center, _ => _objectManager.CenterCamera(cam));
        UIContainer.SubscribeToView<ButtonView, object>(moveCamera, _ => ToggleCamera());
        UIContainer.SubscribeToView<ButtonView, object>(save, _ => SaveMap());
        UIContainer.SubscribeToView<ButtonView, object>(exit, _ => SceneManager.LoadScene("Home"));
        UIContainer.SubscribeToView<ButtonView, object>(clickDetector, _ => _objectManager.DeselectAll(_uiManager));

        _objectManager.SubscribeToViews(OnViewSelected);
        UIContainer.SubscribeToView<ListView, Color>(colorsPicker, val => _uiManager.ApplyColor(val, _objectManager.SelectedView));
        UIContainer.SubscribeToView<ListView, Sprite>(formsPicker, val => _uiManager.ApplyForm(val, _objectManager.SelectedView));
        UIContainer.SubscribeToView<SeatsSettingsView, EditorSeatView.Data>(seatsPicker, val => _uiManager.UpdateSeatSettings(val, _objectManager.SelectedView));
    }

    public void OnViewSelected(EditorView view)
    {
        _objectManager.SelectView(view, _uiManager);
    }

    private void ToggleCamera()
    {
        bool isActive = cam.ToggleCameraActive();
        moveCamera.image.color = isActive
            ? new Color(159f / 255f, 0, 7f / 255f)
            : new Color(47f / 255f, 34f / 255f, 98f / 255f);
    }

    private void SaveMap()
    {
        var mapData = _dataManager.CreateMapData(_objectManager.EditorViews, _uiManager);
        mapData.pathPreview = _previewGenerator.GeneratePreview(_objectManager.EditorViews, _uiManager);
        _dataManager.SaveMap(mapData, data.selectedEvent);
        NativeMobilePlugin.Instance.ShowToast("Map saved successfully!");
    }
}