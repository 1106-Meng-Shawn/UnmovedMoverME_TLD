using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITotalControl : MonoBehaviour
{
    public Image RightColumn;

    private GameValue gameValue;

    private void Start()
    {
        if (gameValue == null) gameValue = GameObject.FindObjectOfType<GameValue>();

        InitUI();
    }

    public void UpTotalUI()
    {
        InitUI();
    }


    void InitUI()
    {
        string iconPath = $"MyDraw/UI/GameUI/";

        switch ((int)gameValue.GetCurrentSeason())
        {
            case 0: RightColumn.sprite = Resources.Load<Sprite>(iconPath + "SpringRightColumn"); break;
            case 1: RightColumn.sprite = Resources.Load<Sprite>(iconPath + "SummerRightColumn");break;
            case 2: RightColumn.sprite = Resources.Load<Sprite>(iconPath + "FallRightColumn"); break;
            case 3: RightColumn.sprite = Resources.Load<Sprite>(iconPath + "WinterRightColumn"); break;
        }
    }


}
