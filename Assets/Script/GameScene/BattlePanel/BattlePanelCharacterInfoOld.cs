using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.TextCore.Text;
using static GetSprite;
using UnityEngine.Localization.Settings;


public class BattlePanelCharacterInfoOld : MonoBehaviour
{
 /*   public Character characterAtBattlePanel;
    public string characterName;



    public Image CharacterImage;
    public Image itemImage;
    public Image roleImage;


    public GameObject prefab;
    public Transform scrollContent;


    public TMP_Text CharacterNameText;

    public Image MaxForceImage;
    public TMP_Text MaxForceText;
    public Sprite MaxSprite;


    public Image ForceImage;
    public TMP_Text ForceText;
    public Sprite ForceSprite;
    public Sprite HealthSprite;


    public Image RelationImage;
    public TMP_Text FavorabilityText;


    public TMP_Text AttackValueText;
    public TMP_Text DefenseValueText;
    public TMP_Text MagicValueText;
    public TMP_Text SpeedValueText;
    public TMP_Text LuckyValueText;
    public TMP_Text Skill1Text;
    public TMP_Text Skill2Text;
    public TMP_Text Skill3Text;
    public TMP_Text Skill4Text;
    public TMP_Text Skill5Text;

    public BattlePanelValue battlePanelValue;

    public BattlePanelButtonOld battlePanelButton;

    public CharacterPanelButton characterPanelButton;

    public bool isExplore ;

    public Button starButton;


    public string playerEmpireName;


    private Character[] allCharacters;

    private void Start()
    {
        if (!isExplore)
        {
            DisplayCharacter();
        }

        InitStar();


    }

    void InitStar()
    {
        UpStarButtonSprite();
        starButton.onClick.AddListener(() => SetStar());

    }

    public void UpStarButtonSprite()
    {
        string iconPath = $"MyDraw/UI/Other/";
        starButton.gameObject.SetActive(true);
        if (characterAtBattlePanel == null) { starButton.gameObject.SetActive(false); }
        else if (characterAtBattlePanel.Star) { starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Star"); }
        else { starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar"); }
    }



    void SetStar()
    {

        characterAtBattlePanel.Star = !characterAtBattlePanel.Star;

        UpStarButtonSprite();
    }


    private void OnEnable()
    {
        setCharacterValueAtBattlePanel(null);
        if (!isExplore)
        {
            DisplayCharacter();
        }
    }


    public void DisplayCharacter()
    {
        battlePanelButton.characterColumControls.Clear();

        // ?????UI??
        foreach (Transform child in scrollContent)
        {
            Destroy(child.gameObject);
        }

        // ??????
        if (!isExplore)
        {
            GameValue gameValue = FindObjectOfType<GameValue>();
            allCharacters = gameValue.GetCurrentCharactersInGame().ToArray();
        }
        else
        {
            allCharacters = SceneTransferManager.Instance.charactersAtBattlePostions.ToArray();
        }

        // ??????????
        BattleColumnControl playerControl = null;
        List<BattleColumnControl> canMoveList = new();
        List<BattleColumnControl> cannotMoveList = new();

        foreach (var character in allCharacters)
        {
            if (character == null) continue;

            if (character.Country != playerEmpireName) continue;

            GameObject characterPrefab = Instantiate(prefab, scrollContent);
            characterPrefab.tag = "Prefab";

            BattleColumnControl characterColumControl = characterPrefab.GetComponent<BattleColumnControl>();

            if (characterColumControl == null) continue;

         //   characterColumControl.characterColumnControlP(character);
            characterColumControl.isExplore = battlePanelValue.GetIsExplore();
            characterColumControl.isInExplore = battlePanelValue.isInExplore;

            // ??favorabilityLevel??????
            if (character.FavorabilityLevel == 0)
            {
                playerControl = characterColumControl;
            }
            else
            {
                if (character.CanMove())
                    canMoveList.Add(characterColumControl);
                else
                    cannotMoveList.Add(characterColumControl);
            }
        }

        // ?????????? + ?? + ???
        List<BattleColumnControl> finalList = new();

        if (playerControl != null)
        {
            if (playerControl.character.CanMove())
            {
                finalList.Add(playerControl);
                finalList.AddRange(canMoveList);
                finalList.AddRange(cannotMoveList);
            }
            else
            {
                finalList.AddRange(canMoveList);
                finalList.Add(playerControl);
                finalList.AddRange(cannotMoveList);
            }
        }
        else
        {
            finalList.AddRange(canMoveList);
            finalList.AddRange(cannotMoveList);
        }

        // ???????????SiblingIndex????characterColumControls??
        for (int i = 0; i < finalList.Count; i++)
        {
            finalList[i].transform.SetSiblingIndex(i);
            battlePanelButton.characterColumControls.Add(finalList[i]);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent.GetComponent<RectTransform>());
    }

    public BattleColumnControl battleColumnControl;

    public void SetBattleColumnControlToBattlePanel(BattleColumnControl battleColumnControl){ 
        if (this.battleColumnControl != null) this.battleColumnControl.RestoreColor();
        setCharacterValueAtBattlePanel(battleColumnControl.character);
        this.battleColumnControl = battleColumnControl;
       // battleColumnControl.gameObject.GetComponent<Image>().color = Color.red;
   }

    public void setCharacterValueAtBattlePanel(Character character)
    {
        characterAtBattlePanel = character;
        SetForceLimit(character);
        ForceLimit.SetActive(!battlePanelValue.GetIsExplore());

        if (character != null)
        {
            CharacterImage.gameObject.SetActive(true);
            CharacterImage.sprite = character.image;
            CharacterNameText.text = character.GetCharacterName();
            characterName = character.GetCharacterENName();

            roleImage.gameObject.SetActive(true);
            roleImage.sprite = GetRoleSprite(character.RoleClass);

            RelationImage.gameObject.SetActive(true);
            FavorabilityText.gameObject.SetActive(true);
            //  RelationImage.sprite = character.favorabilitySprites[character.favorabilityLevel];

            string iconPath = $"MyDraw/UI/Other/CharacterValueIcon/";

            if (character.FavorabilityLevel == 0){RelationImage.sprite = Resources.Load<Sprite>(iconPath + "NormalFavorability");}
            else if (character.FavorabilityLevel == 1){RelationImage.sprite = Resources.Load<Sprite>(iconPath + "Self");}
            else{RelationImage.sprite = Resources.Load<Sprite>(iconPath + "RomanceFavorability");}


            FavorabilityText.text = character.Favorability.ToString();

            MaxForceImage.sprite = MaxSprite;
            if (battlePanelValue != null && battlePanelValue.GetIsExplore())
            {
                MaxForceText.text = character.GetMaxHealthString();
                ForceImage.sprite = HealthSprite;
                ForceText.text = character.Health.ToString("N0");
            }
            else
            {
                MaxForceText.text = character.MaxForce.ToString("N0");
                ForceImage.sprite = ForceSprite;
                ForceText.text = character.Force.ToString("N0");
            }



            AttackValueText.text = FormatfloatNumber(character.GetValue(0,0));
            DefenseValueText.text = FormatfloatNumber(character.GetValue(0, 1));
            MagicValueText.text = FormatfloatNumber(character.GetValue(0, 2));
            SpeedValueText.text = FormatfloatNumber(character.GetValue(0, 3));
            LuckyValueText.text = FormatfloatNumber(character.GetValue(0, 4));

            string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
            Skill1Text.text = character.CurrentSkillName(0);
            Skill2Text.text = character.CurrentSkillName(1);
            Skill3Text.text = character.CurrentSkillName(2);
            Skill4Text.text = character.CurrentSkillName(3);
            Skill5Text.text = character.CurrentSkillName(4);


            if (characterPanelButton != null) { 
                characterPanelButton.SetTheForceType(); 
            }

            } else
        {

            CharacterImage.gameObject.SetActive(false);
            CharacterNameText.text = "NONE";
            characterName = null;

            roleImage.gameObject.SetActive(false);

            RelationImage.gameObject.SetActive(false); 
            FavorabilityText.gameObject.SetActive(false);




            MaxForceImage.sprite = MaxSprite;
            if (battlePanelValue.GetIsExplore())
            {
                ForceImage.sprite = HealthSprite;
            }
            else
            {
                ForceImage.sprite = ForceSprite;
            }



            MaxForceText.text = "NONE";
            ForceText.text = "NONE";

            AttackValueText.text = "A";
            DefenseValueText.text = "D";
            MagicValueText.text = "M";
            SpeedValueText.text = "S";
            LuckyValueText.text = "L";

            Skill1Text.text = "NONE";
            Skill2Text.text = "NONE";
            Skill3Text.text = "NONE";
            Skill4Text.text = "NONE";
            Skill5Text.text = "NONE";

        }






        if (character != null && character.ItemWithCharacter != null)
        {
            itemImage.sprite = character.ItemWithCharacter.icon;
        }
        else
        {
            itemImage.sprite = null;
        }

        UpStarButtonSprite();

    }

    string FormatfloatNumber(float value) => value * 10 % 10 == 0 ? value.ToString("F0") : value.ToString("F1");

    public GameObject ForceLimit;
    public TMP_Text limitText;
    public TMP_Text leaderText;
    public TMP_Text charmText;

    void SetForceLimit(Character character)
    {
        if (character != null)
        {
            limitText.text = character.GetMaxLimit().ToString();
            leaderText.text = character.GetValue(2, 0).ToString();
            charmText.text = character.GetValue(2, 4).ToString();
        } else
        {
            limitText.text = "NONE";
            leaderText.text = "0";
            charmText.text = "0";

        }

    }
 */
}