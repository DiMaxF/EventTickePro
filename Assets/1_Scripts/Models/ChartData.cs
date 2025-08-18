using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ChartData
{
    public string title;
    public Dictionary<string, float> values;
    public List<(string name, Dictionary<string, float> values)> series;

    public ChartData(string title, Dictionary<string, int> values)
    {
        this.title = title;
        this.values = values.ToDictionary(kvp => kvp.Key, kvp => (float)kvp.Value);
        this.series = null;
    }

    public ChartData(string title, Dictionary<string, float> values, List<(string name, Dictionary<string, float> values)> series)
    {
        this.title = title;
        this.values = values;
        this.series = series;
    }
}