using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;


public class MusicPanelManager : MonoBehaviour
{
    public static MusicPanelManager Instance { get; private set; }
    [SerializeField] private FilterAndSortControl filterAndSortControl;
    [SerializeField] GameObject musicPanel;
    [SerializeField] Button musicButton;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitButtons();
    }


    void InitButtons()
    {
        musicButton.onClick.AddListener(ClosePanel);
    }


    private void Start()
    {
        filterAndSortControl.Init(OnFilterChanged, OnSortChanged);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) ClosePanel();
    }




    public void OpenPanel()
    {
        musicPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        musicPanel.SetActive(false);
    }



    private void OnFilterChanged()
    {
        RefreshDisplay();
    }

    private void OnSortChanged()
    {
        RefreshDisplay();
    }


    private void RefreshDisplay()
    {

        //var filteredData = FilterMusicData(allCharacterData);
        //var sortedData = SortMusicData(filteredData);
        //UpdateDisplay(sortedData);
    }


    private void UpdateDisplay(List<CharacterExtrasSaveData> dataList)
    {


    }




}