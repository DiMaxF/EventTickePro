using DG.Tweening;
using E2C;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RowChartView : View
{
    [SerializeField] private E2Chart chart;
    [SerializeField] private Color labelTextColor = Color.black;
    [SerializeField] private Color barColor = new Color(1, 0.851f, 0);

    private E2ChartOptions chartOptions;
    private E2ChartData chartData;
    private ChartData _data;


    private void Awake()
    {


        chartOptions = chart.GetComponent<E2ChartOptions>();
        chartData = chart.GetComponent<E2ChartData>();

        chart.chartType = E2Chart.ChartType.BarChart;

        chartOptions.title.enableTitle = false;
        chartOptions.title.enableSubTitle = false;
        chartOptions.title.titleTextOption = new E2ChartOptions.TextOptions { fontSize = 24, color = Color.clear };

        chartOptions.xAxis.enableTitle = true;
        chartOptions.xAxis.titleTextOption = new E2ChartOptions.TextOptions { fontSize = 30 };
        chartOptions.xAxis.labelTextOption.fontSize = 36;
        chartOptions.xAxis.enableAxisLine = true;
        chartOptions.xAxis.axisLineWidth = 3.0f;
        chartOptions.xAxis.tickColor = Color.white;
        chartOptions.xAxis.axisLineColor = Color.white;
        chartOptions.xAxis.gridLineWidth = 3.0f;

        chartOptions.yAxis.enableTitle = true;
        chartOptions.yAxis.titleTextOption = new E2ChartOptions.TextOptions { fontSize = 30 };
        chartOptions.yAxis.labelTextOption.fontSize = 36;
        chartOptions.yAxis.enableAxisLine = true;
        chartOptions.yAxis.axisLineWidth = 3.0f;
        chartOptions.yAxis.axisLineColor = Color.white;
        chartOptions.yAxis.tickColor = Color.white;
        chartOptions.yAxis.gridLineWidth = 3.0f;
        chartOptions.label.offset = 30f;

        chartOptions.label.enable = false;
        chartOptions.legend.enable = false;
        chartOptions.legend.textOption.fontSize = 44;
        chartOptions.legend.textOption.color = labelTextColor;
        chartOptions.plotOptions.mouseTracking = E2ChartOptions.MouseTracking.BySeries;
        chartOptions.plotOptions.colorMode = E2ChartOptions.ColorMode.BySeries;
        chartOptions.chartStyles.barChart.barWidth = 80f;
        chartOptions.chartStyles.barChart.categoryBackgroundColor = barColor;
        chartOptions.chartStyles.barChart.barBackgroundColor = barColor;

        chartOptions.label.textOption = new E2ChartOptions.TextOptions
        {
            fontSize = 46,
            color = labelTextColor
        };
    }

    public override void UpdateUI()
    {
        if (_data == null || _data.values == null || _data.values.Count == 0)
        {
            Logger.LogWarning("No valid data to display in chart", "RowChartView");
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
                Logger.LogWarning("Adding E2ChartOptions...", "RowChartView");
                chartOptions = chart.gameObject.AddComponent<E2ChartOptions>();
            }

            chartData.title = _data.title;
            chartData.xAxisTitle = "";
            chartData.yAxisTitle = "";
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
                chartOptions.plotOptions.seriesColors = new Color[] { barColor };
            }
            else
            {
                chartData.categoriesX = _data.values.Keys.ToList();
                chartData.series.Add(new E2ChartData.Series
                {
                    name = _data.title,
                    dataY = _data.values.Values.ToList()
                });
                chartOptions.plotOptions.seriesColors = new Color[] { barColor };
            }
        }
        base.Init(data);
    }

}