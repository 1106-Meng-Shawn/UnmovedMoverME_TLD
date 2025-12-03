using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;


public class CharacterOperControlOld : MonoBehaviour
{
  /*  public GameObject Order;

    public BattleOrder battleOrder;


    public BattleCharacterValue battleCharacterValue;
    public BattleCharacter battleCharacter;
    public BattleCharacter battleEnemy;


    public Button[] skillButtons;
    public TMP_Text[] skillTextButtons;
    public Button waitButton;
    public Image icon;

    public int turn = 0;
    public int MaxTurns = 10;

    private bool enemyTurn;

    public bool isPersonBattle;


    public BattleResultPanelControl battleResultPanel;
    public ForceSliderControl forceSliderControl;

    private void Start()
    {
        MaxTurns = battleOrder.TurnMax;
    }


    public void OnWaitButtonClicked()
    {
   //     battleCharacterValue.Speed -= 5;
        StartNextCharacterTurn();
    }

    public void StartNextCharacterTurn()
    {
        turn += 1;


        if (turn > MaxTurns || forceSliderControl.playerForce == 0 || forceSliderControl.enemyForce == 0) { BattleEnd(); }
        else
        {
            battleOrder.UpOrderdate();
            BattleOrderPrefab battleOrderPrefab = Order.transform.GetChild(0).GetComponent<BattleOrderPrefab>();
         //   battleCharacterValue = battleOrderPrefab.orderBattleCharacterValue;


            /*    for (int i = 0; i < Order.transform.childCount; i++)
                {
                    Transform child = Order.transform.GetChild(i);

                    if (child.gameObject.activeSelf)
                    {
                        BattleOrderPrefab battleOrderPrefab = child.GetComponent<BattleOrderPrefab>();
                        if (battleOrderPrefab != null && battleOrderPrefab.orderBattleCharacterValue.characterValue.Force != 0 && battleOrderPrefab.orderBattleCharacterValue.MoveNum > 0)
                        {
                            battleCharacterValue = battleOrderPrefab.orderBattleCharacterValue;
                            break;
                        }

                    }

                }*/


