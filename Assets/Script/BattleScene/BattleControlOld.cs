using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleControlOld : MonoBehaviour
{
    /*
    public List<PositionAtBattle> playerPositionAtBattles = new List<PositionAtBattle>();
    public List<PositionAtBattle> enemyPositionAtBattles = new List<PositionAtBattle>();

    public List<BattleCharacterValue> battlePlayerValue = new List<BattleCharacterValue>();
    public List<BattleCharacterValue> battleEnemyValue = new List<BattleCharacterValue>();


    public GameObject batterPlayer;
    public BattleCharacter batterEnemy;


    public BattleManageOld battleManage;


    private List<Character> playerForce = new List<Character>();
    private List<Character> enemyForce = new List<Character>();

    private CityValue BattleCity;
    private RegionValue BattleRegion;

    
    public ControlBattleCharacter controlBattleCharacter;
    public ControlBattleCharacter controlEnemyCharacter;

    public bool isPersonBattle;


    private void Start()
    {
        BattleCharacter battlePlayerCharacter = batterPlayer.GetComponent<BattleCharacter>();
        BattleCharacter battleEnemyCharacter = batterEnemy.GetComponent<BattleCharacter>();

        BattleRegion = GameValue.Instance.GetBattleData().battleRegion;
        BattleCity = GameValue.Instance.GetBattleData().battleRegion.GetCityValue(GameValue.Instance.GetBattleData().cityIndex);



        if (SceneTransferManager.Instance.type == 1)
        {
            isPersonBattle = false;

            battleManage.isPersonBattle = false;

        //    SetCharactersAtPositions(true, playerPositionAtBattles, playerForce, battlePlayerCharacter, battlePlayerValue);
          //  SetCharactersAtPositions(false, enemyPositionAtBattles, enemyForce, battleEnemyCharacter, battleEnemyValue);


        } else if (SceneTransferManager.Instance.type == 4)
        {
            isPersonBattle = true;

            battleManage.isPersonBattle = true;

           // SetCharactersValueToPosition(true, playerPositionAtBattles, battlePlayerCharacter, battlePlayerValue);
          //  SetCharactersValueToPosition(false, enemyPositionAtBattles, battleEnemyCharacter, battleEnemyValue);

            battleManage.SetHealthSlider(BattleCity, BattleRegion);// maybe need to add by level

        }

        battleManage.BeginSetCharacterToOrder(isPersonBattle);
        controlBattleCharacter.setContolBattleCharacter();
        controlEnemyCharacter.setContolBattleCharacter();

    }


    private float startDelay = 0.5f;  
    private float timer = 0.0f;
    private bool isStart = false;

    private void Update()
    {
        if (!isStart) {
            timer += Time.deltaTime;
            if (timer >= startDelay)
            {
                BattleStart();
                isStart = true;
            }
        } timer += Time.deltaTime;
    }


    public void BattleStart()
    {
        battleManage.StartNextTurn();
        //BattleStartPanel.SetActive(false);
    }

    private void SetCharactersAtPositions(bool isPlayer,List<PositionAtBattle> positions, List<Character> forceList, BattleCharacter battleCharacter, List<BattleCharacterValue> battleCharacterValue)
    {
        battleCharacter.isPersonBattle = false;

        for (int i = 0; i < 9; i++)
        {
            positions[i].isPersonBattle = false;

            if (isPlayer) { positions[i].characterAtPlayerPosition = SceneTransferManager.Instance.charactersAtBattlePostions[i]; };
            if (!isPlayer) { positions[i].characterAtPlayerPosition = SceneTransferManager.Instance.enemysAtBattlePostions[i]; };
         //   positions[i].SetCharacterToPlayerPosition(positions[i].characterAtPlayerPosition);

            if (positions[i].characterAtPlayerPosition != null)
            {
                forceList.Add(positions[i].characterAtPlayerPosition);

            }
            if (battleCharacterValue[i] != null && positions[i].characterAtPlayerPosition != null )
            {
                battleCharacter.SetCharacter(battleCharacterValue[i]);
                battleCharacterValue[i].setCharacterToValue(positions[i].characterAtPlayerPosition);
            }

        }
    }


    private void SetCharactersValueToPosition(bool isPlayer, List<PositionAtBattle> positions, BattleCharacter battleCharacter, List<BattleCharacterValue> battleCharacterValue)
    {
        battleCharacter.isPersonBattle = true;
        for (int i = 0; i < 9; i++)
        {
            positions[i].isPersonBattle = true;
            if (isPlayer && SceneTransferManager.Instance.exploreBattleCharacter[i].isAtPosition) { positions[i].gameObject.GetComponent<BattleCharacterValue>().copyBattleCharacterValue(SceneTransferManager.Instance.exploreBattleCharacter[i]); };
            if (!isPlayer) { positions[i].gameObject.GetComponent<BattleCharacterValue>().copyBattleCharacterValue(SceneTransferManager.Instance.monsters[i]); };
            if (positions[i].gameObject.GetComponent<BattleCharacterValue>().characterValue != null)
            {
              //  positions[i].SetCharacterToPlayerPosition(positions[i].gameObject.GetComponent<BattleCharacterValue>().characterValue);
            }

            if (battleCharacterValue[i] != null && positions[i].characterAtPlayerPosition != null && ((isPlayer && battleCharacterValue[i].isAtPosition) || (!isPlayer)))
            {
                battleCharacter.SetCharacter(battleCharacterValue[i]);
                battleCharacterValue[i].setCharacterToValue(positions[i].characterAtPlayerPosition);
            } else
            {
              //  positions[i].gameObject.SetActive(false);

            }


        }
    }
    */
}
