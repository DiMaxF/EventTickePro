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
        var selected = data.Personal.GetSelectedEvent();
        name.text = selected.name;
        date.text = selected.date;
        time.text = selected.time;
        venue.text = selected.venue;
        previewBg.color = Color.clear;
        desciption.text = selected.description;
        UIContainer.InitView(image, selected.imgPath);
        var map = data.Maps.GetByEvent(selected);
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
        var mapTickets = data.Maps.GetByEvent(data.Personal.GetSelectedEvent());
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
        data.Personal.SetSelectedEvent(null);
    }

    private void OnButtonMapEditor()
    {
        SceneManager.LoadScene("MapEditor");
    }
}
