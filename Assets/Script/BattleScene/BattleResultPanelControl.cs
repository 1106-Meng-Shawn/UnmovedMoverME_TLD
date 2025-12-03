using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BattleResultPanelControl : MonoBehaviour
{
    public static BattleResultPanelControl Instance { get; private set; }

    public GameObject BattleResultPanel;


    public TMP_Text resultTitle;
    public Button check;


    bool canCapture = false;
    public List<PositionAtBattle> playerPositions;
    public List<PositionAtBattle> enemyPositions;


    public List<ResultBattlePosistionControl> resultBattleCharacterPosistion;
    public List<ResultBattlePosistionControl> resultBattleEnemyPosistion;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        check.onClick.AddListener(() => GoToOtherScene());
        BattleResultPanel.gameObject.SetActive(false);
    }

    public void ShowBattleResultPanel(bool isSurrendered = false)
    {
        CharacterOperControl.Instance.HideAllButtons();
        BattleRightCol.Instance.CloseCol(); 
        SetResultTitle(isSurrendered);
        BattleResultPanel.SetActive(true);
        for (int i = 0; i < 9; i++)
        {
            resultBattleCharacterPosistion[i].SetResultBattlePosistion(playerPositions[i], canCapture);
            resultBattleEnemyPosistion[i].SetResultBattlePosistion(enemyPositions[i], canCapture);
        }
    }

    void SetResultTitle(bool isSurrendered = false)
    {
        if (isSurrendered) {
            resultTitle.text = ResultConstants.LOSE;
            canCapture = false;
            return;
        }

        float sliderValue = ForceSliderControl.Instance.forceSlider.value;
        if (sliderValue <= 0.3)
        {
            resultTitle.text = ResultConstants.LOSE ;
            canCapture = false;

        } else if (sliderValue > 0.3 && sliderValue < 0.7)
        {
            resultTitle.text = ResultConstants.DRAW;
            canCapture = true;

        }
        else if (sliderValue >= 0.7)
        {
            resultTitle.text = ResultConstants.WIN;
            canCapture = true;
            GameValue.Instance.GetBattleData().SetWin();
        }
    }


    void GoToOtherScene()
    {
        SceneTransferManager.Instance.LoadScene(Scene.GameScene);   
    }


}


public class ResultConstants
{

    public static string WIN = "Win";
    public static string LOSE = "Lose";
    public static string DRAW = "Draw";

}