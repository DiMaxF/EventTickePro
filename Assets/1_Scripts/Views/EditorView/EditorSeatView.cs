  using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorSeatView : EditorView
{
    [SerializeField] ListView seats;
     public Data data => _data;
     Data _data;
    int rowHeight = 100;
    int columnWidth = 80;
    [Serializable]
    public class Data
    {
        public string numer;
        public int countSeats;
        public int countRow;
        public Color color;
        public bool left;

        public Data(string numer, int countSeats, int countRow, Color color, bool left = true)
        {
            this.numer = numer;
            this.countSeats = countSeats;
            this.countRow = countRow;
            this.color = color;
            this.left = left;
        }   
    }

    private bool _listGenerated;
    public override void Init<T>(T data)
    {
        if (data is Data d)
        {
            _data = d;
            if(rectTransform == null) rectTransform = GetComponent<RectTransform>();

            
            prev = _data;

            UIContainer.RegisterView(seats);
             UIContainer.InitView(seats, GenerateList(_data));
        }
        base.Init(data);
    }

    /*public override void Subscriptions()
    {
        base.Subscriptions();
        if(_listGenerated) seats.UpdateViewsData(GenerateList(_data));
        _listGenerated = true;
    }*/

    private List<Data> GenerateList(Data data)
    {
        var list = new List<Data>();    
        for (int i = 0; i < data.countRow; i++)
        {
            string seatNumber = data.numer;
            if (data.numer == "Alphabet" || data.numer == "Numbers") 
            {
                seatNumber = data.numer == "Alphabet" ?
                        ((char)('A' + i)).ToString() : $"{i + 1}";
            }
            list.Add(new Data(seatNumber, data.countSeats, data.countRow, data.color, data.left));
        }
        return list;
    }

    public List<Data> GetListData() => GenerateList(_data);

    public override void UpdateUI()
    {
        base.UpdateUI();
    }

    private Data prev;

    public void UpdateSizes() 
    {

        if (prev == null)
        {
            rectTransform.sizeDelta = new Vector2(_data.countSeats * columnWidth, _data.countRow * rowHeight);
        }
        else if (prev.countSeats != _data.countSeats || _data.countRow != prev.countRow) 
        {
            rectTransform.sizeDelta = new Vector2(_data.countSeats * columnWidth, _data.countRow * rowHeight);

        }
        
    }

    public override void UpdateColor(Color newColor)
    {
        base.UpdateColor(newColor);
    }

    public List<SeatModel> GenerateSeatModels()
    {
        List<SeatModel> seatModels = new List<SeatModel>();
        var d = GenerateList(data);
        var id = 0;
        foreach (var oneRow in d)
        {
            for (int seat = 0; seat < oneRow.countSeats; seat++)
            {
                id = seat;
                Logger.Log($"{seat} {id}", "EditorTextView");
                SeatModel seatModel = new SeatModel(oneRow.numer, id, false);
                seatModels.Add(seatModel);
            }

        }


        return seatModels;
    }

}