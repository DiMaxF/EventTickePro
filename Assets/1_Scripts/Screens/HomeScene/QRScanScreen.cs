using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QRScanScreen : AppScreen
{
    [SerializeField] Text statusText;
    [SerializeField] StatusScanView status;
    [SerializeField] DeviceCameraController camera;
    [SerializeField] QRCodeDecodeController e_qrController;

    protected override void OnStart()
    {
        base.OnStart();

        Play();
        statusText.text = "Scanning...";
    }

    private void OnDisable()
    {
        Stop();
      
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        e_qrController.onQRScanFinished.RemoveAllListeners();
        e_qrController.onQRScanFinished.AddListener(qrScanFinished);
    }


    public void qrScanFinished(string dataText)
    {

        if(dataText != null) 
        {
            
            var contacts = new EmailModel(ParseTicketString(dataText).name, ParseTicketString(dataText).email);
            NativeMobilePlugin.Instance.ShowToast($"Data okay: {contacts.email} {contacts.name}");

            var t = Data.Tickets.FindTicket(contacts);
            if (t != null && t.valid != false)
            {
                t.valid = false;
                UIContainer.InitView(status, StatusScanning.Success);
                statusText.text = "Scanned";
                Data.SaveData();
                NativeMobilePlugin.Instance.ShowToast("Tickets is scanned");
            }else if (t.valid == false)
            {

                UIContainer.InitView(status, StatusScanning.Failed);
                statusText.text = "Failed";
                NativeMobilePlugin.Instance.ShowToast("Ticket is used");

            }
        }
    }
    public (string name, string email) ParseTicketString(string ticketString)
    {
        string name = null;
        string email = null;

        string[] lines = ticketString.Split('\n');

        foreach (var line in lines)
        {
            if (line.StartsWith("Name: ", StringComparison.OrdinalIgnoreCase))
            {
                name = line.Substring("Name: ".Length).Trim();
            }
            else if (line.StartsWith("Email: ", StringComparison.OrdinalIgnoreCase))
            {
                email = line.Substring("Email: ".Length).Trim();
            }
        }

        return (name, email);
    }

    public void Play()
    {
        UIContainer.InitView(status, StatusScanning.Scanning);
        Reset();
        if (this.e_qrController != null)
        {
            this.e_qrController.StartWork();
        }
    }

    public void Stop()
    {
        if (this.e_qrController != null)
        {
            this.e_qrController.StopWork();
        }

    }
    public void Reset()
    {
        if (this.e_qrController != null)
        {
            this.e_qrController.Reset();
        }
    }
}
