using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class throwItem : MonoBehaviour//, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
 /*   public CharacterPanel characterPanel;
    public BattlePanelCharacterInfo battlePanelCharacterInfo;
    public ItemBase item;

    private GameObject draggedIcon;

    private void Update()
    {
        // Ensure item is updated from the correct source
            if (characterPanel != null && characterPanel.characterAtPanel != null)
            {
                item = characterPanel.characterAtPanel.itemWithCharacter;
            }
            else if (battlePanelCharacterInfo != null && battlePanelCharacterInfo.characterAtBattlePanel != null)
            {
                item = battlePanelCharacterInfo.characterAtBattlePanel.itemWithCharacter;
            }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Destroy any previous dragged icon if it exists
        Destroy(draggedIcon);
        draggedIcon = null;

        if (item == null) return;  // If there is no item, do nothing

        if (eventData.pointerCurrentRaycast.gameObject.name == "ItemImage" || eventData.pointerCurrentRaycast.gameObject.name == "BattleItem") {
            draggedIcon = new GameObject("DraggedIcon");
            draggedIcon.transform.SetParent(transform.root);  // Set parent to root of the Canvas

            Image draggedImage = draggedIcon.AddComponent<Image>();
            draggedImage.sprite = item.icon;
            draggedImage.raycastTarget = false;

            draggedImage.rectTransform.sizeDelta = new Vector2(0.75f, 0.75f);
            RectTransform draggedRectTransform = draggedImage.rectTransform;
            draggedRectTransform.pivot = new Vector2(0.5f, 0.5f);
            draggedRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            draggedRectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            // Position the dragged icon near the mouse cursor
            Vector3 offset = new Vector3(20, 20, 0);
            draggedRectTransform.position = Input.mousePosition + offset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Destroy dragged icon when the pointer is released
        Destroy(draggedIcon);
        draggedIcon = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null || draggedIcon == null) return;

        // Update the position of the dragged icon as the mouse moves
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0;  // Ensure z-position remains constant

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

    public void OnEndDrag(PointerEventData eventData)
    {
        // If there is no valid raycast (no object to drop on), return
        if (eventData.pointerCurrentRaycast.gameObject == null)
        {
            Destroy(draggedIcon);
            draggedIcon = null;
            return;
        }

        // Get the name of the object we are dragging over
        var targetObjectName = eventData.pointerCurrentRaycast.gameObject.name;

        // If the target is not valid for dropping, return
        if (targetObjectName == "CharacterImage" || targetObjectName == "ItemImage" ||
            targetObjectName == "CharacterBattleImage" || targetObjectName == "ItemBattleImage")
        {
            Destroy(draggedIcon);
            draggedIcon = null;
            return;
        }

        // Perform the item throw logic based on where it was dropped
        if (targetObjectName != "CharaterImage" && targetObjectName != "ItemImage" && characterPanel != null)
        {
            
            if (characterPanel.characterAtPanel != null && characterPanel.characterAtPanel.itemWithCharacter)
            {
                characterPanel.characterAtPanel.throwItem(item);
                item = null;
                characterPanel.setCharacterValueAtPanel(characterPanel.characterAtPanel);
            }
        }
        else if (targetObjectName != "CharaterBattleImage" && targetObjectName != "ItemBattleImage" && battlePanelCharacterInfo != null)
        {
            if (battlePanelCharacterInfo.characterAtBattlePanel != null && battlePanelCharacterInfo.characterAtBattlePanel.itemWithCharacter)
            {
                battlePanelCharacterInfo.characterAtBattlePanel.throwItem(item);
                item = null;
                battlePanelCharacterInfo.setCharacterValueAtBattlePanel(battlePanelCharacterInfo.characterAtBattlePanel);
            }
        }

        // After the drop operation, clean up the dragged icon
        Destroy(draggedIcon);
        draggedIcon = null;
    }*/
}
