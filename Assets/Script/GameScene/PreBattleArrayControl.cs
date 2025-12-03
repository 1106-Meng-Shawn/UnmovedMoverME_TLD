using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static GetString;

public class PreBattleArrayControl : MonoBehaviour
{
    public GameValue gameValue;

    public TMP_Text battleArrayForceText;
    public Button checkButton;
    public Button restoreButton;
    public Button backButton;
    public TMP_Text enemyArrayForceText;

    public GameObject enemyArray;
    public Button checkAgainButton;
    public GameObject scoutPanel;
    public Button cancelScoutButton;

    public Button scoutCheckButton;
    public Slider scoutSlider;


    public TMP_Text scoutRemainText;


    public TMP_Text scoutFirstNeedValue;
    public TMP_Text scoutSecondNeedValue;
    public TMP_Text scoutThirdNeedValue;

    public List<BattlePosition> playerBattlePositions;
    public List<BattlePosition> enemyBattlePositions;
    public List<BattlePosition> scoutBattlePositions;

    public BattlePanelButtonOld battlePanelButton;


    public GameObject feedbackPrefab;

    private int playerTotalForce;
    private int playerMaxForce;

    private int EnemyTotalForce;
    private int EnemyMaxForce;



    private float enemyFirstScoutValue = 0;
    private float enemySecondScoutValue = 0;
    private float enemyThirdScoutValue = 0;

    private float playerRemainScout = 0;
    private float playerGameValueScoutUsed = 0;

    private bool isScouted = false;
    private bool isScoutedAll = false;

    private void Start()
    {

        if (scoutSlider != null)
        {
            scoutSlider.onValueChanged.AddListener(OnScoutSliderValueChanged);
        }


        if (gameValue == null) gameValue = FindObjectOfType<GameValue>();

        if (checkButton != null) { checkButton.onClick.AddListener(OpenEnemyArray); }
        if (backButton != null) { backButton.onClick.AddListener(CloseEnemyArray); }
        if (restoreButton != null) { restoreButton.onClick.AddListener(RestoreForce); }

        if (scoutCheckButton != null) { scoutCheckButton.onClick.AddListener(PerformScoutCheck); }
        if (checkAgainButton != null) { checkAgainButton.onClick.AddListener(OpenScoutPanel); }
        if (cancelScoutButton != null) { cancelScoutButton.onClick.AddListener(CloseScoutPanel); }
    }


    private void OnDisable()
    {

        ResetScoutValues();


        EnemyBattlePositionStateChange(true);
        EnemyBattlePositionValueStateChange(true);


        if (enemyArray != null ) enemyArray.gameObject.SetActive(false);
        if (scoutPanel != null ) scoutPanel.gameObject.SetActive(false);

        if (scoutBattlePositions[0] == null) return;
        foreach (var position in scoutBattlePositions)
        {
            if (position.characterAtBattlePosition != null)
            {
               // position.characterAtBattlePosition.battlePosition = null;
                position.characterAtBattlePosition = null;
            }
        }
    }

    void OnScoutSliderValueChanged(float value)
    {
        if (value == 0)
        {
            scoutRemainText.text = $"{gameValue.GetResourceValue().Scout:F1}";
            playerGameValueScoutUsed = 0;
        } else
        {
            playerGameValueScoutUsed = gameValue.GetResourceValue().Scout * value;
            string scoutValueString = Mathf.Round((float)playerGameValueScoutUsed * 10) % 10 == 0 ? $"{playerGameValueScoutUsed:F0}" : $"{playerGameValueScoutUsed:F1}";
            scoutRemainText.text = $"{gameValue.GetResourceValue().Scout:F1} - {scoutValueString}";

        }

        UpdateScoutText();
    }


    private void CalculatePlayerForce()
    {
        playerTotalForce = 0;
        playerMaxForce = 0;

        foreach (var position in playerBattlePositions)
        {
            if (position.characterAtBattlePosition != null)
            {
                playerTotalForce += position.characterAtBattlePosition.Force;
                playerMaxForce += position.characterAtBattlePosition.MaxForce;
            }
        }
    }

