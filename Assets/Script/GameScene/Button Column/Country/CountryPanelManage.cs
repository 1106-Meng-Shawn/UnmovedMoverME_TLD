using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PanelExtensions;


public class CountryPanelManage : PanelBase //, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public static CountryPanelManage Instance { get; private set; }
    public Button countryCloseButton;

    public Button CountryTopButton;

    public Button RegionTopButton;
    public CountryRegionPanelControl countryRegionPanelControl;

    public Animation animation;
    public Button LeftImageButton;
    private bool isHide = false;

    private bool isMouseOverPanel = false;

    [SerializeField] DraggablePanel draggablePanel;

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
        InitTopButton();
        InitAnimationButton();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (draggablePanel.IsMouseOverPanel()) SwitchPanel();
        }
    }


    void InitTopButton()
    {
        CountryTopButton.onClick.AddListener(OpenPanel);
        RegionTopButton.onClick.AddListener(ShowCountryRegionPanel);
        countryCloseButton.onClick.AddListener(ClosePanel);
    }

    void InitAnimationButton()
    {
        LeftImageButton.onClick.AddListener(OnLeftImageButtonClick);
    }

    void OnLeftImageButtonClick()
    {
        isHide = !isHide;
        string iconPath = $"MyDraw/UI/GameUI/";
        if (isHide)
        {
            bool result = animation.Play("LeftImageHide");
            LeftImageButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "RightColShow");
            LeftImageButton.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(iconPath + "RightColShow"), Resources.Load<Sprite>(iconPath + "RightColUnShow"));
        }
        else
        {
            bool result = animation.Play("LeftImageShow");
            LeftImageButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "RightColUnClose");
            LeftImageButton.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(iconPath + "RightColClose"), Resources.Load<Sprite>(iconPath + "RightColUnClose"));

        }
    }

    public void SwitchPanel()
    {
        if (countryRegionPanelControl.gameObject.activeSelf)
        {
            OpenPanel();
        }
        else {
            ShowCountryRegionPanel();
        }
    }

    public override void OpenPanel()
    {
        base.panel.SetActive(true);
        countryRegionPanelControl.gameObject.SetActive(false);
    }



    public override void SetPanelSaveData(PanelSaveData panelSaveData)
    {
        SetSaveData(this,base.panel, panelSaveData);
    }


    public override PanelSaveData GetPanelSaveData()
    {
        PanelSaveData saveData = GetSaveData(this,base.panel, PanelType.Country);
        return saveData;
    }



    public void ToggleCountryRegionPanel()
    {
        if (base.panel.activeSelf && countryRegionPanelControl.gameObject.activeSelf)
        {
            ClosePanel();
        }
        else
        {
            ShowCountryRegionPanel();
        }

    }



    public void ShowCountryRegionPanel()
    {
        base.panel.SetActive(true);
        countryRegionPanelControl.gameObject.SetActive(true);
        //   leftImageControls[1].gameObject.SetActive(true);
    }



}
