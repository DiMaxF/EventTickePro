using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EmailSenderView : View
{
    [SerializeField] ListView list;
    [SerializeField] ButtonView add;
    [SerializeField] ButtonView send;
    [SerializeField] ButtonView cancel;

    List<EmailModel> emails;

    public override void Init<T>(T data)
    {
        emails = new List<EmailModel>();
        UIContainer.RegisterView(list);
        base.Init(data);
        OnButtonAdd();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();

        UIContainer.InitView(list, emails);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(add, _ => OnButtonAdd());
        UIContainer.SubscribeToView<ButtonView, object>(send, _ => OnButtonSend());
        UIContainer.SubscribeToView<ButtonView, object>(cancel, _ => OnButtonCancel());
    }

    private void OnButtonAdd() 
    {
        FetchEmails();
        emails.Add(new EmailModel("", ""));
        UpdateUI();
    }

    private void FetchEmails()
    {
        emails.Clear();
        Logger.Log($"start fetch", "EmailSenderView");

        foreach (var em in list.Items) 
        {
            if(em is EmailListView emailListView) 
            {
                Logger.Log($"{emailListView.GetModel().email} {emailListView.GetModel().name}", "EmailSenderView");
                emails.Add(emailListView.GetModel());
            }
        }
        Logger.Log($"end fecth {emails.Count}", "EmailSenderView");

    }

    private void OnButtonSend()
    {
        if (emails == null || emails.Count == 0) return;

        string subject = UnityWebRequest.EscapeURL("Тема письма");
        string body = UnityWebRequest.EscapeURL("Текст письма");

        TriggerAction(emails);
        NativeMobilePlugin.Instance.OpenEmailMultiple(emails.ConvertAll(e => e.email).ToArray(), subject, body);
    }

    private void OnButtonCancel()
    {
        Hide();
    }
}
