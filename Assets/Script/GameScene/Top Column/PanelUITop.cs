using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static GetSprite;


public class PanelUITop : MonoBehaviour
{
    public string Type; // "Region", "Character", "Item"
    public Button ImageButton;
    public TextMeshProUGUI valueText;
    public Button TextButton;

    public BottomButton bottomButton;
    string iconPath = "MyDraw/Item";

    private void Start()
    {
        if (GameValue.Instance == null)
        {
            Debug.LogError("GameValue.Instance is null at Start!");
            return;
        }

        BindToData(Type);

        ImageButton.onClick.AddListener(TogglePanel);
        TextButton.onClick.AddListener(TogglePanel);
    }

    void BindToData(string type)
    {
        this.Type = type;
        switch (type)
        {
            case "Character":
                // GameValue.Instance.PlayerCharacters.OnListChanged += UpdateValueFromCharacter;
                GameValue.Instance.RegisterPlayerCharacterChanged(UpdateValueFromCharacter);
                GameValue.Instance.RegisterItemsChange(UpdateCharacterIcon);
                UpdateValueFromCharacter(); 
                UpdateCharacterIcon();
                break;
            case "Item":
                GameValue.Instance.RegisterItemsChange(UpdateValueFromItem);
                UpdateValueFromItem();
                break;
            case "CountryRegion":
                //  GameValue.Instance.PlayerRegions.OnListChanged += UpdateValueFromCountryRegion;
                GameValue.Instance.RegisterPlayerRegionsChanged(UpdateValueFromCountryRegion);
                GameValue.Instance.RegisterItemsChange(UpdateCountryRegionIcon);
                UpdateValueFromCountryRegion();
                UpdateCountryRegionIcon();
                break;
            default:
                Debug.LogWarning($"[PanelUITop] Unknown Type: {type}");
                break;
        }
    }

    void UpdateValueFromCharacter()
    {
        valueText.text = GameValue.Instance.GetPlayerCharacters().Count.ToString();
    }

    void UpdateValueFromItem()
    {
        valueText.text = GameValue.Instance.GetPlayerItems().Count.ToString();
        UpdateItemIcon();
    }

    void UpdateCharacterIcon()
    { 

        if (GameValue.Instance.HasItem(11))
        {
            ItemBase item = GameValue.Instance.GetItem(11);
            ImageButton.gameObject.GetComponent<Image>().sprite = item.icon;

        }
        else
        {
            ImageButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "/3/CommandSword");

        }

    }

    void UpdateValueFromCountryRegion()
    {
        valueText.text = GameValue.Instance.GetPlayerRegions().Count.ToString();

    }


    void UpdateCountryRegionIcon()
    {
        SetChildrenActive(GameValue.Instance.HasItem(8));

    }

    void UpdateItemIcon()
    {

        if (GameValue.Instance.HasItem(7))
        {
            ItemBase item = GameValue.Instance.GetItem(7);
            ImageButton.gameObject.GetComponent<Image>().sprite = item.icon;
        }
        else
        {
            ImageButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "/EmptyItemClose");

        }

    }


    void TogglePanel()
    {
        bottomButton?.TogglePanelByKey(Type);
    }

    void SetChildrenActive(bool active)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
    }

    private void OnDestroy()
    {
        switch (Type)
        {
            case "Character":
                // GameValue.Instance.PlayerCharacters.OnListChanged -= UpdateValueFromCharacter;
                GameValue.Instance.UnRegisterPlayerCharacterChanged(UpdateValueFromCharacter);
                GameValue.Instance.UnRegisterItemsChange(UpdateCharacterIcon);
                break;
            case "Item":
                GameValue.Instance.UnRegisterItemsChange(UpdateValueFromItem);
                break;
            case "CountryRegion":
                // GameValue.Instance.PlayerRegions.OnListChanged -= UpdateValueFromCountryRegion;
                GameValue.Instance.UnRegisterPlayerRegionsChanged(UpdateValueFromCountryRegion);
                GameValue.Instance.UnRegisterItemsChange(UpdateCountryRegionIcon);
                break;
        }

    }

}
