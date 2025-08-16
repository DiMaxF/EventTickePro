using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks; // Äëÿ UniTask

public class BaseView : View
{
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float offsetY = -50f;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public override async void Show()
    {
        base.Show();
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = originalPosition + new Vector2(0, offsetY);
        canvasGroup.DOFade(1f, animationDuration);
        rectTransform.DOAnchorPosY(originalPosition.y, animationDuration).SetEase(Ease.OutQuad);
    }

    public override void Hide()
    {
        canvasGroup.DOFade(0f, animationDuration).OnComplete(() =>
        {
            base.Hide();
            rectTransform.anchoredPosition = originalPosition;
            canvasGroup.alpha = 1f;
        });
        rectTransform.DOAnchorPosY(originalPosition.y + offsetY, animationDuration).SetEase(Ease.InQuad);
    }

    private void OnDestroy()
    {
        DOTween.Kill(canvasGroup);
        DOTween.Kill(rectTransform);
    }
}