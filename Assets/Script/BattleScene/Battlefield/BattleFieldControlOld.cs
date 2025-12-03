using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class BattleFieldControlOld : MonoBehaviour//, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Slider ForceRate;
    public GameObject content;

    //private RectTransform contentRect;
    //private bool isDragging = false; // ??????????
  //  private bool isMouseOver = false; // ?????????content?

    public List<PositionAtBattle> totalPostionAtBattles; // 0 - 8 is player , 9 - 17 is enemy


    public GameObject playerPostionPanel;
    public GameObject enemyPostionPanel;

    public Arrow arrowAtTheField;

    public Canvas uiCanvas;

    public Arrow arrowInstance;



    public void SetTheArrowAtTheBattleField(BattleCharacterValue battleCharacterValue, Skill useSkill, bool isEnemy, BattleCharacterValue targets)
    {
     /*   int userPositionIndex = FindCharacterPosition(totalPostionAtBattles, battleCharacterValue);
        int targetsPositionIndex = FindCharacterPosition(totalPostionAtBattles, targets);

        if (userPositionIndex == -1 || targetsPositionIndex == -1)
        {
            
            Debug.LogWarning(userPositionIndex + " Character position not found. " + targetsPositionIndex);
            return;
        }


        arrowInstance = Instantiate(arrowAtTheField, uiCanvas.transform);
         Vector3 startPos = totalPostionAtBattles[userPositionIndex].gameObject.transform.position;
          Vector3 endPos = totalPostionAtBattles[targetsPositionIndex].gameObject.transform.position;

     /*   Vector3 startPos = totalPostionAtBattlePostions[userPositionIndex];
        Vector3 endPos = totalPostionAtBattlePostions[targetsPositionIndex];

        arrowInstance.SetStartPos(startPos, useSkill.type, isEnemy,true, endPos);
        arrowInstance.SetColor(true);*/
    }

    private int FindCharacterPosition(List<PositionAtBattle> positionList, BattleCharacterValue characterValue)
    {
        for (int i = 0; i < positionList.Count; i++)
        {
         /*   if (positionList[i].characterAtPlayerPosition == characterValue.characterValue)
            {
                return i;
            }*/
        }
        return -1;  
    }




    /*  private void Start()
      {
          contentRect = content.GetComponent<RectTransform>();
      }

      private void Update()
      {
          if (!isDragging && !isMouseOver)
          {
              SetContentTransparency(0.7f); // ???????????????????
          }

          if (!isDragging && isMouseOver)
          {
              SetContentTransparency(1f); // ?????content???????
          }

          if (!isDragging)
          {
            //  controlContent(ForceRate.value); // ????????? ForceRate ????
          }
      }

      // ??Slider???pivot
      void controlContent(float value)
      {
          if (value >= 0.65f)
          {
              contentRect.pivot = new Vector2(contentRect.pivot.x, 0.65f);
          }
          else if (value > 0.35f && value < 0.65f)
          {
              contentRect.pivot = new Vector2(contentRect.pivot.x, value);
          }
          else if (value <= 0.35f)
          {
              contentRect.pivot = new Vector2(contentRect.pivot.x, 0.35f);
          }
      }

      // ?????
      public void OnBeginDrag(PointerEventData eventData)
      {
          isDragging = true;
          SetContentTransparency(1f); // ????????
      }

      // ????????
      private Vector2 previousMousePosition; // ???????????

      public void OnDrag(PointerEventData eventData)
      {
          if (isDragging)
          {
              // ????????????????
              float deltaMovement = eventData.position.y - previousMousePosition.y;

              // ??????????????????????
              ForceRate.value += deltaMovement * 0.005f; // 0.005f ??????????

              // ??Slider???0?1??
              ForceRate.value = Mathf.Clamp(ForceRate.value, 0f, 1f);

              // ?????????
              previousMousePosition = eventData.position;

              // ????????content???
              controlContent(ForceRate.value);
          }
      }

      // ?????
      public void OnEndDrag(PointerEventData eventData)
      {
          isDragging = false;
          // ??????? ForceRate.value ??????
          controlContent(ForceRate.value);
      }

      // ?????content???
      public void OnPointerEnter(PointerEventData eventData)
      {
          isMouseOver = true;
          SetContentTransparency(1f); // ??????????
      }

      // ?????content???
      public void OnPointerExit(PointerEventData eventData)
      {
          isMouseOver = false;
          if (!isDragging)
          {
              SetContentTransparency(0.7f); // ???????????
          }
      }

      // ???????????
      private void SetContentTransparency(float alpha)
      {
          foreach (Transform child in content.transform)
          {
              CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
              if (canvasGroup == null)
              {
                  canvasGroup = child.gameObject.AddComponent<CanvasGroup>();
              }
              canvasGroup.alpha = alpha;
          }
      }

      public void AdjustPanelsY()
      {
          int playerYAdjustment = 0;
          int enemyYAdjustment = 0;

          for (int i = 0; i <= 2; i++)
          {
              if (i < playerPostionAtBattles.Count && !playerPostionAtBattles[i].gameObject.activeSelf)
              {
                  playerYAdjustment += 150;
              }
              if (i < enemyPostionAtBattles.Count && !enemyPostionAtBattles[i].gameObject.activeSelf)
              {
                  enemyYAdjustment -= 150;
              }
          }

          for (int i = 3; i <= 5; i++)
          {
              if (i < playerPostionAtBattles.Count && !playerPostionAtBattles[i].gameObject.activeSelf)
              {
                  playerYAdjustment += 150;
              }
              if (i < enemyPostionAtBattles.Count && !enemyPostionAtBattles[i].gameObject.activeSelf)
              {
                  enemyYAdjustment -= 150;
              }

          }

          RectTransform playerPanelRect = playerPostionPanel.GetComponent<RectTransform>();
          RectTransform enemyPanelRect = enemyPostionPanel.GetComponent<RectTransform>();

          playerPanelRect.anchoredPosition = new Vector2(playerPanelRect.anchoredPosition.x, playerPanelRect.anchoredPosition.y + playerYAdjustment);
          enemyPanelRect.anchoredPosition = new Vector2(enemyPanelRect.anchoredPosition.x, enemyPanelRect.anchoredPosition.y + enemyYAdjustment);
      }*/

}


