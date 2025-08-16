using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID || UNITY_IOS
using NativeShareNamespace;
#endif
public class TicketsScreen : AppScreen
{
    [SerializeField] ListView tickets;
    [SerializeField] SearchView search;
    [SerializeField] AsyncImageView qrCode;
    [SerializeField] Button share;

    private List<TicketModel> d;
    private TicketModel ticket;
    protected override void OnStart()
    {
        if (data.selectedEvent != null)
        {
            d = data.selectedEvent.tickets;

        }
        else
        {
            d = data.AllTickets();
        }
        ui.InitView(search, d);
        base.OnStart();

    }
    protected override void UpdateViews()
    {
        base.UpdateViews();
        ui.InitView(tickets, d);
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        ui.SubscribeToView<ListView, TicketModel>(tickets, ViewTicket);
        ui.SubscribeToView<SearchView, List<TicketModel>>(search, Search);
        ui.InitView(search, d);
        ui.SubscribeToView<SearchView, object>(search, _ => { Loger.Log($"{typeof(object)}", "TicketsScreen"); });
        ui.SubscribeToComponent<Button, object>(share, _ => OnButtonShare());
        

    }

    private void Search(List<TicketModel> tickets) 
    {
        d = tickets;
        UpdateViews();
    }

    private void ViewTicket(TicketModel ticket) 
    {
        ui.InitView(qrCode, ticket.qrPath);
        qrCode.Show();
        this.ticket = ticket;
    }

    private void OnButtonShare() 
    {
        var exist = File.Exists(ticket.qrPath);
        new NativeShare().AddFile(ticket.qrPath)
        .SetSubject("Subject goes here").SetText($"{ticket.contacts.name} ticket")
        //.SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
        .Share();
        //NativeMobilePlugin.Instance.ShareImage(ticket.qrPath, $"qr {exist}", $"{ticket.contacts.name} ticket");
    }
}
