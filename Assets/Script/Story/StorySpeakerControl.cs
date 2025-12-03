using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using static GetSprite;
using UnityEngine.Localization.Tables;
using Unity.VisualScripting;

public class StorySpeakerControl : MonoBehaviour
{
    public GameObject iconOj;
    public GameObject nameOj;

    public Image speakerIcon;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI speakerTitleText;

    private string iconPath;



    public void SetSpeakerTitle(string speakerName, string iconName)
    {
        iconOj.SetActive(!string.IsNullOrWhiteSpace(iconName));
        nameOj.SetActive(!string.IsNullOrWhiteSpace(speakerName));
        speakerTitleText.gameObject.SetActive(!string.IsNullOrWhiteSpace(speakerName));


        iconName = speakerName;  // maybe need to change ,right now i only want just show player who is it, not show the expression,because too much character and need a lot cut
        iconPath = iconName;

        if (iconOj.activeSelf) speakerIcon.sprite = GetCharacterStorySprite(speakerName, iconName);
        if (nameOj.activeInHierarchy) speakerNameText.text = LocalizationSettings.StringDatabase.GetLocalizedString("CharacterStoryName", speakerName);
        if (!string.IsNullOrWhiteSpace(speakerName)) StartCoroutine(LoadLocalizedTitle(speakerName));


    }

    private IEnumerator LoadLocalizedTitle(string speakerName)
    {
        var tableOp = LocalizationSettings.StringDatabase.GetTableAsync("CharacterStoryTitle");
        yield return tableOp;

        StringTable table = tableOp.Result;
        if (table == null)
        {
            Debug.LogWarning("???????? CharacterStoryName");
            speakerTitleText.gameObject.SetActive(false);
            yield break;
        }

        var entry = table.GetEntry(speakerName);
        if (entry != null)
        {
            string localized = entry.GetLocalizedString();
            speakerTitleText.text = localized;
            speakerTitleText.gameObject.SetActive(true);
        }
        else
        {
            speakerTitleText.gameObject.SetActive(false);
        }
    }


    public string GetSpeakerName()
    {
        return speakerNameText.text;
    }

    public string GetIconPath()
    {
        return iconOj.gameObject.activeSelf ? iconPath : null;
    }


}
