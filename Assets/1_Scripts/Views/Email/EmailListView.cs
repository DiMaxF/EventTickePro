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

}
