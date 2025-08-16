using System;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SeatModel
{
    public string seatId; 
    public int count; 
    public bool isTaken;

    public SeatModel(string seatId, int count, bool isTaken = false)
    {
        this.seatId = seatId;
        this.count = count;
        this.isTaken = isTaken;
    }
}


