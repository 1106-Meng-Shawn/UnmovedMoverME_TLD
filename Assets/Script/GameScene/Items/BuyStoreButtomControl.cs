using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FormatNumber;

public class BuyStoreButtomControl : MonoBehaviour
{
    public Slider negotiationSlider;
    public TextMeshProUGUI initTotalPrice;
    public TextMeshProUGUI negotiationText;
    public TextMeshProUGUI finalTotalPrice;

    public Button CheckButton;
    public Button negotiationButton;

    public CharacterAssistRowControl characterAssistRowControl;
    public List<ProductPreFabControl> buyProducts;

    private GameValue gameValue;

    private float initPrice = 0;
    private float negotiationUsed = 0;
    private float finalPrice = 0;

    public StoreBuyControl storeBuyControl;
    public ItemTopColumnButton itemTopColumnButton;
    public ItemPanel itemPanel;


    private void Start()
    {
        gameValue = GameValue.Instance;
        CheckButton.onClick.AddListener(OnCheckButtonClick);
        UpUIData();
    }


    public void SetSpentPrice(List<ProductPreFabControl> products)
    {
        this.buyProducts = null;
        this.buyProducts = products;
        this.initPrice = CalculatePrice();
        UpUIData();
    }

    float CalculatePrice()
    {
        float TotalPrice = 0;
        for (int i = 0; i < buyProducts.Count; i++)
        {
            TotalPrice += buyProducts[i].GetPrice();
        }

        return TotalPrice;
    }



    void OnCheckButtonClick()
    {
        if (finalPrice < gameValue.GetResourceValue().Gold)
        {
            initPrice = 0;

            gameValue.GetResourceValue().Negotiation -= negotiationUsed;
            negotiationUsed = 0;

            gameValue.GetResourceValue().Gold -= finalPrice;
            finalPrice = 0;
            UpProduct();
            UpUIData();
        }
    }

    void UpProduct()
    {

        foreach (var product in buyProducts)
        {
            if (product == storeBuyControl.GetCurrentProduct())
                storeBuyControl.SetCurrentProduct(null);

            ItemBase targetItem = product.GetProduct().GetItemBase();
            GameValue.Instance.GetItem(targetItem.GetID()).ItemNumAdd(1);
        }

        itemTopColumnButton.ItemDisplay();
        itemPanel.UpItemPanelUI();
        storeBuyControl.ClearBuyProducts();
    }

    void UpUIData()
    {
     initTotalPrice.text = FormatNumberToString(initPrice) + " - ";
     negotiationText.text = FormatNumberToString(negotiationUsed) + " = ";
     finalTotalPrice.text = FormatNumberToString(CalculateFinalPrice());
      if (gameValue.GetResourceValue().Gold < finalPrice) { finalTotalPrice.color = Color.red; }
      else { finalTotalPrice.color = Color.green; }
    }


    float CalculateFinalPrice()
    {
        finalPrice = initPrice - negotiationUsed * 1000;

        return finalPrice;

    }

}
