using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static FormatNumber;
using static GetSprite;
public class ItemPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IItemPanel
{

    //public string playerEmpireName;
    //public ItemBase itemAtPanel;
    private ItemPrefabControl itemPrefabControl;
    private ItemBase itemAtPanel;

   // public Button ItemCloseButton;
    public Image ItemImage;
    public Image ItemBackground;
    public Button starButton;


    public Image totalImage;
    public Image usedImage;
    public Image remainingImage;

    public TMP_Text ItemNameText;
    public TMP_Text ItemTotalText;
    public TMP_Text ItemUsedText;
    public TMP_Text ItemRemaningText;
    public TMP_Text ItemEffectText;
    public TMP_Text ItemDescribeText;

    public ItemTopColumnButton itemTopColumnButton;
    public DraggablePanel draggablePanel;

    // public GameObject itemsListScrollView;


    private Vector2 itemsPanelPosition;

    private void Start()
    {
        itemsPanelPosition = new Vector2(transform.position.x, transform.position.y);
        UpItemPanelUI();
        GameValue.Instance.RegisterItemsChange(itemTopColumnButton.ItemDisplay);
        starButton.onClick.AddListener(OnStarButtonClick);

    }

    void OnDestroy()
    {
        GameValue.Instance.UnRegisterItemsChange(itemTopColumnButton.ItemDisplay);
        if (itemAtPanel != null)
        {
            itemAtPanel.OnItemValueChange -= UpItemPanelUIWithItem;
        }
    }

    void OnStarButtonClick()
    {
        itemAtPanel.IsStar = !itemAtPanel.IsStar;
        UpStarSprie();
    }

    void UpStarSprie()
    {
        starButton.image.sprite = UpStarButtonSprite(itemAtPanel.IsStar);
    }


    public void SetItemPrefabControl(ItemPrefabControl itemPrefabControl)
    {
        if (itemAtPanel != null)
        {
            itemAtPanel.OnItemValueChange -= UpItemPanelUIWithItem;
            itemAtPanel = null;
        }

        this.itemPrefabControl = itemPrefabControl;

        if (itemPrefabControl == null)
        {
            UpItemPanelUI();
            return;
        }

        itemAtPanel = itemPrefabControl.GetItem();
        if (itemAtPanel != null)
        {
            itemAtPanel.OnItemValueChange += UpItemPanelUIWithItem;
        }

        UpItemPanelUI();
    }

    public void UpItemPanelUI()
    {
        if (itemAtPanel != null)
        {
            UpItemPanelUIWithItem();
        }
        else
        {
            UpItemPanelUIWithoutItem();
        }
    }


    void UpItemPanelUIWithItem()
    {
        ItemImage.sprite = itemAtPanel.icon;

        ItemPanelUIActive(true);
        SetStarButton(true);
        ItemNameText.text = itemAtPanel.GetItemName();
        ItemTotalText.text = FormatNumberToString(itemAtPanel.GetPlayerHasCount());
        ItemUsedText.text = FormatNumberToString(itemAtPanel.GetPlayerUsed());
        ItemRemaningText.text = FormatNumberToString(itemAtPanel.GetRemainingNum());
        ItemEffectText.text = itemAtPanel.GetEffectDescription();
        ItemDescribeText.text = itemAtPanel.GetItemDescription();
    }


    void UpItemPanelUIWithoutItem()
    {
        Sprite sprite = Resources.Load<Sprite>("MyDraw/Item/EmptyItem");

        ItemImage.sprite = sprite;
        SetStarButton(false);
        ItemPanelUIActive(false);

    }


    void SetStarButton(bool isActive)
    {
        if (!isActive)
        {
            starButton.interactable = false;
            starButton.image.sprite = UpStarButtonSprite(false);
            
        } else
        {
            starButton.interactable = true;
            UpStarSprie();
        }
    }

    public ItemPrefabControl GetItemPrefabControl()
    {
        return itemPrefabControl;
    }


    void ItemPanelUIActive(bool isActive)
    {
        totalImage.gameObject.SetActive(isActive);
        usedImage.gameObject.SetActive(isActive);
        remainingImage.gameObject.SetActive(isActive);
        ItemNameText.gameObject.SetActive(isActive);
        ItemTotalText.gameObject.SetActive(isActive);
        ItemRemaningText.gameObject.SetActive(isActive);
        ItemUsedText.gameObject.SetActive(isActive);
        ItemEffectText.gameObject.SetActive(isActive);
        ItemDescribeText.gameObject.SetActive(isActive);
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        SetItemPrefabControl(null);
        itemTopColumnButton.ItemDisplay();
    }

    public void ClosePanel()
    {
        SetItemPrefabControl(null);
        gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsPointerOnItemBackground(eventData) && itemPrefabControl != null)
        {
            draggablePanel.shouldBlockPanelDrag = true;
            itemPrefabControl.OnBeginDrag(eventData);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (itemPrefabControl != null)
        {
            itemPrefabControl.OnDrag(eventData);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        draggablePanel.shouldBlockPanelDrag = false;

        if (itemPrefabControl != null)
        {
            itemPrefabControl.OnEndDrag(eventData);
        }
    }


    private bool IsPointerOnItemBackground(PointerEventData eventData)
    {
        return eventData.pointerEnter != null && eventData.pointerEnter == ItemBackground.gameObject;
    }
}
