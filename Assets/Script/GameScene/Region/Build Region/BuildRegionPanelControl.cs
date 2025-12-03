using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildRegionPanelControl : MonoBehaviour
{
    public static BuildRegionPanelControl Instance { get; private set; }
    public GameObject BuildPanel;
    public HexGridUIManager hexGridUI;
    public BuildPanelTopRowControl buildPanelTopRowControl;

    public Button closeButton;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        closeButton.onClick.AddListener(ClosePanel);
    }



    private void Start()
    {
        OpenPanel(GameValue.Instance.GetBuildData());
        //gameObject.SetActive(false);
        //BuildPanel.SetActive(false);

    }

    //  public List<GameObject> closeObject;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) OnRightMouseButtonDown();
    }


    public void OpenPanel(BuildData buildData)
    {
        RegionValue regionValue = buildData.buildRegion;
        int index = buildData.cityIndex;
        gameObject.SetActive(true);
        BuildPanel.SetActive(true);
        hexGridUI.ShowHexGridUI(regionValue, index);
        buildPanelTopRowControl.SetRegionValue(regionValue,regionValue.GetCityValue(index));

    }

    public void OnRightMouseButtonDown()
    {
        if (hexGridUI.GetIsShowBuild())
        {
            hexGridUI.HideBuildPanel();
        }
        //else
        //{
        //   ClosePanel();
        //}
    }
    public void ClosePanel()
    {
        //hexGridUI.HideHexGridUI();
        GameValue.Instance.SetBuildData(null);
        if (hexGridUI.GetIsShowBuild()) hexGridUI.HideBuildPanel();
        if (!buildPanelTopRowControl.GetIsLock()) buildPanelTopRowControl.UpValueTypeUI(0);
        SceneTransferManager.Instance.LoadScene(Scene.GameScene);
     //   ActiveNoAboutObject(true);

    }

}


public class BuildData
{
    public RegionValue buildRegion;
    public int cityIndex;

    public BuildData(RegionValue regionValue, int index)
    {
        buildRegion = regionValue;
        cityIndex = index;
    }

}