using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class MapObjectManager
{
    private readonly Transform _area;
    private readonly MapEditorScreen _screen;
    private readonly List<EditorView> _editorViews = new();
    private EditorView _selectedView;

    public IReadOnlyList<EditorView> EditorViews => _editorViews;
    public EditorView SelectedView => _selectedView;
    public Transform Area => _area;

    public MapObjectManager(Transform area, MapEditorScreen screen)
    {
        _area = area;
        _screen = screen;
    }

    public EditorTextView AddText(GameObject prefab, Color color)
    {
        var view = Object.Instantiate(prefab, DisplayManager.Center, Quaternion.identity, _area).GetComponent<EditorTextView>();
        AddView(view, v => v.UpdateColor(color));
        return view;
    }

    public EditorFigureView AddFigure(GameObject prefab, Color color, Sprite form)
    {
        var view = Object.Instantiate(prefab, DisplayManager.Center, Quaternion.identity, _area).GetComponent<EditorFigureView>();
        AddView(view, v =>
        {
            v.UpdateColor(color);
            ((EditorFigureView)v).UpdateForm(form);
        });
        return view;
    }

    public EditorSeatView AddSeat(GameObject prefab, EditorSeatView.Data settings)
    {
        if (settings == null || settings.countRow != 1)
            settings = new EditorSeatView.Data("1", 5, 1, settings?.color ?? Color.white);

        var view = Object.Instantiate(prefab, DisplayManager.Center, Quaternion.identity, _area).GetComponent<EditorSeatView>();
        AddView(view, v => UIContainer.InitView(v, settings));
        return view;
    }

    public EditorSeatView AddSeats(GameObject prefab, EditorSeatView.Data settings)
    {
        if (settings == null || settings.countRow == 1)
            settings = new EditorSeatView.Data("Numbers", 5, 3, settings?.color ?? Color.white);

        var view = Object.Instantiate(prefab, DisplayManager.Center, Quaternion.identity, _area).GetComponent<EditorSeatView>();
        AddView(view, v => UIContainer.InitView(v, settings));
        return view;
    }

    private void AddView(EditorView view, Action<EditorView> initialize)
    {
        _editorViews.Add(view);
        view.Select();
        initialize(view);
        UIContainer.RegisterView(view);
        UIContainer.SubscribeToView(view, (EditorView v) => _screen.OnViewSelected(v));
        _screen.OnViewSelected(view);
    }

    public void SubscribeToViews(Action<EditorView> onSelected)
    {
        foreach (var view in _editorViews)
            UIContainer.SubscribeToView(view, onSelected);
    }

    public void SelectView(EditorView view, MapEditorUIManager uiManager)
    {
        _selectedView = view;
        DeselectOthers();
        if (view is EditorTextView)
            uiManager.OpenTextSettings(true);
        else if (view is EditorFigureView)
            uiManager.OpenFigureSettings(true);
        else if (view is EditorSeatView seatView)
            uiManager.OpenSeatsSettings(true, seatView.data);
    }

    public void DeselectAll(MapEditorUIManager uiManager)
    {
        if (_selectedView != null)
        {
            _selectedView.Deselect();
            uiManager?.OpenTextSettings(false);
            uiManager?.OpenFigureSettings(false);
            uiManager?.OpenSeatsSettings(false, null);
            _selectedView = null;
        }
    }

    public void DeleteSelected()
    {
        if (_selectedView != null)
        {
            _editorViews.Remove(_selectedView);
            Object.Destroy(_selectedView.gameObject);
            DeselectAll(null);
        }
    }

    public void DuplicateSelected(GameObject textPrefab, GameObject figurePrefab, GameObject seatPrefab, MapEditorUIManager uiManager)
    {
        if (_selectedView == null) return;

        var offset = new Vector3(0.5f, 0.5f, 0);
        EditorView newView = null;

        if (_selectedView is EditorTextView)
        {
            newView = Object.Instantiate(textPrefab, _selectedView.transform.position + offset, _selectedView.transform.rotation, _area).GetComponent<EditorTextView>();
            newView.UpdateColor(uiManager.TextColor);
        }
        else if (_selectedView is EditorFigureView)
        {
            newView = Object.Instantiate(figurePrefab, _selectedView.transform.position + offset, _selectedView.transform.rotation, _area).GetComponent<EditorFigureView>();
            newView.UpdateColor(uiManager.ViewColor);
            ((EditorFigureView)newView).UpdateForm(uiManager.CurrentForm);
        }
        else if (_selectedView is EditorSeatView seatView)
        {
            newView = Object.Instantiate(seatPrefab, _selectedView.transform.position + offset, _selectedView.transform.rotation, _area).GetComponent<EditorSeatView>();
            newView.UpdateColor(uiManager.SeatColor);
            var seatData = seatView.data ?? new EditorSeatView.Data("1", 5, 1, uiManager.SeatColor);
            UIContainer.InitView(newView, seatData);
        }

        if (newView != null)
        {
            newView.transform.localScale = _selectedView.transform.localScale;
            var rectS = _selectedView.GetComponent<RectTransform>();
            var rectN = newView.GetComponent<RectTransform>();
            if (rectS != null && rectN != null)
                rectN.sizeDelta = rectS.sizeDelta;

            _editorViews.Add(newView);
            UIContainer.RegisterView(newView);
            UIContainer.SubscribeToView(newView, (EditorView v) => _screen.OnViewSelected(v));
            newView.Select();
            _screen.OnViewSelected(newView);
        }
    }

    public void RotateSelected(float angle)
    {
        if (_selectedView != null)
            _selectedView.Rotate(angle);
    }

    public void MoveSelectedUp()
    {
        if (_selectedView != null)
        {
            int newIndex = _selectedView.transform.GetSiblingIndex() + 1;
            if (newIndex < _selectedView.transform.parent.childCount)
                _selectedView.transform.SetSiblingIndex(newIndex);
        }
    }

    public void MoveSelectedDown()
    {
        if (_selectedView != null)
        {
            int newIndex = Mathf.Max(0, _selectedView.transform.GetSiblingIndex() - 1);
            _selectedView.transform.SetSiblingIndex(newIndex);
        }
    }

    public async void CenterCamera(CameraController cam)
    {
        if (_editorViews.Count == 0) return;

        Vector3 center = _editorViews.Aggregate(Vector3.zero, (sum, view) => sum + view.transform.position) / _editorViews.Count;
        await cam.MoveToPosition(center);
    }

    public void UpdateViews()
    {
        foreach (var view in _editorViews)
            view.UpdateUI();
    }

    private void DeselectOthers()
    {
        foreach (var view in _editorViews)
        {
            if (view != _selectedView)
                view.Deselect();
        }
    }
}