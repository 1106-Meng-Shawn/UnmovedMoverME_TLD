using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayTime : MonoBehaviour
{
    public TMP_Text timeText;
    private bool use24HourFormat = true;

    public GameObject TimeAndAchievement;
    public GameObject Achievement;

    void Start()
    {
        SetTimeType(SettingsManager.Instance.GetDisplaySettingValue().DisplayTimeType);
    }

    void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        DateTime currentTime = DateTime.Now;
        string format = use24HourFormat ? "HH:mm" : "hh:mm"; 
        timeText.text = currentTime.ToString(format);
    }

    public void SetTimeType(int type)
    {

        if (type == 0)
        {
            use24HourFormat = false;
            TimeAndAchievement.gameObject.SetActive(true);
            Achievement.gameObject.SetActive(false);
        }
        else if (type == 1)
        {
            use24HourFormat = true;
            TimeAndAchievement.gameObject.SetActive(true);
            Achievement.gameObject.SetActive(false);

        }
        else if (type == 2)
        {
            TimeAndAchievement.gameObject.SetActive(false);
            Achievement.gameObject.SetActive(true);

        }

    }


}