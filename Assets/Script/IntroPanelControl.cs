using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.U2D;
using UnityEngine.UI;

public class IntroPanelControl : MonoBehaviour
{
    public Image introImage;

    public TMP_Text nameText;

    public TMP_Text introText;
    private RectTransform rectTransform;
    private Sprite introSprite;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetValue(Sprite introSprite, string name, string intro, float x, float y)
    {
        this.introImage.sprite = introSprite;
        introImage.gameObject.SetActive(introSprite != null);

        nameText.text = name;
        introText.text = intro;

        rectTransform.pivot = new Vector2(x, y);

    }

    public void SetName(string name,float x,float y)
    {
        nameText.text = name ;
        rectTransform.pivot = new Vector2 (x,y);
    }


    public void SetImageAndName(Sprite sprite, string name,float x, float y)
    {
        if (introImage != null)
        {
            this.introImage.sprite = sprite;
            introSprite = sprite;
            introImage.gameObject.SetActive(introSprite != null);
        }
        nameText.text = name ;
        rectTransform.pivot = new Vector2(x, y);

    }



}
