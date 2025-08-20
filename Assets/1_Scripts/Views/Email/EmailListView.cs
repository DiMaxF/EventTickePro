using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class EmailListView : View
{
    [SerializeField] Text count;
    [SerializeField] InputTextView name;
    [SerializeField] InputTextView email;
    [SerializeField] RectTransform container;

    [Header("Animations")]
    [SerializeField] AnimationConfig fadeIn;
    [SerializeField] AnimationConfig moveAnim;
    Vector3 containerOriginalPos;
    private void Awake()
    {
        Vector3 containerOriginalPos = container.localPosition;

    }

    public EmailModel GetModel() 
    {         
        return new EmailModel(name.text, email.text);
    }   
    public override void Init<T>(T data)
    {
        if(data is EmailModel m) 
        {
            UIContainer.RegisterView(name);
            UIContainer.RegisterView(email);
            UIContainer.InitView(email, m.email);
            UIContainer.InitView(name, m.name);
        }
        base.Init(data);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();

        count.text = $"Ticket owner {transform.GetSiblingIndex()}";
    }


    public override void Show()
    {
        base.Show();
        /*container.localPosition = containerOriginalPos + new Vector3(200, 0);
        container.localScale = Vector3.one * 0.8f;

        StartAnimation()
            .Append(container.DOLocalMoveX(containerOriginalPos.x, 0.5f).SetEase(Ease.OutCubic))
            .Join(container.DOScale(1.05f, 0.3f).SetEase(Ease.OutBack))
            .Append(container.DOScale(1f, 0.2f).SetEase(Ease.InOutSine));*/
    }
}
