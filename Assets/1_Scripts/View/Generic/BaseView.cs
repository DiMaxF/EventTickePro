using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class BaseView : View
{
    [SerializeField] AnimationConfig fadeIn;
    [SerializeField] AnimationConfig fadeOut;
    [SerializeField] AnimationConfig scaleAnim;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 _initialScale;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        rectTransform = GetComponent<RectTransform>();
        _initialScale = transform.localScale;
    }

    public override void Show()
    {
        base.Show();
        transform.localScale = _initialScale * 0.8f;
        canvasGroup.alpha = 0f;

        Tween fade = canvasGroup.DOFade(1f, fadeIn.Duration).SetEase(fadeIn.Ease);
        Tween scaleTween = transform.DOScale(_initialScale, scaleAnim.Duration).SetEase(scaleAnim.Ease);
        StartAnimation().Append(fade).Join(scaleTween);
    }

    public override void Hide()
    {
        StartAnimation().Append(
            canvasGroup.DOFade(0f, fadeOut.Duration).OnComplete(() =>
            {
                base.Hide();
                canvasGroup.alpha = 1f;
            }).SetEase(fadeOut.Ease));
    }

    private void OnDestroy()
    {
        DOTween.Kill(canvasGroup);
        DOTween.Kill(rectTransform);
    }
}