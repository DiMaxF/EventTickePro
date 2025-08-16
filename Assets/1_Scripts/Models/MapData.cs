using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapData
{
    public EventModel Event;
    public string pathPreview;
    public List<TextViewData> texts = new List<TextViewData>();
    public List<FigureViewData> figures = new List<FigureViewData>();
    public List<SeatViewData> seats = new List<SeatViewData>();
}

[Serializable]
public class TextViewData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector2 sizeDelta;
    public Color color;
    public int siblingIndex;
    public string text; 
}

[Serializable]
public class FigureViewData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector2 sizeDelta;
    public Color color;
    public int formIndex;
    public int siblingIndex;
}

[Serializable]
public class SeatViewData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector2 sizeDelta;
    public EditorSeatView.Data seatSettings;
    public int siblingIndex;
}