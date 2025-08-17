using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeatsSettingsView : View
{

    private const int MAX_SEATS = 20;  
    private const int MAX_ROWS = 20;  
    [SerializeField] InputTextView numer;
    [SerializeField] ButtonView numerAction;
    [SerializeField] InputTextView countRow;
    [SerializeField] InputTextView seats;
    [SerializeField] ButtonView toggleLeft;
    [SerializeField] ButtonView toggleRight;

    public EditorSeatView.Data Data => _data;
    private EditorSeatView.Data _data;

    public override void Init<T>(T data)
    {
        if (data is EditorSeatView.Data d)
        {
            _data = d;
            UIContainer.RegisterView(numer);
            UIContainer.RegisterView(seats);
            UIContainer.RegisterView(countRow);


            UIContainer.SubscribeToView<InputTextView, string>(numer, HandleNumer);
            UIContainer.SubscribeToView<InputTextView, string>(seats, HandleSeats);
            UIContainer.SubscribeToView<InputTextView, string>(countRow, HandleRows);

            UIContainer.SubscribeToView<ButtonView, object>(numerAction, _ =>
            {
                UIContainer.InitView(numer, _data.numer == "Alphabet" ? "Numbers" : "Alphabet");
            });
            UIContainer.SubscribeToView<ButtonView, object>(toggleLeft, _ => Toggle(false));
            UIContainer.SubscribeToView<ButtonView, object>(toggleRight, _ => Toggle(true));
        }
        base.Init(data);
    }
    public override void UpdateUI()
    {
        base.UpdateUI();
        UIContainer.RegisterView(numer);
        UIContainer.RegisterView(seats);
        UIContainer.RegisterView(countRow);
        UIContainer.InitView(seats, _data.countSeats.ToString());
        UIContainer.InitView(countRow, _data.countRow.ToString());
        UIContainer.InitView(numer, _data.numer);
        toggleLeft.gameObject.SetActive(_data.left);
        toggleRight.gameObject.SetActive(!_data.left);
        if (_data.countRow > 1) 
        {
            countRow.Show();
        }
        else 
        {
            countRow.Hide();
        }
    }


    private void HandleNumer(string val) 
    {
        if(val == "Numbers" || val == "Alphabet") 
        {
            numer.Lock();
            numerAction.gameObject.SetActive(true);
        }
        else 
        {
            numer.Unlock();
            numerAction.gameObject.SetActive(false);
        }
        _data.numer = val;  
        TriggerAction(_data);
    }

    private void HandleSeats(string val) 
    {
        if (!int.TryParse(val, out var value)) return;
        if(value > MAX_SEATS) 
        {
            value = MAX_SEATS;
            UIContainer.InitView(seats, value.ToString());
        }
        _data.countSeats = value;
        TriggerAction(_data);

    }
    private void HandleRows(string val)
    {
        if (!int.TryParse(val, out var value)) return;
        if (value > MAX_ROWS)
        {
            value = MAX_ROWS;
            UIContainer.InitView(countRow, value.ToString());
        }
        _data.countRow = value;
        TriggerAction(_data);
    }

    private void Toggle(bool val) 
    {
        _data.left = val;
        toggleLeft.gameObject.SetActive(val);
        toggleRight.gameObject.SetActive(!val);
        TriggerAction(_data);
    }
}
