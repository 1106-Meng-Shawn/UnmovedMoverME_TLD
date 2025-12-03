using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;

public class CharacterClassFilter : MonoBehaviour
{
    [Header("Filter Preview")]
    public Image characterClassFilterImage;

    [Header("Button Groups")]
    public Transform normalClassButtonList;
    public Transform specialClassButtonList;
    public Transform specialOperationsList;

    private List<Button> normalClassButtons = new List<Button>();
    private List<Button> specialClassButtons = new List<Button>();
    private List<Button> specialOperationsButtons = new List<Button>();

    private Button allButton;
    private Button cancelButton;

    private Action OnFilterClick;

    #region ClassStringsList ReadOnly
    private readonly List<CharacterRole> normalClassStrings = new List<CharacterRole>()
    {
        CharacterRole.Shieldbearer,
        CharacterRole.Swordsman,
        CharacterRole.Lancer,
        CharacterRole.Archer,
        CharacterRole.Assassin,
        CharacterRole.Priest,
        CharacterRole.Magician
    };

    private readonly List<CharacterRole> specialClassStrings = new List<CharacterRole>()
    {
        CharacterRole.Commander,
        CharacterRole.Brawler,
        CharacterRole.Knight,
        CharacterRole.Cavalry,
        CharacterRole.Alchemist,
        CharacterRole.Bard,
        CharacterRole.Berserker
    };

    private readonly List<CharacterRole> specialOperationStrings = new List<CharacterRole>()
    {
        CharacterRole.All,
        CharacterRole.Special,
        CharacterRole.Commoner,
        CharacterRole.Monster,
        CharacterRole.Cance
    };
    #endregion

    private List<CharacterRole> currentFilter = new List<CharacterRole>() { CharacterRole.All };

    private void Awake()
    {
        currentFilter = new List<CharacterRole>() { CharacterRole.All };
        InitButtons();
    }

    private void InitButtons()
    {
        InitClassButtons(normalClassButtonList, normalClassButtons, normalClassStrings);
        InitClassButtons(specialClassButtonList, specialClassButtons, specialClassStrings);
        InitClassButtons(specialOperationsList, specialOperationsButtons, specialOperationStrings);
    }

