using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryLineControl : MonoBehaviour
{
    public TMP_Text ContentText;
    public TMP_Text NameText;
    public Image Icon;
  //  public RectTransform DialogBoxRectTransform;

    public void SetTheLine(string content, string name, Sprite icon)
    {
        ContentText.text = content;
        NameText.text = name;
        if (name == null) NameText.gameObject.SetActive(false);
        Icon.sprite = icon;
        if (Icon == null) Icon.gameObject.SetActive(false);

    }

    public void SetTheLine(HistoryData historyData)
    {
        ContentText.text = historyData.content;
        NameText.text = historyData.speaker;
        if (NameText.text == null) NameText.gameObject.SetActive(false);
        string IconPath =  Constants.AVATAR_PATH +  historyData.speaker + "/" + historyData.iconPath + "Icon";
        Icon.sprite = Resources.Load<Sprite>(IconPath);
      //  Debug.Log(IconPath);
        if (Icon.sprite == null) Icon.gameObject.SetActive(false);


/*        Debug.Log("NameText sizeDelta.y" + NameText.gameObject.GetComponent<RectTransform>().sizeDelta.y);
        Debug.Log("ContentText sizeDelta.y" + ContentText.gameObject.GetComponent<RectTransform>().sizeDelta.y);
        Debug.Log("sizeDelta.y" + DialogBoxRectTransform.sizeDelta.y);


        if (DialogBoxRectTransform.sizeDelta.y > Icon.gameObject.GetComponent<RectTransform>().sizeDelta.y)
        {
            gameObject.GetComponent<RectTransform>().sizeDelta =
            new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x,
                DialogBoxRectTransform.sizeDelta.y);

        }*/


    }

}

[System.Serializable]
public struct HistoryData
{
    public string speaker;
    public string content;
    public string iconPath;

    public HistoryData(string speaker, string content, string iconPath)
    {
        this.speaker = speaker;
        this.content = content;
        this.iconPath = iconPath;
    }
}