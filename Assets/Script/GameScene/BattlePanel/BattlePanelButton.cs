using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using System;

public class BattlePanelButtonOld : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
   /* public List<BattleColumnControl> characterColumControls = new List<BattleColumnControl>();
    public List<BattlePosition> battlePositions = new List<BattlePosition>();
    public List<BattlePosition> enemysBattlePositions = new List<BattlePosition>();


    public Transform scrollContent;

        public Sprite SToBSortSprite;
        public Sprite BToSSortSprite;
        public Sprite NoSortSprite;
        public List<Button> sortButton;
    //0 is name
    // 1 is force or scout. 2 is attack. 3 is defense, 4 is magic, 5 is speed, 6 is lucky
    // 7 is favorability button

    public List<int> isSort; // 0 is not sort or smallest to biggest, 1 is biggest to smallest


    public Button characterScrollContentButton;
    public Button itemScrollContentButton;


    public Button TopCancelButton;
    public Button CancelButton;

    public GameObject battlePanelCharacterInfo;

    public BattlePanelValue battlePanelValue;

    public BattlePanelCharacterInfoOld battlePanelCharacterInfoScript;
    public ItemPanelManger itemPanelManger;


    public GameObject enemyArray;


    public Button BattleButton;
    public Image ForceImage;

    public List<Sprite> ForceSprite;


    public Sprite startBattleSprite;
    public Sprite startExploreSprite;
    public Image typeIcon;


    public Button starButton;
    private bool isStar = false;

    public int FavorabilityType = 1; // 0 is normal. 1 is all. 2 is love
    public Button FavorabilityTypeButton;
    public Image FavorabilityImage;
    public GameObject FavorabilityButtonPanel;
    public List<Sprite> FavorabilityTypeSprites;
    public List<Button> FavorabilityTypeButtons;

    bool isScout;


    public GameObject feedbackPrefab;
    public PreBattleArrayControl preBattleArrayControl;
    float moveCooldown = 0.2f;
    float lastMoveTime = 0f;


    void OnEnable()
    {
        SetForceImage(1);
        ClearIsSort();
        if (typeIcon != null) SetTypeIcon();
    }

    void SetTypeIcon(){
        if (battlePanelValue.GetIsExplore()) {typeIcon.sprite =   startExploreSprite  ;}
        else {typeIcon.sprite =   startBattleSprite;}
    }


    public void SetForceImage(int type){
        if (type == 0){
            ForceImage.sprite = ForceSprite[0];
        } else {
            ForceImage.sprite =ForceSprite[1];
            if (battlePanelValue.GetIsExplore() || battlePanelValue.isInExplore) ForceImage.sprite =ForceSprite[2];
        }

       if (type == 0) {
        isScout = true;
       isSort[0] = 0;}
       else {
        isScout = false;
        isSort[0] = 0;
        }

        for (int i = 0; i < characterColumControls.Count; i++)
        {
          //  characterColumControls[i].isScout = isScout;
        }

        sortButton[0].gameObject.GetComponent<Image>().sprite = NoSortSprite;


    }


    void Start()
    {

    for (int i = 0; i < sortButton.Count; i++){
        int index = i;
        if (sortButton[i] != null) 
        {
            sortButton[i].onClick.AddListener(() => SortValue(index, isSort));
        }
    }
        if (characterScrollContentButton != null) { characterScrollContentButton.onClick.AddListener(() => characterScrollContentToggle()); }
        if (itemScrollContentButton != null) { itemScrollContentButton.onClick.AddListener(() => itemScrollContentToggle()); }

        if (TopCancelButton != null) TopCancelButton.onClick.AddListener(() => TogglePanel());
        if (CancelButton != null) CancelButton.onClick.AddListener(() => TogglePanel());

        if (BattleButton != null) BattleButton.onClick.AddListener(() => goToDifferentScence());

        if (starButton != null)
        {
            starButton.onClick.AddListener(() => SetStar());
        }


        for (int i = 0; i < FavorabilityTypeButtons.Count; i++)
        {
            int index = i;
            if (FavorabilityTypeButtons[i] != null)
            {
                FavorabilityTypeButtons[i].onClick.AddListener(() => SetFavorabilityType(index));
                FavorabilityTypeButtons[i].gameObject.GetComponent<Image>().sprite = FavorabilityTypeSprites[index];
            }
        }

        if (FavorabilityTypeButton != null)
            FavorabilityTypeButton.onClick.AddListener(() => SetFavorabilityTypeByButton());


    }



    void SetFavorabilityTypeByButton()
    {
        if ((FavorabilityType + 1) > FavorabilityTypeSprites.Count - 1) { SetFavorabilityType(0); }
        else { SetFavorabilityType(FavorabilityType + 1); };
    }

    void SetFavorabilityType(int favorabilityType)
    {
        if (this.FavorabilityType == favorabilityType) return;
        this.FavorabilityType = favorabilityType;
        FavorabilityImage.sprite = FavorabilityTypeSprites[FavorabilityType];
        UpdateCharacterDisplay();
    }




    void SetStar()
    {

        isStar = !isStar;
        string iconPath = $"MyDraw/UI/Other/";
        if (isStar)
        {
            starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Star");
        }
        else
        {
            starButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar");

        }
        UpdateCharacterDisplay();

    }

    void SortValue(int type, List<int> isSort)
    {
        if (type == 0) 
        {
            SortByField("force",(isSort[type] == 1));
        }else if (type == 1) {
            SortByField("attack", isSort[type] == 1);
        }else if (type == 2) {
            SortByField("defense", isSort[type] == 1);
        }else if (type == 3) {
            SortByField("magic", isSort[type] == 1);
        }else if (type == 4) {
            SortByField("speed", isSort[type] == 1);
        }else if (type == 5) 
        {
            SortByField("lucky", isSort[type] == 1);
        } else if (type == 6){
            SortByField("favorability", isSort[type] == 1);
        }

        isSort[type] = 1 - isSort[type];


        for (int i = 0 ; i < isSort.Count ; i++){
            if (i != type) {
            isSort[i] = 0; 
            }
        }
        
        UpdateButtonIcons(type, isSort);

    }


    void UpdateButtonIcons(int activeType, List<int> isSort){
        //NoSortSprite
        for (int i = 0; i < sortButton.Count ; i++){
            if (activeType != i) sortButton[i].gameObject.GetComponent<Image>().sprite = NoSortSprite;
            if (activeType == i){
                if (isSort[i] == 0)sortButton[i].gameObject.GetComponent<Image>().sprite = SToBSortSprite;
                if (isSort[i] == 1)sortButton[i].gameObject.GetComponent<Image>().sprite = BToSSortSprite;
            }
        }
    }


    void goToDifferentScence()
    {
        if (battlePanelValue.GetIsExplore())
        {
            goToExploreScence();
        }else
        {
            goToBattleScene();
        }
    }


    void goToExploreScence()
    {
        bool hasCharacter = false;


        for (int i = 0; i < 9; i++)
        {
            if (battlePositions[i].characterAtBattlePosition != null)
            {
                SceneTransferManager.Instance.charactersAtBattlePostions[i] = battlePositions[i].characterAtBattlePosition;
                battlePositions[i].characterAtBattlePosition.IsMoved = true;
                hasCharacter = true;
            }
            else
            {
                SceneTransferManager.Instance.charactersAtBattlePostions[i] = null;
            }
        }



        BattlePanelValue battlePanelValue = gameObject.GetComponent<BattlePanelValue>();
        SceneTransferManager.Instance.saveExploreInfo(battlePanelValue.BattleRegionValue);
        SceneTransferManager.Instance.type = 3;



        if (!hasCharacter)
        {
            NotificationManage.Instance.ShowToTop("There should be at least one character in the battle position!");
            return;
        }

        SceneManager.LoadScene("ExploreScene");
    }


    void goToBattleScene()
    {
        bool hasCharacter = false;


        for (int i = 0; i < 9; i++)
        {
            if (battlePositions[i].characterAtBattlePosition != null)  
            {
                SceneTransferManager.Instance.charactersAtBattlePostions[i] = battlePositions[i].characterAtBattlePosition;
                battlePositions[i].characterAtBattlePosition.IsMoved = true;
                hasCharacter = true;
            }
            else
            {
                SceneTransferManager.Instance.charactersAtBattlePostions[i] = null;
            }

            if (enemysBattlePositions[i].characterAtBattlePosition != null)  // Ensure there's a character in this position
            {
                SceneTransferManager.Instance.enemysAtBattlePostions[i] = enemysBattlePositions[i].characterAtBattlePosition;
            }
            else
            {
                SceneTransferManager.Instance.enemysAtBattlePostions[i] = null;
            }
        }
        BattlePanelValue battlePanelValue = gameObject.GetComponent<BattlePanelValue>();
        SceneTransferManager.Instance.saveBattleInfo(battlePanelValue.battleCity, battlePanelValue.BattleRegionValue);


        if (!hasCharacter)
        {
            NotificationManage.Instance.ShowToTop("There should be at least one character in the battle position!");
            return;  
        }


        SceneTransferManager.Instance.type = 1;


        SceneManager.LoadScene("BattleScene");
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) { ClosePanel(); }
        // if (Input.GetKeyDown(KeyCode.Return) && gameObject.activeSelf) { //Debug.Log("?"); }//goToDifferentScence(); }


        if ( Input.GetKey(KeyCode.UpArrow) && Time.time - lastMoveTime > moveCooldown)
        {
            lastMoveTime = Time.time;
            MoveTheSelectedCharaceterColumControl(1);
        }
        if ( Input.GetKey(KeyCode.DownArrow) && Time.time - lastMoveTime > moveCooldown)
        {
            lastMoveTime = Time.time;
            MoveTheSelectedCharaceterColumControl(-1);
        }


        Vector2 mousePosition = Input.mousePosition;

        bool isPointerOverButton = IsPointerOverUIElement(FavorabilityTypeButton.gameObject);
        bool isPointerOverImage = IsPointerOverUIElement(FavorabilityImage.gameObject);
        bool isPointerOverPanel = IsPointerOverUIElement(FavorabilityButtonPanel);


        if (isPointerOverButton || isPointerOverImage || isPointerOverPanel)
        {
            FavorabilityButtonPanel.SetActive(true);
        }
        else
        {
            FavorabilityButtonPanel.SetActive(false);
        }


    }

    void MoveTheSelectedCharaceterColumControl(int num)
    {

        if (battlePanelCharacterInfoScript.battleColumnControl == null)
        {
            battlePanelCharacterInfoScript.SetBattleColumnControlToBattlePanel(characterColumControls[0]);
            return;
        }

        int i = characterColumControls.IndexOf(battlePanelCharacterInfoScript.battleColumnControl);

        if (i == -1) return;

        i -= num;

        if (i < 0) { i = characterColumControls.Count - 1; }
        else if (i >= characterColumControls.Count) { i = 0; }

        while (!characterColumControls[i].gameObject.activeSelf)
        {
            i -= num;

            if (i < 0) { i = characterColumControls.Count - 1; }
            else if (i >= characterColumControls.Count) { i = 0; }
        }

        battlePanelCharacterInfoScript.SetBattleColumnControlToBattlePanel(characterColumControls[i]);
    }



    private bool IsPointerOverUIElement(GameObject gameObject)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPointerPosition);

        return rectTransform.rect.Contains(localPointerPosition);
    }


    void ClosePanel(){
        if ((preBattleArrayControl != null && !preBattleArrayControl.scoutPanel.activeSelf &&  !itemPanelManger.gameObject.activeSelf)|| preBattleArrayControl == null)
        {
            TogglePanel();
        } else if (itemPanelManger.gameObject.activeSelf){
            itemPanelManger.ClosePanel();

        }else if (preBattleArrayControl != null && preBattleArrayControl.scoutPanel.activeSelf)
        {
            preBattleArrayControl.CloseScoutPanel();
        } 

    }

    void TogglePanel()
    {
        if (!battlePanelValue.isInExplore)
        {
            foreach (var battlePosition in battlePositions)
            {
                if (battlePosition.characterAtBattlePosition != null)
                {
                   // battlePosition.characterAtBattlePosition.battlePosition = null;
                    battlePosition.characterAtBattlePosition = null;
                }
            }

            if (battlePanelCharacterInfoScript != null)
            {
                battlePanelCharacterInfoScript.characterAtBattlePanel = null;
                battlePanelCharacterInfoScript.setCharacterValueAtBattlePanel(battlePanelCharacterInfoScript.characterAtBattlePanel);
            }


            EnemyArraySet enemyArraySet = enemyArray.GetComponent<EnemyArraySet>();

            enemyArraySet.ClearPosition();
            enemyArray.SetActive(false);

            itemPanelManger.ClosePanel();
        


        }


        if (battlePanelValue.isInExplore)
        {
            bool hasCharacter = false;


            for (int i = 0; i < 9; i++)
            {

                if (battlePositions[i].characterAtBattlePosition != null)
                {
                    hasCharacter = true;
                }

            }

            if (!hasCharacter)
            {
                NotificationManage.Instance.ShowToTop("There should be at least one character in the battle position!");
                return;

            }

        }

        ClearIsSort();

        gameObject.SetActive(false);
    }

    void ClearIsSort(){
        for (int i = 0 ; i < isSort.Count ; i++){
            isSort[i] = 0;
            if ( i < sortButton.Count) sortButton[i].gameObject.GetComponent<Image>().sprite = NoSortSprite;
        }
   
    }


    void characterScrollContentToggle()
    {
        itemPanelManger.ClosePanel();

    }

    void itemScrollContentToggle()
    {
        if (itemPanelManger.gameObject.activeSelf)
        {
            itemPanelManger.ClosePanel();
        }
        else
        {
            itemPanelManger.ShowItemPanel();
        }
    }



    public void SortByField(string field, bool ascending)
    {
        field = field.ToLower();
        Func<BattleColumnControl, IComparable> keySelector = field switch
        {
            "force" => !isScout && !battlePanelValue.GetIsExplore() && !battlePanelValue.isInExplore
                ? c => c.character.Force
                : c => c.character.GetValue(2, 3), 

            "attack" => c => c.character.GetValue(0, 0),
            "defense" => c => c.character.GetValue(0, 1),
            "magic" => c => c.character.GetValue(0, 2),
            "speed" => c => c.character.GetValue(0, 3),
            "lucky" => c => c.character.GetValue(0, 4),
            "favorability" => c => c.character.Favorability,
            _ => c => c.character.Health 
        };

        if (battlePanelValue.GetIsExplore() || battlePanelValue.isInExplore)
        {
            keySelector = c => c.character.Health;
        }

        characterColumControls = ascending ?
            characterColumControls.OrderBy(keySelector).ToList() :
            characterColumControls.OrderByDescending(keySelector).ToList();

        UpdateCharacterDisplay();
    }



    // ??????
    void UpdateCharacterDisplay()
    {
        bool noSort = isSort.All(s => s == 0);

        // ??????????
        var visibleCharacters = characterColumControls
            .Where(PassesFilter)
            .ToList();

        // ??/??????
        foreach (var column in characterColumControls)
            column.gameObject.SetActive(visibleCharacters.Contains(column));

        if (noSort)
        {
            var player = GetPlayerCharacter(visibleCharacters);

            var canMoveList = visibleCharacters
                .Where(c => c.character.CanMove() && c != player)
                .ToList();

            var cannotMoveList = visibleCharacters
                .Where(c => !c.character.CanMove() && c != player)
                .ToList();

            var finalList = new List<BattleColumnControl>();

            if (player != null)
            {
                if (player.character.CanMove())
                {
                    finalList.Add(player);
                    finalList.AddRange(canMoveList);
                    finalList.AddRange(cannotMoveList);
                }
                else
                {
                    finalList.AddRange(canMoveList);
                    finalList.Add(player);
                    finalList.AddRange(cannotMoveList);
                }
            }
            else
            {
                finalList.AddRange(canMoveList);
                finalList.AddRange(cannotMoveList);
            }

            ApplyDisplayOrder(finalList);
        }
        else
        {
            // ???????????????????? canMove?
            ApplyDisplayOrder(visibleCharacters);
        }
    }


    // ?????????????????
    bool PassesFilter(BattleColumnControl column)
    {
        var c = column.character;
        bool passStar = !isStar || c.Star;
        bool passFavor = FavorabilityType == 1 || c.FavorabilityLevel == FavorabilityType;

        if (c.FavorabilityLevel == 0)
            return true;

        return passStar && passFavor;
    }

    BattleColumnControl GetPlayerCharacter(List<BattleColumnControl> list)
    {
        return list.FirstOrDefault(c => c.character.FavorabilityLevel == 0);
    }

    void ApplyDisplayOrder(List<BattleColumnControl> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].transform.SetSiblingIndex(i);
        }

        characterColumControls = list;
    }*/


}

