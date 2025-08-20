using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ListView : View
{
    public bool persistent;

    [SerializeField] private Transform _contentParent;
    [SerializeField] private View _itemPrefab;
    [SerializeField] private View noItemPrefab;
    [SerializeField] private float spawnDelayPerItem = 0.05f;

    private readonly List<View> _items = new();
    public List<View> Items => _items;
    private bool _isUpdating;
    List<object> _dataSource = new List<object>();
    public bool isGenerated => _items.Count > 0;

    public override void Init<TData>(TData data)
    {
        if (data is IEnumerable<object> enumerable)
        {
            _dataSource = enumerable.Cast<object>().ToList();
        }
        base.Init(data);
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
        UIContainer.RegisterView(view, persistent);
        UIContainer.InitView(view, item);
        _items.Add(view);
        UIContainer.SubscribeToView(view, (object data) => TriggerAction(data), persistent);
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
        if(!persistent) foreach (var i in items) i.gameObject.SetActive(false);

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            await UniTask.Delay(TimeSpan.FromSeconds(spawnDelayPerItem), cancellationToken: this.GetCancellationTokenOnDestroy());

            if (item != null) item.Show();
        }
    }

    public void UpdateViewsData<TData>(List<TData> newData)
    {
        if (_isUpdating) return;

        _isUpdating = true;
        var newDataList = newData.Cast<object>().ToList();

        for (int i = 0; i < newDataList.Count; i++)
        {
            if (i < _items.Count)
            {
                UIContainer.InitView(_items[i], newDataList[i]);
                Logger.Log($"({name}): Updated View {_items[i].name} with data {newDataList[i]}", "ListView");
            }
            else
            {
                SpawnView(newDataList[i]);
            }
        }

        foreach (Transform c in _contentParent)
        {
            if (c.GetComponent<View>() == noItemPrefab)
            {
                Destroy(c.gameObject);
            }
        }

        if (newDataList.Count == 0)
        {
            Instantiate(noItemPrefab, _contentParent, false);
        }

        _dataSource = newDataList;
        _isUpdating = false;

        //AnimateItemsSpawn(_items);
    }
}