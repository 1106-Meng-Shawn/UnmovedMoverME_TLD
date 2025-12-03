using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ExploreControlOld : MonoBehaviour
{
    public List<BattlePosition> exploreBattlePosition = new List<BattlePosition>();

    public List<BattleCharacterValue> battleCharacterValues = new List<BattleCharacterValue>();

    public List<BattleCharacterValue> monsterValue = new List<BattleCharacterValue>();


    public RegionValue ExploreRegion;



    public List<ItemBase> ExploreItems = new List<ItemBase>();
    public List<GameObject> ExploreMonster = new List<GameObject>();


   // public ExploreCardTree exploreCardTree;

    public ExpoloreSelectCard expoloreSelectCard;

    public TMP_Text LevelText;

    public BattlePanelCharacterInfoOld battlePanelCharacterInfo;

    public Transform content;

    /*


    void Start()
    {
        // Initial Settings
      //  battlePanelCharacterInfo.DisplayCharacter();
        ExploreRegion = SceneTransferManager.Instance.BattleRegionValue;
    //    ExploreItems = ExploreRegion.ExploreItems;
      //  ExploreMonster = ExploreRegion.ExploreMonster;
   //     exploreCardTree.level = ExploreRegion.ExploreLevel; Get explore level form explore city 

        // game to explore set
        if (SceneTransferManager.Instance.type == 3)// game to explore
        {
            for (int i = 0; i < 9; i++)
            {
                exploreBattlePosition[i].characterAtBattlePosition = SceneTransferManager.Instance.charactersAtBattlePostions[i];
                if (exploreBattlePosition[i].characterAtBattlePosition != null)
                {
                   // exploreBattlePosition[i].characterAtBattlePosition.battlePosition = exploreBattlePosition[i];
                    foreach (Transform child in content)
                    {
                        if (child.GetComponent<BattleColumnControl>().characterName == exploreBattlePosition[i].characterAtBattlePosition.GetCharacterENName())
                        {
                            BattleCharacterValue battleCharacterValue = child.gameObject.AddComponent<BattleCharacterValue>();

                          //  battleCharacterValue.setCharacterToValue(exploreBattlePosition[i].characterAtBattlePosition);
                            battleCharacterValues.Add(battleCharacterValue);
                            exploreBattlePosition[i].battleCharacterValue = battleCharacterValue;

                        }
                    }
                }

            }


            exploreCardTree.GenerateExploreCardTree();

        }
        else if (SceneTransferManager.Instance.type == 5)//  explore battle to explore
        {

            for (int i = 0; i < 9; i++)
            {
                if (SceneTransferManager.Instance.exploreBattleCharacter[i].isAtPosition)
                {
                    exploreBattlePosition[i].characterAtBattlePosition = SceneTransferManager.Instance.exploreBattleCharacter[i].characterValue;
                }

                if (exploreBattlePosition[i].characterAtBattlePosition != null)
                {
                //    exploreBattlePosition[i].characterAtBattlePosition.battlePosition = exploreBattlePosition[i];
                    foreach (Transform child in content)
                    {
                        if (child.GetComponent<BattleColumnControl>().characterName == exploreBattlePosition[i].characterAtBattlePosition.GetCharacterENName())
                        {
                            BattleCharacterValue battleCharacterValue = child.gameObject.AddComponent<BattleCharacterValue>();

                      //      battleCharacterValue.setCharacterToValue(exploreBattlePosition[i].characterAtBattlePosition);
                            battleCharacterValues.Add(battleCharacterValue);
                            exploreBattlePosition[i].battleCharacterValue = battleCharacterValue;

                        }
                    }
                }

            }

            exploreCardTree.level = SceneTransferManager.Instance.ExploreLevel;
          //  exploreCardTree.setTheLevelText();

            exploreCardTree.firstLevel[0].gameObject.SetActive(false);
            exploreCardTree.firstLevel[4].gameObject.SetActive(false);

            // ??????
            for (int i = 1; i < 4; i++)
            {
             //   exploreCardTree.firstLevel[i].setTheCard(SceneTransferManager.Instance.ExploreFirstLevelCardType[i], SceneTransferManager.Instance.ExploreFirstLevelIsFront[i], exploreCardTree.level+1);
            }

            var levels = new[] {
    new { Cards = exploreCardTree.secondLevel, CardTypes = SceneTransferManager.Instance.ExploreSecondLevelCardType, IsFronts = SceneTransferManager.Instance.ExploreSecondLevelIsFront },
    new { Cards = exploreCardTree.thirdLevel, CardTypes = SceneTransferManager.Instance.ExploreThirdLevelCardType, IsFronts = SceneTransferManager.Instance.ExploreThirdLevelIsFront },
    new { Cards = exploreCardTree.fourthLevel, CardTypes = SceneTransferManager.Instance.ExploreFourthLevelCardType, IsFronts = SceneTransferManager.Instance.ExploreFourthLevelIsFront },
    new { Cards = exploreCardTree.fifthLevel, CardTypes = SceneTransferManager.Instance.ExploreFifthLevelCardType, IsFronts = SceneTransferManager.Instance.ExploreFifthLevelIsFront }
};

            for (int levelIndex = 0; levelIndex < levels.Length; levelIndex++)
            {
                var currentLevel = levels[levelIndex].Cards;
                var currentCardTypes = levels[levelIndex].CardTypes;
                var currentIsFronts = levels[levelIndex].IsFronts;
                int levelLength = levels[levelIndex].Cards.Count;

                for (int i = 0; i < levelLength; i++)
                {
            //        currentLevel[i].setTheCard(currentCardTypes[i], currentIsFronts[i], exploreCardTree.level + 2 + levelIndex);


                }


            }

        }
    }

    // Update is called once per frame
    void Update()
    {
    }
     */


}
