using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateFormatter 
{
    public static string Format = "dd.MM.yyyy";
    public static string TimeFormat = @"hh\:mm";
    public static string FormatShort = "dd.MM";
    public static string Full = "dd.MM.yyyy HH:mm:ss";
    public static int Id() 
    {
        var strId = DateTime.Now.ToString(Full).Replace(".", "").Replace(":", "").Replace(" ", "");
        if (!long.TryParse(strId, out var id)) return -1;
        return (int)id;
    }

}
