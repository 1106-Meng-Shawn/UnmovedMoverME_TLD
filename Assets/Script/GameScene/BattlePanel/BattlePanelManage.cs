using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;
using System;
using static GetColor;
using static GetSprite;
using static GetString;
using static FormatNumber;
using UnityEngine.Localization.Settings;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;


public class BattlePanelManage : MonoBehaviour
{
    public GameObject BattlePanel;
    [Header("Top Buttons")]
    public TopTitle topTitle;

    [System.Serializable]
    public class TopTitle
    {
        public Image cityIcon;
        public Image countryIcon;
        public TextMeshProUGUI titleText;
    }

    public Image BackgroundImage;

    [Header("Top Row")]
    public TopRow topRow;

    [System.Serializable]
    #region TopRow
    public class TopRow
    {
        public Image itemImage;
        public Button tagButton;
        public List<Button> tagButtons;

        public Button starButton;
        public Button favorabilityButton;
        public List<Button> favorabilityButtons;
        public Image foreceType;
        public List<Button> sortButtons;
        public CharacterClassFilter characterClassFilter;
    }
    #endregion


    public CharacterInfo playerCharacterInfo;
    public CharacterInfo enemyCharacterInfo;

    public Image characterIcon;
    public Image typeIcon;
    public Image itemIcon;

    public Button battleButton;
    public Button closeButton;
    public Button cancelButton;
    public Button characterButton;
    public Button itemButton;
    public ItemPanelManager itemPanelManger;

    [Header("Battle Array")]
    public BattleArray battleArray;
    [System.Serializable]
    #region BattleArray
    public class BattleArray
    {
        public TextMeshProUGUI totalForce;

        public Button restoreButton;
        public Button scoutButton;

        public List<BattlePosition> playerPositions;

        // for scout
        public GameObject scoutPanel;
        public Button scoutBackButton;
        public Button scoutCheckButton;
        public TextMeshProUGUI scoutText;
        public List<BattlePosition> scoutPositions;

        public int scoutCurrentValue;
        public int scoutNeedValue;
        public Slider scoutSlider;
        public List<Button> scoutLevelButton;
        public List<TextMeshProUGUI> scoutLevelText;

        // for enemy
        public GameObject enemyPanel;
        public List<BattlePosition> enemyPositions;

        public Button enemyBackButton;
        public Button enemyScoutButton;
        public TextMeshProUGUI enemyForceText;

        public bool isChecked = false;
        private int _unlockScoutLevel = 0;
        public int unlockScoutLevel
        {
            get => _unlockScoutLevel;
            set
            {
                if (_unlockScoutLevel != value)
                {
                    _unlockScoutLevel = value;
                    OnUnlockScoutLevelChanged?.Invoke(_unlockScoutLevel);
                }
            }
        }
        public event Action<int> OnUnlockScoutLevelChanged;

        // private double havdScoutValue = 0f;
        private float baseScoutValue = 0; // 当前上阵角色贡献的基础侦察力
        private float maxScoutValue = 0;  // Lv3对应的最大侦察值
        private List<float> scoutLevelThresholds = new List<float>(); // [Lv1, Lv2, Lv3]
        private float usedScout = 0f;
        private float scoutBaseRandomFactor;

        public bool ClosePanel()
        {
            if (scoutPanel.activeSelf)
            {
                OnScoutBackButtonClick();
                return true;
            }
            else if (enemyPanel.activeSelf) { 
                CloseEnemyArray();
                return true;
            }

            return false;
        }

        public void InitBattleArrayButtons()
        {
            restoreButton.onClick.AddListener(OnRestoreButtonClick);
            scoutButton.onClick.AddListener(OnScoutButtonClick);

            scoutBackButton.onClick.AddListener(OnScoutBackButtonClick);
            scoutCheckButton.onClick.AddListener(OnScoutCheckButtonClick);

            enemyBackButton.onClick.AddListener(OnEnemyBackButtonClick);
            enemyScoutButton.onClick.AddListener(OnEnemyScoutButtonClick);

            scoutSlider.onValueChanged.AddListener(OnScoutSliderChanged);

            for (int i = 0; i <  scoutLevelButton.Count; i++)
            {
                int index = i;
                scoutLevelButton[i].onClick.AddListener(() => OnScoutLevelButtonClick(index));
            }


        }

        void OnScoutBackButtonClick()
        {
            ClearScoutPosistions();

            // 返还用掉的侦察点
            GameValue.Instance.GetResourceValue().Scout += usedScout;

            // 重置 slider 到当前 base 值
            if (unlockScoutLevel == 0)
                scoutSlider.SetValueWithoutNotify(0f);
            else
                scoutSlider.SetValueWithoutNotify(scoutLevelThresholds[unlockScoutLevel - 1]);

            usedScout = 0f;

            UpdateScoutText(); // 建议顺手更新一下显示
            SetScoutArrayActive(false);
        }


        // ✅ 固定保底值（不变）
        float GetBaseScoutValue()
        {
            return baseScoutValue;
        }

        // ✅ 动态角色带来的额外值
        float GetPositionScoutValue()
        {
            float value = 0f;
            foreach (var pos in scoutPositions)
            {
                if (pos.characterAtBattlePosition != null)
                    value += pos.characterAtBattlePosition.GetHelplerValue(1);
            }
            return value;
        }


        void ClearScoutPosistions()
        {
            foreach (var position in scoutPositions)
            {
                position.SetCharacterToPosition(null);
            }
        }

        void OnScoutCheckButtonClick()
        {
            float currentValue = scoutSlider.value;
            int newLevel = unlockScoutLevel;
            for (int i = scoutLevelThresholds.Count - 1; i >= 0; i--)
            {
                if (currentValue >= scoutLevelThresholds[i] && unlockScoutLevel < i + 1)
                {
                    newLevel = i + 1;
                    GameValue.Instance.GetResourceValue().Scout += currentValue - scoutLevelThresholds[i];
                    baseScoutValue = scoutLevelThresholds[i];
                    break;
                }
            }

            if (newLevel == unlockScoutLevel)
            {
                //NotificationManage.Instance.ShowToTop("You don't have enough Scout points!");
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Scout));
                return;
            }


