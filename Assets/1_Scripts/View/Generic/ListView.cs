using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ListView : View
{
    [SerializeField] private Transform _contentParent;
    [SerializeField] private View _itemPrefab;
    [SerializeField] private View noItemPrefab;
    [Header("Spawn Animation Settings")]
    [SerializeField] private float spawnAnimationDuration = 0.3f; 
    [SerializeField] private float spawnStartScale = 0.8f; 
    [SerializeField] private float spawnDelayPerItem = 0.1f;
    private readonly List<View> _items = new();
    public List<View> Items => _items;
    private bool _isUpdating;
    List<object> _dataSource = new List<object>();
    public void Init<TData>(List<TData> dataSource)
    {
        _dataSource = dataSource.Cast<object>().ToList();
        Loger.Log($"[ListView][{name}]{_dataSource.Count}");
        UpdateUI();
    }
    public void Init(Type enumType)
    {
        if (!enumType.IsEnum)
        {
            Loger.LogError($"[ListView][{name}] Provided type {enumType.Name} is not an enum. Initializing with empty list.");
            Init(new List<object>());
            return;
        }

        _dataSource = Enum.GetValues(enumType).Cast<object>().ToList();
        Loger.Log($"[ListView][{name}] Initialized with enum {enumType.Name}, {_dataSource.Count} values");
        UpdateUI();
    }
    public override void Init<TData>(TData data)
    {
        if (data is Type type && type.IsEnum)
        {
            Loger.Log($"[ListView][{name}] Type (enum: {type.Name})");
            Init(type);
        }
        else if (data is List<object> list)
        {
            Loger.Log($"[ListView][{name}] List<object>");
            Init(list);
        }
        else if (data is IEnumerable<object> enumerable)
        {
            Loger.Log($"[ListView][{name}] IEnumerable<object>");
            Init(enumerable.ToList());
        }
        else
        {
            Init(new List<object>());
        }
    }

    public override void UpdateUI()
    {
        if (_isUpdating)
        {
            return;
        }

        _isUpdating = true;
        ClearItems();
        foreach (var item in _dataSource)
        {
            var view = Instantiate(_itemPrefab, _contentParent, false);
            if (view is View truelyView)
            {
                UIContainer.RegisterView(truelyView);
                UIContainer.InitView(truelyView, item);
                _items.Add(truelyView);
                UIContainer.SubscribeToView(truelyView, (object data) => TriggerAction(data));
            }

        }
        AnimateItemsSpawn(_items);
        if (_items.Count == 0) 
        {
            Instantiate(noItemPrefab, _contentParent, false);
        }
        _isUpdating = false;
    }

    private void ClearItems()
    {
        foreach (var item in _items)
        {
            UIContainer.UnregisterView(item);
            Destroy(item.gameObject);
        }
        foreach (Transform c in _contentParent)
        {
            Destroy(c.gameObject);
        }
        _items.Clear();
    }

    private void AnimateItemsSpawn(List<View> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var transform = item.transform;
            var canvasGroup = item.GetComponent<CanvasGroup>();

            // Добавляем CanvasGroup, если отсутствует
            if (canvasGroup == null)
            {
                canvasGroup = item.gameObject.AddComponent<CanvasGroup>();
            }

            // Начальное состояние
            canvasGroup.alpha = 0f;
            transform.localScale = Vector3.one * spawnStartScale;

            var sequence = DOTween.Sequence();
            sequence.AppendInterval(spawnDelayPerItem * i); 
            sequence.Append(transform.DOScale(Vector3.one, spawnAnimationDuration)
                .SetEase(Ease.OutBack));
            sequence.Join(canvasGroup.DOFade(1f, spawnAnimationDuration)
                .SetEase(Ease.OutQuad));
            sequence.SetTarget(item).Play();

            Loger.Log($"[ListView][{name}] Animating item {i} spawn");
        }
    }
}