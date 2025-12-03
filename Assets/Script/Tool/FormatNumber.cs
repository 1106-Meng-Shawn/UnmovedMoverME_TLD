using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FormatNumber;
public class FormatNumber 
{
    public static string FormatDoubleNumberToFormatNumber(double value)
    {
        double temp = 1000;
        if (value < temp)
        {
            return FormatDoubleNumberd(value);

        }
        else
        {
            return FormatNumberToString((float)value);

        }
    }
    public static string FormatDoubleNumberd(double value)
    {
        int valueInt = (int)Math.Round(value * 10);

        if (valueInt % 10 == 0)
        {
            return value.ToString("F0");
        }
        else
        {
            return value.ToString("F1");
        }
    }

    public static string FormatfloatNumber(float value)
    {
        int valueInt = (int)(value * 10);
        if (valueInt % 10 == 0)
        {
            return value.ToString("F0");
        }
        else
        {
            return value.ToString("F1");
        }
    }

    public static string FormatfloatNumberByN(float value)
    {
        int valueInt = (int)(value * 10);
        if (valueInt % 10 == 0)
        {
            return value.ToString("N0");
        }
        else
        {
            return value.ToString("N1");
        }
    }


    public static string FormatNumberToString(float value)
    {
        if (value >= 1000000000)
        {
            int billions = Mathf.FloorToInt(value / 100000000f);
            float temp = Mathf.RoundToInt(billions) / 10f;

            if (temp == Mathf.Floor(temp))
            {
                return temp + "B";
            }
            else
            {
                return temp.ToString("0.0") + "B";
            }
        }
        else if (value >= 1000000)
        {
            int millions = Mathf.FloorToInt(value / 100000f);
            float temp = Mathf.RoundToInt(millions) / 10f;

            if (temp == Mathf.Floor(temp))
            {
                return temp + "M";
            }
            else
            {
                return temp.ToString("0.0") + "M";
            }
        }
        else if (value >= 1000)
        {
            int hundred = Mathf.FloorToInt(value / 100f);
            float temp = Mathf.RoundToInt(hundred) / 10f;


            if (temp == Mathf.Floor(temp))
            {

                return ((int)temp).ToString() + "K";  
            }
            else
            {

                return temp.ToString("0.0") + "K";
            }
        }
        else
        {
            return value.ToString("F0");
        }
    }
}
