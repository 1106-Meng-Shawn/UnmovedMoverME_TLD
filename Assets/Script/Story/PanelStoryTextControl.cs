using UnityEngine;
using TMPro;
public class PanelStoryTextControl : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI ContentText;



    public void SetTheTextToPanelStoryText(string name, string Content)
    {
        nameText.text = name;
        ContentText.text = Content;

        bool isCenter = ContentText.text.Contains("<align=center>");
        nameText.gameObject.SetActive(!(string.IsNullOrEmpty(name) && isCenter));
    }


    public void SetTheTextToPanelStoryText(PanelSotryData panelSotryData)
    {
        nameText.text = panelSotryData.name;

        ContentText.text = panelSotryData.content;
    }


}

public class PanelSotryData
{
    public string name;
    public string content;
    public PanelSotryData(string name, string content)
    {
        this.name = name;
        this.content = content;
    }

}