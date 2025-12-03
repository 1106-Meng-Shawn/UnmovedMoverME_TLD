using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemImageDrag : MonoBehaviour//, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
  /*  public ItemPanelManger itemPanelManger;

    private GameObject draggedIcon;
    public ItemPanel itemPanel;
    public CharacterPanel characterPanel;
    public BattlePanelCharacterInfoOld battlePanelCharacterInfo;

    private RegionInfo regionInfoUI;

    void Start()
    {
        regionInfoUI = FindObjectOfType<RegionInfo>();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Destroy(draggedIcon);
        draggedIcon = null;

       if (itemPanel == null || itemPanel.itemAtPanel == null) return;
        if (itemPanel != null && draggedIcon == null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.name == "ItemsImage")
            {
                draggedIcon = new GameObject("DraggedIcon");
                draggedIcon.transform.SetParent(transform.root);
                Image draggedImage = draggedIcon.AddComponent<Image>();

                draggedImage.sprite = itemPanel.itemAtPanel.icon;
               draggedImage.raycastTarget = false;

                draggedImage.rectTransform.sizeDelta = new Vector2(0.75f, 0.75f);
                RectTransform draggedRectTransform = draggedImage.rectTransform;
                draggedRectTransform.pivot = new Vector2(0.5f, 0.5f);
                draggedRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                draggedRectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                Vector3 offset = new Vector3(20, 20, 0);
                draggedRectTransform.position = Input.mousePosition + offset;

            }

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Destroy(draggedIcon);
        draggedIcon = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (draggedIcon != null && itemPanelManger != null) itemPanelManger.ShowRightImage();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedIcon != null)
        {
            RectTransform draggedIconRect = draggedIcon.GetComponent<RectTransform>();
            RectTransform canvasRect = transform.root.GetComponent<RectTransform>();

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                null, // Screen Space - Overlay ?? null
                out localPoint
            );

            draggedIconRect.localPosition = localPoint;

            // ?????? 1??????250f ??????
            draggedIconRect.localScale = new Vector3(250f, 250f, 1f);


        }
    }



    /*   public void OnDrag(PointerEventData eventData)
       {

           if (itemPanel.itemAtPanel == null) return;
           if (itemPanel.itemAtPanel.remainingNum == 0) return;

           if (draggedIcon != null)
           {

               Vector3 mouseScreenPosition = Input.mousePosition;

               Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
               mouseWorldPosition.z = 0;

               RectTransform draggedIconRectTransform = draggedIcon.GetComponent<RectTransform>();
               draggedIconRectTransform.localScale = new Vector2(200f, 200f);


               if (battlePanelCharacterInfo != null && battlePanelCharacterInfo.battlePanelValue.isInExplore)
               {
                   draggedIconRectTransform.position = mouseScreenPosition;
               }
               else
               {
                   draggedIconRectTransform.position = mouseWorldPosition;


               }
           }
       }*/

    /*
    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(draggedIcon);

        if (eventData.pointerCurrentRaycast.gameObject == null) return;

        if (eventData.pointerCurrentRaycast.gameObject.name == "CharacterImage" || eventData.pointerCurrentRaycast.gameObject.name == "ItemImage")
        {
            if (characterPanel == null) characterPanel = FindObjectOfType<CharacterPanel>();
            if (characterPanel.characterAtPanel != null && itemPanel.itemAtPanel.GetRemainingNum() > 0)
            {
                characterPanel.characterAtPanel.SetItem(itemPanel.itemAtPanel);
                characterPanel.setCharacterValueAtPanel(characterPanel.characterAtPanel);

            };

        }

        if (eventData.pointerCurrentRaycast.gameObject.name == "CharacterBattleImage" || eventData.pointerCurrentRaycast.gameObject.name == "ItemBattleImage")
        {

            if (battlePanelCharacterInfo.characterAtBattlePanel != null //&& itemPanel.itemAtPanel != null && itemPanel.itemAtPanel.remainingNum > 0
                )
            {
                battlePanelCharacterInfo.characterAtBattlePanel.SetItem(itemPanel.itemAtPanel);
                battlePanelCharacterInfo.setCharacterValueAtBattlePanel(battlePanelCharacterInfo.characterAtBattlePanel);

            }

        }

        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<BattlePosition>() != null && !eventData.pointerCurrentRaycast.gameObject.GetComponent<BattlePosition>().isEnemy)
        {

            if (battlePanelCharacterInfo.characterAtBattlePanel != null && itemPanel.itemAtPanel != null && itemPanel.itemAtPanel.GetRemainingNum() > 0)
            {
                battlePanelCharacterInfo.characterAtBattlePanel.SetItem(itemPanel.itemAtPanel);
                battlePanelCharacterInfo.setCharacterValueAtBattlePanel(battlePanelCharacterInfo.characterAtBattlePanel);

            }

        }


        if (eventData.pointerCurrentRaycast.gameObject.name == "Lord Name" || eventData.pointerCurrentRaycast.gameObject.name == "Lord Image")
        {
        /*    if (regionInfoUI.regionAtUI.lord != null// && itemPanel.itemAtPanel.remainingNum > 0
                )
            {
                regionInfoUI.regionAtUI.lord.SetItem(itemPanel.itemAtPanel);
            }*/
   /*     }

        draggedIcon = null;
        if(itemPanelManger != null) itemPanelManger.ShowLeftImage();

    }*/
}
