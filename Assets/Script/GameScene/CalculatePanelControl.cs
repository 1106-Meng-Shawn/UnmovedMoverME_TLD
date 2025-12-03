using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CalculatePanelControl : MonoBehaviour
{
    public TMP_Text valueText;
    public TMP_Text calculateText;


    public void ShowTheCalulatePanel(int value, int calculateValue){
        valueText.text = (value - calculateValue).ToString("N0");
        calculateText.text = calculateValue.ToString("N0");

    }

    public void ShowTheCalulatePanel(float value, int calculateValue){
        valueText.text = (value - calculateValue).ToString("N0");
        calculateText.text = calculateValue.ToString("N0");

    }

}