    private void CalculateEnemyForce()
    {
        EnemyTotalForce = 0;
        EnemyMaxForce = 0;

        foreach (var position in enemyBattlePositions)
        {
            if (position.characterAtBattlePosition != null)
            {
                EnemyTotalForce += position.characterAtBattlePosition.Force;
                EnemyMaxForce += position.characterAtBattlePosition.MaxForce;
            }
        }
    }

    private void OpenEnemyArray()
    {
        enemyArray.SetActive(true);
        if (!isScouted)
        {
            SetEnemyScoutValues();
            OpenScoutPanel();
            UpdateScoutText();
        }
    }

    private void SetEnemyScoutValues()
    {

        enemyFirstScoutValue = 0;
        foreach (var position in enemyBattlePositions)
        {
            if (position.characterAtBattlePosition != null)
            {
                enemyFirstScoutValue += ( position.characterAtBattlePosition.GetValue(2,3) + position.characterAtBattlePosition.GetValue(2, 4) / 10);
            }
        }

        enemySecondScoutValue = (int)(1.5 * enemyFirstScoutValue);
        enemyThirdScoutValue = (int)(2 * enemyFirstScoutValue);
    }

    private void PerformScoutCheck()
    {

        if (isScoutedAll)
        {
            // NotificationManage.Instance.ShowToTop("You already Scout All!");
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Scout_AllLevel);
            return;
        }


        float playerScout = 0;//= GetPlayerScoutAtPositions() + playerRemainScout + playerGameValueScoutUsed;

