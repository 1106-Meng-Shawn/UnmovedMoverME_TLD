using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class ExploreCardNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image cardTypeImage;
    [SerializeField] private Image cardBoxImage;
    [SerializeField] private Image highLightImage;
    [SerializeField] private TextMeshProUGUI cardTypeText;
    [SerializeField] private TextMeshProUGUI LevelText;
    [SerializeField] private Button exploreCardButton;
    [SerializeField] private GameObject eventPanelPrefab;

    private ExploreCardData cardData;
    private Coroutine highlightCoroutine;
    private bool isMouseOver = false;

    public List<ExploreCardNode> nextCardNodes = new List<ExploreCardNode>();

    private void Start()
    {
        if (highLightImage != null)
        {
            highLightImage.color = GetColor.GetRowColor(RowType.sel);
            SetImageAlpha(0);
            highLightImage.enabled = false;
        }

        if (exploreCardButton != null)
            exploreCardButton.onClick.AddListener(OnCardClicked);
    }

    public void SetNextCards(List<ExploreCardNode> cards)
    {
        nextCardNodes = cards;
    }

    public void HighLightNextCards(bool isActive)
    {
        foreach (var next in nextCardNodes)
            next.HighLightNextCards(isActive);

        SetHighLight(isActive);
    }

    public void SetHighLight(bool isActive)
    {
        if (isActive)
            StartHighlight();
        else
            StopHighlight();
    }

    void StartHighlight()
    {
        if (highlightCoroutine == null)
        {
            highlightCoroutine = StartCoroutine(FlashSprite());
            ExploreUI.Instance.ChangeDuration(0.25f);
        }
    }

    void StopHighlight()
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
            highlightCoroutine = null;
        }

        highLightImage.enabled = false;
        ExploreUI.Instance.ChangeDuration(0.5f);
    }

    IEnumerator FlashSprite()
    {
        highLightImage.enabled = true;
        float alphaMin = 0.2f;
        float alphaMax = 0.8f;
        float interval = 0.25f;
        bool isBright = true;

        while (true)
        {
            SetImageAlpha(isBright ? alphaMax : alphaMin);
            isBright = !isBright;
            yield return new WaitForSeconds(interval);
        }
    }

    void SetImageAlpha(float alpha)
    {
        Color color = highLightImage.color;
        color.a = Mathf.Clamp01(alpha);
        highLightImage.color = color;
    }

    void OnCardClicked()
    {
        ExploreManager.Instance.SetExploreCardData(cardData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isMouseOver)
        {
            isMouseOver = true;
            HighLightNextCards(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isMouseOver)
        {
            isMouseOver = false;
            HighLightNextCards(false);
        }
    }

    void UpdateCardUI()
    {
        int cityIndex = GameValue.Instance.GetExploreData().cityIndex;
        var region = GameValue.Instance.GetExploreData().exploreRegion;

        backgroundImage.color = GameValue.Instance.GetCountryColor(region.GetCityCountry(cityIndex));
        cardTypeImage.sprite = GetSprite.GetCardTypeSprite(cardData.cardType);
        LevelText.text = cardData.level.ToString();
        if (cardData.isFront)
        {
            cardTypeText.gameObject.SetActive(true);
            cardTypeText.text = cardData.cardType.ToString();
            cardBoxImage.sprite = GetSprite.GetCardTypeSprite(-1);
        }
        else
        {
            cardTypeText.gameObject.SetActive(false);
            cardBoxImage.sprite = GetSprite.GetCardTypeSprite(-2);
        }
    }


    public void SetExploreCardData(ExploreCardData data)
    {
        this.cardData = data;
        UpdateCardUI();
        SetNextCardsFromData(data.nextCardData);
    }

    void SetNextCardsFromData(List<ExploreCardData> nextData)
    {
        if (nextData == null || nextData.Count == 0)
        {
            GenerateNextCards();
            return;
        }

        for (int i = 0; i < nextCardNodes.Count; i++)
        {
            nextCardNodes[i].SetExploreCardData(nextData[i]);
        }
    }

    void GenerateNextCards()
    {
        if (nextCardNodes == null || nextCardNodes.Count == 0)
            return;
        if (cardData.cardType == "battle")
        {
            var childTypes = new List<string> { "heal", "escape", "item" };
            Shuffle(childTypes);

            for (int i = 0; i < nextCardNodes.Count; i++)
            {
                string type = i < childTypes.Count ? childTypes[i] : "nothing"; 
                var newData = new ExploreCardData(cardData.level + 1, type);

                nextCardNodes[i].SetExploreCardData(newData);
                cardData.nextCardData.Add(newData);
            }
        } else
        {
            for (int i = 0; i < nextCardNodes.Count; i++)
            {
                var child = nextCardNodes[i];
                ExploreCardData exploreCardData;
                if (child.GetCardData() != null && child.GetCardData().level == cardData.level +1)
                {
                    exploreCardData = child.GetCardData();
                }
                else
                {
                    exploreCardData = new ExploreCardData(cardData.level + 1);
                }
                child.SetExploreCardData(exploreCardData);
                cardData.nextCardData.Add(exploreCardData);
            }

        }

    }

    public ExploreCardData GetCardData()
    {
        return cardData;
    }


    /*void SetNextCardsFromData(List<ExploreCardData> nextData)
    {
        // 如果没有数据或数量不对，就重新生成
        if (nextData == null || nextData.Count != nextCardNodes.Count)
        {
            GenerateNextCards();
            return;
        }

        for (int i = 0; i < nextCardNodes.Count; i++)
        {
            nextCardNodes[i].SetExploreCardData(nextData[i]);
        }
    }*/

    /* void GenerateNextCards()
     {
         if (nextCardNodes == null || nextCardNodes.Count == 0)
             return;

         cardData.nextCardData = new List<ExploreCardData>();
         if (cardData.cardType == "battle")
         {
             var childTypes = new List<string> { "heal", "escape", "item" };
             Shuffle(childTypes);

             for (int i = 0; i < nextCardNodes.Count; i++)
             {
                 string type = i < childTypes.Count ? childTypes[i] : "nothing"; // 防止越界
                 var newData = new ExploreCardData(cardData.level + 1, type);

                 nextCardNodes[i].SetExploreCardData(newData);
                 cardData.nextCardData.Add(newData);
             }
         }
         else
         {
             for (int i = 0; i < nextCardNodes.Count; i++)
             {
                 var child = nextCardNodes[i];
                 ExploreCardData exploreCardData = null;


                 if (child.GetNextCardType(i) != null)
                 {
                     exploreCardData = child.cardData.nextCardData[i];
                 }                
                 if (exploreCardData == null)
                 {
                     exploreCardData = new ExploreCardData(cardData.level + 1);
                 }
                 // 设置子节点的数据
                 child.SetExploreCardData(exploreCardData);

                 // 加入当前的 next list
                 cardData.nextCardData.Add(exploreCardData);
             }
         }
     }*/


    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    // --- Utility Accessors ---
    public string GetCardType()
    {
        if (cardData == null) return null;
        return cardData.cardType;
    }

  /*  public string GetNextCardType(int index)
    {
      //  if (cardData == null) return null;
        if (GetCardType() == null) return null;
        if (cardData.nextCardData == null) return null;
        if (cardData.nextCardData.Count <= index) return null;
        if (cardData.nextCardData[index].level != cardData.level+1) return null;

        // if (nextCardNodes.Count == 0) return null;
        Debug.Log($"cardData.nextCardData.Count IS {cardData.nextCardData.Count} AND INDEX IS {index} ");
        return cardData.nextCardData[index].cardType;
    }*/



    public ExploreCardData GetExploreCardData() => cardData;
    public int GetLevel() => cardData?.level ?? -1;
    public void SetLevel(int newLevel) => cardData.level = newLevel;

    // --- Event Handlers (can be refactored further if needed) ---
    public void EventHappen()
    {
        if (cardData.cardType == "nothing") return;

        var panel = Instantiate(eventPanelPrefab);
        panel.transform.localPosition = new Vector3(0, -45, 0);

        var eventPanel = panel.GetComponent<ExploreEventPanel>();
        eventPanel.CreatEvent(cardData.cardType);
        eventPanel.exploreCardNode = this;
    }

    public void TrapEvent() { }
    public void HealEvent() { }
    public void BattleEvent() => SceneManager.LoadScene("BattleScene");
    public void ItemEvent() { }
    public void EscapeEvent() => SceneManager.LoadScene("GameScene");
    public void PassEvent() => SceneManager.LoadScene("GameScene");
    public void MonsterGenerate() { }
}




