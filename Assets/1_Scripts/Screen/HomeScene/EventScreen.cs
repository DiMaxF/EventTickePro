using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventScreen : AppScreen
{
    [SerializeField] AsyncImageView image;
    [SerializeField] AsyncImageView preview;
    [SerializeField] Text name;
    [SerializeField] Text date;
    [SerializeField] Text time;
    [SerializeField] Text venue;
    [SerializeField] Text desciption;
    [SerializeField] Image previewBg;
    [Header("Buttons")]
    [SerializeField] ButtonView back;
    [SerializeField] ButtonView mapEditor;
    [SerializeField] ButtonView ticketsDistribution;
    [SerializeField] ButtonView tickets;

    protected override void UpdateViews()
    {
        base.UpdateViews();
        name.text = data.selectedEvent.name;
        date.text = data.selectedEvent.date;
        time.text = data.selectedEvent.time;
        venue.text = data.selectedEvent.venue;
        previewBg.color = Color.clear;
        desciption.text = data.selectedEvent.description;
        UIContainer.InitView(image, data.selectedEvent.imgPath);
        var map = data.GetByEvent(data.selectedEvent);
        if (map != null) 
        {
            previewBg.color = Color.white;
            UIContainer.InitView(preview, map.pathPreview);
        }
        else
        {
            UIContainer.InitView(preview, "");
        }
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(tickets, _ => OnButtonTickets());
        UIContainer.SubscribeToView<ButtonView, object>(back, _ => OnButtonBack());
        UIContainer.SubscribeToView<ButtonView, object>(ticketsDistribution, _ => OnButtonTicketsDistribution());
        UIContainer.SubscribeToView<ButtonView, object>(mapEditor, _ => OnButtonMapEditor());
    }
    private void OnButtonTickets() 
    {
        container.Show<TicketsScreen>();
    }
    private void OnButtonTicketsDistribution()
    {
        var mapTickets = data.GetByEvent(data.selectedEvent);
        if (mapTickets != null) 
        {
            container.Show<TicketsDistributionScreen>();
        }
        else
        {
            NativeMobilePlugin.Instance.ShowToast("First set map seats");
        }
        
    }
    private void OnButtonBack()
    {
        container.Show<HomeScreen>();
        data.selectedEvent = null;
    }

    private void OnButtonMapEditor()
    {
        SceneManager.LoadScene("MapEditor");
    }
}
