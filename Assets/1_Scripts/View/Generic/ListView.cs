using Cysharp.Threading.Tasks;
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
    [SerializeField] private float spawnDelayPerItem = 0.05f;

    private readonly List<View> _items = new();
    public List<View> Items => _items;
    private bool _isUpdating;
    List<object> _dataSource = new List<object>();

    public override void Init<TData>(TData data)
    {
        if (data is IEnumerable<object> enumerable)
        {
            Init(enumerable.ToList());
        }
    }
    private void Init<TData>(List<TData> dataSource)
    {
        _dataSource = dataSource.Cast<object>().ToList();
        UpdateUI();
    }

    public override void UpdateUI()
    {
        if (_isUpdating) return;

        _isUpdating = true;
        ClearItems();

        foreach (var item in _dataSource) SpawnView(item);

        if (_items.Count == 0) Instantiate(noItemPrefab, _contentParent, false);

        _isUpdating = false;

        AnimateItemsSpawn(_items);
    }

    private void SpawnView(object item) 
    {
        var view = Instantiate(_itemPrefab, _contentParent, false);
        if (view is View truelyView)
        {
            UIContainer.RegisterView(truelyView);
            UIContainer.InitView(truelyView, item);
            Loger.Log($"({name}): Init View {truelyView.name} by {item}", "ListView");
            _items.Add(truelyView);
            UIContainer.SubscribeToView(truelyView, (object data) => TriggerAction(data));
        }
    }

    private void ClearItems()
    {
        foreach (var item in _items)
        {
            UIContainer.UnregisterView(item);
            Destroy(item.gameObject);
        }
        foreach (Transform c in _contentParent) Destroy(c.gameObject);
        _items.Clear();
    }

    private async void AnimateItemsSpawn(List<View> items)
    {
        foreach (var i in items) i.gameObject.SetActive(false);

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            await UniTask.Delay(TimeSpan.FromSeconds(spawnDelayPerItem), cancellationToken: this.GetCancellationTokenOnDestroy());

            if (item != null) item.Show();
        }
    }
}