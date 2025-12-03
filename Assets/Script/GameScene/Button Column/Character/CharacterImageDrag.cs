using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;


public class CharacterImageDrag : MonoBehaviour//, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
  /*  private GameObject draggedIcon;
    public CharacterPanel characterPanel;
    public BattlePanelCharacterInfoOld battlePanelCharacterInfo;




    public void OnPointerDown(PointerEventData eventData)
    {
        Destroy(draggedIcon);
        draggedIcon = null;

        if (characterPanel != null && characterPanel.characterAtPanel == null) return;
        if (battlePanelCharacterInfo != null && battlePanelCharacterInfo.characterAtBattlePanel == null) return;



        if (eventData.pointerCurrentRaycast.gameObject.name == "CharacterImage" || eventData.pointerCurrentRaycast.gameObject.name == "CharacterBattleImage")
            {
                draggedIcon = new GameObject("DraggedIcon");
                draggedIcon.transform.SetParent(transform.root);
                Image draggedImage = draggedIcon.AddComponent<Image>();


            if (characterPanel != null && characterPanel.characterAtPanel != null)
            {
                draggedImage.sprite = characterPanel.characterAtPanel.icon;
            }
            else if (battlePanelCharacterInfo != null && battlePanelCharacterInfo.characterAtBattlePanel != null)
            {
                draggedImage.sprite = battlePanelCharacterInfo.characterAtBattlePanel.icon;

            }




            draggedImage.raycastTarget = false;

                draggedImage.rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
                RectTransform draggedRectTransform = draggedImage.rectTransform;
                draggedRectTransform.pivot = new Vector2(0.5f, 0.5f);
                draggedRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                draggedRectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                Vector3 offset = new Vector3(20, 20, 0);
                draggedRectTransform.position = Input.mousePosition + offset;

            }

        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
         Destroy(draggedIcon);
         draggedIcon = null;
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (characterPanel != null && characterPanel.characterAtPanel == null) return;
        if (battlePanelCharacterInfo != null && battlePanelCharacterInfo.characterAtBattlePanel == null) return;

        if (draggedIcon != null)
        {
            RectTransform draggedIconRect = draggedIcon.GetComponent<RectTransform>();
            RectTransform parentRect = draggedIcon.transform.parent.GetComponent<RectTransform>();

            // 缩放设置（可自定义）
            draggedIconRect.localScale = new Vector2(250f, 250f);

            // 将屏幕坐标转为 UI 的局部坐标（Overlay 模式必须传 null）
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                null, // Screen Space - Overlay 必须传 null
                out localPoint))
            {
                draggedIconRect.localPosition = localPoint;
            }
        }
    }


    /* public void OnDrag(PointerEventData eventData)
     {

         if (characterPanel != null && characterPanel.characterAtPanel == null) return;
         if (battlePanelCharacterInfo != null && battlePanelCharacterInfo.characterAtBattlePanel == null) return;

         if (draggedIcon != null)
         {
             Vector3 mouseScreenPosition = Input.mousePosition;

             Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
             mouseWorldPosition.z = 0;

             RectTransform draggedIconRectransform = draggedIcon.GetComponent<RectTransform>();


             draggedIconRectransform.localScale = new Vector2(250f, 250f);


             if (battlePanelCharacterInfo != null &&  !battlePanelCharacterInfo.battlePanelValue.isInExplore)
             {
                 draggedIconRectransform.position = mouseWorldPosition;

             }
             else
             {
                 if (battlePanelCharacterInfo == null)
                 {
                     draggedIconRectransform.position = mouseWorldPosition;
                 } else
                 {
                     draggedIconRectransform.position = mouseScreenPosition;
                 }
             }

         }
     } */
  /*

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(draggedIcon);
        draggedIcon = null;
 
        if (eventData.pointerCurrentRaycast.gameObject == null) return;
        if (characterPanel != null && characterPanel.characterAtPanel == null) return;
        if (battlePanelCharacterInfo != null && battlePanelCharacterInfo.characterAtBattlePanel == null) return;



        if (characterPanel != null && (eventData.pointerCurrentRaycast.gameObject.name == "Lord Name" || eventData.pointerCurrentRaycast.gameObject.name == "Lord Image"))
        {
       //     characterPanel.regionInfoUI.setLord(characterPanel.characterAtPanel);
        }


        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<RegionColumControl>())
        {

            Character character = characterPanel.characterColumnControl.character;
            if (!character.IsMoved)
            {
             //   eventData.pointerCurrentRaycast.gameObject.GetComponent<RegionColumControl>().Region.SetLord(character);
            }
            return;
        }



        if (battlePanelCharacterInfo != null)
        {

            if (battlePanelCharacterInfo.characterAtBattlePanel.HasLord()) return;
            if (battlePanelCharacterInfo.characterAtBattlePanel.IsMoved) return;


            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;

            BattlePosition battlePosition = hitObject.GetComponent<BattlePosition>();

            if (battlePosition != null)
            {
                var character = battlePanelCharacterInfo.characterAtBattlePanel;

            /*    if (character.battlePosition == null)
                {
                    character.battlePosition = battlePosition;
                    battlePosition.characterAtBattlePosition = character;
                }
                else
                {
                    if (battlePosition.characterAtBattlePosition != null) battlePosition.characterAtBattlePosition.battlePosition = null;
                    character.battlePosition.characterAtBattlePosition = null;
                    character.battlePosition = battlePosition;
                    character.battlePosition.characterAtBattlePosition = character;

                }
            }

        }






    }*/
}
