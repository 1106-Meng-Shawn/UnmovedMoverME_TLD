using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TipPanelControl : MonoBehaviour
{
    [SerializeField] Image TipIcon;
    [SerializeField] TextMeshProUGUI TipNameText;
    [SerializeField] TextMeshProUGUI TipDescribeText;
    [SerializeField] Button NextTipButton;

    private List<string> textString = new List<string>() { "Text 1","Text 2", "Text 3", "Text 4", "Text 5", "Text 6", "Text 7", "Text 8" };


    void Start()
    {
        NextTipButton.onClick.AddListener(ShowRandomTip);
    }

    void Update()
    {
        
    }

    public void ShowRandomTip()
    {
        int index = Random.Range(0, textString.Count);
        TipNameText.text = textString[index];
    }

}
