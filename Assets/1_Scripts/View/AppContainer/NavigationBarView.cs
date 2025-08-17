using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationBarView : View
{
    [Serializable]
    public class Data
    {
        public AppScreen screen;
        public Sprite icon;
        [HideInInspector] public bool selected;
    }
    private RectTransform rectTransform;
    [SerializeField] private Transform content;
    [SerializeField] private NavigationButton buttonPrefab;
    [SerializeField] private List<Data> screens;

    private AppContainer container;
    private Vector3 initialPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.localPosition;
    }

    public override void Init<T>(T data)
    {
        if (data is AppContainer app) 
        {
            container = app;
        }
        base.Init(data);
    }

    public override void UpdateUI()
    {
        foreach (Transform t in content) Destroy(t.gameObject);

        foreach (var screen in screens)
        {
            var view = Instantiate(buttonPrefab, content);
            view.name = $"AppScreen{screen.screen.name}";
            UIContainer.RegisterView(view);
            var selected = container.OpenedScreen == null ? false : screen.screen == container.OpenedScreen;
            screen.selected = selected;
            UIContainer.InitView(view, screen);
            UIContainer.SubscribeToView<View, object>(view, _ => OnButtonClicked(screen.screen));
        }
    }

    private void OnButtonClicked(AppScreen screen)
    {
        container.Show(screen);
        //UpdateUI();
    }


    public override void Show()
    {
        base.Show();
        if (initialPosition != rectTransform.localPosition) 
        {
            float height = rectTransform.rect.height;

            rectTransform.localPosition = initialPosition - new Vector3(0, height, 0);
            rectTransform.DOLocalMove(initialPosition, 0.5f).SetEase(Ease.OutCirc);
        }

    }

    public override void Hide()
    {
        if (initialPosition == rectTransform.localPosition)
        {
            float height = rectTransform.rect.height;

            rectTransform.DOLocalMove(initialPosition - new Vector3(0, height, 0), 0.4f).SetEase(Ease.InQuint)
                .OnComplete(() => gameObject.SetActive(false));
        }
        base.Hide();
    }

}