using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class BattleManageOld : MonoBehaviour
{
/*
    // order
    [Header("order")]
    private List<BattleCharacterValue> totalBattleCharacterValueList = new List<BattleCharacterValue>();
    private List<BattleOrderPrefab> characterOrderList = new List<BattleOrderPrefab>();
    private List<BattleValueNode> tempValueNodes = new List<BattleValueNode>();
    private List<BattleValueNode> realCharacterOder = new List<BattleValueNode>();

    public ScrollRect scrollView;
    public GameObject orderPrefab;

    public bool isPersonBattle;
    public int TurnMax = 43;

    public GameObject Order;

    //public BattleOrder battleOrder;

    // battle Control
    [Header("Battle Control")]
    public BattleCharacterValue battleCharacterValue;
    public BattleCharacter battleCharacter;
    public BattleCharacter battleEnemy;

    public Button[] skillButtons;
    public TMP_Text[] skillTextButtons;
    public Button waitButton;
    public Image icon;
    public int turn = 0;
    private bool enemyTurn;


    // battle force slider
    [Header("Battle Force Slider")]

    public BattleResultPanelControl battleResultPanel;
    //public ForceSliderControl forceSliderControl;

    public TMP_Text playerForceText;
    public TMP_Text enemyForceText;
    public TMP_Text playerPercentText;
    public TMP_Text enemyPercentText;

    public Slider forceSlider;
    public List<BattleCharacterValue> playerValueList;
    public List<BattleCharacterValue> enemyValueList;

    public int playerForce;
    public int enemyForce;

    public int BattleCity;
    public RegionValue battleRegion;

    private void Start()
    {
        waitButton.onClick.AddListener(() => OnWaitButtonClicked());
    }

    void SetTotalList()
    {
        foreach (var battleEnemyCharacterValue in enemyValueList)
        {
            if(battleEnemyCharacterValue.characterValue != null) totalBattleCharacterValueList.Add(battleEnemyCharacterValue);
        }

        foreach (var battlePlayerCharacterValue in playerValueList)
        {
            if (battlePlayerCharacterValue.characterValue != null)  totalBattleCharacterValueList.Add(battlePlayerCharacterValue);
        }

    }

    public void BeginSetCharacterToOrder(bool isPersonBattle)
    {
        this.isPersonBattle = isPersonBattle;
        foreach (Transform child in scrollView.content)
        {
            Destroy(child.gameObject);
        }

        //    FillScrollViewUntil40();
        SetTempValueNodes();

        // FillScrollView();
    }

    void SetTempValueNodes()
    {
        if (totalBattleCharacterValueList.Count == 0) SetTotalList();
        tempValueNodes.Clear();
        foreach (var singeBattleCharacterValue in totalBattleCharacterValueList)
        {
            if (singeBattleCharacterValue.characterValue != null)
            {
                BattleValueNode battleValueNode = new BattleValueNode();
                battleValueNode.SetNode(singeBattleCharacterValue);
                tempValueNodes.Add(battleValueNode);
            }

        }
    }

    bool CanMove(BattleCharacterValue battleCharacterValue)
    {
        Character character = battleCharacterValue.characterValue;
        return (character.Force != 0 && character.Health != 0) && battleCharacterValue.MoveNum > 0;
    }

    void FillScrollView()
    {
        SetTempValueNodes();
        characterOrderList.Clear();
        foreach (Transform child in scrollView.content)
        {
            Destroy(child.gameObject);
        }
        realCharacterOder.Clear();


        for (int i = 0; i < TurnMax - turn+1; i++)
        {
            BattleValueNode highestSpeedCharacterValueNode = null;

            // find the highest speed BattleCharacterValue
            foreach (var characterValue in tempValueNodes)
            {
                if (characterValue.battleCharacterValue != null && CanMove(characterValue.battleCharacterValue))
                {
                    if (highestSpeedCharacterValueNode == null || characterValue.orderSpeed > highestSpeedCharacterValueNode.orderSpeed)
                    {
                        highestSpeedCharacterValueNode = characterValue;
                    }
                }
            }

            if (highestSpeedCharacterValueNode == null)
            {
                BattleEnd();
                return;
            }
            BattleCharacterValue highestSpeedCharacterValue = highestSpeedCharacterValueNode.battleCharacterValue;


            if (highestSpeedCharacterValue != null)
            {
                foreach (var battleValueNode in tempValueNodes)
                {
                    if (battleValueNode.battleCharacterValue.characterValue == highestSpeedCharacterValue.characterValue)
                    {
                        battleValueNode.orderSpeed -= 5;
                        break;
                    }
                }


                BattleValueNode highestSpeedCharacterNode = new BattleValueNode();
                highestSpeedCharacterNode.SetNode(highestSpeedCharacterValue);
                realCharacterOder.Add(highestSpeedCharacterNode);
                UpOrderPreFabDate(highestSpeedCharacterValue);

            }
        }

    }

    void UpOrderPreFabDate(BattleCharacterValue highestSpeedCharacterValue)
    {
        int CharacterCount = 0;

        foreach (var characterOrder in characterOrderList)
        {
            if (characterOrder.orderBattleCharacterValue.characterValue.GetCharacterENName() == highestSpeedCharacterValue.characterValue.GetCharacterENName())
            {
                CharacterCount++;
            }
        }

        if (CharacterCount < Mathf.Min(3, highestSpeedCharacterValue.MoveNum))
        {
            GameObject orderObject = Instantiate(orderPrefab, scrollView.content);
            BattleOrderPrefab battleOrderPrefab = orderObject.GetComponent<BattleOrderPrefab>();
            battleOrderPrefab.isPersonBattle = isPersonBattle;
            battleOrderPrefab.setCharacterToOrder(highestSpeedCharacterValue, highestSpeedCharacterValue.isEnemy, highestSpeedCharacterValue.Speed);
            characterOrderList.Add(battleOrderPrefab);
        }
    }


    public void OnWaitButtonClicked()
    {
        if (battleCharacterValue == null) return;
        battleCharacterValue.Speed -= 5;
        StartNextTurn();
    }

    BattleCharacterValue FindTheHighestSpeedBattleCharacterValue()
    {
        List<BattleCharacterValue> CanMoveBattleCharacterValueList = new List<BattleCharacterValue>();
        foreach (var CanMoveBattleCharacterValue in totalBattleCharacterValueList)
        {
            if (CanMove(CanMoveBattleCharacterValue)){ 
                CanMoveBattleCharacterValueList.Add(CanMoveBattleCharacterValue); 
            }

        }

        if (CanMoveBattleCharacterValueList.Count == 0) return null;

        BattleCharacterValue highestSpeedBattleCharacterValue = CanMoveBattleCharacterValueList[0];

        foreach (var battleCharacterValue in CanMoveBattleCharacterValueList)
        {
            if (highestSpeedBattleCharacterValue.Speed < battleCharacterValue.Speed) highestSpeedBattleCharacterValue = battleCharacterValue;
        }

        return highestSpeedBattleCharacterValue;
    }

    public void StartNextTurn()
    {
        turn += 1;

        UpdateSlider();
        battleCharacterValue = FindTheHighestSpeedBattleCharacterValue();


        if (turn > TurnMax || playerForce == 0 || enemyForce == 0) { BattleEnd(); }
        else
        {
            FillScrollView();
        //    Debug.Log($"{totalBattleCharacterValueList.Count} + {highestSpeedCharacterValue.characterValue.characterName}");


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

                }


            if (battleCharacterValue != null)
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
            StartNextTurn();
        }
    }

    private IEnumerator HandleEnemySkillEffect(Character enemy, int randomSkill)
    {

        yield return new WaitForSeconds(2f);

        battleCharacterValue.Speed -= 5;

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


            Destroy(battleFieldControl.arrowInstance.gameObject);

            StartNextTurn();
        


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
        waitButton.gameObject.SetActive(true);


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

                if (battleCharacterValue != battleCharacter.battleCharacterValue) { battleCharacter.SetCharacter(battleCharacterValue); }
                ;
                arrowInstance.SetStartPos(battleCharacter.gameObject.transform.position, skill.type, enemyTurn, false, Vector3.zero);
                Mouse.current.WarpCursorPosition(battleEnemyTransform.position);

                battleFieldControl.SetTheArrowAtTheBattleField(battleCharacter.battleCharacterValue, skill, enemyTurn, battleEnemy.battleCharacterValue);



            }
            else if (skill.type == 2 || skill.type == 4)
            {
                arrowInstance.SetStartPos(characterIcon.position, skill.type, enemyTurn, false, Vector3.zero);
                Mouse.current.WarpCursorPosition(battleCharacterTransform.position);


                battleFieldControl.SetTheArrowAtTheBattleField(battleCharacterValue, skill, enemyTurn, battleCharacter.battleCharacterValue);

            }


        }
        else if (enemyTurn)
        {
            if (skill.type == 1 || skill.type == 3)
            {

                //  if (battleCharacterValue != battleCharacter.battleCharacterValue) { battleCharacter.SetCharacter(battleCharacterValue); }; // set character to battleCharacter, maybe add one can back to play select battleCharacter
                arrowInstance.SetStartPos(battleEnemyTransform.position, skill.type, enemyTurn, false, battleCharacterTransform.position);

                battleFieldControl.SetTheArrowAtTheBattleField(battleEnemy.battleCharacterValue, skill, enemyTurn, battleCharacter.battleCharacterValue);

            }
            else if (skill.type == 2 || skill.type == 4)
            {

                arrowInstance.SetStartPos(characterIcon.position, skill.type, enemyTurn, false, battleEnemyTransform.position);

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
            var characterControl = result.gameObject.GetComponent<CharacterOperControl>();

            if (battleCharacter != null)
            {
                targetBattleCharacter = battleCharacter.battleCharacterValue;
                hitBattleCharacter = true;
                break;
            }
            else if (characterControl != null)
            {
                targetBattleCharacter = characterControl.battleCharacterValue;
                hitBattleCharacter = true;
                break;
            }


        }


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



                    battleCharacterValue.Speed -= 5;

                    Destroy(battleFieldControl.arrowInstance.gameObject);

                    StartNextTurn();
                
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
            waitButton.gameObject.SetActive(false);
        }
    }

    public void BattleEnd()
    {

        battleResultPanel.showBattleResultPanel();
        // SceneManager.LoadScene("GameScene"); // ???????????
    }


    private void UpdateSlider()
    {
        playerForce = isPersonBattle ? CalculateTotal(playerValueList, true) : CalculateTotal(playerValueList, false);
        enemyForce = isPersonBattle ? CalculateTotal(enemyValueList, true) : CalculateTotal(enemyValueList, false);

        playerForceText.text = playerForce.ToString();
        enemyForceText.text = enemyForce.ToString();

        float totalForce = playerForce + enemyForce;
        if (totalForce > 0)
        {
            float adjustedSliderValue = 0;
            if (BattleCity != 0)
            {
                adjustedSliderValue = (float)playerForce / totalForce - BattleParameter(BattleCity, battleRegion);

            }
            else
            {
                adjustedSliderValue = (float)playerForce / totalForce; // need change by level maybe

            }
            forceSlider.value = Mathf.Clamp(adjustedSliderValue, 0.01f, 0.99f);
        }

        playerPercentText.text = ToPercentString(forceSlider.value);
        enemyPercentText.text = ToPercentString(1 - forceSlider.value);
    }

    public void SetForceSlider(int battleCity, RegionValue battleRegion)
    {
        playerForce = CalculateTotal(playerValueList, false);
        enemyForce = CalculateTotal(enemyValueList, false);
        this.battleRegion = battleRegion;
        UpdateSlider();
    }

    public void SetHealthSlider(CityValue level, RegionValue battleRegion)
    {
        playerForce = CalculateTotal(playerValueList, true);
        enemyForce = CalculateTotal(enemyValueList, true);
        this.battleRegion = battleRegion;

        UpdateSlider();
    }

    private int CalculateTotal(List<BattleCharacterValue> characters, bool isHealth)
    {
        int total = 0;
        foreach (var character in characters)
        {
            if (character != null && character.characterValue != null)
            {
                total += isHealth ? character.characterValue.Health : character.characterValue.Force;
            }
        }
        return total;
    }

    private float BattleParameter(int battleCity, RegionValue battleRegion)
    {
        // Simplified to remove unnecessary switch statements
        float[,] parameters = new float[,] {
            {0.25f, 0.2f, 0.1f},  // cityNum == 3
            {0.3f, 0.15f, 0},     // cityNum == 2
            {0.35f, 0, 0}         // cityNum == 1
        };

        int cityIndex = battleRegion.GetCityCountryNum() - 1;
        if (cityIndex >= 0 && cityIndex < parameters.GetLength(0) && BattleCity >= 0 && BattleCity < parameters.GetLength(1))
        {
            return parameters[cityIndex, BattleCity];
        }


        return 0f;
    }

    private string ToPercentString(float value)
    {
        return (value * 100).ToString("F1") + "%";
    }
}


public class BattleValueNode
{
    public BattleCharacterValue battleCharacterValue;
    public float orderSpeed;


    public void SetNode(BattleCharacterValue battleCharacterValue)
    {
        this.battleCharacterValue = battleCharacterValue;
        this.orderSpeed = battleCharacterValue.Speed;
    }

    */
}