   /*         if (battleCharacterValue != null)
            {


                if (icon != null && battleCharacterValue.characterValue.icon != null)
                {
                    icon.sprite = battleCharacterValue.characterValue.icon;
                }

                enemyTurn = battleCharacterValue.isEnemy;


                if (battleCharacterValue.isEnemy)
                {
                    battleEnemy.SetCharacter(battleCharacterValue);


                    // enemy turn AI have do yet;
                    enemyTurnAI(battleCharacterValue.characterValue);
                    HideAllButtons();
                }
                else
                {
                    battleCharacter.SetCharacter(battleCharacterValue);
                    UpdateCharacterUI(battleCharacterValue.characterValue);
                }
            }

        }

    }



    // need to be more complex
    private void enemyTurnAI(Character enemy)
    {
        List<int> availableSkills = new List<int>();

        for (int i = 0; i < 5; i++)
        {
            if (enemy.CurrentSkillName(i) != "NONE")
            {
                availableSkills.Add(i);
            }
        }

        if (availableSkills.Count > 0)
        {
            // Select random skill from available ones
            int randomSkillIndex = availableSkills[UnityEngine.Random.Range(0, availableSkills.Count)];
            Debug.Log("Enemy uses skill: " + enemy.CurrentSkillName(randomSkillIndex));

            CreateArrowEffect(enemy.GetSkill(randomSkillIndex));



            StartCoroutine(HandleEnemySkillEffect(enemy, randomSkillIndex));
        }
        else
        {
            StartNextCharacterTurn();
        }
    }

    private IEnumerator HandleEnemySkillEffect(Character enemy, int randomSkill)
    {

        yield return new WaitForSeconds(2f);

     //   battleCharacterValue.Speed -= 5;

        if (enemy.GetSkill(randomSkill).type == 1 || enemy.GetSkill(randomSkill).type == 3)
        {
            enemy.GetSkill(randomSkill).SkillEffect(battleCharacterValue, new BattleCharacterValue[] { battleCharacter.battleCharacterValue }, isPersonBattle);
        }
        else if (enemy.GetSkill(randomSkill).type == 2 || enemy.GetSkill(randomSkill).type == 4)
        {
            enemy.GetSkill(randomSkill).SkillEffect(battleCharacterValue, new BattleCharacterValue[] { battleEnemy.battleCharacterValue }, isPersonBattle);
        }

        useSkill = null;
        Destroy(arrowInstance.gameObject);


        // battleOrder.DestroyForceZeroBattleOrderPrefabs();


        if (forceSliderControl.playerForce == 0 || forceSliderControl.enemyForce == 0) { BattleEnd(); } else
        {


            Destroy(battleFieldControl.arrowInstance.gameObject);

            StartNextCharacterTurn();
        }


    }
    //

    private void UpdateCharacterUI(Character character)
    {

            if (skillButtons != null && skillTextButtons != null && skillButtons.Length == skillTextButtons.Length)
            {
                for (int i = 0; i < skillButtons.Length; i++)
                {
                    if (i < 5)
                    {
                        UpdateSkillButton(skillButtons[i], skillTextButtons[i], character.GetSkill(i), i);
                    }
                }
            }
        
    }

    private void UpdateSkillButton(Button skillButton, TMP_Text skillTextButton, SkillBase skill, int index)
    {
        if (skill != null)
        {
            bool hasSkill = skill != null;

            skillButton.gameObject.SetActive(hasSkill);
            skillTextButton.gameObject.SetActive(hasSkill);

            if (hasSkill)
            {
                if (skillTextButton != null)
                {
                    string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
                    skillTextButton.text = skill.GetSkillName(currentLanguage);
                }

                skillButton.onClick.RemoveAllListeners();
                skillButton.onClick.AddListener(() => OnSkillButtonClicked(skill));
            }
        }
        else
        {
            skillButton.gameObject.SetActive(false);
            skillTextButton.gameObject.SetActive(false);
        }
    }

    public Arrow arrowPrefab;
    public Canvas uiCanvas;
    public Transform characterIcon;

    public Transform battleCharacterTransform;
    public Transform battleEnemyTransform;

    private Vector2 currentMouse;


    private Arrow arrowInstance;

    public BattleFieldControl battleFieldControl;


    private void OnSkillButtonClicked(SkillBase skill)
    {
        if (skill != null)
        {
            CreateArrowEffect(skill);
        }
    }

    public SkillBase useSkill;


    private void CreateArrowEffect(SkillBase skill)
    {
        if (arrowPrefab == null) return;

        arrowInstance = Instantiate(arrowPrefab, uiCanvas.transform); // ????????UI Canvas?
        useSkill = skill;
        // ?????????
        if (!enemyTurn)
        {
            Cursor.visible = false;
            currentMouse = Input.mousePosition;

            if (skill.type == 1 || skill.type == 3)
            {

                if (battleCharacterValue != battleCharacter.battleCharacterValue) { battleCharacter.SetCharacter(battleCharacterValue); };
                arrowInstance.SetStartPos(battleCharacter.gameObject.transform.position, skill.type,enemyTurn,false ,Vector3.zero);
                Mouse.current.WarpCursorPosition(battleEnemyTransform.position);

                battleFieldControl.SetTheArrowAtTheBattleField(battleCharacter.battleCharacterValue, skill, enemyTurn, battleEnemy.battleCharacterValue);



            }
            else if (skill.type == 2 || skill.type == 4)
            {
                arrowInstance.SetStartPos(characterIcon.position, skill.type, enemyTurn,false, Vector3.zero);
                Mouse.current.WarpCursorPosition(battleCharacterTransform.position);


                battleFieldControl.SetTheArrowAtTheBattleField(battleCharacterValue, skill, enemyTurn, battleCharacter.battleCharacterValue);

            }


        }
        else if (enemyTurn)
        {
            if (skill.type == 1 || skill.type == 3)
            {

              //  if (battleCharacterValue != battleCharacter.battleCharacterValue) { battleCharacter.SetCharacter(battleCharacterValue); }; // set character to battleCharacter, maybe add one can back to play select battleCharacter
                arrowInstance.SetStartPos(battleEnemyTransform.position, skill.type,enemyTurn, false,battleCharacterTransform.position);

                battleFieldControl.SetTheArrowAtTheBattleField(battleEnemy.battleCharacterValue, skill, enemyTurn, battleCharacter.battleCharacterValue);

            }
            else if (skill.type == 2 || skill.type == 4)
            {

                arrowInstance.SetStartPos(characterIcon.position, skill.type,enemyTurn,false, battleEnemyTransform.position);

                battleFieldControl.SetTheArrowAtTheBattleField(battleCharacterValue, skill, enemyTurn, battleEnemy.battleCharacterValue);

            }

        }





        arrowInstance.SetColor(true); 


    }



    private void RaycastArrowToTarget(Arrow arrowInstance)
    {
        Vector2 mousePosition = Input.mousePosition;

        Vector2 arrowEndPos = battleCharacter.transform.position;

        Canvas canvas = uiCanvas.GetComponent<Canvas>();
        GraphicRaycaster graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        EventSystem eventSystem = EventSystem.current;

        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        bool hitBattleCharacter = false;
        BattleCharacterValue targetBattleCharacter = null;


        foreach (RaycastResult result in results)
        {
            var battleCharacter = result.gameObject.GetComponent<BattleCharacter>();
//            var characterControl = result.gameObject.GetComponent<CharacterOperControl>();

            if (battleCharacter != null)
            {
                targetBattleCharacter = battleCharacter.battleCharacterValue;
                hitBattleCharacter = true;
                break;
            }
      /*      else if (characterControl != null)
            {
                targetBattleCharacter = characterControl.battleCharacterValue;
                hitBattleCharacter = true;
                break;
            }*/


