using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.SmartFormat.Utilities;

public class BattleOrder : MonoBehaviour
{
    public BattleCharacterInfo playerBattleInfo;
    public BattleCharacterInfo enemyBattleInfo;

    public List<BattleCharacterValue> currentBattleValues = new List<BattleCharacterValue>();
    public List<BattleOrderPrefab> characterOrderList = new List<BattleOrderPrefab>();

    public ScrollRect scrollView;

    public GameObject orderPrefab;

    public bool isPersonBattle;

    public int currentTurn = 1;

    public TextMeshProUGUI turnText;

    public GameObject nextMoveOrderPoint;

    public static BattleOrder Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }



    public void SetBattleOrder(List<BattleCharacterValue> players, List<BattleCharacterValue> enemys)
    {
        CancelNextMoveOrderPoint();
        // ??????????
        currentBattleValues.Clear();

        // ??????
        AddValidCharacters(players);
        AddValidCharacters(enemys);

        // ???????????
        currentBattleValues = currentBattleValues
            .OrderByDescending(c => c.MoveSpeed)
            .ToList();

        // ???? UI ??
        ClearCharacterOrderUI();

        // ?????? UI
        foreach (var character in currentBattleValues)
        {
            CreateOrderUI(character);
        }

        SetTurnText(1);

        // ?????????????
        if (currentBattleValues.Count > 0)
            CharacterOperControl.Instance.SetBattleCharacterValue(currentBattleValues[0]);

        // ????????????????????
        var firstPlayer = currentBattleValues.FirstOrDefault(c => !c.isEnemy);
        if (firstPlayer != null) firstPlayer.positionAtBattle.SetBattleValueToInfo();

        var firstEnemy = currentBattleValues.FirstOrDefault(c => c.isEnemy);
        if (firstEnemy != null) firstEnemy.positionAtBattle.SetBattleValueToInfo();

    }

    // ??????
    private void AddValidCharacters(List<BattleCharacterValue> characters)
    {
        foreach (var character in characters)
        {
            if (character.characterValue != null)
                currentBattleValues.Add(character);
        }
    }

    // ?????? UI
    private void ClearCharacterOrderUI()
    {
        foreach (var prefab in characterOrderList)
        {
            Destroy(prefab.gameObject);
        }
        characterOrderList.Clear();
    }

    // ????????? UI
    private void CreateOrderUI(BattleCharacterValue character)
    {
        GameObject go = Instantiate(orderPrefab, scrollView.content);
        var prefabScript = go.GetComponent<BattleOrderPrefab>();
        prefabScript.SetCharacterToOrder(character);
        characterOrderList.Add(prefabScript);
    }

    public void RefreshOrderList()
    {
        // ??????
        if (!AdvanceTurn()) return;
        CleanUpCharacterOrderList();
        if (characterOrderList.Count == 0 || ForceSliderControl.Instance.forceSlider.value == 0f || ForceSliderControl.Instance.forceSlider.value == 1f)
        {
            EndBattle();

            return;
        }
        SortAndArrangeCharacters();
        CharacterOperControl.Instance.SetBattleCharacterValue(characterOrderList[0].GetBattleCharacterValue());
    }

    private bool AdvanceTurn()
    {
        currentTurn++;
        if (currentTurn >= GameValue.Instance.GetBattleData().battleMaxTurn)
        {
            EndBattle();
            return false;
        }

        SetTurnText(currentTurn);
        return true;
    }

    private void EndBattle()
    {
        BattleResultPanelControl.Instance.ShowBattleResultPanel();
    }

    private void CleanUpCharacterOrderList()
    {
        bool isExplore = GameValue.Instance.GetBattleData().isExplore;
        List<BattleOrderPrefab> newList = new();

        foreach (var character in characterOrderList)
        {
            var battleValue = character.GetBattleCharacterValue();
            var characterValue = battleValue?.characterValue;

            bool isDead = characterValue == null;
            bool isOutOfMoves = battleValue?.MoveNum <= 0;
            bool isDeadExplore = isExplore && characterValue?.Health <= 0;
            bool isDeadBattle = !isExplore && characterValue?.Force <= 0;

            if (isDead || isOutOfMoves || isDeadExplore || isDeadBattle)
            {
                Destroy(character.gameObject);
                continue;
            }

            newList.Add(character);
        }

        characterOrderList = newList;
    }

    private void SortAndArrangeCharacters()
    {
        characterOrderList = characterOrderList
            .OrderByDescending(p => p.GetBattleCharacterValue().MoveSpeed)
            .ToList();

        for (int i = 0; i < characterOrderList.Count; i++)
        {
            characterOrderList[i].transform.SetSiblingIndex(i);
        }
    }

    private void SetTurnText(int turn)
    {
        turnText.text = $"{turn} / {GameValue.Instance.GetBattleData().battleMaxTurn}";
    }


    public void SetNextMoveOrderPoint(BattleCharacterValue battleCharacter, int moveSpeed)
    {
        if (nextMoveOrderPoint == null || scrollView == null) return;

        nextMoveOrderPoint.SetActive(true);
        Transform content = scrollView.content;

        if (characterOrderList == null || characterOrderList.Count == 0)
        {
            nextMoveOrderPoint.transform.SetParent(content, false);
            nextMoveOrderPoint.transform.SetAsFirstSibling();
            return;
        }

        nextMoveOrderPoint.transform.SetParent(content, false);

        // ? ????????????????????????
        var nextRoundList = new List<(BattleCharacterValue character, int speed)>();

        foreach (var orderPrefab in characterOrderList)
        {
            if (orderPrefab == null) continue;
            var otherCharacter = orderPrefab.GetBattleCharacterValue();
            if (otherCharacter == null) continue;

            // ? ??? battleCharacter???? moveSpeed????????
            int speed = (otherCharacter == battleCharacter) ? moveSpeed : otherCharacter.MoveSpeed;

            nextRoundList.Add((otherCharacter, speed));
        }

        // ? ?????????
        nextRoundList = nextRoundList
            .OrderByDescending(p => p.speed)
            .ToList();

        // ? ?? battleCharacter ???????
        int positionIndex = nextRoundList.FindIndex(p => p.character == battleCharacter);

        nextMoveOrderPoint.transform.SetSiblingIndex(positionIndex+1); // There may be bugs because the order is not correct.

    }

    public void CancelNextMoveOrderPoint()
    {
        nextMoveOrderPoint.SetActive(false);
    }

}

