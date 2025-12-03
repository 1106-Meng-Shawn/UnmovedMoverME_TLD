using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterPanelButton : MonoBehaviour
{
   /* public GameValue gameValue;
    public CharacterPanel characterPanel;
    public BattlePanelCharacterInfoOld battlePanelCharacterInfo;

    [HideInInspector] public string characterName;
    public TopRowTextShow topRowTextShow;

    public GameObject feedbackPrefab;



    public Button ForceValueTypeChange;
    public Button HealthValueTypeChange;
    public List<Button> ValueTypes;
    public List<Button> characterValueAddButtons;


    public Slider ForceChangeSlider;
    public Button ForceChangeCheck;
    public Slider MaxForceChangeSlider;
    public Button MaxForceChangeCheck;

    [HideInInspector] private Character character;
    private int forceType;
    private TMP_Text ForceText;
    private TMP_Text MaxForceText;

    public Button starButton;



    void UpType()
    {
        if (characterPanel != null)
        {
            character = characterPanel.characterAtPanel;
            ForceText = characterPanel.characterForceText;
            MaxForceText = characterPanel.characterMAXForceText;
            forceType = characterPanel.ForceType;

        }
        else if (battlePanelCharacterInfo != null)
        {
            character = battlePanelCharacterInfo.characterAtBattlePanel;
            ForceText = battlePanelCharacterInfo.ForceText;
            MaxForceText = battlePanelCharacterInfo.MaxForceText;

            if (battlePanelCharacterInfo.battlePanelValue.GetIsExplore())
            {
                forceType = 1;
            }
            else
            {
                forceType = 0;
            }
        }

    }


    void Start()
    {


    for (int i = 0; i < ValueTypes.Count; i++)
    {
        int index = i;  
        ValueTypes[i].onClick.AddListener(() => characterPanel.SetValueType(index));
    }

        for (int i = 0; i < characterValueAddButtons.Count; i++)
        {
            int index = i;
            characterValueAddButtons[i].onClick.AddListener(() => AddValue(index));
        }


        if (ForceValueTypeChange != null)
    {
        ForceValueTypeChange.onClick.AddListener(() => ChangeTheForceType(0));
    }

    if (HealthValueTypeChange != null)
    {
        HealthValueTypeChange.onClick.AddListener(() => ChangeTheForceType(1));
    }

    if (ForceChangeSlider != null)
    {
        ForceChangeSlider.onValueChanged.AddListener(OnForceChangeSliderValueChanged);
    }

    if (MaxForceChangeSlider != null)
    {
        MaxForceChangeSlider.onValueChanged.AddListener(OnMaxForceChangeSliderValueChanged);
    }

    if (ForceChangeCheck != null)
    {
        ForceChangeCheck.onClick.AddListener(() => CheckCurrent());
    }

    if (MaxForceChangeCheck != null)
    {
        MaxForceChangeCheck.onClick.AddListener(() => CheckMax());
    }
         if (starButton != null) InitStar();

    }

    void UpCharacterRowData()
    {
        if (characterPanel.characterColumnControl == null) return;
        characterPanel.characterColumnControl.UpdateCharacterPrefab();
        UpStarButtonSprite();

    }

    void CheckCurrent(){
        UpType();
        if (character == null) return;
        if (forceType == 0){
            checkForce();
        } else if (forceType == 1) {
            checkHealth();
        }
        UpCharacterRowData();
    }


    void InitStar()
    {
        starButton.onClick.AddListener(() => SetIsStar());
        UpStarButtonSprite();

    }


    public void SetCharacter(Character character)
    {
        this.character = character;
        UpStarButtonSprite();
    }

    public void UpStarButtonSprite()
    {
        string iconPath = $"MyDraw/UI/Other/";
        if (character == null) {
            starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar");
            starButton.interactable = false;
            // starButton.gameObject.SetActive(false);
            return; }

        // starButton.gameObject.SetActive(true);
        starButton.interactable = true;

        if (character.Star) { starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Star"); }
        else { starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar"); }
    }

    void SetIsStar()
    {
        if (character != null) character.SetStar();
        UpStarButtonSprite();
        UpCharacterRowData();
    }


    void CheckMax(){
        UpType();
        if (character == null) return;
        if (forceType == 0){
            CheckMaxForce();
        } else if (forceType == 1) {
            CheckMaxHealth();
        }
        UpCharacterRowData();
    }

    void CheckMaxForce()
{
        UpType();


        int newMaxForce = CalculateNewMaxForce();

        gameValue.Gold -= (newMaxForce - character.MaxForce) * 100;
        character.MaxForce = newMaxForce;

        if (character.Force > character.MaxForce)
        {
            gameValue.TotalRecruitedPopulation += (character.Force - character.MaxForce);
            character.Force = character.MaxForce;
        }

        ForceChangeSlider.onValueChanged.Invoke(MaxForceChangeSlider.value);



        SetTheForce();

}
    // 计算新的力量值
    int CalculateNewMaxForce()
{

        UpType();
        float difference =  MaxForceChangeSlider.value;
        int newMaxForce;

    if ((character.GetMaxLimit() - character.MaxForce) * 100 <= gameValue.Gold)
    {
            newMaxForce = Mathf.RoundToInt(difference * character.GetMaxLimit());
    }
    else
    {
            newMaxForce = Mathf.RoundToInt(difference * (character.MaxForce + gameValue.Gold / 100f));
    }

        if (newMaxForce <= 0)
        {
            newMaxForce = 1;
        }



        return newMaxForce;


    }


    void CheckMaxHealth(){
        UpType();
    if (character.MaxHealth == 9999) return;

    if (MaxForceChangeSlider.value == 0)
    {
        MaxForceChangeSlider.value = 1;
        MaxForceChangeSlider.onValueChanged.Invoke(MaxForceChangeSlider.value);
    }
    else
    {
        float difference = MaxForceChangeSlider.value;

    int newMaxHealth;
    if ((9999 - character.MaxHealth) <= gameValue.Science)
    {
        newMaxHealth = Mathf.RoundToInt(difference * (9999 - character.MaxHealth));
    }
    else
    {
        newMaxHealth = Mathf.RoundToInt(difference * gameValue.Science);
    }

        gameValue.Science -= newMaxHealth;
            character.MaxHealth += newMaxHealth;
        MaxForceChangeSlider.onValueChanged.Invoke(MaxForceChangeSlider.value);
        SetTheHealth();
        // gamevalue change


    }
    }

void checkForce()
{

    // 如果 ForceChangeSlider 的值已经是目标值，则直接设为 1，避免卡住
    
        // 计算新的 Force，并更新 Force 和游戏值
        int newForce = CalculateNewForce(CalculateTargetValue());
        UpdateForce(newForce);
    

    ForceChangeSlider.onValueChanged.Invoke(ForceChangeSlider.value);
    SetTheForce();
}

// 计算目标值
float CalculateTargetValue()
{
    UpType();
    if (character.MaxForce == character.Force) return 1;

    if (character.MaxForce - character.Force >= gameValue.TotalRecruitedPopulation)
    {
        return (float)character.Force / character.MaxForce;
    }
    else
    {
        return (float)character.Force / (gameValue.TotalRecruitedPopulation + character.Force);
    }
}

// 计算新的 Force 值
int CalculateNewForce(float targetValue)
{
    UpType();
    float difference = ForceChangeSlider.value;
    int newForce;

    if (character.MaxForce - character.Force <= gameValue.TotalRecruitedPopulation)
    {
        newForce = Mathf.RoundToInt(difference * character.MaxForce);
    }
    else
    {
        newForce = Mathf.RoundToInt(difference * (gameValue.TotalRecruitedPopulation + character.Force));
    }

    if (newForce <= 0)
    {
        newForce = 1;
    }

    return newForce;
}

void UpdateForce(int newForce)
{
    UpType();

    gameValue.TotalRecruitedPopulation -= (newForce-character.Force);
    character.Force = newForce;
}

void checkHealth()
{
    UpType();

    if (character.Health == character.MaxHealth || gameValue.Gold <= 0 )
    {
        ForceChangeSlider.value = 1;
        ForceChangeSlider.interactable = false;  
    }
    else
    {
        float difference = ForceChangeSlider.value;
        int healthDifference;
        
        if (character.MaxHealth - character.Health <= gameValue.Gold){
            healthDifference  = Mathf.RoundToInt(difference * (character.MaxHealth - character.Health));
        } else {
            healthDifference  = Mathf.RoundToInt(difference * gameValue.Gold);
        }
        
        character.Health = Mathf.Max(1, character.Health + healthDifference);

        gameValue.Gold -= healthDifference * 1;

    }

    ForceChangeSlider.onValueChanged.Invoke(ForceChangeSlider.value);
    SetTheHealth();

}


    void ChangeTheForceType(int changeNum)
    {
        characterPanel.ForceType = changeNum;
        SetTheForceType();
    }

    public void SetTheForceType(){

        UpType();
    if (character == null){
        MaxForceChangeSlider.interactable = false;  
        MaxForceChangeSlider.value = 1;
        ForceChangeSlider.interactable = false;  
        ForceChangeSlider.value = 0;

        return;
    }else{
        MaxForceChangeSlider.interactable = true; 
          ForceChangeSlider.interactable = true; 
    }


        if (forceType == 0){
            SetTheForce();
        } else if (forceType == 1) {
            SetTheHealth();
        }

    }

void SetTheForce()
{// ForceChangeSlider will alwasy can use?
        UpType();
    if (character.MaxForce - character.Force <= gameValue.TotalRecruitedPopulation ){
            ForceChangeSlider.value =  (float)character.Force / character.MaxForce;
    } else {
            ForceChangeSlider.value =  (float)character.Force / (gameValue.TotalRecruitedPopulation + character.Force);
    }

        if (character.Force == 1 && gameValue.TotalRecruitedPopulation <= 0)
    {
        ForceChangeSlider.value = 1;
        ForceChangeSlider.interactable = false;  
    }


        if (character.Force == character.MaxForce)
        {
            ForceChangeSlider.value = 1;
            if (character.Force == 1) ForceChangeSlider.interactable = false;

        }



        if (character.GetMaxLimit() - character.MaxForce <= gameValue.Gold/100)
        {
            MaxForceChangeSlider.value = (float)character.MaxForce / character.GetMaxLimit();
        }
        else
        {
            MaxForceChangeSlider.value = (float)character.MaxForce / (gameValue.Gold / 100 + character.MaxForce);
        }

        ForceText.text = character.Force.ToString("N0");
        MaxForceText.text = character.MaxForce.ToString("N0");

        /*   if (characterPanel.characterAtPanel.MaxForce == characterPanel.characterAtPanel.GetMaxLimit() || gameValue.gold < 100)
           {
               MaxForceChangeSlider.value = 1;
             //  MaxForceChangeSlider.interactable = false;  
           }
           else
           {
               MaxForceChangeSlider.value = (characterPanel.characterAtPanel.MaxForce / characterPanel.characterAtPanel.GetMaxLimit());
            //   MaxForceChangeSlider.interactable = true;  
           }*/

   /*     if (characterPanel != null) characterPanel.SetForceType(0);

}

void SetTheHealth()
{
        UpType();

    if (character.MaxHealth == character.Health || gameValue.Gold <= 0){
        ForceChangeSlider.value = 1;
        ForceChangeSlider.interactable = false;  }
    else{ForceChangeSlider.value = 0;
    ForceChangeSlider.interactable = true;
    }

    if (character.MaxHealth == 9999 || gameValue.Science <= 0){
        MaxForceChangeSlider.value = 1;
        MaxForceChangeSlider.interactable = false;  
    }else{
        MaxForceChangeSlider.value = 0;
        MaxForceChangeSlider.interactable = true;  
    }

        ForceText.text = character.Health.ToString("N0");
        MaxForceText.text = character.MaxHealth.ToString("N0");


        if (characterPanel != null) characterPanel.SetForceType(1);
}

void OnForceChangeSliderValueChanged(float value)
{

        UpType();


        if (character == null)
        {
            ForceChangeSlider.interactable = false;
            ForceChangeSlider.value = 1;
            return;
        }
        else
        {
            ForceChangeSlider.interactable = true;
        }


if (forceType == 0)
{

    int newForce; // check force and recruited population
    if (character.MaxForce - character.Force <= gameValue.TotalRecruitedPopulation){
        newForce =  Mathf.RoundToInt( (value * character.MaxForce));
    } else {
        newForce =  Mathf.RoundToInt(value * (gameValue.TotalRecruitedPopulation + character.Force));
    }
    
    newForce = Mathf.Max(newForce, 1);

    if (newForce != character.Force)
    {
        string sign = newForce < character.Force ? "-" : "+";
        ForceText.text = character.Force.ToString("N0") + " " + sign + " " + Mathf.Abs(newForce- character.Force).ToString("N0");
    }
    else
    {
        ForceText.text = character.Force.ToString("N0");
    }
}
    else if (forceType == 1)
    {

    int newHealth; // check health and game value gold
    if (character.MaxHealth - character.Health <= gameValue.Gold){
        newHealth =  Mathf.RoundToInt(value * (character.MaxHealth - character.Health));
    } else {
        newHealth =  Mathf.RoundToInt(value * gameValue.Gold);
    }

        if (character.Health + newHealth != character.Health)
        {
            ForceText.text = character.Health.ToString("N0") + " + " + newHealth.ToString("N0");

        }
    }


}

void OnMaxForceChangeSliderValueChanged(float value)
{

        UpType();

        if (character == null)
    {
        MaxForceChangeSlider.interactable = false;  
        MaxForceChangeSlider.value = 1;
        return;
    }
    else
    {
        MaxForceChangeSlider.interactable = true; 
    }

    if (forceType == 0)
    {
        int newMaxForce;

            if (character.GetMaxLimit() - character.MaxForce <= gameValue.Gold/100)
            {
                newMaxForce = Mathf.RoundToInt((value * character.GetMaxLimit()));
            }
            else
            {
                newMaxForce = Mathf.RoundToInt(value * (gameValue.Gold/100 + character.MaxForce));
            }

            newMaxForce = Mathf.Max(newMaxForce, 1);

            if (newMaxForce != character.MaxForce)
            {
                string sign = newMaxForce < character.MaxForce ? "-" : "+";
                MaxForceText.text = character.MaxForce.ToString("N0") + " " + sign + " " + Mathf.Abs(newMaxForce - character.MaxForce).ToString("N0");
            }
            else
            {
                MaxForceText.text = character.MaxForce.ToString("N0");
            }

    }
    else if (forceType == 1)
    {

        int newMaxHealth;
        if(9999 - character.MaxHealth <= gameValue.Science){
            newMaxHealth  = Mathf.RoundToInt(value * (9999-character.MaxHealth));
        } else {
            newMaxHealth  = Mathf.RoundToInt(value * gameValue.Science);
        }




        if (newMaxHealth != character.MaxHealth)
        {
            MaxForceText.text = character.MaxHealth.ToString("N0") + " + " + newMaxHealth.ToString("N0");
        }
        else
        {
            MaxForceText.text = character.MaxHealth.ToString("N0");
        }
    }

}

    private void AddHoldButtonListener(Button button, UnityAction action)
    {
        if (button != null)
        {
            HoldButton holdButton = button.GetComponent<HoldButton>();
            if (holdButton != null)
            {
                holdButton.onHoldClick.AddListener(action);
            }
        }
    }

    void Update()  
    {

        if (gameValue == null) gameValue = GameObject.FindObjectOfType<GameValue>();


    }

    void AddValue(int index)
    {
        int type = 0;
        if (characterPanel != null && characterPanel.characterAtPanel != null)
        {
            type = characterPanel.ValueType;
            character = characterPanel.characterAtPanel;

        } else if (battlePanelCharacterInfo != null && battlePanelCharacterInfo.characterAtBattlePanel != null)
        {
            character = battlePanelCharacterInfo.characterAtBattlePanel;

        } else
        {
            return;
        }

            var key = Tuple.Create(type, index);

        // Check if the tuple key exists in the dictionary
     //   Increase(ref gameValue.Science, ref character, type,index, "science");
        UpCharacterRowData();
    }


    public void Increase(ref float gameResource, ref Character character, int type , int index, string costResourceName)
    {
        if (character == null ) return;

        int characterValue = character.GetValue(type,index);

        if (characterValue == 99)
        {
            NotificationManage.Instance.ShowToTop($"Your character value already is MAX");
            return;
        }


        if (gameResource == 0)
        {
            NotificationManage.Instance.ShowToTop($"Your {costResourceName} resource is not enough");
            return;
        }

        int increaseAmount = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ? 10 : 1;
        int cost = CalculateCost(characterValue, increaseAmount);

        if (gameResource >= cost)
        {
            gameResource -= cost;
            IncreaseCharacterValue(ref character, type, index, increaseAmount);
        }
    }
    void IncreaseCharacterValue(ref Character character, int type , int index, int increaseAmount)
    {
        character.AddValue(type, index, increaseAmount);
        if (characterPanel != null) characterPanel.setCharacterValueAtPanel(character);
        if (battlePanelCharacterInfo != null) battlePanelCharacterInfo.setCharacterValueAtBattlePanel(character);

    }





    private int CalculateCost(int characterValue, int increaseAmount)
{
    int cost;
    if (characterValue < 10)
    {
        cost = 2000;
    }
    else
    {
        cost = 2000 + (int)(Math.Floor(characterValue / 10f)) * 1000;
    }

    return cost * increaseAmount;
}

    public void IncreaseMaxForce()
    {

        if (gameValue.Gold == 0) { NotificationManage.Instance.ShowToTop("Your gold resouce is not enough"); return; }



        if (character == null) return;

        int increaseAmount = 100;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            increaseAmount *= 10;
        }

        int cost = character.MaxForce / 100;

        if (character != null)
        {
            if (gameValue.Gold >= cost * increaseAmount)
            {
                gameValue.Gold -= cost * increaseAmount;
                character.MaxForce += increaseAmount;
            } else
            {
                increaseAmount = (int)( gameValue.Gold / cost);
                gameValue.Gold = 0;
                character.MaxForce += increaseAmount;
            }
        }
    }

    public void ForceToMax()
    {
        if (gameValue.TotalRecruitedPopulation == 0) { NotificationManage.Instance.ShowToTop("The total number of recruits is not enough!"); return; }
        if (character.MaxForce == character.Force) { NotificationManage.Instance.ShowToTop($"{character.GetCharacterENName()}'s force is already at the maximum"); return; }


        if (character != null)
        {

            if (gameValue.TotalRecruitedPopulation >= (character.MaxForce - character.Force))
            {
                gameValue.TotalRecruitedPopulation -= (character.MaxForce - character.Force);
                character.Force = character.MaxForce;
            }
            else
            {
                character.Force += gameValue.TotalRecruitedPopulation;
                gameValue.TotalRecruitedPopulation = 0;
            }
        }
    }

    public void ForceToMin()
    {
        if (character.Force == 1) { NotificationManage.Instance.ShowToTop($"{character.GetCharacterENName()}'s force is already at the minimum"); return; }

        if (character != null)
        {
            if (character.Force > 1)
            {
                gameValue.TotalRecruitedPopulation += (character.Force-1);
                character.Force = 1;
            }
        }
    }

    public void ForceIncrease100()
    {
        if (character == null) return;

        if (gameValue.TotalRecruitedPopulation == 0) { NotificationManage.Instance.ShowToTop("The total number of recruits is not enough!"); return; }


        int Amount = 100;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            Amount *= 10;
        }

        if (character != null)
        {
            int availableAmount = Mathf.Min(gameValue.TotalRecruitedPopulation, Amount);
            int newForce = character.Force + availableAmount;

            // Ensure Force doesn't exceed MaxForce
            if (newForce > character.MaxForce)
            {
                availableAmount = character.MaxForce - character.Force;
                character.Force = character.MaxForce;
            }
            else
            {
                character.Force = newForce;
            }

            gameValue.TotalRecruitedPopulation -= availableAmount;
        }
    }


    public void ForceDecrease100()
    {
        if (character == null) return;

        if (character.Force == 1) { NotificationManage.Instance.ShowToTop($"{character.GetCharacterENName()}'s force is already at the minimum"); return; }


        int Amount = 100;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            Amount *= 10;
        }


        if (character != null)
        {
            // Ensure Force does not go below 0
            if (character.Force - Amount < 1)
            {
                gameValue.TotalRecruitedPopulation += (character.Force-1);
                character.Force = 1;
            }
            else
            {
                gameValue.TotalRecruitedPopulation += Amount;
                character.Force -= Amount;
            }
        }
    }


    float CalculateTargetMAXForceValue()
    {
        var character = characterPanel.characterAtPanel;

        if (character.MaxForce == character.GetMaxLimit()) return 1;

        float limit = character.GetMaxLimit();
        float max = character.MaxForce;
        if ((limit - max) * 100 <= gameValue.Gold)
        {
            Debug.Log(max + limit);
            return max / limit;
        }
        else
        {

            return max / (max + gameValue.Gold / 100);
        }

    }*/


}

