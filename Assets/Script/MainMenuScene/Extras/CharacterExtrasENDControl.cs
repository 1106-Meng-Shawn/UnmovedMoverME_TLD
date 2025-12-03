using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterExtrasENDControl : MonoBehaviour
{
    [SerializeField] private Image CharacterImage;
    [SerializeField] private IntroPanelShow IntroPanelShow;

    public void SetCharacterENDData(bool isGE,int ENDID)
    {
        if (isGE)
        {
            IntroPanelShow.SetPivot(new Vector2(1.1f, 0));
        }
        else
        {
            IntroPanelShow.SetPivot(new Vector2(-0.1f, 0));
        }
    }
}
