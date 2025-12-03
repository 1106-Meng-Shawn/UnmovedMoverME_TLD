using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.UI;

[System.Serializable]
public class BattlePanelValue : MonoBehaviour
{
    public RegionValue BattleRegionValue;
    public CityValue battleCity;
    public string battleCountryName;
    public string battleRegionName;

    public List<Character> enemyCharacters = new List<Character>();
    public EnemyArraySet enemyArraySet;

    private bool isExplore;
    public bool isInExplore;
    public GameObject enemyArrayRowInfo;
    public Button BattleButton;

    public void SetEnemyToPosition()
    {
        FindEnemyCharacters();
        enemyArraySet.SetEnemyToPosition();

    }

    void FindEnemyCharacters()
    {

        // List<Character> allCharacters = new List<Character>(FindObjectsOfType<Character>());
        GameValue gameValue = FindObjectOfType<GameValue>();
     //   enemyCharacters = gameValue.GetCurrentCharactersInGame(BattleRegionValue.GetCountry());
    }

    public void SetIsExplore(bool isExplore)
    {
        this.isExplore = isExplore;
        enemyArrayRowInfo.SetActive(!isExplore);
        if (isExplore) {
            BattleButton.GetComponentInChildren<LocalizeStringEvent>().StringReference = new LocalizedString { TableReference = "GameSetting", TableEntryReference = "Explore" };
        } else
        {
            BattleButton.GetComponentInChildren<LocalizeStringEvent>().StringReference = new LocalizedString { TableReference = "GameSetting", TableEntryReference = "Battle" };
        }
    }

    public bool GetIsExplore()
    {
        return isExplore;
    }



}




