using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticaUpdater : MonoBehaviour
{
     private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private float animationDuration = 1f;
    private Tween spacingTween;

    private void OnEnable()
    {
        UpdateSpacing();
    }
    public void UpdateSpacing() 
    {
        if (verticalLayoutGroup == null) verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        spacingTween?.Kill();

        spacingTween = DOTween.To(
            () => verticalLayoutGroup.spacing,
            value => verticalLayoutGroup.spacing = value,
            verticalLayoutGroup.spacing++,
            animationDuration
        ).SetEase(Ease.Linear);
    }
    private void OnDisable()
    {
        spacingTween?.Kill();
    }
}
