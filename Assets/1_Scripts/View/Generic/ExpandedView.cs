using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpandedView : View
{
    [SerializeField] View[] views;
    [SerializeField] Button expand;
    [SerializeField] int defaultSize;
    [SerializeField] int expandedSize;
    bool _active;

    private RectTransform _rectTransform;
    public override void Init<T>(T data)
    {
        _rectTransform = GetComponent<RectTransform>();
        if (data is bool active) 
        {
            _active = active;
        }

        UIContainer.SubscribeToComponent<Button, object>(expand, _ => ToggleExpand());
        base.Init(data);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();

        float targetSize = _active ? expandedSize : defaultSize;
        _rectTransform.DOSizeDelta(new Vector2(_rectTransform.sizeDelta.x, targetSize), 0.5f)
            .SetEase(Ease.InOutSine);

        float targetRotation = _active ? 0f : 180f;
        expand.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, targetRotation), 0.5f)
            .SetEase(Ease.InOutSine);

        foreach (var view in views)
        {
           if(_active) view.Show();
           else view.Hide();    
        }
    }
    VerticaUpdater updater;
    private void ToggleExpand()
    {
        if (updater == null) updater = GetComponentInParent<VerticaUpdater>();
        _active = !_active;
        UpdateUI();
        if (updater != null) updater.UpdateSpacing();
        
    }

}
