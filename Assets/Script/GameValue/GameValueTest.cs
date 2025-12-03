using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameValueTest : MonoBehaviour
{

    public static GameValueTest instance;

    public int currentYear;
    public Season currentSeason;



    public float food;
    public float science;
    public float politics;
    public float gold;
    public float faith;

    public int totalRecruitedPopulation;
    public int Achievement;
    public float scout;
    public float build;
    public float negotiation;
    public string playerCountry;
    public string playerName;


    [Header("Character Battle Test")]

    public BattleDataTest battleDataTest;

    public List<string> playerCharacterIDs = new List<string>();
    public List<string> enemyCharacterIDs = new List<string>();
    public bool isExplore;
    public int battleMaxTurn = 43;

    public int regionID;

    [System.Serializable]
    public class BattleDataTest
    {
        public bool isTest = true;
        public int cityIndex;
        public int exploreLevel;
        public bool isExplore;
        public int battleMaxTurn = 43;

        public List<Character> playerBattleCharacters = new List<Character>();
        public List<Character> enemyBattleCharacters = new List<Character>();
        public RegionValue battleRegion;
        
        public BattleDataTest(GameValue gameValue,List<string> playerCharacterIDs, List<string> enemyCharacterIDs, bool isExplore, int regionID,int battleMaxTurn)
        {
            foreach (var id in playerCharacterIDs)
            {
                Character c = gameValue.GetCharacterByKey(id);
                playerBattleCharacters.Add(c);
            }

            foreach (var id in enemyCharacterIDs)
            {
                Character c = gameValue.GetCharacterByKey(id);
                enemyBattleCharacters.Add(c);
            }

            this.isExplore = isExplore;
            this.battleMaxTurn = battleMaxTurn;
            battleRegion = gameValue.GetRegionValue(regionID);
            Debug.Log($"BattleDataTest.playerBattleCharacters.Count {playerBattleCharacters.Count} AND BattleDataTest.enemyBattleCharacters.Count {enemyBattleCharacters.Count}");

        }

    }

    // Start is called before the first frame update
    public void SetTestValue(GameValue gameValue)
    {
        gameValue.GetResourceValue().Food = food;
        gameValue.GetResourceValue().Science = science;
        gameValue.GetResourceValue().Politics = politics;
        gameValue.GetResourceValue().Gold = gold;
        gameValue.GetResourceValue().Faith = faith;

        gameValue.GetResourceValue().TotalRecruitedPopulation = totalRecruitedPopulation;
        gameValue.GetResourceValue().Achievement = Achievement;
        gameValue.GetResourceValue().Scout = scout;
        gameValue.GetResourceValue().Build = build;
        gameValue.GetResourceValue().Negotiation = negotiation;

        gameValue.GetPlayerState().CurrentYear = currentYear;
        gameValue.GetPlayerState().CurrentSeason = currentSeason;

        battleDataTest = new BattleDataTest(gameValue,playerCharacterIDs,enemyCharacterIDs, isExplore, regionID, battleMaxTurn);
        if (battleDataTest.isTest)
        {
            BattleData battleData = new BattleData(battleDataTest.playerBattleCharacters, battleDataTest.enemyBattleCharacters, battleDataTest.battleRegion, battleDataTest.cityIndex, battleDataTest.exploreLevel, battleDataTest.isExplore, battleDataTest.battleMaxTurn);
            gameValue.SetBattleData(battleData);
        }

      /*  GameValue.Instance.GetPlayerState().CountryENName = playerCountry;
        GameValue.Instance.GetPlayerState().CountryENName = playerCountry;*/
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}