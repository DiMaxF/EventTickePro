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
    [SerializeField] ButtonView exportButton;
    [Header("Toggles")]
    [SerializeField] ButtonView week;
    [SerializeField] ButtonView month;
    [SerializeField] ButtonView year;
    private TimePeriod _currentPeriod = TimePeriod.Week;
    protected override void OnStart()
    {
        base.OnStart(); 
        UIContainer.InitView(kpis, false);
        UIContainer.InitView(sales, false);
        UIContainer.InitView(conversation, false);
        SetPeriod(TimePeriod.Week);
    }

    private ChartData GetSentVsUsedData(TimePeriod period)
    {
        var filteredEvents = Data.Events.FilterEventsByPeriod(period);
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

    private void ButtonActive(ButtonView button, bool active) 
    {
        button.image.color = active ? new Color(70f/255f, 99f/255f, 188f/255f) : Color.clear;    
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(visitsChart, Data.Analytics.GetKPIs(_currentPeriod));
        UIContainer.InitView(salesChart, Data.Analytics.GetTicketSalesByPeriod(_currentPeriod));
        UIContainer.InitView(conversationSentvsUsed, GetSentVsUsedData(_currentPeriod));
        UIContainer.InitView(conversationRate, Data.Analytics.GetConversionRates(_currentPeriod));
        occupancy.SetValue(Data.Analytics.GetPersentOccupancy(_currentPeriod));
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(week, _ => SetPeriod(TimePeriod.Week));
        UIContainer.SubscribeToView<ButtonView, object>(month, _ => SetPeriod(TimePeriod.Month));
        UIContainer.SubscribeToView<ButtonView, object>(year, _ => SetPeriod(TimePeriod.Year));
        UIContainer.SubscribeToView<ButtonView, object>(exportButton, _ => ExportCsv());
    }
    public void ExportCsv()
    {
        var kpiData = Data.Analytics.GetKPIs(_currentPeriod);
        var salesData = Data.Analytics.GetTicketSalesByPeriod(_currentPeriod);
        var conversionData = Data.Analytics.GetConversionRates(_currentPeriod);
        var occupancyPercent = Data.Analytics.GetPersentOccupancy(_currentPeriod);

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