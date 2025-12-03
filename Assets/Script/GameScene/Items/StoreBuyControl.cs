using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreBuyControl : MonoBehaviour, IItemPanel
{
    private GameValue gameValue;

    private ProductPreFabControl currentProduct;

    private ProductPreFabControl starProduct;

    private List<ProductPreFabControl> buyProducts = new List<ProductPreFabControl>();

    [Header("UI")]
    public Image itemImage;
    public Button starButton;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemEffect;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI itemDescribe;
    public BuyStoreButtomControl buyStoreButtomControl;
    public ItemTopColumnButton itemTopColumnButton;

    void Start()
    {
        gameValue = GameValue.Instance;
        starButton.onClick.AddListener(OnStarButtonClick);
        UpCurrentProductUI();
    }

    // =============== ???? =================
    void OnStarButtonClick()
    {
        if (currentProduct == null) return;
        currentProduct.SetIsStar(!currentProduct.GetIsStar());
        SetStarProduct(currentProduct);
        UpStarButtonSprite();
    }

    public void SetStarProduct(ProductPreFabControl product)
    {
        starProduct = product;
    }

    public ProductPreFabControl GetStarProduct()
    {
        return starProduct;
    }

    public void UpStarButtonSprite()
    {
        string iconPath = "MyDraw/UI/Other/";
        if (currentProduct == null)
        {
            starButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar");
            starButton.interactable = false;
            return;
        }

        starButton.interactable = true;
        if (currentProduct.GetIsStar())
            starButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Star");
        else
            starButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(iconPath + "Unstar");
    }

    // =============== ???? =================
    public void SetCurrentProduct(ProductPreFabControl currentProduct)
    {
        this.currentProduct = currentProduct;
        UpCurrentProductUI();
    }

    public ProductPreFabControl GetCurrentProduct()
    {
        return currentProduct;
    }

    void UpCurrentProductUI()
    {
        if (currentProduct == null)
        {
            itemImage.sprite = Resources.Load<Sprite>("MyDraw/Item/EmptyItemBuy");
            itemName.gameObject.SetActive(false);
            itemPrice.gameObject.SetActive(false);
            itemDescribe.text = null;
            itemEffect.text = null;
        }
        else
        {
            ItemBase item = currentProduct.GetProduct().GetItemBase();
            itemImage.sprite = item.icon;

            itemName.gameObject.SetActive(true);
            itemName.text = item.GetItemName();

            itemPrice.gameObject.SetActive(true);
            itemPrice.text = currentProduct.GetPrice().ToString("N0");

            itemDescribe.text = item.GetItemDescription();
            itemEffect.text = item.GetEffectDescription();
        }
        UpStarButtonSprite();
    }

    // =============== ???? =================
    public void AddBuyProducts(ProductPreFabControl productPreFab)
    {
        if (!buyProducts.Contains(productPreFab))
        {
            buyProducts.Add(productPreFab);
            buyStoreButtomControl.SetSpentPrice(buyProducts);
        }
    }

    public void RemoveBuyProducts(ProductPreFabControl productPreFab)
    {
        if (buyProducts.Contains(productPreFab))
        {
            buyProducts.Remove(productPreFab);
            buyStoreButtomControl.SetSpentPrice(buyProducts);
        }
    }

    public void ClearBuyProducts()
    {
        buyProducts.Clear();
        buyStoreButtomControl.SetSpentPrice(buyProducts);
    }

    float CalculatePrice()
    {
        float totalPrice = 0;
        foreach (var product in buyProducts)
        {
            totalPrice += product.GetPrice();
        }
        return totalPrice;
    }

    // =============== ???? =================
    public void ShowPanel()
    {
        gameObject.SetActive(true);
        itemTopColumnButton.DisplayBuy();
        UpCurrentProductUI();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