public class ExploreCardData
{
    private static readonly List<string> cardTypeString = new List<string>
        { "nothing", "trap", "heal", "battle", "item", "escape", "pass" };

    public string cardType = null;
    public bool isFront = true;
    public int level = -1;
    public List<ExploreCardData> nextCardData = new List<ExploreCardData>();

    public ExploreCardData(int level, string forcedCardType = null)
    {
        this.level = level;

        // 判断是否正面显示
        isFront = (level % 100 == 0 && level != 0) ? true : (UnityEngine.Random.value < 0.7f);

        // 如果外部强制指定 cardType，则使用
        if (!string.IsNullOrEmpty(forcedCardType))
        {
            cardType = forcedCardType;
        }
        else
        {
            cardType = GenerateCardType();
        }

        nextCardData = new List<ExploreCardData>();
    }

    private string GenerateCardType()
    {
        if (level % 100 == 0 && level != 0)
            return "pass";

        int index = UnityEngine.Random.Range(0, 6); // 不包含 pass
        return cardTypeString[index];
    }

    public string GetTypeDescribe()
    {
        switch (cardType)
        {
            case "nothing": return "Nothing Happend";
            case "trap": return "Trap happend";
            case "heal": return "Heal happend";
            case "battle": return "Battle happend";
            case "item": return "Get item !";
            case "escape": return "Escape";
            case "pass": return "Pass Explore";
        }
        return "Bug in ExploreCardData, not type descrite";
    }
}
