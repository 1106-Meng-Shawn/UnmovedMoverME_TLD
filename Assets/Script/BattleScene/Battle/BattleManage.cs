using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BattleManage : MonoBehaviour
{
    public List<BattleCharacterValue> playerBattleValue = new List<BattleCharacterValue>();
    public List<BattleCharacterValue> enemyBattleValue = new List<BattleCharacterValue>();
    private BattleCharacterValue[,] playerBattleValueArray = new BattleCharacterValue[3, 3];
    private BattleCharacterValue[,] enemyBattleValueArray = new BattleCharacterValue[3, 3];

    public static BattleManage Instance { get; private set; }
    BattleData battleData;

    public Button BackButton;
    public Canvas uiCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        battleData = GameValue.Instance.GetBattleData();
        if (battleData == null ) Debug.LogWarning($"battleData == null");
        //  Debug.LogWarning($"player has {battleData.playerBattleCharacters.Count} and enemy has {battleData.enemyBattleCharacters.Count}");
        InitBattle();
        FillBattleArrays();
        BackButton.onClick.AddListener(GoToGameScene);
    }

    void InitBattle()
    {
        SetBattleCharactersToPositions();
        BattleRightCol.Instance.StartBattleRecord();
        BattleOrder.Instance.SetBattleOrder(playerBattleValue, enemyBattleValue);
        ForceSliderControl.Instance.InitTopRow();
        FillBattleArrays();
    }


    void Update()
    {
        
    }

    public void FillBattleArrays()
    {
        playerBattleValueArray = new BattleCharacterValue[3, 3];
        enemyBattleValueArray = new BattleCharacterValue[3, 3];

        for (int i = 0; i < playerBattleValue.Count && i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;
            playerBattleValueArray[row, col] = playerBattleValue[i];
        }

        for (int i = 0; i < enemyBattleValue.Count && i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;
            enemyBattleValueArray[row, col] = enemyBattleValue[i];
        }
    }


    void SetBattleCharactersToPositions()
    {
        for (int i = 0; i < playerBattleValue.Count; i++) {
            playerBattleValue[i].SetCharacterToValue(battleData.playerBattleCharacters[i]);
            enemyBattleValue[i].SetCharacterToValue(battleData.enemyBattleCharacters[i]);
        }

    }

    public bool IsExploreBattle()
    {
        return battleData.isExplore;
    }

    public void GoToGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public BattleCharacterValue[,] GetPlayerArray()
    {
        return playerBattleValueArray;
    }

    public BattleCharacterValue[,] GetEnemyArray()
    {
        return enemyBattleValueArray;
    }

}

public class BattleData
{
    public List<Character> playerBattleCharacters;
    public List<Character> enemyBattleCharacters;
    public RegionValue battleRegion;
    public int cityIndex;
    public int exploreLevel;
    public bool isExplore = false;
    public int battleMaxTurn = 43;
    public BattleData(List<Character> playerChars, List<Character> enemyChars, RegionValue region, int cityIdx, int exploreLvl = -1, bool isExploring = false,int battleMaxTurn = 43)
    {
        playerBattleCharacters = playerChars ?? new List<Character>();
        enemyBattleCharacters = enemyChars ?? new List<Character>();
        battleRegion = region;
        cityIndex = cityIdx;
        exploreLevel = exploreLvl;
        isExplore = isExploring;
        this.battleMaxTurn = battleMaxTurn;
        Debug.Log($"playerBattleCharacters.Count {playerBattleCharacters.Count} AND enemyBattleCharacters.Count {enemyBattleCharacters.Count}");
    }
    public void SetWin()
    {
        if (isExplore)
        {

        } else
        {
            battleRegion.GetCityValue(cityIndex).cityCountry = GameValue.Instance.GetPlayerCountryENName();
            if (cityIndex == 0) {
               // battleRegion.Country = GameValue.Instance.PlayerCountry;
                battleRegion.MoveLord();
            }
        }
    }
}

