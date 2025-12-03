using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;
using static Unity.Burst.Intrinsics.X86.Sse4_2;

public enum FavorabilityLevel
{
    Self, Normal, Romance
}

public class FavorabilityFilter : MonoBehaviour
{
    public Button FavorabilityChangeButton;

    public Button SelfButton;
    public Button NormalFavorabilityButton;
    public Button RomanceFavorabilityButton;
    public Button SortFavorabilityButton;


    private FavorabilityLevel currentFavorabilityLevel;
    private Action OnFilterClick;
    private Action OnSortClick;


    private void Awake()
    {
        InitButtons();
    }


    void InitButtons()
    {
        SelfButton.onClick.AddListener(() => OnFilterButtonClick(FavorabilityLevel.Self));
        NormalFavorabilityButton.onClick.AddListener(() => OnFilterButtonClick(FavorabilityLevel.Normal));
        RomanceFavorabilityButton.onClick.AddListener(() => OnFilterButtonClick(FavorabilityLevel.Romance));

    }

    void OnFilterButtonClick(FavorabilityLevel newFavorabilityLevel)
    {
        SetCurrentFavorabilityLevel(newFavorabilityLevel);
        SetFilterImage();
        OnFilterClick?.Invoke();
    }


    void SetCurrentFavorabilityLevel(FavorabilityLevel newFavorabilityLevel)
    {
        currentFavorabilityLevel = newFavorabilityLevel;
    }

    void SetFilterImage()
    {
        FavorabilityChangeButton.image.sprite = GetFavorabilitySprite(currentFavorabilityLevel);
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


    public void OnSortClickListener(bool isAdd,Action callback)
    {
        if (isAdd)
        {
            OnSortClick += callback;
        } else
        {
            OnSortClick -= callback;
        }
    }


    public bool PassFilter(Character character)
    {
        if (character == null) return false;
        if (currentFavorabilityLevel == FavorabilityLevel.Self) return true;
        if (character.FavorabilityLevel == FavorabilityLevel.Self) return true;
        return character.FavorabilityLevel == currentFavorabilityLevel;
    }

    public bool PassFilter(CharacterExtrasSaveData character)
    {
        if (character == null) return false;
        if (currentFavorabilityLevel == FavorabilityLevel.Self) return true;
        if (character.FavorabilityLevel == FavorabilityLevel.Self) return true;
        return character.FavorabilityLevel == currentFavorabilityLevel;
    }



    public void SetSortButton(SortStatus sortStatus, Action callback)
    {
        SortFavorabilityButton.onClick.AddListener(() => OnSortButtonClick(sortStatus));
        OnSortClickListener(true, callback);
    }


    public void OnSortButtonClick(SortStatus sortStatus)
    {
        sortStatus.ToggleSort(SortField.Favorability);
        OnSortClick?.Invoke();
    }


}
