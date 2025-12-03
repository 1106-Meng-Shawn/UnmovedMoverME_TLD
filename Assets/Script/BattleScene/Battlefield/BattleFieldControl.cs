using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static GetColor;
using TMPro;


public class BattleFieldControl : MonoBehaviour
{
    public Image enemyBackground;
    public GameObject enemyCorners;
    public List<PositionAtBattle> enemyPositions;


    public Image playerBackground;
    public GameObject playerCorners;
    public List<PositionAtBattle> playerPositions;
    public Button tabButton;

    private Dictionary<KeyCode, int> keyToIndex = new Dictionary<KeyCode, int>
    {
        { KeyCode.Q, 0 }, { KeyCode.W, 1 }, { KeyCode.E, 2 },
        { KeyCode.A, 3 }, { KeyCode.S, 4 }, { KeyCode.D, 5 },
        { KeyCode.Z, 6 }, { KeyCode.X, 7 }, { KeyCode.C, 8 },
    }; 


    bool isPlayerField = false;

    private void Start()
    {
        tabButton.onClick.AddListener(() => SwitchBattleField(false)); 
        SetBattleField();
    }

    private void Update()
    {
        // 检测 Tab 键按下（只触发一次）
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchBattleField(true); // 复原颜色
        }

        foreach (var entry in keyToIndex)
        {
            if (Input.GetKeyDown(entry.Key))
            {
                SetPostionToInfo(entry.Value);
            }
        }

    }

    void SetPostionToInfo(int index)
    {
        var targets = isPlayerField ? playerPositions : enemyPositions;

        if (index >= 0 && index < targets.Count && targets[index] != null)
            targets[index].SetBattleValueToInfo();

    }




    void SwitchBattleField(bool restoreColor = true)
    {
        isPlayerField = !isPlayerField;
        SetBattleField();
        PlayButtonPressFeedback(restoreColor);
    }


    public void PlayButtonPressFeedback(bool restoreColor)
    {
        if (tabButton != null && tabButton.gameObject.activeSelf)
        {
            DOTween.Kill(tabButton.gameObject, true);

            Transform btnTransform = tabButton.transform;
            Image image = tabButton.GetComponent<Image>();
            ButtonEffect effect = tabButton.GetComponent<ButtonEffect>();
            TextMeshProUGUI text = tabButton.GetComponentInChildren<TextMeshProUGUI>();

            Color pressedImageColor = effect != null ? effect.selectedImageColor : Color.gray;
            Color pressedTextColor = effect != null ? effect.selectedTextColor : Color.black;

            Sequence seq = DOTween.Sequence().SetId("tabPress");

            // 设置按下颜色
            if (image != null)
                seq.Append(image.DOColor(pressedImageColor, 0.2f));
            if (text != null)
                seq.Join(text.DOColor(pressedTextColor, 0.2f));

            // 缩放动画
            seq.Join(btnTransform.DOScale(0.85f, 0.1f).SetEase(Ease.OutQuad));
            seq.Append(btnTransform.DOScale(1.0f, 0.1f).SetEase(Ease.OutBack));

            // 可选的复原颜色
            if (restoreColor)
            {
                if (image != null)
                    seq.Join(image.DOColor(effect.unselectedImageColor, 0.1f));
                if (text != null)
                    seq.Join(text.DOColor(effect.unselectedTextColor, 0.1f));
            }
        }
    }






    void SetBattleField()
    {
        if (isPlayerField)
        {
            enemyBackground.color = GetRowColor(RowType.enemy);
            playerBackground.color = GetRowColor(RowType.sel);
            enemyCorners.SetActive(false);
            playerCorners.SetActive(true);
        }else
        {
            enemyBackground.color = GetRowColor(RowType.sel);
            playerBackground.color = GetRowColor(RowType.enemy);
            enemyCorners.SetActive(true);
            playerCorners.SetActive(false);

        }
        AssignKeyInputsToPositions();
    }

    void AssignKeyInputsToPositions()
    {
        foreach (var pair in keyToIndex)
        {
            string keyString = pair.Key.ToString();
            int index = pair.Value;

            if (isPlayerField)
            {
                if (index >= 0 && index < enemyPositions.Count)
                    enemyPositions[index].SetPositionButton(null);
                if (index >= 0 && index < playerPositions.Count)
                    playerPositions[index].SetPositionButton(keyString);

            } else
            {
                if (index >= 0 && index < enemyPositions.Count)
                    enemyPositions[index].SetPositionButton(keyString);
                if (index >= 0 && index < playerPositions.Count)
                    playerPositions[index].SetPositionButton(null);

            }
        }
    }


}
