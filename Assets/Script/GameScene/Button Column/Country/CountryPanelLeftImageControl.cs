using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CountryPanelLeftImageControl : MonoBehaviour
{
    public string pathParameter;

    [Header("Electoral State Buttons")]
    public ElectoralButtons electoralButtons;
    string staticIconPath = "MyDraw/UI/CountryPanel/";
    string iconPath = "MyDraw/UI/CountryPanel/";

    [System.Serializable]
    public class ElectoralButtons
    {
        public Button TeirerButton;
        public Button CologneButton;
        public Button MainzButton;
        public Button PlatinateButton;
        public Button SaxonyButton;
        public Button BrandenburgButton;
        public Button BohemiaButton;
    }

    [Header("Reichskleinodien Buttons")]
    public Reichskleinodien reichskleinodien;
    [System.Serializable]
    public class Reichskleinodien
    {
        public Button regionButton;
        public Button electorButton;
        public Button recruitButton;
        public Button characterButton;
        public Button countryButton;
        public Button itemButton;
        public ButtonEffect recruitButtonEffect;
    }

    private int lastClickedElectorIndex = -1;
    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.4f;

    [Header("Other")]
    private BottomButton bottomButton;

    private void OnEnable()
    {
        iconPath = staticIconPath + pathParameter + "/";
        UpdateElectorButtonsByRegion();
        UpdateInitReichskleinodienButtonsByItem();
    }


    private void Start()
    {
        bottomButton = BottomButton.Instance;
        InitElectoralStateButtons();
        GameValue.Instance.RegisterPlayerRegionsChanged(UpdateElectorButtonsByRegion);
        UpdateElectorButtonsByRegion();


        InitReichskleinodienButtons();
        GameValue.Instance.RegisterItemsChange(UpdateInitReichskleinodienButtonsByItem);
        UpdateInitReichskleinodienButtonsByItem();


    }

    void InitElectoralStateButtons()
    {
        electoralButtons.TeirerButton?.onClick.AddListener(() => OnElectorClicked(4));
        electoralButtons.CologneButton?.onClick.AddListener(() => OnElectorClicked(3));
        electoralButtons.MainzButton?.onClick.AddListener(() => OnElectorClicked(6));
        electoralButtons.PlatinateButton?.onClick.AddListener(() => OnElectorClicked(7));
        electoralButtons.SaxonyButton?.onClick.AddListener(() => OnElectorClicked(2));
        electoralButtons.BrandenburgButton?.onClick.AddListener(() => OnElectorClicked(1));
        electoralButtons.BohemiaButton?.onClick.AddListener(() => OnElectorClicked(5));
    }

    void UpdateElectorButtonsByRegion()
    {
        electoralButtons.TeirerButton?.gameObject.SetActive(HasRegion(4));
        electoralButtons.CologneButton?.gameObject.SetActive(HasRegion(3));
        electoralButtons.MainzButton?.gameObject.SetActive(HasRegion(6));
        electoralButtons.PlatinateButton?.gameObject.SetActive(HasRegion(7));
        electoralButtons.SaxonyButton?.gameObject.SetActive(HasRegion(2));
        electoralButtons.BrandenburgButton?.gameObject.SetActive(HasRegion(1));
        electoralButtons.BohemiaButton?.gameObject.SetActive(HasRegion(5));
    }


    void UpdateInitReichskleinodienButtonsByItem()
    {
        reichskleinodien.regionButton?.gameObject.SetActive(HasItem(10));
        reichskleinodien.electorButton?.gameObject.SetActive(HasItem(5));
        reichskleinodien.countryButton?.gameObject.SetActive(HasItem(8));
        RecruitSet();
        CharacterSet();
        ItemSet();

    }

    void RecruitSet()
    {

        if (HasItem(6))
        {
            ItemBase item = GameValue.Instance.GetItem(6);

            Sprite UnselSprite = Resources.Load<Sprite>(iconPath + "GlovesUnsel");
            Sprite SelSprite = Resources.Load<Sprite>(iconPath + "GlovesSel");

            reichskleinodien.recruitButtonEffect.SetChangeSprite(SelSprite, UnselSprite);

        }
        else
        {
            reichskleinodien.characterButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "CommonSword");

            Sprite UnselSprite = Resources.Load<Sprite>(iconPath + "AppointmentGlovesUnsel");
            Sprite SelSprite = Resources.Load<Sprite>(iconPath + "AppointmentGlovesSel");

            reichskleinodien.recruitButtonEffect.SetChangeSprite(SelSprite, UnselSprite);


        }

    }



    void CharacterSet()
    {
        var spriteName = HasItem(11) ? "Zeremonienschwert" : "CommonSword";
        var sprite = Resources.Load<Sprite>(iconPath + spriteName);
        reichskleinodien.characterButton.gameObject.GetComponent<Image>().sprite = sprite;
    }

    void ItemSet()
    {

        if (HasItem(7))
        {

            reichskleinodien.itemButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Reichsapfel");

        }
        else
        {
            reichskleinodien.itemButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "EmptyItemClose");

        }

    }



    bool HasItem(int itemID)
    {
        return GameValue.Instance.HasItem(itemID);
    }



    public bool HasRegion(int regionID)
    {
        List<RegionValue>  playerRegions = GameValue.Instance.GetPlayerRegions();
        return playerRegions != null && playerRegions.Any(r => r.GetRegionID() == regionID);
    }


    void OnElectorClicked(int index)
    {
        if (lastClickedElectorIndex == index && Time.time - lastClickTime < doubleClickThreshold)
        {
            // ?????????????/??
            Region region = GameValue.Instance.GetAllRegionValues()[index].region;
            region.ZoomToCity(0);
            region.ShowRegionPanel(0); // ??????
            lastClickedElectorIndex = -1; // ??
        }
        else
        {
            // ??????????
            Region region = GameValue.Instance.GetAllRegionValues()[index].region;
            region.ShowRegionPanel();

            lastClickedElectorIndex = index;
            lastClickTime = Time.time;
        }
    }

    void InitReichskleinodienButtons()
    {
        reichskleinodien.regionButton.onClick.AddListener(() => bottomButton.TogglePanelByKey("Region"));
        reichskleinodien.electorButton.onClick.AddListener(() => bottomButton.TogglePanelByKey("Elector"));
        reichskleinodien.recruitButton.onClick.AddListener(() => bottomButton.TogglePanelByKey("Recruit"));
        reichskleinodien.characterButton.onClick.AddListener(() => bottomButton.TogglePanelByKey("Character"));
        reichskleinodien.countryButton.onClick.AddListener(() => CountryPanelManage.Instance.SwitchPanel());
        reichskleinodien.itemButton.onClick.AddListener(() => bottomButton.TogglePanelByKey("Item"));

    }
}
