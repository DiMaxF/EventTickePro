using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeatView : View
{
    [SerializeField] GameObject numerPrefab;
    [SerializeField] GameObject seatPrefab;
    [SerializeField] Transform content;
    EditorSeatView.Data _data;
    public override void Init<T>(T data)
    {
        if(data is EditorSeatView.Data d) 
        {
            _data = d;
        }
        base.Init(data);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        foreach (Transform t in content) Destroy(t.gameObject);

        if (_data.left) SpawnNumer();
        for (int i = 0; i < _data.countSeats; i++)
        {
            Logger.Log($"{i}", "SeatView");
            var s = Instantiate(seatPrefab, content);
            s.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = $"{i+1}";
            s.transform.GetChild(0).GetComponent<Image>().color = _data.color;  
        }
        if (!_data.left) SpawnNumer();
        Logger.Log($"{_data.left}", "SeatView");
    }

    private void SpawnNumer() 
    {
        var numer = Instantiate(numerPrefab, content);
        numer.transform.GetChild(0).GetComponent<Text>().text = _data.numer;
    }
}
