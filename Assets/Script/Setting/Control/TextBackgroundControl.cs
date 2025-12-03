using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextBackgroundControl : MonoBehaviour
{
    [SerializeField] StoryCharacterImageControl StoryCharacterImageControl;
    [SerializeField] Image characterIcon;
    [SerializeField] Image dialogBox;
    [SerializeField] TMP_Text characterName;
    [SerializeField] TMP_Text Content;


    public void OpenPanel(string characterKey)
    {
        SettingValue.Instance.GetTextSettingValue().OnValueChanged += RefreshUI;
        RefreshUI();
    }


    void OnDisable()
    {
        SettingValue.Instance.GetTextSettingValue().OnValueChanged -= RefreshUI;
    }


    void RefreshUI()
    {
        Debug.Log("RefreshUI");
        dialogBox.color = SettingValue.Instance.GetTextSettingValue().DialogBoxColor;
        Content.color = SettingValue.Instance.GetTextSettingValue().NormalTextColor;
    }
}
