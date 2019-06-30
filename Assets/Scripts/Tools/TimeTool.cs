using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTool
{
    public static int GetUnixTimeStamp(DateTime dateTime)
    {
        DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
        int timestamp = (int)((dateTime - dateStart).TotalSeconds);
        return timestamp;
    }

    public static DateTime GetDateTime(int timestamp)
    {
        DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
        DateTime dateTime = dateStart.AddSeconds(timestamp);
        return dateTime;
    }
}
