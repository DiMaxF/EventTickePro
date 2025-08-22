using DG.Tweening;
using E2C;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LineChartView : View
{
    [SerializeField] private E2Chart chart;
    [SerializeField] private Color labelTextColor = Color.black;
    [SerializeField] private Color[] lineColors = { Color.blue, Color.green };
    [SerializeField] private Color pointColor = Color.red;

    private E2ChartOptions chartOptions;
    private E2ChartData chartData;
    private ChartData _data;

    private void Awake()
    {


        chartOptions = chart.GetComponent<E2ChartOptions>();
        chartData = chart.GetComponent<E2ChartData>();

        chart.chartType = E2Chart.ChartType.LineChart;

        chartOptions.title.enableTitle = false;
        chartOptions.title.enableSubTitle = false;
        chartOptions.title.titleTextOption = new E2ChartOptions.TextOptions { fontSize = 24, color = Color.clear };

        chartOptions.xAxis.enableTitle = true;
        chartOptions.xAxis.titleTextOption = new E2ChartOptions.TextOptions { fontSize = 30 };
        chartOptions.xAxis.labelTextOption.fontSize = 36;
        chartOptions.xAxis.enableAxisLine = true;
        chartOptions.xAxis.axisLineWidth = 3.0f;
        chartOptions.xAxis.tickColor = labelTextColor;
        chartOptions.xAxis.axisLineColor = labelTextColor;
        chartOptions.xAxis.gridLineWidth = 3.0f;

        chartOptions.yAxis.enableTitle = true;
        chartOptions.yAxis.titleTextOption = new E2ChartOptions.TextOptions { fontSize = 30 };
        chartOptions.yAxis.labelTextOption.fontSize = 36;
        chartOptions.yAxis.enableAxisLine = true;
        chartOptions.yAxis.axisLineWidth = 3.0f;
        chartOptions.yAxis.axisLineColor = labelTextColor;
        chartOptions.yAxis.tickColor = labelTextColor;
        chartOptions.yAxis.gridLineWidth = 3.0f;
        chartOptions.label.offset = 30f;

        chartOptions.label.enable = false;
        chartOptions.legend.enable = false;
        chartOptions.legend.textOption.fontSize = 44;
        chartOptions.legend.textOption.color = labelTextColor;
        chartOptions.plotOptions.mouseTracking = E2ChartOptions.MouseTracking.BySeries;
        chartOptions.chartStyles.lineChart.pointOutlineColor = pointColor;
        chartOptions.chartStyles.lineChart.lineWidth = 15.0f;
        chartOptions.chartStyles.lineChart.pointSize = 30.0f;

        chartOptions.label.textOption = new E2ChartOptions.TextOptions
        {
            fontSize = 46,
            color = labelTextColor
        };
    }

    public override void UpdateUI()
    {
        if (_data == null)
        {
            Logger.LogWarning("Adding E2ChartOptions...", "LineChartView");
            return;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(chart.GetComponent<RectTransform>());
        chart.UpdateChart();
    }

    public override void Init<T>(T data)
    {
        if (data is ChartData d) 
        {
            _data = d;

            if (chartData == null)
            {
                chartData = chart.gameObject.AddComponent<E2ChartData>();
            }


            if (chartOptions == null)
            {
                Logger.LogWarning("Adding E2ChartOptions...", "LineChartView");
                chartOptions = chart.gameObject.AddComponent<E2ChartOptions>();
            }

            chartData.title = _data.title;
            chartData.xAxisTitle = "";
            chartData.yAxisTitle = _data.title.Contains("Conversion") ? "%" : "";
            chartData.categoriesX = new List<string>();
            chartData.series = new List<E2ChartData.Series>();

            if (_data.series != null && _data.series.Count > 0)
            {
                foreach (var series in _data.series)
                {
                    chartData.categoriesX = series.values.Keys.ToList();
                    chartData.series.Add(new E2ChartData.Series
                    {
                        name = series.name,
                        dataY = series.values.Values.ToList()
                    });
                }
                chartOptions.plotOptions.seriesColors = lineColors;
            }
            else
            {
                chartData.categoriesX = _data.values.Keys.ToList();
                chartData.series.Add(new E2ChartData.Series
                {
                    name = _data.title,
                    dataY = _data.values.Values.ToList()
                });
                chartOptions.plotOptions.seriesColors = new Color[] { lineColors[0] };
            }
        }
        base.Init(data);
    }
    public override void Show()
    {
        base.Show();
        UpdateUI();
    }

}