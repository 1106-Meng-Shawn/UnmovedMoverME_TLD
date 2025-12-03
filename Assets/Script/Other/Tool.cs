using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tool;


public class Tool 
{
    public static T ParseEnumOrDefault<T>(string value) where T : Enum
    {
        if (Enum.TryParse(typeof(T), value, true, out object result))
        {
            if (Enum.IsDefined(typeof(T), result))
            {
                return (T)result;
            }
        }
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(0);
    }
}