    private void InitClassButtons(Transform parent, List<Button> buttonList, List<CharacterRole> roleList)
    {
        buttonList.Clear();
        buttonList.AddRange(parent.GetComponentsInChildren<Button>(true));

        for (int i = 0; i < roleList.Count && i < buttonList.Count; i++)
        {
            CharacterRole role = roleList[i];
            Button button = buttonList[i];

            // ?? All ? Cance
            if (role == CharacterRole.All) allButton = button;
            if (role == CharacterRole.Cance) cancelButton = button;

            // ??????
            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = GetRoleSprite(role);
            }

            // ????
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnClassButtonClicked(role));
        }
    }

    public void OnFilterClickListener(bool isAdd, Action callback)
    {
        if (isAdd)
        {
            OnFilterClick += callback;
        }
        else
        {
            OnFilterClick -= callback;
        }
    }


    private void OnClassButtonClicked(CharacterRole role)
    {
        if (role == CharacterRole.All)
        {
            SetCurrentFilterAll();
        }
        else if (role == CharacterRole.Cance)
        {
            SetCurrentFilterCance();
        }
        else
        {
            AddCurrentFilter(role);
        }

        UpdateButtonUI();
        SetCharacterClassFilterImage();
        OnFilterClick?.Invoke();
    }


    #region Filter Logic
    private void SetCurrentFilterAll()
    {
        if (currentFilter.Count == 1 && currentFilter.Contains(CharacterRole.All))
        {
            SetCurrentFilterCance();
            return;
        }

        currentFilter.Clear();
        currentFilter.Add(CharacterRole.All);
    }

    private void SetCurrentFilterCance()
    {
        if (currentFilter.Count == 1 && currentFilter.Contains(CharacterRole.Cance))
        {
            SetCurrentFilterAll();
            return;
        }

        currentFilter.Clear();
        currentFilter.Add(CharacterRole.Cance);
    }

    private void AddCurrentFilter(CharacterRole role)
    {
        if (currentFilter.Contains(CharacterRole.All) || currentFilter.Contains(CharacterRole.Cance))
        {
            currentFilter.Clear();
        }

        if (currentFilter.Contains(role))
        {
            currentFilter.Remove(role);
        }
        else
        {
            currentFilter.Add(role);
        }
    }
    #endregion

    #region UI Update
    private void UpdateButtonUI()
    {
        bool isAllMode = (currentFilter.Count == 1 && currentFilter.Contains(CharacterRole.All));
        bool isCanceMode = (currentFilter.Count == 1 && currentFilter.Contains(CharacterRole.Cance));

        if (isAllMode)
        {
            UpdateAllModeUI();
        }
        else if (isCanceMode)
        {
            UpdateCanceModeUI();
        }
        else
        {
            UpdateNormalModeUI();
        }
    }

    private void UpdateAllModeUI()
    {
        SetAllButtonsColor(new Color32(255, 255, 255, 255), new Color32(180, 180, 180, 255));

        if (cancelButton != null)
            SetButtonColor(cancelButton, new Color32(255, 255, 255, 180), new Color32(255, 255, 255, 255));
    }

    private void UpdateCanceModeUI()
    {
        SetAllButtonsColor(new Color32(255, 255, 255, 100), new Color32(255, 255, 255, 120));

        if (cancelButton != null)
            SetButtonColor(cancelButton, new Color32(255, 255, 255, 255), new Color32(180, 180, 180, 255));
    }

    private void UpdateNormalModeUI()
    {
        UpdateButtonList(normalClassButtons, normalClassStrings);
        UpdateButtonList(specialClassButtons, specialClassStrings);
        UpdateButtonList(specialOperationsButtons, specialOperationStrings);
    }

    private void UpdateButtonList(List<Button> buttons, List<CharacterRole> roles)
    {
        for (int i = 0; i < roles.Count && i < buttons.Count; i++)
        {
            CharacterRole role = roles[i];
            Button button = buttons[i];

            if (currentFilter.Contains(role))
            {
                SetButtonColor(button, new Color32(255, 255, 255, 255), new Color32(180, 180, 180, 255));
            }
            else
            {
                SetButtonColor(button, new Color32(255, 255, 255, 180), new Color32(255, 255, 255, 255));
            }
        }
    }

    private void SetAllButtonsColor(Color32 normal, Color32 highlight)
    {
        ApplyColorsToList(normalClassButtons, normal, highlight);
        ApplyColorsToList(specialClassButtons, normal, highlight);
        ApplyColorsToList(specialOperationsButtons, normal, highlight);
    }

    private void ApplyColorsToList(List<Button> buttons, Color32 normal, Color32 highlight)
    {
        foreach (var button in buttons)
        {
            SetButtonColor(button, normal, highlight);
        }
    }

    private void SetButtonColor(Button button, Color32 normal, Color32 highlight)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = highlight;
        colors.selectedColor = normal;
        button.colors = colors;
    }
    #endregion

    #region Filter Image
    private void SetCharacterClassFilterImage()
    {
        if (characterClassFilterImage == null) return;

        if (currentFilter.Count == 0)
        {
            characterClassFilterImage.sprite = GetRoleSprite(CharacterRole.Cance);
        }
        else if (currentFilter.Count == 1)
        {
            CharacterRole role = currentFilter[0];
            if (role == CharacterRole.All)
            {
                characterClassFilterImage.sprite = GetRoleSprite(CharacterRole.All);
            }
            else if (role == CharacterRole.Cance)
            {
                characterClassFilterImage.sprite = GetRoleSprite(CharacterRole.Cance);
            }
            else
            {
                characterClassFilterImage.sprite = GetRoleSprite(role);
            }
        }
        else
        {
            characterClassFilterImage.sprite = GetRoleSprite(CharacterRole.Custom);
        }
    }
    #endregion


    public bool PassFilter(Character character)
    {
        if (character == null) return false;

        if (currentFilter.Count == 1 && currentFilter.Contains(CharacterRole.All))
        {
            return true;
        }

        if (currentFilter.Count == 1 && currentFilter.Contains(CharacterRole.Cance))
        {
            return false;
        }
        return currentFilter.Contains(character.RoleClass);
    }

    public bool PassFilter(MultiplePlaythroughsGameCharacterRowControlData character)
    {
        if (character == null) return false;

        if (currentFilter.Count == 1 && currentFilter.Contains(CharacterRole.All))
        {
            return true;
        }

        if (currentFilter.Count == 1 && currentFilter.Contains(CharacterRole.Cance))
        {
            return false;
        }
        return currentFilter.Contains(character.RoleClass);
    }

    public bool PassFilter(CharacterExtrasSaveData character)
    {
        if (character == null) return false;

        if (currentFilter.Count == 1 && currentFilter.Contains(CharacterRole.All))
        {
            return true;
        }

        if (currentFilter.Count == 1 && currentFilter.Contains(CharacterRole.Cance))
        {
            return false;
        }
        return currentFilter.Contains(character.RoleClass);
    }



}