    /*    }


        // Use skill
        if (hitBattleCharacter)
        {
            arrowInstance.SetColor(true);
            if (Input.GetMouseButtonDown(0))
            {

//use skill
                useSkill.SkillEffect(battleCharacterValue, new BattleCharacterValue[] { targetBattleCharacter }, isPersonBattle);
          //      battleOrder.DestroyForceZeroBattleOrderPrefabs();

                arrowInstance.Cance();
                useSkill = null;
                Cursor.visible = true;
                Mouse.current.WarpCursorPosition(currentMouse);

                Debug.Log( forceSliderControl.playerForce + " " + forceSliderControl.enemyForce );

                if (forceSliderControl.playerForce == 0 || forceSliderControl.enemyForce == 0) { 

                    Debug.Log("battle end");

                    BattleEnd();
                }else{
                    /* for (int i = 0; i < battleOrder.characterOrderList.Count; i++)
                     {
                         if (Order.transform.GetChild(i).gameObject.activeSelf)
                         {
                             Order.transform.GetChild(i).gameObject.SetActive(false);
                             break;

                         }
                     }*/

                 //   battleCharacterValue.Speed -= 5;

              /*      Destroy(battleFieldControl.arrowInstance.gameObject);

                    StartNextCharacterTurn();
                }
            }
        }
        else
        {
            arrowInstance.SetColor(false);
        }
    }




    private void Update()
    {
        if (arrowInstance != null && !enemyTurn)
        {
            RaycastArrowToTarget(arrowInstance);
        }
    }



    private void HideAllButtons()
    {
        if (skillButtons != null)
        {
            foreach (Button skillButton in skillButtons)
            {
                skillButton.gameObject.SetActive(false);
            }
        }

        if (skillTextButtons != null)
        {
            foreach (TMP_Text skillTextButton in skillTextButtons)
            {
                skillTextButton.gameObject.SetActive(false);
            }
        }

        if (waitButton != null)
        {
            waitButton.gameObject.SetActive(true);
        }
    }

    public void BattleEnd()
    {

        battleResultPanel.showBattleResultPanel();
       // SceneManager.LoadScene("GameScene"); // ???????????
    }*/
}
