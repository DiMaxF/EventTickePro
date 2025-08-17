using DG.Tweening;
using E2C;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowChartView : View
{
    [SerializeField] private E2Chart chart;
    [SerializeField] private Color colorBar = new Color(1, 0.851f, 0);
    private ChartData _data;
    private CanvasGroup canvasGroup;

    public override void Init<T>(T data)
    {
        if (data is ChartData d)
        {
            _data = d;
        }

        base.Init(data);
    }

    public override void UpdateUI()
    {
        if (_data == null || _data.values == null || _data.values.Count == 0)
        {
            Debug.LogWarning("[RowChartView] No valid data to display in chart");
            return;
        }

        if (chart == null)
        {
            Debug.LogError("[RowChartView] E2Chart component is not assigned");
            return;
        }

        RectTransform chartRect = chart.GetComponent<RectTransform>();
        if (chartRect.sizeDelta.x <= 0 || chartRect.sizeDelta.y <= 0)
        {
            Debug.LogWarning("[RowChartView] Chart RectTransform has invalid size. Setting default size.");
            chartRect.sizeDelta = new Vector2(400, 300); 
        }

        chart.gameObject.SetActive(true);
        chart.chartType = E2Chart.ChartType.BarChart;

        E2ChartOptions chartOptions = chart.GetComponent<E2ChartOptions>();
        if (chartOptions == null)
        {
            Debug.Log("[RowChartView] Adding E2ChartOptions...");
            chartOptions = chart.gameObject.AddComponent<E2ChartOptions>();
        }

        chartOptions.title.enableTitle = true;
        chartOptions.title.enableSubTitle = false;
        chartOptions.title.titleTextOption = new E2ChartOptions.TextOptions { fontSize = 24 };

        chartOptions.xAxis.enableTitle = true;
        chartOptions.xAxis.titleTextOption = new E2ChartOptions.TextOptions { fontSize = 30 };
        chartOptions.xAxis.labelTextOption.fontSize = 36;
        chartOptions.xAxis.enableAxisLine = true;
        chartOptions.xAxis.axisLineWidth = 3.0f;
        chartOptions.xAxis.axisLineColor = Color.white;
        chartOptions.xAxis.gridLineWidth = 3.0f;

        chartOptions.yAxis.enableTitle = true;
        chartOptions.yAxis.titleTextOption = new E2ChartOptions.TextOptions { fontSize = 30 };
        chartOptions.yAxis.labelTextOption.fontSize = 36;
        chartOptions.yAxis.enableAxisLine = true;
        chartOptions.yAxis.axisLineWidth = 3.0f;
        chartOptions.yAxis.axisLineColor = Color.white;
        chartOptions.yAxis.gridLineWidth = 3.0f;

        chartOptions.label.enable = true;
        chartOptions.label.textOption = new E2ChartOptions.TextOptions
        {
            fontSize = 46,
            color = Color.white
        };
        chartOptions.label.offset = 26f;

        chartOptions.legend.enable = true;
        chartOptions.legend.textOption.fontSize = 44;
        chartOptions.plotOptions.mouseTracking = E2ChartOptions.MouseTracking.BySeries;
        chartOptions.plotOptions.colorMode = E2ChartOptions.ColorMode.BySeries;
        chartOptions.chartStyles.barChart.barWidth = 80f;
        //chartOptions.chartStyles.
        chartOptions.chartStyles.barChart.categoryBackgroundColor = colorBar;
        chartOptions.chartStyles.barChart.barBackgroundColor = colorBar;


        E2ChartData chartData = chart.GetComponent<E2ChartData>();
        if (chartData == null)
        {
            Loger.Log("Adding E2ChartData...", "RowChartView");
            chartData = chart.gameObject.AddComponent<E2ChartData>();
        }

        chartData.title = _data.title;
        chartData.xAxisTitle = "";
        chartData.yAxisTitle = "";
        chartData.categoriesX = new List<string>();
        List<float> values = new List<float>();

        foreach (var data in _data.values)
        {
            chartData.categoriesX.Add(data.Key);
            values.Add(data.Value);
            Loger.Log($"Adding data: Key={data.Key}, Value={data.Value}", "RowChartView");
        }

        E2ChartData.Series valueSeries = new E2ChartData.Series
        {
            name = _data.title,
            dataY = values
        };

        chartData.series = new List<E2ChartData.Series> { valueSeries };

        Loger.Log($"Updating chart with {chartData.categoriesX.Count} categories and {values.Count} values", "RowChartView");
        LayoutRebuilder.ForceRebuildLayoutImmediate(chartRect);
        chart.UpdateChart();
    }

    public override void Show()
    {
        base.Show();
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Loger.Log("Adding CanvasGroup...", "RowChartView");
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        chart.gameObject.SetActive(true);

        DOTween.Sequence()
            .AppendCallback(() => LayoutRebuilder.ForceRebuildLayoutImmediate(chart.GetComponent<RectTransform>()))
            .Append(canvasGroup.DOFade(1f, 0.5f))
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Loger.Log("Chart show animation completed", "RowChartView");
                UpdateUI();
            });
    }

    public override void Hide()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        canvasGroup.DOKill();
        DOTween.Sequence()
            .Append(canvasGroup.DOFade(0f, 0.5f))
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                chart.gameObject.SetActive(false);
                base.Hide();
                Loger.Log("Chart hidden", "RowChartView");
            });
    }
}