using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapDataManager
{
    private readonly DataCore _data;

    public MapDataManager(DataCore data)
    {
        _data = data;
    }

    public MapData CreateMapData(IReadOnlyList<EditorView> editorViews, MapEditorUIManager uiManager)
    {
        var mapData = new MapData();

        foreach (var view in editorViews.OfType<EditorTextView>())
        {
            var rect = view.GetComponent<RectTransform>();
            mapData.texts.Add(new TextViewData
            {
                position = view.transform.position,
                rotation = view.transform.rotation.eulerAngles,
                sizeDelta = rect != null ? rect.sizeDelta : Vector2.zero,
                color = uiManager.TextColor,
                siblingIndex = view.transform.GetSiblingIndex(),
                text = view.Text
            });
        }

        foreach (var view in editorViews.OfType<EditorFigureView>())
        {
            var rect = view.GetComponent<RectTransform>();
            int formIndex = 0;//System.Array.IndexOf(uiManager.GetForms(), view.Form);
            mapData.figures.Add(new FigureViewData
            {
                position = view.transform.position,
                rotation = view.transform.rotation.eulerAngles,
                sizeDelta = rect != null ? rect.sizeDelta : Vector2.zero,
                color = uiManager.ViewColor,
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

        var allSeats = editorViews.OfType<EditorSeatView>()
            .SelectMany(v => v.GenerateSeatModels())
            .Distinct()
            .ToList();

        mapData.Event = _data.Personal.GetSelectedEvent();
        _data.Personal.GetSelectedEvent().seats = allSeats;
        return mapData;
    }

    public void SaveMap(MapData mapData, EventModel selectedEvent)
    {
        var existingMap = _data.Maps.GetByEvent(selectedEvent);
        if (existingMap != null)
            _data.Maps.Remove(existingMap);

        _data.Maps.Add(mapData);
        _data.SaveData();
    }

    public void LoadMap(MapData mapData, MapObjectManager objectManager, GameObject textPrefab, GameObject figurePrefab, GameObject seatPrefab, Sprite[] forms)
    {
        /*objectManager.Clear();

        var viewsWithIndices = new List<(EditorView view, int siblingIndex)>();

        foreach (var textData in mapData.texts)
        {
            var view = Object.Instantiate(textPrefab, textData.position, Quaternion.Euler(textData.rotation), objectManager.Area).GetComponent<EditorTextView>();
            if (view.GetComponent<RectTransform>() is RectTransform rect) rect.sizeDelta = textData.sizeDelta;
            UIContainer.InitView(view, textData.text);
            objectManager.AddText(view, textData.siblingIndex);
        }

        foreach (var figureData in mapData.figures)
        {
            var view = Object.Instantiate(figurePrefab, figureData.position, Quaternion.Euler(figureData.rotation), objectManager.Area).GetComponent<EditorFigureView>();
            if (view.GetComponent<RectTransform>() is RectTransform rect) rect.sizeDelta = figureData.sizeDelta;
            view.UpdateColor(figureData.color);
            view.UpdateForm(figureData.formIndex < forms.Length ? forms[figureData.formIndex] : forms[0]);
            objectManager.AddFigure(view, figureData.siblingIndex);
        }

        foreach (var seatData in mapData.seats)
        {
            var view = Object.Instantiate(seatPrefab, seatData.position, Quaternion.Euler(seatData.rotation), objectManager.Area).GetComponent<EditorSeatView>();
            if (view.GetComponent<RectTransform>() is RectTransform rect) rect.sizeDelta = seatData.sizeDelta;
            UIContainer.InitView(view, seatData.seatSettings);
            objectManager.AddSeat(view, seatData.siblingIndex);
        }

        objectManager.SortViewsBySiblingIndex();*/
    }
}