        if (playerScout >= enemyThirdScoutValue)
        {
            ProcessScout(playerScout, enemyThirdScoutValue);
            EnemyBattlePositionStateChange(false);
            EnemyBattlePositionValueStateChange(false);
            isScoutedAll = true;

        }
        else if (playerScout >= enemySecondScoutValue && !enemyArrayForceText.gameObject.activeSelf)
        {
            ProcessScout(playerScout, enemySecondScoutValue);
            EnemyBattlePositionStateChange(false);
        }
        else if (playerScout >= enemyFirstScoutValue && !isScouted)
        {
            ProcessScout(playerScout, enemyFirstScoutValue);
        }
        else
        {
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource,GetResourceStringWithSprite(ValueType.Scout));
            //   NotificationManage.Instance.ShowToTop("You don't have enough Scout points!");
        }
    }

    private void ProcessScout(float playerScout, float requiredScoutValue)
    {
      //  battlePanelButton.SetForceImage(1);
        playerRemainScout = playerScout - requiredScoutValue;
        // NotificationManage.Instance.ShowToTop($"Scout check successful!");
        NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Scout_Success);
        gameValue.GetResourceValue().Scout += playerRemainScout;
        gameValue.GetResourceValue().Scout -= playerGameValueScoutUsed;
       // playerRemainScout = 0;
        playerGameValueScoutUsed = 0;
        scoutSlider.value = 0;
        isScouted = true;
        moveScoutPositions();
        scoutPanel.SetActive(false);
    }



    private void EnemyBattlePositionStateChange(bool state)
    {
        if (enemyArrayForceText == null) return;
        enemyArrayForceText.gameObject.SetActive(!state);

        foreach (var position in enemyBattlePositions)
        {
                position.hideValueBlock.SetActive(state);
        }


    }

    private void EnemyBattlePositionValueStateChange(bool state)
    {
        if (enemyBattlePositions[0] == null) return;
        foreach (var position in enemyBattlePositions)
        {
                position.hideValueRow.SetActive(state);
            
        }


    }

    private void moveScoutPositions()
    {
        foreach (var position in scoutBattlePositions)
        {
            if (position.characterAtBattlePosition != null)
            {
               // position.characterAtBattlePosition.battlePosition = null;
                position.characterAtBattlePosition.IsMoved = true;
                position.characterAtBattlePosition = null;
            }
        }
    }



    private void ClearScoutPositions()
    {
        foreach (var position in scoutBattlePositions)
        {
            if (position.characterAtBattlePosition != null)
            {
               // position.characterAtBattlePosition.battlePosition = null;
                position.characterAtBattlePosition = null;
            }
        }
    }

    private double GetPlayerScoutAtPositions()
    {
        int totalScout = 0;
        foreach (var position in scoutBattlePositions)
        {
            if (position.characterAtBattlePosition != null)
            {
                totalScout += position.characterAtBattlePosition.GetValue(2,3) + (position.characterAtBattlePosition.GetValue(2, 3) / 10);
            }
        }
        return totalScout;
    }

    private void UpdateScoutText()
    {
        double playerScout = GetPlayerScoutAtPositions() + playerRemainScout + playerGameValueScoutUsed;

       // double playerScout = GetPlayerScoutAtPositions() + gameValue.scout;

       // scoutRemainText.text = $"{gameValue.scout}";

        UpdateScoutColor(scoutFirstNeedValue, playerScout, enemyFirstScoutValue, isScouted);
        UpdateScoutColor(scoutSecondNeedValue, playerScout, enemySecondScoutValue, enemyArrayForceText.gameObject.activeSelf);
        UpdateScoutColor(scoutThirdNeedValue, playerScout, enemyThirdScoutValue, isScoutedAll);

        string scoutValueString = Mathf.Round((float)playerScout*10) % 10 == 0 ? $"{playerScout:F0}" : $"{playerScout:F1}";
        scoutFirstNeedValue.text = $"{scoutValueString}/{enemyFirstScoutValue}";
        scoutSecondNeedValue.text = $"{scoutValueString}/{enemySecondScoutValue}";
        scoutThirdNeedValue.text = $"{scoutValueString}/{enemyThirdScoutValue}";
    }

    private void UpdateScoutColor(TMP_Text text, double playerScout, double enemyScoutValue, bool condition)
    {
        text.color = (condition) ? Color.green : Color.black;
    }


    private void OpenScoutPanel()
    {
        scoutPanel.SetActive(true);
        //battlePanelButton.SetForceImage(0);
        scoutRemainText.text = $"{gameValue.GetResourceValue().Scout:F1}";

    }
    private void CloseEnemyArray() => enemyArray.SetActive(false);

    public void CloseScoutPanel()
    {
        if (isScouted)
        {
            scoutPanel.SetActive(false);
        }
        else
        {
            enemyArray.SetActive(false);
            scoutPanel.SetActive(false);
        }


       // battlePanelButton.SetForceImage(1);
        ClearScoutPositions();

    }

    private void RestoreForce()
    {
        bool isFull = true;

        foreach (var position in playerBattlePositions)
        {
            if (position.characterAtBattlePosition != null &&
                position.characterAtBattlePosition.MaxForce > position.characterAtBattlePosition.Force)
            {
                isFull = false;
                break;
            }
        }

        if (isFull) return;

        while (gameValue.GetResourceValue().TotalRecruitedPopulation > 0)
        {
            bool allFull = true;

            foreach (var position in playerBattlePositions)
            {
                if (position.characterAtBattlePosition != null)
                {
                    var character = position.characterAtBattlePosition;
                    if (character.MaxForce > character.Force)
                    {
                        character.Force += 1;
                        gameValue.GetResourceValue().TotalRecruitedPopulation -= 1;
                        allFull = false;
                        if (gameValue.GetResourceValue().TotalRecruitedPopulation == 0) return;
                    }
                }
            }

            if (allFull) break;
        }
    }

    private void ResetScoutValues()
    {
        enemyFirstScoutValue = 0;
        enemySecondScoutValue = 0;
        enemyThirdScoutValue = 0;
        if (scoutSlider != null)  scoutSlider.value = 0;
        playerRemainScout = 0;
        playerGameValueScoutUsed = 0;
        isScouted = false;
        isScoutedAll = false;
    }

}

