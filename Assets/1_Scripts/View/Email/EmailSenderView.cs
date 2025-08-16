using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EmailSenderView : View
{
    [SerializeField] ListView list;
    [SerializeField] Button add;
    [SerializeField] Button send;
    [SerializeField] Button cancel;

    List<EmailModel> emails;

    public override void Init<T>(T data)
    {
        Loger.Log($"Init 0", "EmailSenderView");

        emails = new List<EmailModel>();
        UIContainer.RegisterView(list);
        Loger.Log($"Init", "EmailSenderView");
        base.Init(data);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        UIContainer.SubscribeToComponent<Button, object>(add, _ => OnButtonAdd());
        UIContainer.SubscribeToComponent<Button, object>(send, _ => OnButtonSend());
        UIContainer.SubscribeToComponent<Button, object>(cancel, _ => OnButtonCancel());
        UIContainer.InitView(list, emails);
    }

    private void OnButtonAdd() 
    {
        FetchEmails();
        emails.Add(new EmailModel("", ""));
        UpdateUI();
       
        //UIContainer.SubscribeToComponent<ListView, EmailModel>(list, emails);
    }

    private void FetchEmails()
    {
        emails.Clear();
        Loger.Log($"start fetch", "EmailSenderView");

        foreach (var em in list.Items) 
        {
            if(em is EmailListView emailListView) 
            {
                Loger.Log($"{emailListView.GetModel().email} {emailListView.GetModel().name}", "EmailSenderView");
                emails.Add(emailListView.GetModel());
            }
        }    
    }

    private void OnButtonSend()
    {
        if (emails == null || emails.Count == 0)
            return;

        var emailAddresses = string.Join(",", emails.ConvertAll(e => e.email));

        string subject = UnityWebRequest.EscapeURL("Тема письма");
        string body = UnityWebRequest.EscapeURL("Текст письма");
        string mailto = $"mailto:{emailAddresses}?subject={subject}&body={body}";

        TriggerAction(emails);
        NativeMobilePlugin.Instance.OpenEmailMultiple(emails.ConvertAll(e => e.email).ToArray(), subject, body);
    }

    private void OnButtonCancel()
    {
        Hide();
    }

}
