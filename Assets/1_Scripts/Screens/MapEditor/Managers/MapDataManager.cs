using NUnit.Framework.Interfaces;
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
            int formIndex = System.Array.IndexOf(uiManager.Forms, view.Form);
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
        var viewsWithIndices = new List<(EditorView view, int siblingIndex)>();

        foreach (var textData in mapData.texts)
        {
            var view = objectManager.AddText(textPrefab, textData.color); 
            view.RectTransform.sizeDelta = textData.sizeDelta;
            view.RectTransform.position = textData.position;
            UIContainer.InitView(view, textData.text);
            view.UpdateColor(textData.color);
            viewsWithIndices.Add((view, textData.siblingIndex));
        }

        foreach (var figureData in mapData.figures)
        {
            var view = objectManager.AddFigure(figurePrefab, figureData.color, forms[figureData.formIndex]);
            view.RectTransform.sizeDelta = figureData.sizeDelta;
            view.RectTransform.position = figureData.position;
            view.UpdateColor(figureData.color);
            view.UpdateForm(figureData.formIndex < forms.Length ? forms[figureData.formIndex] : forms[0]);
            viewsWithIndices.Add((view, figureData.siblingIndex));

        }

        foreach (var seatData in mapData.seats)
        {
            var view = objectManager.AddSeat(seatPrefab, seatData.seatSettings);
            view.RectTransform.sizeDelta = seatData.sizeDelta;
            view.RectTransform.position = seatData.position;    
            UIContainer.InitView(view, seatData.seatSettings);
            viewsWithIndices.Add((view, seatData.siblingIndex));
        }

        foreach (var view in viewsWithIndices) 
            view.view.transform.SetSiblingIndex(view.siblingIndex);
    }
}