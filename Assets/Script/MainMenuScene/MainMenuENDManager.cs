using UnityEngine;
using UnityEngine.UI;


public class MainMenuENDManager : MonoBehaviour
{
    public Image BackgroundImage;
    public GameObject ButtonsPanel;
    void Start()
    {
        GameValue gameValue = GameValue.Instance;
        if (gameValue != null)
        {
            string ENDName = gameValue.GetPlayerState().ENDName;
            SetENDMainMenu(ENDName);
        }
    }

    void SetENDMainMenu(string endName)
    {
        switch (endName)
        {
            case "Alexei_END_VictoryOfExaltatus": SetAlexei_END_VictoryOfExaltatusMainMenu();break;
            default: SetDefaultMainMenu();break;
        }
        GameValue.Instance.GetPlayerState().ENDName = null;
    }

    void SetAlexei_END_VictoryOfExaltatusMainMenu()
    {
        string iconPath = $"MyDraw/CG/AlexeiWinEnd";

        BackgroundImage.sprite = Resources.Load<Sprite>(iconPath);
    }

    void SetDefaultMainMenu()
    {
        string iconPath = $"MyDraw/MainMenu/Map";

        BackgroundImage.sprite = Resources.Load<Sprite>(iconPath);

    }



}
