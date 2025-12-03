using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static FormatNumber;
using UnityEngine.Localization.Settings;

public class ProductPreFabControl : MonoBehaviour
{
    private ProductData productData;
    private StoreBuyControl storeBuyControl;

    public Image itemImage;
    public TextMeshProUGUI productTitle;
    public TextMeshProUGUI productEffct;
    public TextMeshProUGUI productPrice;


    public Image productTotal;
    public Button ProductButton;
    public Button starButton;
    private bool isEditing = false;

    private bool isSelect = false;

    private void Start()
    {
        ButtonInit();
    }

 
    void ButtonInit()
    {
        starButton.onClick.AddListener(OnStarButtonClick);
        ProductButton.onClick.AddListener(OnProductButtonClick);
    }

    void OnStarButtonClick()
    {
        productData.isStar = !productData.isStar;
        UpdateStarButtonSprite();
        storeBuyControl.SetCurrentProduct(this);

    }

    void OnProductButtonClick()
    {
        storeBuyControl.SetCurrentProduct(this);
        isSelect = !isSelect;
        if (isSelect)
        {
            productTotal.color = Color.red;
            storeBuyControl.AddBuyProducts(this);
        }
        else
        {
            productTotal.color = Color.white;
            storeBuyControl.RemoveBuyProducts(this);

        }
    }

    void UpdateStarButtonSprite()
    {
        string imagePath = "MyDraw/UI/Other/";
        string spriteName = productData.isStar ? "Star" : "Unstar";
        starButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath + spriteName);
    }

    public bool GetIsStar() => productData.isStar;
    public void SetIsStar(bool isStar)
    {
        productData.isStar = isStar;
        UpdateStarButtonSprite();
    }


 
    public void SetProduct(ItemBase item, float priceFloat, StoreBuyControl storeBuyControl)
    {
        productData = new ProductData(item,priceFloat);
        this.storeBuyControl = storeBuyControl;
        UpdateUI();
    }


    public ProductData GetProduct() { return productData; }
    ItemBase GetItem() => productData.GetItemBase();

    void UpdateUI()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        itemImage.sprite = GetItem().icon;
        productTitle.text = GetItem().GetItemName();
        productEffct.text = GetItem().GetEffectDescription();
        ChangePriceText();
    }


    void ChangePriceText()
    {
        productPrice.text = FormatNumberToString(productData.GetPrice()); // itemBase.price maybe up
    }

    public bool GetProductIsStar()
    {
        return productData.isStar;
    }

    public string GetProductName()
    {
        return productData.GetProductName();
    }

    public int GetRare()
    {
        return productData.GetRare();
    }

    public float GetPrice() {
        return productData.GetPrice();
    }
}


public class ProductData
{
    private ItemBase itemBase;
    private float priceFloat;
    public RegionValue Originregion;
    public bool isStar;
    public string BusinessGroup;// wait haven't write yet

    public ProductData(ItemBase itemBase, float priceFloat) {
        this.itemBase = itemBase;
        this.priceFloat = priceFloat;
        isStar = false;
    }

    public ItemBase GetItemBase()
    {
        return itemBase;
    }


    public string GetProductName()
    {
        return itemBase.GetItemName();
    }

    public float GetPrice()
    {
        return priceFloat;
    }

    public int GetRare()
    {
        return itemBase.rare;
    }

}