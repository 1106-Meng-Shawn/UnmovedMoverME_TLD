using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static CharacterExtrasPanelControl;


public class ExtrasPanelManager : MonoBehaviour
{
    public static ExtrasPanelManager Instance { get; private set; }
    [SerializeField] Button CloseButton;
    [SerializeField] GameObject ExtrasPanel;


    [Header("Left Buttons")]
    [SerializeField] protected LeftButtons leftButtons;

    [System.Serializable]
    public struct LeftButtons
    {
        public Button CGCollectionButton;
        public Button StoryCollectionButton;
        public Button AchievementCollectionButton;
        public Button MusicCollectionButton;
        public Button CharacterCollectionButton;
        public CharacterExtrasPanelControl CharacterExtrasPanelControl;
        public Button ItemCollectionButton;
    }
    private void Awake()
    {
        InitInstance();
        InitButtons();
    }

    void InitInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    void InitButtons()
    {
        leftButtons.CGCollectionButton.onClick.AddListener(OnCGCollectionButtonClick);
        leftButtons.StoryCollectionButton.onClick.AddListener(OnStoryCollectionButtonClick);
        leftButtons.AchievementCollectionButton.onClick.AddListener(OnAchievementCollectionButtonClick);
        leftButtons.MusicCollectionButton.onClick.AddListener(OnMusicCollectionButtonClick);
        leftButtons.CharacterCollectionButton.onClick.AddListener(OnCharacterCollectionButtonClick);
        leftButtons.ItemCollectionButton.onClick.AddListener(OnItemCollectionButtonClick);
        CloseButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void OnCGCollectionButtonClick()
    {
    }

    private void OnStoryCollectionButtonClick()
    {
    }

    private void OnAchievementCollectionButtonClick()
    {
    }

    private void OnMusicCollectionButtonClick()
    {
    }

    private void OnCharacterCollectionButtonClick()
    {
        leftButtons.CharacterExtrasPanelControl.ShowPanel();
        CharacterExtrasSaveData characterExtrasSave =  ExtrasValue.Instance.GetCharacterExtrasSaveData(CharacterConstants.PlayerKey);
        SetCharacterExtrasSave(characterExtrasSave);
    }


    public void SetCharacterExtrasSave(CharacterExtrasSaveData characterExtrasSave)
    {
        leftButtons.CharacterExtrasPanelControl.SetCharacterExtrasSave(characterExtrasSave);
    }


    private void OnItemCollectionButtonClick()
    {
    }

    private void OnCloseButtonClick()
    {
        HideExtrasPanel();
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) HideExtrasPanel();
    }

    public void ShowExtrasPanel()
    {
        ShowButtons();
        ExtrasPanel.SetActive(true);
    }

    void ShowButtons()
    {
        leftButtons.CharacterCollectionButton.gameObject.SetActive(ExtrasValue.Instance.HasCharacterExtrasSaveData());
    }


    public void HideExtrasPanel()
    {
        ExtrasPanel.SetActive(false);
    }

}
