using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static FormatNumber;
using static GetSprite;

public class ItemPrefabControl : MonoBehaviour , IBeginDragHandler,IDragHandler, IEndDragHandler
{
    public Image itemIcon;
    public TMP_Text ItemNum;
    public Button starButton;
    public Button itemPrefabButton;

    private ItemBase item;
    private ItemPanel itemPanel;

    private ItemPanelManager itemPanelManger;
    private GameObject draggedIcon;
    public Image highlight;
    private Canvas uiCanvas;

    void Start()
    {
        itemPrefabButton.onClick.AddListener(OnItemPrefabClick);
        starButton.onClick.AddListener(OnStarButtonClick);
        uiCanvas = GeneralManager.Instance.uiCanvas;
    }

    void OnDestroy()
    {
        this.item.OnItemValueChange -= UpItemPrefabUI;
    }

    void OnItemPrefabClick()
    {
        if (itemPanel.GetItemPrefabControl() != this)
        {
            itemPanel.SetItemPrefabControl(this);
        } else
        {
            itemPanel.SetItemPrefabControl(null);
        }
    }

    void OnStarButtonClick()
    {
        item.IsStar = !item.IsStar;
        UpStarSprie();
    }

    void UpStarSprie()
    {
        starButton.image.sprite = UpStarButtonSprite(item.IsStar);
    }

    public void SetItem(ItemBase item,ItemPanel itemPanel,ItemPanelManager itemPanelManger)
    {
        this.item = item;
        this.item.OnItemValueChange += UpItemPrefabUI;
        this.itemPanel = itemPanel;
        this.itemPanelManger = itemPanelManger;
        UpItemPrefabUI();
    }

    public ItemBase GetItem()
    {
        return item;
    }

    public void UpItemPrefabUI()
    {
        itemIcon.sprite = item.icon;

        ItemNum.text = FormatNumberToString(item.GetRemainingNum());
        UpStarSprie();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemPanelManger != null) itemPanelManger.ShowRightImage();

        draggedIcon = new GameObject("DraggedIcon");
        draggedIcon.transform.SetParent(transform.root, false); // false ?? local ??
        Image draggedImage = draggedIcon.AddComponent<Image>();

        draggedImage.sprite = item.icon;

        RectTransform draggedRect = draggedImage.rectTransform;
        draggedRect.sizeDelta = new Vector2(200, 200); // ????????
        draggedRect.pivot = new Vector2(0.5f, 0.5f);
        draggedRect.anchorMin = new Vector2(0.5f, 0.5f);
        draggedRect.anchorMax = new Vector2(0.5f, 0.5f);
        draggedRect.position = Input.mousePosition + new Vector3(20, 20, 0);
        draggedImage.raycastTarget = true;


    }


    public void OnDrag(PointerEventData eventData)
    {
        if (draggedIcon != null)
        {
            RectTransform draggedIconRect = draggedIcon.GetComponent<RectTransform>();
            RectTransform parentRect = draggedIcon.transform.parent.GetComponent<RectTransform>();

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                uiCanvas.worldCamera,  
                out localPoint))
            {
                draggedIconRect.localPosition = localPoint;
                draggedIconRect.sizeDelta = new Vector2(250f, 250f);
                draggedIconRect.localScale = new Vector3(1, 1, 1);
            }
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedIcon != null)
        {
            Destroy(draggedIcon);
            draggedIcon = null;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        if (raycastResults.Count > 0)
        {
            GameObject target = raycastResults[0].gameObject;
            Debug.Log($"Hit object: {target.name}");

            var characterColumn = target.GetComponentInParent<CharacterColumnControl>();
            if (characterColumn != null)
            {
                characterColumn.SetItem(item);
            }

            var battleColumn = target.GetComponentInParent<BattleColumnControl>();
            if (battleColumn != null && battleColumn.character != null)
            {
                battleColumn.character.SetItem(item);
            }

            if (target != null &&
                (target == CharacterPanelManage.Instance.characterBackground.gameObject
                 || target.transform.IsChildOf(CharacterPanelManage.Instance.characterBackground.transform)))
            {
                CharacterPanelManage.Instance.SetItem(item);
            }
        }


        if (itemPanelManger != null)
            itemPanelManger.ShowLeftImage();
    }

}
