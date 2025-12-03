using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static BattlePanelManage;
using static GetSprite;
using static UnityEngine.GraphicsBuffer;
public class CharacterOperControl : MonoBehaviour
{
    public static CharacterOperControl Instance { get; private set; }

    private BattleCharacterValue battleCharacterValue;
    private List<SkillButtonControl> activeSkillButtons = new();
    private List<GameObject> movePointList = new();
    private List<PositionAtBattle> HighLightPositions = new();

    [Header("UI Elements")]
    public SkillButtonControl skillButtonPrefab;
    public Transform skillButtonContainer;
    public Button nothingButton;
    public Image operCharacter;
    public TextMeshProUGUI characterOperName;
    public Transform movePointsRow;
    public SkillPanelControl skillPanelControl;

    [Header("Arrow & UI Canvas")]
    public Arrow arrowPrefab;
    public Canvas uiCanvas;
    public Transform characterIcon;

    [Header("Battle Transforms")]
    public ArcButtonController arcButtonController;
    public GameObject movePointPrefab;

    private Arrow arrowInstance;
    private Vector2 currentMouse;
    private Skill useSkill;
    private bool isShow = true;

    private bool enemyTurn => battleCharacterValue != null && battleCharacterValue.isEnemy;
    private Vector3 prevPosition;

    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        nothingButton.onClick.AddListener(OnNothingButtonClick);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) ShowAllButtons();

        if (arrowInstance != null && !enemyTurn)
        {
            RaycastArrowToTarget(arrowInstance);
        }
    }

    private void OnDestroy()
    {
        if (battleCharacterValue != null)
        {
            battleCharacterValue.OnValueChanged -= GenerateMovePoints;
        }
    }
    #endregion

    #region Character Setup
    public void SetBattleCharacterValue(BattleCharacterValue battleCharacterValue)
    {
        BattleRightCol.Instance.SetTurnRecord(BattleOrder.Instance.currentTurn, battleCharacterValue.isEnemy);
        useSkill = null;
        UnsubscribePreviousValue();
        this.battleCharacterValue = battleCharacterValue;
        if (battleCharacterValue == null || battleCharacterValue.characterValue == null) return;

        SubscribeNewValue();

        UpdateCommonUI();

        if (battleCharacterValue.isEnemy)
        {
            SetupEnemyUI();
            StartEnemyTurn(battleCharacterValue);
        }
        else
        {
            SetupPlayerUI();
        }
        battleCharacterValue.positionAtBattle.SetBattleValueToInfo();
    }

    private void UnsubscribePreviousValue()
    {
        if (this.battleCharacterValue != null)
        {
            this.battleCharacterValue.OnValueChanged -= GenerateMovePoints;
        }
    }

    private void SubscribeNewValue()
    {
        this.battleCharacterValue.OnValueChanged += GenerateMovePoints;
        GenerateMovePoints();
        ClearSkillButtons();
    }

    private void UpdateCommonUI()
    {
        ClearHighLightPosition();
        Character character = battleCharacterValue.characterValue;
        operCharacter.sprite = character.icon;
        characterOperName.text = battleCharacterValue.GetBattleCharacterName();
        movePointsRow.gameObject.SetActive(!battleCharacterValue.IsPersonBattle());
    }

    private void SetupEnemyUI()
    {
        nothingButton.gameObject.SetActive(false);
        arcButtonController.SetButtons(null);
        arcButtonController.HideAllButtons();
        EffectManager.instance.enemyBattleCharacter.SetPositionAtBattle(battleCharacterValue.positionAtBattle);
    }

    private void SetupPlayerUI()
    {
        nothingButton.gameObject.SetActive(true);
        activeSkillButtons.Clear();
        EffectManager.instance.playerBattleCharacter.SetPositionAtBattle(battleCharacterValue.positionAtBattle);
        
        foreach (Skill skill in battleCharacterValue.characterValue.GetSkills())
        {
            if (skill == null || skill.triggerType != SkillTriggerType.Active) continue;

            SkillButtonControl newButton = Instantiate(skillButtonPrefab, skillButtonContainer);
            newButton.InitSkillButton(battleCharacterValue.IsPersonBattle(), skill, skillPanelControl);
            int index = activeSkillButtons.Count;
            newButton.skillButton.onClick.AddListener(() => OnSkillButtonClick(index));
            AddHoverEffect(newButton.skillButton,() => HighLightPosition(skill),()=> ClearHighLightPosition());
            activeSkillButtons.Add(newButton);
        }

        arcButtonController.SetButtons(activeSkillButtons);
        ShowAllButtons();
    }

    private void AddHoverEffect(Button button, Action onEnter, Action onExit)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entryEnter.callback.AddListener((data) => onEnter?.Invoke());
        trigger.triggers.Add(entryEnter);
        EventTrigger.Entry entryExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        entryExit.callback.AddListener((data) => onExit?.Invoke());
        trigger.triggers.Add(entryExit);
    }



    #endregion

    #region Enemy AI
    public void StartEnemyTurn(BattleCharacterValue enemy)
    {
        StartCoroutine(HandleEnemyTurnCoroutine(enemy));
    }

    private IEnumerator HandleEnemyTurnCoroutine(BattleCharacterValue enemy)
    {
        if (!enemy.isEnemy || enemy.characterValue == null)
        {
            EndEnemyTurn();
            yield break;
        }

        List<Skill> availableSkills = enemy.characterValue.GetSkills()
            .Where(skill => skill.triggerType == SkillTriggerType.Active)
            .ToList();

        // TODO: ??????
    }

    private void EndEnemyTurn()
    {
        OnNothingButtonClick();
    }
    #endregion

    #region Skill UI
    private void ClearSkillButtons()
    {
        foreach (SkillButtonControl btn in activeSkillButtons)
        {
            if (btn != null) Destroy(btn.gameObject);
        }
        activeSkillButtons.Clear();
    }

    private void OnSkillButtonClick(int index)
    {
        if (battleCharacterValue == null) return;
        if (EffectManager.instance.IsPlaying()) return;
        Skill skill = battleCharacterValue.characterValue.GetSkill(index);
        Debug.Log($"skill use {skill.GetSkillENName()}");
        if (skill == null) return;

        OnSkillButtonClicked(skill);
    }

    private void OnSkillButtonClicked(Skill skill)
    {
        HideAllButtons();
        ClearHighLightPosition();
        Debug.Log($"skill use {skill.GetSkillENName()}");
        if (skill.RequiresTargetSelection())
        {
            useSkill = skill;
            CreateArrowEffect(skill);
        }
        else
        {
            // skill.UseEffect(battleCharacterValue,battleCharacterValue,BattleManage.Instance.GetPlayerArray(),BattleManage.Instance.GetEnemyArray());
            skill.UseSkill(battleCharacterValue, battleCharacterValue, BattleManage.Instance.GetPlayerArray(), BattleManage.Instance.GetEnemyArray(), BattleOrder.Instance.RefreshOrderList);
            
        }
    }
    #endregion

    #region Highlight
    private void HighLightPosition(Skill skill)
    {

        ClearHighLightPosition();

        BattleOrder.Instance.SetNextMoveOrderPoint(battleCharacterValue, skill.EstimatedSpeed(battleCharacterValue));

        if (skill.IsForAlly())
        {
            HighLightAllyOrEnemy(skill, true);
        }
        else if (skill.IsForEnemy()) 
        {
            HighLightAllyOrEnemy(skill, false);
        }
        else if (skill.IsForSelf())
        {
            HighLightPositionSelfOnly(skill, battleCharacterValue);

        }
    }

    private void HighLightPositionSelfOnly(Skill skill, BattleCharacterValue useSkillCharacter)
    {
        useSkillCharacter.positionAtBattle.SetHighLight(true, skill);
        HighLightPositions.Clear();
        HighLightPositions.Add(useSkillCharacter.positionAtBattle);
    }


    void HighLightAllyOrEnemy(Skill skill, bool isAlly)
    {
        var targets = isAlly ? BattleManage.Instance.GetPlayerArray() : BattleManage.Instance.GetEnemyArray();
        HighLightPositionsByArray(skill, targets);
    }


    void HighLightPositionsByArray(Skill skill, BattleCharacterValue[,] targets)
    {
        int rows = targets.GetLength(0);
        int cols = targets.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var target = targets[i, j];
                if (target == null) continue;

                var battleArray = new BattleArray(
                    battleCharacterValue, // ?????
                    target,               // ????
                    BattleManage.Instance.GetPlayerArray(),
                    BattleManage.Instance.GetEnemyArray()
                );

                if (skill.IsInRange(battleArray))
                {
                    target.positionAtBattle.SetHighLight(true, skill);
                    HighLightPositions.Add(target.positionAtBattle);
                }
            }
        }
    }

    private void ClearHighLightPosition()
    {
        BattleOrder.Instance.CancelNextMoveOrderPoint();
        foreach (PositionAtBattle pos in HighLightPositions)
        {
            pos.StopHighlightFlash();
        }
        HighLightPositions.Clear();
    }
    #endregion

    #region Arrow

    void DestroyArrow()
    {
        if (arrowInstance != null)
        {
            arrowInstance.Cance();
            arrowInstance = null;
        }
    }


    private void CreateArrowEffect(Skill skill)
    {
        if (arrowPrefab == null || battleCharacterValue == null) return;

        if (arrowInstance != null) Destroy(arrowInstance.gameObject);
        arrowInstance = Instantiate(arrowPrefab, uiCanvas.transform);

        Vector3 skillEndPos = Vector3.zero;
        bool isEnemyTurn = battleCharacterValue.isEnemy;

        if (!isEnemyTurn)
        {
            Cursor.visible = false;
            currentMouse = Input.mousePosition;
            SetMouseEndPos(skill);
        }
        else
        {
            skillEndPos = skill.IsForAlly() ? EffectManager.instance.enemyBattleCharacter.transform.position : EffectManager.instance.playerBattleCharacter.transform.position;
        }

        arrowInstance.SetStartPos(characterIcon.position, skill, isEnemyTurn, false, skillEndPos);
        arrowInstance.SetColor(true);
    }

    private void SetMouseEndPos(Skill skill)
    {
        Vector3 pos = skill.IsForAlly() ? EffectManager.instance.playerBattleCharacter.transform.position : EffectManager.instance.enemyBattleCharacter.transform.position;
        Mouse.current.WarpCursorPosition(pos);
    }

    private void RaycastArrowToTarget(Arrow arrow)
    {
        BattleCharacterValue target = GetRaycastTarget();

        if (target == null)
        {
            arrow.SetColor(false);
            return;
        }

        bool isValid = IsValidTarget(target);
        arrow.SetColor(isValid);

        if (!isValid || !Mouse.current.leftButton.wasPressedThisFrame) return;

        if (IsCurrentBattleCharacter(target))
        {
            UseSkill(arrow, target);
        } 
    }

    private bool IsCurrentBattleCharacter(BattleCharacterValue target)
    {
        if (target.isEnemy)
        {
            return target == EffectManager.instance.enemyBattleCharacter.GetBattleCharacterValue();
        }
        else
        {
            return target == EffectManager.instance.playerBattleCharacter.GetBattleCharacterValue();
        }
    }


    private BattleCharacterValue GetRaycastTarget()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        GraphicRaycaster raycaster = uiCanvas.GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == EffectManager.instance.playerBattleCharacter.battleImage.gameObject)
                return EffectManager.instance.playerBattleCharacter.GetBattleCharacterValue();

            if (result.gameObject == EffectManager.instance.enemyBattleCharacter.battleImage.gameObject)
                return EffectManager.instance.enemyBattleCharacter.GetBattleCharacterValue();

            if (result.gameObject.GetComponent<PositionAtBattle>() != null)
                return result.gameObject.GetComponent<PositionAtBattle>().GetBattleCharacterValue();
        }

        return null;
    }


    private bool IsValidTarget(BattleCharacterValue target)
    {
        if (useSkill == null) return false;
        if (useSkill.IsForEnemy())
        {
            if (target.isEnemy != battleCharacterValue.isEnemy)
            {
                BattleArray battleArray = new BattleArray(battleCharacterValue, target, BattleManage.Instance.GetPlayerArray(), BattleManage.Instance.GetEnemyArray());
                return useSkill.IsInRange(battleArray);

            }

        }else if (useSkill.IsForAlly())
        {
            if (target.isEnemy == battleCharacterValue.isEnemy)
            {
                BattleArray battleArray = new BattleArray(battleCharacterValue, target, BattleManage.Instance.GetPlayerArray(), BattleManage.Instance.GetPlayerArray());
                return useSkill.IsInRange(battleArray);
            }
        }

        return false;
    }


    private void UseSkill(Arrow arrow, BattleCharacterValue targetBattleCharacter)
    {
        useSkill.UseSkill(battleCharacterValue, targetBattleCharacter,BattleManage.Instance.GetPlayerArray(), BattleManage.Instance.GetEnemyArray(), BattleOrder.Instance.RefreshOrderList);
        HideAllButtons();
    }


    #endregion

        #region Move Points
    private void GenerateMovePoints()
    {
        int totalMoveNum = battleCharacterValue.characterValue.BattleMoveNum;
        int remainNum = Mathf.Clamp(battleCharacterValue.MoveNum, 0, totalMoveNum);

        totalMoveNum = CharacterConstants.GetMaxMoveShowCount(totalMoveNum);

        foreach (GameObject point in movePointList) Destroy(point);
        movePointList.Clear();

        for (int i = 0; i < totalMoveNum; i++)
        {
            GameObject newPoint = Instantiate(movePointPrefab, movePointsRow);
            movePointList.Add(newPoint);

            Image img = newPoint.GetComponent<Image>();
            if (img != null)
                img.sprite = GetCharacterStateSprite(i < remainNum ? "move" : "cantmove");
        }
    }
    #endregion

    #region Button Control
    public void SwitchButtonActive()
    {
        if (isShow) HideAllButtons();
        else ShowAllButtons();
    }

    public void HideAllButtons()
    {
        arcButtonController.HideAllButtons();
        nothingButton.gameObject.SetActive(false);
        isShow = false;
        DestroyArrow();
    }

    private void ShowAllButtons()
    {
        if (battleCharacterValue.isEnemy) return;
        HideAllButtons();
        nothingButton.gameObject.SetActive(true);
        isShow = true;
        useSkill = null;
        DestroyArrow();
        arcButtonController.ShowAllButtons();

    }



    private void OnNothingButtonClick()
    {
        battleCharacterValue.MoveSpeed -= 5;
        HideAllButtons();
        BattleOrder.Instance.RefreshOrderList();
    }
    #endregion
}
