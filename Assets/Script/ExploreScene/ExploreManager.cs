using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ExploreManager : MonoBehaviour
{
    public static ExploreManager Instance { get; private set; }
    public Button exploreButton;
    private ExploreData exploreData;
    private int currentExploreCharacterIndex = 0;


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
        InitializeExploreData();
        SetupExploreButton();
    }

    private void InitializeExploreData()
    {
        exploreData = GameValue.Instance.GetExploreData();

        if (exploreData == null)
        {
            Debug.LogError("ExploreData is null.");
            return;
        }

        if (exploreData.stillExplore)
        {
            // exploreCardTree.SetExploreCardTree(exploreData);
           // GenerateTree();
        }
        else
        {
            exploreData.stillExplore = true;
            GenerateTree();
        }
    }

    public void SetExploreCardData(ExploreCardData exploreCardData)
    {
        ExploreUI.Instance.SetExploreCardData(exploreCardData);
    }


    private void SetupExploreButton()
    {
        if (exploreButton != null)
        {
            exploreButton.onClick.AddListener(CycleExploreCharacter);
            ShowExploreCharacter(0);
        }
        else
        {
            Debug.LogWarning("ExploreButton is not assigned.");
        }
    }

    private void CycleExploreCharacter()
    {
        List<Character> characters = exploreData?.playerExploreCharacters;

        if (characters == null || characters.Count == 0)
        {
            Debug.LogWarning("No available explore characters.");
            return;
        }

        currentExploreCharacterIndex = (currentExploreCharacterIndex + 1) % characters.Count;
        ShowExploreCharacter(currentExploreCharacterIndex);
    }

    private void ShowExploreCharacter(int index)
    {
        List<Character> characters = exploreData?.playerExploreCharacters;
        if (characters == null || characters.Count == 0)
            return;

        if (index < 0 || index >= characters.Count)
        {
            Debug.LogWarning("Index out of range.");
            return;
        }

        Character character = characters[index];
        if (character != null && character.icon != null)
        {
            exploreButton.image.sprite = character.icon;
        }
        else
        {
            Debug.LogWarning("Character or icon is null at index " + index);
        }
    }

    // Update is called once per frame
    private void GenerateTree()
    {
        int InitLevel = GameValue.Instance.GetExploreData().exploreLevel;
        ExploreUI.Instance.GenerateTree(InitLevel);

    }

}

public class ExploreData
{
    public List<Character> playerExploreCharacters = new List<Character>();
    public List<Character> playerExploreBattleCharacters = new List<Character>();
    public bool stillExplore = false;
    public RegionValue exploreRegion;
    public int cityIndex;
    public int exploreLevel;

    public List<ExploreCardData> firstRow = new List<ExploreCardData>();
    public List<ExploreCardData> secondRow = new List<ExploreCardData>();
    public List<ExploreCardData> thirdRow = new List<ExploreCardData>();
    public List<ExploreCardData> fourthRow = new List<ExploreCardData>();
    public List<ExploreCardData> fifthRow = new List<ExploreCardData>();

    public List<Character> captureCharacter = new List<Character>();
    public List<ItemBase> caputureItemBase = new List<ItemBase>();



    public ExploreData(List<Character> playerChars, RegionValue region, int cityIdx)
    {
        playerExploreBattleCharacters = playerChars;
        foreach (var character in playerChars)
        {
            if (character != null) { 
                playerExploreCharacters.Add(character);
                character.IsMoved = true;
            }
        }

        exploreRegion = region;
        cityIndex = cityIdx;
        exploreLevel = region.GetCityValue(cityIdx).exploreLevel;
    }

}
