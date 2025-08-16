using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TicketModel 
{
    public EmailModel contacts;
    public string qrPath;
    public bool valid;
    public SeatModel seat;
}
