using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using static PanelExtensions;

public class ItemPanelManager : PanelBase, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public SideImageControl sideImageControl;

    public ItemPanel storagePanel;
    public StoreBuyControl buyPanel;
    public StoreSellControl sellPanel;

    public Button storageButton;
    public Button buyButton;
    public Button sellButton;
    public Button closeButton;

    public enum ItemPanelType
    {
        Storage,
        Buy,
        Sell
    }

    private ItemPanelType currentType = ItemPanelType.Storage;
    private Dictionary<ItemPanelType, IItemPanel> panelMap;

    private bool isMouseOverPanel = false;

    void Awake()
    {
        // ?????
        panelMap = new Dictionary<ItemPanelType, IItemPanel>
        {
            { ItemPanelType.Storage, storagePanel },
            { ItemPanelType.Buy, buyPanel },
            { ItemPanelType.Sell, sellPanel }
        };

        // ??????
        storageButton.onClick.AddListener(() => SwitchPanel(ItemPanelType.Storage));
        buyButton.onClick.AddListener(() => SwitchPanel(ItemPanelType.Buy));
        sellButton.onClick.AddListener(() => SwitchPanel(ItemPanelType.Sell));
        closeButton.onClick.AddListener(ClosePanel);
    }

    // ========== ???? ==========
    public void OnPointerEnter(PointerEventData eventData) => isMouseOverPanel = true;
    public void OnPointerExit(PointerEventData eventData) => isMouseOverPanel = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isMouseOverPanel && eventData.button == PointerEventData.InputButton.Right)
            ClosePanel();
    }

    private void SwitchPanel(ItemPanelType newType)
    {
        if (currentType == newType) return;

        panelMap[currentType].ClosePanel(); // ????
        currentType = newType;
        panelMap[currentType].ShowPanel();  // ????

        // ?????
        if (newType == ItemPanelType.Storage)
            sideImageControl?.ChangeMaidSprite();
        else
            sideImageControl?.ChangeStoreSprite();
    }

    public override void ClosePanel()
    {
        panel.SetActive(false);
        panelMap[currentType].ClosePanel();
    }

    public override void OpenPanel()
    {
        panel.SetActive(true);
        SwitchPanel(ItemPanelType.Storage); // ??????
    }


    public override void SetPanelSaveData(PanelSaveData panelSaveData)
    {
        SetSaveData(this,panel, panelSaveData);
    }


    public void ShowRightImage() => sideImageControl?.ShowRightGameObeject();
    public void ShowLeftImage() => sideImageControl?.ShowLeftGameObeject();

    public override PanelSaveData GetPanelSaveData()
    {
        PanelSaveData saveData = GetSaveData(this,panel, PanelType.Items);
        return saveData;
    }

}

public interface IItemPanel
{
    void ShowPanel();
    void ClosePanel();
}