            unlockScoutLevel = newLevel;
            usedScout = 0;  // ✅ 重置
            isChecked = true;

            foreach (var pos in scoutPositions)
            {
                if (pos.characterAtBattlePosition != null)
                    pos.characterAtBattlePosition.IsMoved = true;

                pos.SetCharacterToPosition(null);
            }

            scoutSlider.value = (baseScoutValue); // ✅ 保证 slider 正确
            UpdateScoutText();

            SetScoutArrayActive(false);
            enemyPanel.SetActive(true);
            UpEnemyForceText();
        }

        void UpEnemyForceText()
        {
            if (unlockScoutLevel < 2) return;
            int enemyForce = 0;
            int enemyMaxForce = 0;
            foreach (var position in enemyPositions)
            {
                if (position.characterAtBattlePosition != null)
                {
                    enemyForce += position.characterAtBattlePosition.Force;
                    enemyMaxForce += position.characterAtBattlePosition.MaxForce;
                }
            }
            enemyForceText.text = GetValueColorString($"{enemyForce} / {enemyMaxForce}", ValueColorType.Pop);
        }

        void OnEnemyBackButtonClick()
        {
            //enemyPanel.SetActive(false); 
            CloseEnemyArray();
        }

        void OnEnemyScoutButtonClick()
        {
            if (unlockScoutLevel < scoutLevelThresholds.Count)
            {
                SetScoutArrayActive(true);
            } else
            {
                //NotificationManage.Instance.ShowToTop("You already scouted all levels.");
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Scout_AllLevel);
            }
        }


        void OnScoutButtonClick()
        {
            if (isChecked)
            {

                enemyPanel.gameObject.SetActive(true);
            } else
            {
                ClearScoutPosistions();
                SetScoutArrayActive(true);
            }
        }

        public void SetScout()
        {
            scoutSlider.value = GetBaseScoutValue() + GetPositionScoutValue() + usedScout;

            UpdateScoutText();
        }


         void OnScoutSliderChanged(float value)
        {
            float baseValue = GetBaseScoutValue() + GetPositionScoutValue(); // 不包含 usedScout
            float minLockedValue = 0f;

            if (unlockScoutLevel > 0 && unlockScoutLevel <= scoutLevelThresholds.Count)
            {
                minLockedValue = scoutLevelThresholds[unlockScoutLevel - 1];
            }

            // 保底值不能小于角色阵位+确认基础
            if (minLockedValue < baseValue)
                minLockedValue = baseValue;

            // 拖动过低，回弹
            if (value < minLockedValue)
            {
                scoutSlider.value = (minLockedValue);
                return;
            }

            // 吸附
            value = GetSnappedScoutValue(value);

            // 玩家想要使用的新额外点数
            float newUsedScout = value - baseValue;

            // 最大值限制 = 当前可用点 + 之前已经使用的 + baseValue
            float maxAllowed = baseValue + usedScout + GameValue.Instance.GetResourceValue().Scout;

            if (value > maxAllowed)
            {
                // NotificationManage.Instance.ShowToTop("You don't have enough Scout points.");
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Scout));

                scoutSlider.value = maxAllowed;
                return;
            }

            // ⚠️ 先返还旧值，再扣新值（差值）
            float delta = newUsedScout - usedScout;
            GameValue.Instance.GetResourceValue().Scout -= delta;
            usedScout = newUsedScout;

            // 更新
            scoutSlider.value = value;
            UpdateScoutText();
        }


        float GetSnappedScoutValue(float value)
        {
            foreach (var threshold in scoutLevelThresholds)
            {
                if (Mathf.Abs(value - (float)threshold) < 3f) // 允许贴靠
                    return (float)threshold;
            }
            return value;
        }

        void UpdateScoutText()
        {
            if (scoutLevelThresholds.Count == 0) return;
            double current = scoutSlider.value;
            int level = 1;
            double levelThreshold = scoutLevelThresholds[0];

            for (int i = 0; i < scoutLevelThresholds.Count; i++)
            {
                if (current <= scoutLevelThresholds[i])
                {
                    level = i + 1;
                    levelThreshold = scoutLevelThresholds[i];
                    break;
                }
                else
                {
                    level = scoutLevelThresholds.Count;
                    levelThreshold = scoutLevelThresholds.Last();
                }
            }

            for (int i = 0; i < scoutLevelText.Count; i++)
            {
                if (scoutSlider.value < scoutLevelThresholds[i])
                {
                    scoutLevelText[i].color = GetValueColor(ValueColorType.Decrease);
                }
                else
                {
                    scoutLevelText[i].color = GetValueColor(ValueColorType.Increase);
                }
                scoutLevelText[i].text = scoutLevelThresholds[i].ToString();
            }

            if (scoutSlider.value == scoutSlider.maxValue)
            {
                current = GetBaseScoutValue() + GetPositionScoutValue() + usedScout;
            }



            Color32 colorTag = GetValueColor(current < levelThreshold ? ValueColorType.Decrease : ValueColorType.Increase);
            string colorHex = $"#{colorTag.r:X2}{colorTag.g:X2}{colorTag.b:X2}";  // 转为#RRGGBB格式
            string coloredCurrent = $"<color={colorHex}>{FormatDoubleNumberToFormatNumber(current)}</color>";
            string normalThreshold = FormatDoubleNumberToFormatNumber(levelThreshold);
            scoutText.text = $"{coloredCurrent} / Lv{level} ({normalThreshold})";
        }


        void OnScoutLevelButtonClick(int index)
        {
            if (index < 0 || index >= scoutLevelThresholds.Count)
            {
                Debug.LogWarning("Invalid scout level index.");
                return;
            }

            if (index < unlockScoutLevel-1)
            {
                // NotificationManage.Instance.ShowToTop("You have unlocked this level");
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.ScoutLevel_Unlocked_Already);
                return;
            }

            // 四舍五入以避免误差
            float RoundToFloat(float val, int digits = 3)
            {
                float factor = Mathf.Pow(10, digits);
                return Mathf.Round(val * factor) / factor;
            }

            float targetValue = RoundToFloat(scoutLevelThresholds[index]);

            float baseValue = RoundToFloat(GetBaseScoutValue() + GetPositionScoutValue());
            float currentValue = baseValue + usedScout;

            float maxAvailable = RoundToFloat(baseValue + GameValue.Instance.GetResourceValue().Scout + usedScout);
            if (targetValue > maxAvailable)
            {
                //  NotificationManage.Instance.ShowToTop("You don't have enough Scout points for this level.");
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Scout));

                scoutSlider.SetValueWithoutNotify(maxAvailable);
                float newU = maxAvailable - baseValue;
                float D = RoundToFloat(newU - usedScout);
                GameValue.Instance.GetResourceValue().Scout -= D;
                usedScout = newU;
                UpdateScoutText();

                return;
            }

            scoutSlider.SetValueWithoutNotify(targetValue);
            float newUsed = targetValue - baseValue;
            float delta = RoundToFloat(newUsed - usedScout);
            GameValue.Instance.GetResourceValue().Scout -= delta;
            usedScout = newUsed;

            UpdateScoutText();
        }

        float GetEnemyScout(int scoutLevel)
        {
            float[] scoutLevelMultipliers = { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f }; // 自定义倍率
            float multiplier = scoutLevelMultipliers[Mathf.Clamp(scoutLevel - 1, 0, scoutLevelMultipliers.Length - 1)];

            float scoutNeed = 0;
            foreach (var position in enemyPositions)
            {
                if (position.characterAtBattlePosition != null)
                    scoutNeed += position.characterAtBattlePosition.GetHelplerValue(1);
            }
            float result = scoutNeed * scoutBaseRandomFactor * multiplier;
            return Mathf.Round(result * 10f) / 10f; // 保留1位小数
        }


        void OnRestoreButtonClick()
        {
            var playerBattle = playerPositions
                .Select(pos => pos.characterAtBattlePosition)
                .Where(c => c != null)
                .ToList();

            if (playerBattle.Count == 0)
            {
                //  NotificationManage.Instance.ShowToTop("No character at the position");
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.AllPosition_NoCharacter);
                return;
            }

            var restoreList = playerBattle.Where(c => c.Force < c.MaxForce).ToList();

            if (restoreList.Count == 0)
            {
                //NotificationManage.Instance.ShowToTopByKey("All characters are at full strength!");
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Force_AllFull);

                return;
            }

            if (GameValue.Instance.GetResourceValue().TotalRecruitedPopulation <= 0)
            {
                //NotificationManage.Instance.ShowToTop("No available troops to restore!");
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource,GetResourceStringWithSprite(ValueType.TotalRecruitedPopulation));
                return;
            }

            int totalRestored = 0;
            int availableTroops = GameValue.Instance.GetResourceValue().TotalRecruitedPopulation;

            foreach (var character in restoreList)
            {
                int needed = character.MaxForce - character.Force;
                int toAdd = Mathf.Min(needed, availableTroops);

                character.Force += toAdd;
                totalRestored += toAdd;
                availableTroops -= toAdd;

                if (availableTroops <= 0)
                    break;
            }

            GameValue.Instance.GetResourceValue().TotalRecruitedPopulation = availableTroops;

            //NotificationManage.Instance.ShowToTop($"Restored total {totalRestored} Force.");
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Force_Restored,totalRestored.ToString());

            Debug.Log("i don;t know how to wirte avangen add force , shit");
        }



        public void SetEnemyPostion(string battleCountry, int num)
        {
            unlockScoutLevel = 0;
            baseScoutValue = 0;
            usedScout = 0;
            isChecked = false;
            SetScoutArrayActive(false);
            enemyForceText.text = GetValueColorString("??? ??? ???", ValueColorType.Pop);
            // Get a random list of enemy characters
            // List<Character> enemys = GameValue.Instance.GetBattleEnemys(battleCountry, num);
            // Get a random list of enemy characters
            List<Character> allEnemies = GameValue.Instance.GetCountryCharacters(battleCountry);
            Debug.Log($"may need to Ai to decide pick what characters.");
            List<Character> enemys = allEnemies.OrderBy(c => UnityEngine.Random.value).Take(num).ToList();

            // Shuffle the positions to randomize assignment
            List<BattlePosition> shuffledPositions = enemyPositions.OrderBy(p => UnityEngine.Random.value).ToList();
            Debug.Log($"may need to decide how many characters palyer have based on the number of cities.");
            Debug.Log($"Random index is {num}");
            Debug.Log($"The combat position needs to be optimized according to the class description, enemy.Count");
            // Assign each enemy character to a position
            for (int i = 0; i < shuffledPositions.Count; i++)
            {
                if (i < enemys.Count)
                {
                    // If there is a valid enemy, set it to the position
                    shuffledPositions[i].SetCharacterToPosition(enemys[i]);
                }
                else
                {
                    // Otherwise, clear or initialize the position to a default state (null)
                    shuffledPositions[i].SetCharacterToPosition(null);
                }
            }

            scoutLevelThresholds = new List<float>();
            scoutBaseRandomFactor = UnityEngine.Random.Range(0.5f, 1.5f);
            for (int i = 1; i <= 6; i++)
            {
                scoutLevelThresholds.Add(GetEnemyScout(i));
            }

            maxScoutValue = scoutLevelThresholds[scoutLevelThresholds.Count-1];
            scoutSlider.minValue = 0; // ✅ 最小值固定为 0
            scoutSlider.maxValue = (float)maxScoutValue;
            scoutSlider.wholeNumbers = false;
            scoutSlider.value = (float)baseScoutValue; // ✅ 初始值等于基础


        }

        void CloseEnemyArray()
        {
            enemyPanel.SetActive(false);
            BattlePanelManage.Instance.enemyCharacterInfo.gameObject.SetActive(false);
        }

       public void SetScoutArrayActive(bool isActive)
        {
            scoutPanel.SetActive(isActive);
            BattlePanelManage.Instance.SetCharacterRowControlsScout(isActive);
        }


        public void SetExploreCharacter(List<Character> exploreCharacters)
        {
            for (int i = 0; i < exploreCharacters.Count; i++)
            {
                playerPositions[i].SetCharacterToPosition(exploreCharacters[i]);
            }
        }



    }
    #endregion


    public Transform scrollContent;

    public GameObject BattleRowPrefab;
    public BattleColumnControl selBattleColumnControl;
    private List<BattleColumnControl> battleRowControls = new List<BattleColumnControl>();
    private FavorabilityLevel favorabilityType = FavorabilityLevel.Self; // 0 is self, 1 is normal, 2 is love
    private bool isStar = false;
    private int tagType = 0;

    private Character character;

   List<SortField> sortFieldMapping = new List<SortField>();

    public enum SortField
    {
        Name, Favorability, Force, Health, Scout, Attack, Defense, Magic, Speed, Lucky,None
    }

    public enum SortDirection
    {
        None, Ascending, Descending
    }

    public struct SortStatus
    {
        public SortField Field;
        public SortDirection Direction;

        public bool IsSorted => Field != SortField.None && Direction != SortDirection.None;
    }

    private SortStatus currentSort;


    public RegionValue battleRegion;
    public int battleCityIndex;


    public bool isExplore;
    public bool isScout;
    public bool isInExplore;

    float moveCooldown = 0.2f;
    float lastMoveTime = 0f;

    public static BattlePanelManage Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        ClearBattleArray();
        GameValue.Instance.UnRegisterItemsChange(HasCharacter);
        GameValue.Instance.UnRegisterItemsChange(HasItem);
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        BattlePanelManage.Instance.battleArray.OnUnlockScoutLevelChanged -= OnScoutLevelChanged;
        topRow.characterClassFilter.OnFilterClickListener(false, DisplayerCharacters);

    }

    private void Start()
    {
        GameValue.Instance.RegisterItemsChange(HasCharacter);
        GameValue.Instance.RegisterItemsChange(HasItem);
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        BattlePanelManage.Instance.battleArray.OnUnlockScoutLevelChanged += OnScoutLevelChanged;


        InitOperateButtons();
        InitTopRowButtons();
        //InitCharacterPanel();
        battleArray.InitBattleArrayButtons();

        foreach (var pos in battleArray.playerPositions)
        {
            pos.onCharacterChanged.AddListener(() => SetForce());
        }

        foreach (var pos in battleArray.scoutPositions)
        {
            pos.onCharacterChanged.AddListener(() => battleArray.SetScout());
            pos.onCharacterChanged.AddListener(() => SetForce());

        }

        InitBattlePanelButtons();

    }

    void InitBattlePanelButtons()
    {
        itemButton.onClick.AddListener(OnItemButtonClick);
        characterButton.onClick.AddListener(OnCharacterButtonClick);

    }

    void OnItemButtonClick()
    {
        if (itemPanelManger.IsActive())
        {
            itemPanelManger.ClosePanel();
        } else
        {
            itemPanelManger.OpenPanel();

        }
    }

    void OnCharacterButtonClick()
    {
        itemPanelManger.ClosePanel();
    }



    void InitOperateButtons()
    {
        if (closeButton != null) closeButton.onClick.AddListener(() => ClosePanel());
        if (cancelButton != null) cancelButton.onClick.AddListener(() => ClosePanel());
        if (battleButton != null) battleButton.onClick.AddListener(() => GoToDifferentScence());

    }
    void GoToDifferentScence()
    {
        if (isInExplore)
        {
            ClosePanel();
            return;
        }



        if (isExplore)
        {
            GoToExploreScence();
        }
        else
        {
            GoToBattleScene();
        }

    }

    void GoToExploreScence()
    {

        bool hasCharacter = false;
        List<Character> playerBattle = new List<Character>();
        for (int i = 0; i < 9; i++)
        {
            if (battleArray.playerPositions[i].characterAtBattlePosition != null)
            {
                hasCharacter = true;
            }
            playerBattle.Add(battleArray.playerPositions[i].characterAtBattlePosition);
        }


        if (hasCharacter)
        {
            ExploreData exploreData = new ExploreData(playerBattle,battleRegion,battleCityIndex);
            GameValue.Instance.SetExploreData(exploreData);
            LoadPanelManage.Instance.AutoSaveGameExploreBegin();
            SceneTransferManager.Instance.LoadScene(Scene.ExploreScene);
        }
        else
        {
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Position_AtLeastOneCharacter);

        }
    }


    void GoToBattleScene()
    {
        bool hasCharacter = false;
        List<Character> playerBattle = new List<Character>();
        List<Character> enemyBattle = new List<Character>();

        for (int i = 0; i < 9; i++)
        {
            if (battleArray.playerPositions[i].characterAtBattlePosition != null)
            {
                hasCharacter = true;
            }
            playerBattle.Add(battleArray.playerPositions[i].characterAtBattlePosition);
            enemyBattle.Add(battleArray.enemyPositions[i].characterAtBattlePosition);

        }
        if (hasCharacter)
        {

            BattleData battleData = new BattleData(playerBattle, enemyBattle, battleRegion,battleCityIndex);
            GameValue.Instance.SetBattleData(battleData);

            foreach (var player in playerBattle)
            { 
                if (player != null)
                {
                    player.SetBattlePositionToCharacter(null);
                    player.IsMoved = true;
                }
            }

            foreach (var enemy in enemyBattle)
            {
                if (enemy != null)
                {
                    enemy.SetBattlePositionToCharacter(null);
                }
            }
            LoadPanelManage.Instance.AutoSaveGameBattleBegin();
            SceneTransferManager.Instance.LoadScene(Scene.BattleScene);
        }
        else
        {
            NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Position_AtLeastOneCharacter);
        }

    }


    void ClosePanel()
    {
        if (itemPanelManger.IsActive())
        {
            itemPanelManger.ClosePanel();
            return;
        }

        if (battleArray.ClosePanel()) return;

        if (isInExplore)
        {
            bool hasCharacter = false;
            List<Character> ExploreBattleCharacterAtPositions = new List<Character>();

            foreach (var position in battleArray.playerPositions)
            {
                if (position.characterAtBattlePosition != null) hasCharacter = true;
                ExploreBattleCharacterAtPositions.Add(position.characterAtBattlePosition);
            }
            if (!hasCharacter)
            {
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.Position_AtLeastOneCharacter);
                return;
            }
            GameValue.Instance.GetExploreData().playerExploreBattleCharacters = ExploreBattleCharacterAtPositions;
        }


        ClearBattleArray();
        BattlePanel?.gameObject?.SetActive(false);
    }

    

    void ClearBattleArray()
    {
        if (!isInExplore)
        {
            foreach (var battlePosition in battleArray.playerPositions)
            {
                battlePosition.SetCharacterToPosition(null);
            }
            foreach (var battlePosition in battleArray.enemyPositions)
            {
                battlePosition.SetCharacterToPosition(null);
            }

        }

    }

    void InitTopRowButtons()
    {
        InitSortButtons();
        InitStarButton();
        InitTagButton();
        InitFavorabilityButton();
        topRow.characterClassFilter.OnFilterClickListener(true,DisplayerCharacters);
    }

    void InitTagButton()
    {
        topRow.tagButton.onClick.AddListener(OnTagButtonClick);
        for (int i = 0; i < topRow.tagButtons.Count; i++)
        {
            int index = i;
            topRow.tagButtons[i].onClick.AddListener(() => OnTagButtonsClick(index));
        }

    }

    void OnTagButtonClick()
    {
        int index = (tagType + 1) % topRow.tagButtons.Count;
        SetTagType(index);

    }

    void OnTagButtonsClick(int index)
    {
        SetTagType(index);
    }

    void SetTagType(int tag)
    {
        tagType = tag;
        topRow.tagButton.image.sprite = GetCharacterTag((CharacterTag)tag);
        DisplayerCharacters();
    }

    void InitFavorabilityButton()
    {
        topRow.favorabilityButton.onClick.AddListener(OnFavorabilityButtonClick);
        for (int i = 0;i < 3; i++)
        {
            int index = i;
            topRow.favorabilityButtons[i].onClick.AddListener(()=> OnFavorabilityButtonClick((FavorabilityLevel)index));
        }
    }

   

    void OnFavorabilityButtonClick()
    {
        OnFavorabilityButtonClick(EnumHelper.GetEnumByOffset<FavorabilityLevel>(favorabilityType,1));
    }

    void OnFavorabilityButtonClick(FavorabilityLevel index)
    {
        SetFavorabilityType(index);
        DisplayerCharacters();
    }

    void SetFavorabilityType(FavorabilityLevel index)
    {
        favorabilityType = index;
        topRow.favorabilityButton.gameObject.GetComponent<Image>().sprite = GetFavorabilitySprite(favorabilityType);

    }

    void Update()
    {
        if (BattlePanel.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(1)) { ClosePanel(); }

            if (Input.GetKey(KeyCode.UpArrow) && Time.time - lastMoveTime > moveCooldown)
            {
                lastMoveTime = Time.time;
                MoveSelCharaceterRowControl(1);
            }
            if (Input.GetKey(KeyCode.DownArrow) && Time.time - lastMoveTime > moveCooldown)
            {
                lastMoveTime = Time.time;
                MoveSelCharaceterRowControl(-1);
            }
        }

    }



    bool IsMouseOverUI(RectTransform rect)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            rect,
            Input.mousePosition,
            null 
        );
    }


    private void InitStarButton()
    {
      //  SetStarButtonSprite();
        topRow.starButton.onClick.AddListener(OnStarButtonClick);
    }

    void OnStarButtonClick()
    {
        SetStarButton(!isStar);
        DisplayerCharacters(); 
    }

    void SetStarButton(bool star)
    {
        isStar = star;
        topRow.starButton.image.sprite = UpStarButtonSprite(isStar);
    }



    #region SortButton
    void InitSortButtons()
    {
        topRow.sortButtons[0].onClick.AddListener(() => OnSortButtonClicked(SortField.Name));
        topRow.sortButtons[1].onClick.AddListener(() => OnSortButtonClicked(SortField.Favorability));

        //SortField dynamicField = GetForceTypeSortField();
        topRow.sortButtons[2].onClick.AddListener(() => OnSortButtonClicked(GetForceTypeSortField()));

        topRow.sortButtons[3].onClick.AddListener(() => OnSortButtonClicked(SortField.Attack));
        topRow.sortButtons[4].onClick.AddListener(() => OnSortButtonClicked(SortField.Defense));
        topRow.sortButtons[5].onClick.AddListener(() => OnSortButtonClicked(SortField.Magic));
        topRow.sortButtons[6].onClick.AddListener(() => OnSortButtonClicked(SortField.Speed));
        topRow.sortButtons[7].onClick.AddListener(() => OnSortButtonClicked(SortField.Lucky));

        // 更新字段映射

         sortFieldMapping = new List<SortField>
           {
              SortField.Name,
              SortField.Favorability,
              GetForceTypeSortField(),  // index 2
              SortField.Attack,
              SortField.Defense,
              SortField.Magic,
              SortField.Speed,
              SortField.Lucky
          };


    }

    SortField GetForceTypeSortField()
    {
        if (isScout) {
            return SortField.Scout;
        }
        
        if (isExplore || isInExplore)
        {
            return SortField.Health;
        } else
        {
            return SortField.Force;
        }
        
        
    }


    void OnSortButtonClicked(SortField field)
    {
        if (currentSort.Field != field)
        {
            if (field == SortField.Name)
            {
                ApplySort(field, SortDirection.Ascending);
            }
            else
            {
                ApplySort(field, SortDirection.Descending);
            }
        }
        else
        {
            // ???????????????????
            if (field == SortField.Name)
            {
                switch (currentSort.Direction)
                {
                    case SortDirection.Ascending:
                        ApplySort(field, SortDirection.Descending); // ??->??
                        break;
                    case SortDirection.Descending:
                        ApplySort(SortField.None, SortDirection.None); // ??->??
                        break;
                    default:
                        ApplySort(field, SortDirection.Ascending); // ??->??
                        break;
                }
            }
            else
            {
                switch (currentSort.Direction)
                {
                    case SortDirection.Descending:
                        ApplySort(field, SortDirection.Ascending); // ??->??
                        break;
                    case SortDirection.Ascending:
                        ApplySort(SortField.None, SortDirection.None); // ??->??
                        break;
                    default:
                        ApplySort(field, SortDirection.Descending); // ??->??
                        break;
                }
            }
        }
    }
    void UpdateSortIcons()
    {
        for (int i = 0; i < topRow.sortButtons.Count; i++)
        {
            SortField field = sortFieldMapping[i];  // ✅ 用映射表而不是枚举序号
            Image icon = topRow.sortButtons[i].GetComponent<Image>();

            if (currentSort.Field == field && currentSort.Direction != SortDirection.None)
            {
                if (field == SortField.Name)
                {
                    if (currentSort.Direction == SortDirection.Ascending)
                    {
                        icon.sprite = GetSortSprite(SortDirection.Descending.ToString());
                    }
                    else if (currentSort.Direction == SortDirection.Descending)
                    {
                        icon.sprite = GetSortSprite(SortDirection.Ascending.ToString());
                    }
                }
                else
                {
                    icon.sprite = GetSortSprite(currentSort.Direction.ToString());
                }
            }
            else
            {
                icon.sprite = GetSortSprite(SortDirection.None.ToString());
            }
        }
    }
    void OnLocaleChanged(UnityEngine.Localization.Locale newLocale)
    {
        TitleSet();
        if (currentSort.Field == SortField.Name && currentSort.Direction != SortDirection.None)
        {
            ApplySort(currentSort.Field, currentSort.Direction);
        }
      //  characterPanel.UpCharacterUI();

    }
    #endregion


    public void ShowBattlePanel(RegionValue regionValue,int city,bool explore)
    {
        BattlePanel.SetActive(true);
        battleRegion = regionValue;
        battleCityIndex = city;
        isExplore = explore;
        SetBattleColumnControlToBattlePanel(null);
        InitPanel();
        GenerateCharacterRows();
        DisplayerCharacters();
        TitleSet();
        InitBattleArray();
        itemPanelManger.ClosePanel();
        SetBackgroundMural();

        BottomButton bottomButton = BottomButton.Instance;
        if (bottomButton != null) {
            bottomButton.CloseAllPanel();
        }
    }


    void SetBackgroundMural()
    {
        if (isExplore || isInExplore)
        {
            BackgroundImage.sprite = GetMuralSprite("OriginalSin");
        } else
        {
            BackgroundImage.sprite = GetMuralSprite("ExpulsionEden");
        }
    }

    void InitBattleArray()
    {
        if (isInExplore)
        {
            battleArray.SetExploreCharacter(GameValue.Instance.GetExploreData().playerExploreBattleCharacters);
        } else
        {
            battleArray.SetScoutArrayActive(false);
            ClearBattleArray();
            if (isExplore)
            {
                enemyCharacterInfo.gameObject.SetActive(false);
                return;
            }
            SetForce();
            int num = UnityEngine.Random.Range(1, 10); // 结果是 1~9 之间的整数
            battleArray.SetEnemyPostion(battleRegion.GetCityCountry(battleCityIndex), num);
            enemyCharacterInfo.gameObject.SetActive(false);
        }
    }


    void InitPanel()
    {
        SetFavorabilityType(0);
        SetStarButton(false);
        SetTagType(0);
      //  SetStarButtonSprite();
        currentSort = new SortStatus { Field = SortField.None, Direction = SortDirection.None };

        if (isExplore)
        {
            topRow.foreceType.sprite = GetBattleForceType("health");
        } else
        {
            topRow.foreceType.sprite = GetBattleForceType("force");

        }


        HasItem();
        HasCharacter();
        SetTypeImage();
        SetBattleColumnControlToBattlePanel(null);
    }

    void SetTypeImage()
    {
        string iconPath = "MyDraw/UI/Other/";
        if (isExplore)
        {
            typeIcon.sprite = Resources.Load<Sprite>(iconPath + "StartExplore");
            battleButton.GetComponentInChildren<LocalizeStringEvent>().StringReference = new LocalizedString { TableReference = "GameSetting", TableEntryReference = "Explore" };

        }
        else
        {
            typeIcon.sprite = Resources.Load<Sprite>(iconPath + "StartBattle");
            battleButton.GetComponentInChildren<LocalizeStringEvent>().StringReference = new LocalizedString { TableReference = "GameSetting", TableEntryReference = "Battle" };

        }
    }


    void GenerateCharacterRows()
    {
        // ????
        foreach (var control in battleRowControls)
            if (control != null)
                Destroy(control.gameObject);
        battleRowControls.Clear();


        if (isInExplore)
        {
            List<Character> allCharacters = GameValue.Instance.GetExploreData().playerExploreCharacters;
            string cityCountry = battleRegion.GetCityCountry(battleCityIndex);

            foreach (var character in allCharacters)
            {
                if (character == null) continue;
                GameObject go = Instantiate(BattleRowPrefab, scrollContent);
                BattleColumnControl control = go.GetComponent<BattleColumnControl>();
                control.Init(character);
                control.character = character;
                battleRowControls.Add(control);
            }

        }
        else
        {
            List<Character> allCharacters = GameValue.Instance.GetPlayerCharacters();
            string cityCountry = battleRegion.GetCityCountry(battleCityIndex);

            foreach (var character in allCharacters)
            {
                if (character == null) continue;
                GameObject go = Instantiate(BattleRowPrefab, scrollContent);
                BattleColumnControl control = go.GetComponent<BattleColumnControl>();
                control.Init(character);
                control.character = character;
                battleRowControls.Add(control);
            }

        }
    }

    // ????????
    bool PassesFilter(BattleColumnControl column)
    {
        var c = column.character;
        bool passStar = !isStar || c.Star;
        bool passFavor = favorabilityType == 0 || c.FavorabilityLevel == favorabilityType || c.FavorabilityLevel == 0;
        bool passTag = tagType == 0 || c.Tag == (CharacterTag)tagType;
        if ((CharacterTag)tagType == CharacterTag.Lord && c.HasLord()) passTag = true;
        bool passClass = topRow.characterClassFilter.PassFilter(column.character);

        bool result = passStar && passFavor && passTag && passClass;
        return result;
    }

    // ?????favorabilityLevel == 0?
    BattleColumnControl GetPlayerCharacter(List<BattleColumnControl> list)
    {
        return list.FirstOrDefault(c => c.character.FavorabilityLevel == 0);
    }

    // ???????????????
    Func<BattleColumnControl, IComparable> GetKeySelector(SortField field)
    {
        return field switch
        {
            SortField.Name => c => c.character.GetCharacterName(),
            SortField.Favorability => c =>
            {
                bool isLevelZero = c.character.FavorabilityLevel == 0;

                if (currentSort.Direction == SortDirection.Ascending)
                    return isLevelZero ? int.MinValue : c.character.Favorability;
                else
                    return isLevelZero ? int.MaxValue : c.character.Favorability;
            },
            /*  SortField.Force => (!isScout && !isExplore && !isInExplore)
                  ? c => c.character.Force
                  : c => c.character.GetValue(2, 3),*/
            SortField.Force => c =>
                (c.character.IsPersonBattle)
                    ? c.character.Health
                    : c.character.Force,
            SortField.Health => c => c.character.Health,
            SortField.Scout => c => c.character.GetHelplerValue(1),
            SortField.Attack => c => c.character.GetValue(0, 0),
            SortField.Defense => c => c.character.GetValue(0, 1),
            SortField.Magic => c => c.character.GetValue(0, 2),
            SortField.Speed => c => c.character.GetValue(0, 3),
            SortField.Lucky => c => c.character.GetValue(0, 4),
            _ => c => -c.character.GetCharacterID()
        };
    }

    // ???????????
    void DisplayerCharacters()
    {

        var visibleCharacters = battleRowControls.Where(PassesFilter).ToList();


        foreach (var column in battleRowControls)
            column.gameObject.SetActive(visibleCharacters.Contains(column));

        var player = GetPlayerCharacter(visibleCharacters);

        // ???????????????????????
        if (!currentSort.IsSorted)
        {
            var canMoveList = visibleCharacters.Where(c => c.character.CanMove() && c != player).ToList();
            var cannotMoveList = visibleCharacters.Where(c => !c.character.CanMove() && c != player).ToList();

            // ??????????????????
            canMoveList = canMoveList.OrderBy(c => c.character.Force).ToList();
            cannotMoveList = cannotMoveList.OrderByDescending(c => c.character.Force).ToList();

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
            // ????????????????????????
            var keySelector = GetKeySelector(currentSort.Field);

            if (currentSort.Direction == SortDirection.Ascending)
            {
                visibleCharacters = visibleCharacters.OrderBy(keySelector).ToList();
            }
            else
            {
                visibleCharacters = visibleCharacters.OrderByDescending(keySelector).ToList();
            }

            ApplyDisplayOrder(visibleCharacters);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent.GetComponent<RectTransform>());

    }

    void ApplyDisplayOrder(List<BattleColumnControl> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].transform.SetSiblingIndex(i);
        }
        // battleRowControls = list;

    }

    void ApplySort(SortField field, SortDirection direction)
    {
        currentSort = new SortStatus { Field = field, Direction = direction };

      /*  if (field == SortField.None || direction == SortDirection.None)
        {
            DisplayerCharacters();
            UpdateSortIcons();
            return;
        }*/

        var keySelector = GetKeySelector(field);

        // ??????
        battleRowControls = direction == SortDirection.Ascending
            ? battleRowControls.OrderBy(keySelector).ToList()
            : battleRowControls.OrderByDescending(keySelector).ToList();

        DisplayerCharacters();
        UpdateSortIcons();
    }

    void TitleSet()
    {
        // city country icon
        if (!BattlePanel.activeSelf) { return; }
        topTitle.cityIcon.sprite = GetCitySprite(battleCityIndex);


        topTitle.countryIcon.sprite = battleRegion.GetCityIcon(battleCityIndex);
        if (isExplore)
        {
            Color32 cityColor = GameValue.Instance.GetCountryColor(battleRegion.GetCityCountry(battleCityIndex));
            string cityHex = ColorUtility.ToHtmlStringRGB(cityColor);
            //    topTitle.titleText.text = $"<color=#{cityHex}>{battleRegion.GetCityName(battleCityIndex).ToUpper()}</color> {battleRegion.GetCityValue(battleCityIndex).GetExploreLevel()}";
            topTitle.titleText.text = $"<color=#{cityHex}>{battleRegion.GetCityName(battleCityIndex)}</color> {battleRegion.GetCityValue(battleCityIndex).GetExploreLevel()}";
        }
        else
        {
            Color32 regionColor = GameValue.Instance.GetCountryColor(battleRegion.GetCountryName());
            Color32 cityColor = GameValue.Instance.GetCountryColor(battleRegion.GetCityCountry(battleCityIndex));

            string regionHex = ColorUtility.ToHtmlStringRGB(regionColor);
            string cityHex = ColorUtility.ToHtmlStringRGB(cityColor);

            topTitle.titleText.text =$"<color=#{cityHex}>{battleRegion.GetCityName(battleCityIndex)}</color> / <color=#{regionHex}><b>{battleRegion.GetRegionName().ToUpper()}</b></color>";

        }
    }

    void HasItem()
    {
        string iconPath = "MyDraw/Item";
        if (GameValue.Instance.HasItem(ItemConstants.ReichsapfelID))
        {
            ItemBase item = GameValue.Instance.GetItem(7);
            itemIcon.sprite = item.icon;
            topRow.itemImage.sprite = item.icon;
        }
        else
        {
            itemIcon.sprite = Resources.Load<Sprite>(iconPath + "/EmptyItemClose");
            topRow.itemImage.sprite = itemIcon.sprite;
        }
    }

    void HasCharacter()
    {
        string iconPath = "MyDraw/Item";
        if (GameValue.Instance.HasItem(ItemConstants.ZeremonienschwertID))
        {
            ItemBase item = GameValue.Instance.GetItem(ItemConstants.ZeremonienschwertID);
            characterIcon.sprite = item.icon;

        }
        else
        {
            characterIcon.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "/3/CommandSword");

        }
    }

    void OnScoutLevelChanged(int newLevel)
    {
        enemyCharacterInfo.UnlockHideOject();
        foreach (var position in battleArray.enemyPositions)
        {
            position.SetHideObject(newLevel); 
        }
    }


    public void SetBattleColumnControlToBattlePanel(BattleColumnControl battleColumnControl)
    {
        if (this.selBattleColumnControl != null) selBattleColumnControl.RestoreColor();
        if (selBattleColumnControl != battleColumnControl)
        {
            selBattleColumnControl = battleColumnControl;
            Character character = battleColumnControl?.character;
            SetCharacter(character);
        } else
        {
            selBattleColumnControl = null;
            SetCharacter(null);

        }
    }


    public void SetCharacterToPanel(Character character,bool isEnemy = false)
    {
        if (character == null) return;

        enemyCharacterInfo.gameObject.SetActive(false);
        BattleColumnControl match = battleRowControls
            .FirstOrDefault(c => c.character == character);

        if (match != null)
        {
            SetBattleColumnControlToBattlePanel(match);
        }
        else
        {
            SetBattleColumnControlToBattlePanel(null);
        }

        if (isEnemy)
        {
            SetEnemyCharacterInfo(character);
        }

        //  SetCharacter(character);
    }

    void SetEnemyCharacterInfo(Character character)
    {
        this.character = character;
        enemyCharacterInfo.gameObject.SetActive(true);
        enemyCharacterInfo.SetCharacter(character);

    }

    public void SetCharacterRowControlsScout(bool isActive)
    {
        if (isActive)
        {
            topRow.foreceType.sprite = GetValueSprite("scout");
        } else
        {
            topRow.foreceType.sprite = GetBattleForceType("force");
        }
        foreach (var row in battleRowControls)
            {
                row.SetIsScout(isActive);
            }

        isScout = isActive;

        SortField dynamicField = GetForceTypeSortField();

        sortFieldMapping = new List<SortField>
         {
            SortField.Name,
            SortField.Favorability,
            dynamicField,  // index 2
            SortField.Attack,
            SortField.Defense,
            SortField.Magic,
            SortField.Speed,
            SortField.Lucky
        };


        UpdateSortIcons();

    }



    void MoveSelCharaceterRowControl(int num)
    {
        int childCount = scrollContent.childCount;
        if (childCount == 0) return;

        // ???? active ? BattleColumnControl
        List<BattleColumnControl> visibleControls = new();
        for (int j = 0; j < childCount; j++)
        {
            var ctrl = scrollContent.GetChild(j).GetComponent<BattleColumnControl>();
            if (ctrl != null && ctrl.gameObject.activeSelf)
            {
                visibleControls.Add(ctrl);
            }
        }

        // ???????????
        if (visibleControls.Count == 0) return;

        // ???????????
        if (visibleControls.Count == 1)
        {
            if (selBattleColumnControl == null)
                SetBattleColumnControlToBattlePanel(visibleControls[0]);
            SoundManager.Instance.PlaySelSFX();
            return;
        }

        // ?????????????? -1?
        int currentIndex = visibleControls.IndexOf(selBattleColumnControl);

        // ???????????
        if (currentIndex == -1)
        {
            SoundManager.Instance.PlaySelSFX();
            SetBattleColumnControlToBattlePanel(visibleControls[0]);
            return;
        }

        // ??????????????
        int targetIndex = currentIndex;
        do
        {
            targetIndex = (targetIndex - num + visibleControls.Count) % visibleControls.Count;

            if (targetIndex != currentIndex)
            {
                SoundManager.Instance.PlaySelSFX();
                SetBattleColumnControlToBattlePanel(visibleControls[targetIndex]);
                return;
            }

        } while (targetIndex != currentIndex);
    }


    public void RefreshHighlightStatus()
    {
        foreach (var row in battleRowControls)
        {
            if (row.character == character)
                row.Highlight(true);
            else
                row.Highlight(false);
        }

        foreach (var position in battleArray.playerPositions)
        {
            if (position.characterAtBattlePosition == character && position.characterAtBattlePosition != null)
                position.Highlight(true);
            else
                position.Highlight(false);
        }
    }


    void SetCharacter(Character character)
    {
        if (character == null || this.character == character) {
            this.character = null;
        }
        else
        {
            this.character = character;
        }
        RefreshHighlightStatus();
        playerCharacterInfo.SetCharacter(character);
    }




    void SetForce()
    {
        int totalForce = 0;
        int totalMaxForce = 0;
        foreach (var position in battleArray.playerPositions)
        {
            if (position.characterAtBattlePosition != null)
            {
                if (Instance.IsPersonBattleInBattlePanel() || position.characterAtBattlePosition.IsPersonBattle)
                {
                    totalForce += position.characterAtBattlePosition.Health;
                    totalMaxForce += position.characterAtBattlePosition.GetMaxHealth();

                }
                else
                {
                    totalForce += position.characterAtBattlePosition.Force;
                    totalMaxForce += position.characterAtBattlePosition.MaxForce;

                }
            }
        }
        string FormatNumber(int value)
        {
            return value < 10000 ? value.ToString() : value.ToString("N0"); // ???
        }


        battleArray.totalForce.text = GetValueColorString($"{FormatNumber(totalForce)} / {FormatNumber(totalMaxForce)}", ValueColorType.Pop);
    }

    public bool IsPersonBattleInBattlePanel()
    {
        return isExplore || isInExplore;
    }


}
