using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsScreen : AppScreen
{
    [SerializeField] ExpandedView kpis;
    [SerializeField] ImgsFillDynamic occupancy;
    [SerializeField] LineChartView visitsChart;
    [SerializeField] ExpandedView sales;
    [SerializeField] RowChartView salesChart;
    [SerializeField] ExpandedView conversation;
    [SerializeField] LineChartView conversationSentvsUsed;
    [SerializeField] LineChartView conversationRate;
    [SerializeField] Button exportButton;
    [Header("Toggles")]
    [SerializeField] Button week;
    [SerializeField] Button month;
    [SerializeField] Button year;
    private TimePeriod _currentPeriod = TimePeriod.Week;
    protected override void OnStart()
    {
        base.OnStart(); 
        ui.InitView(kpis, false);
        ui.InitView(sales, false);
        ui.InitView(conversation, false);
        SetPeriod(TimePeriod.Week);
    }

    private ChartData GetSentVsUsedData(TimePeriod period)
    {
        var filteredEvents = data.FilterEventsByPeriod(period);
        var sentValues = new Dictionary<string, int>();
        var usedValues = new Dictionary<string, int>();

        foreach (var ev in filteredEvents)
        {
            int sentTickets = ev.tickets.Count;
            int usedTickets = ev.tickets.Count(ticket => ticket.valid);

            string key = period == TimePeriod.Year ? ev.Date.ToString("MMM yyyy") : ev.Date.ToString("dd.MM.yyyy");

            if (sentValues.ContainsKey(key))
            {
                sentValues[key] += sentTickets;
                usedValues[key] += usedTickets;
            }
            else
            {
                sentValues[key] = sentTickets;
                usedValues[key] = usedTickets;
            }
        }

        return new ChartData(
            title: "Sent vs Used tickets",
            values: new Dictionary<string, float>(), 
            series: new List<(string name, Dictionary<string, float> values)>
            {
                ("Sent tickets", sentValues.OrderBy(kvp => DateTime.Parse(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => (float)kvp.Value)),
                ("Used tickets", usedValues.OrderBy(kvp => DateTime.Parse(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => (float)kvp.Value))
            }
        );
    }

    private void SetPeriod(TimePeriod period) 
    {
        ButtonActive(week, period == TimePeriod.Week);
        ButtonActive(month, period == TimePeriod.Month);
        ButtonActive(year, period == TimePeriod.Year);
        _currentPeriod = period;
        UpdateViews();
    }

    private void ButtonActive(Button button, bool active) 
    {
        button.image.color = active ? Color.white : Color.clear;    
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        ui.InitView(visitsChart, data.GetKPIs(_currentPeriod));
        ui.InitView(salesChart, data.GetTicketSalesByPeriod(_currentPeriod));
        ui.InitView(conversationSentvsUsed, GetSentVsUsedData(_currentPeriod));
        ui.InitView(conversationRate, data.GetConversionRates(_currentPeriod));
        occupancy.SetValue(data.GetPersentOccupancy(_currentPeriod));
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        ui.SubscribeToComponent<Button, object>(week, _ => SetPeriod(TimePeriod.Week));
        ui.SubscribeToComponent<Button, object>(month, _ => SetPeriod(TimePeriod.Month));
        ui.SubscribeToComponent<Button, object>(year, _ => SetPeriod(TimePeriod.Year));
        ui.SubscribeToComponent<Button, object>(exportButton, _ => ExportCsv());
    }
    public void ExportCsv()
    {
        var kpiData = data.GetKPIs(_currentPeriod);
        var salesData = data.GetTicketSalesByPeriod(_currentPeriod);
        var conversionData = data.GetConversionRates(_currentPeriod);
        var occupancyPercent = data.GetPersentOccupancy(_currentPeriod);

        string csvContent = "";

        csvContent += "KPIs (Visits)\n";
        csvContent += "Data,Visits\n";
        foreach (var kvp in kpiData.values)
        {
            csvContent += $"{kvp.Key},{kvp.Value}\n";
        }
        csvContent += "\n";

        csvContent += "Sales\n";
        csvContent += "Data,Sales\n";
        foreach (var kvp in salesData.values)
        {
            csvContent += $"{kvp.Key},{kvp.Value}\n";
        }
        csvContent += "\n";

        csvContent += "Conversion (%)\n";
        csvContent += "Data,Conversion (%)\n";
        foreach (var kvp in conversionData.values)
        {
            csvContent += $"{kvp.Key},{kvp.Value}\n";
        }
        csvContent += "\n";

        csvContent += "Occupancy\n";
        csvContent += "Occupancy Rate\n";
        csvContent += $"{occupancyPercent * 100:F1}%\n";

        string fileName = $"statistics_{DateTime.Now:yyyyMMddHHmmss}.csv";
        FileManager.WriteToFile(fileName, csvContent);

        string filePath = FileManager.GetFilePath(fileName);
        new NativeShare().AddFile(filePath)
        .SetSubject("Subject goes here").SetText($"Statistics for {_currentPeriod}")
        .Share();
    }

}