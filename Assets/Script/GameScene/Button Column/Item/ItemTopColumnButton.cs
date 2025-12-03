using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GetSprite;

public class ItemTopColumnButton : MonoBehaviour
{
    public bool isStorage;
    public bool isBuy;
    public bool isSell;


    public ItemPanelManager itemPanelManger;
    public ItemPanel itemPanel;
    [SerializeField] private StoreBuyControl storeBuyControl;


    public List<ItemPrefabControl> items = new List<ItemPrefabControl>();
    private List<ProductPreFabControl> products = new List<ProductPreFabControl>();


    public GameObject itemPrefab;

    public ScrollRect scrollRect;

    [Header("Filter Buttons")]
    public Button starButton;
    private bool isStar = false;
    [Header("Sort Buttons")]
    public Button nameSortButton;
    public Button numSortButton;
    public Button remainingNumSortButton;
    public Button rareSortButton;
    public Button priceSortButton;

    private SortField currentSortField = SortField.None;
    private SortState currentSortState = SortState.None;

    enum SortField { None, Name, Num, RemainingNum, Rare, Price }
    enum SortState { None, Ascending, Descending }

    void Start()
    {
        ItemDisplay();
        if (nameSortButton != null) nameSortButton.onClick.AddListener(() => OnClickSort(SortField.Name));
        if (numSortButton != null)  numSortButton.onClick.AddListener(() => OnClickSort(SortField.Num));
        if (remainingNumSortButton != null) remainingNumSortButton.onClick.AddListener(() => OnClickSort(SortField.RemainingNum));
        if (rareSortButton != null) rareSortButton.onClick.AddListener(() => OnClickSort(SortField.Rare));
        if (priceSortButton != null) priceSortButton.onClick.AddListener(() => OnClickSort(SortField.Price));
        starButton.onClick.AddListener(OnStarButtonClick);
    }

    void OnStarButtonClick()
    {
        isStar = !isStar;
        starButton.image.sprite = UpStarButtonSprite(isStar);
        RefreshItems();
    }



    void OnClickSort(SortField field)
    {
        if (currentSortField == field)
        {
            currentSortState = NextSortState(currentSortState);
        }
        else
        {
            currentSortField = field;
            currentSortState = SortState.Descending; // ???????
        }
        RefreshItems();
    }

    SortState NextSortState(SortState state) => state switch
    {
        SortState.None => SortState.Descending,
        SortState.Descending => SortState.Ascending,
        SortState.Ascending => SortState.None,
        _ => SortState.None
    };


    void UpdateSortIcons()
    {
        if (nameSortButton != null) nameSortButton.GetComponent<Image>().sprite = GetSortSprite(currentSortField == SortField.Name ? currentSortState.ToString() : SortState.None.ToString());

        if (numSortButton != null) numSortButton.GetComponent<Image>().sprite = GetSortSprite(currentSortField == SortField.Num ? currentSortState.ToString() : SortState.None.ToString());

        if (remainingNumSortButton != null) remainingNumSortButton.GetComponent<Image>().sprite = GetSortSprite(currentSortField == SortField.RemainingNum ? currentSortState.ToString() : SortState.None.ToString());

        if (rareSortButton != null) rareSortButton.GetComponent<Image>().sprite = GetSortSprite(currentSortField == SortField.Rare ? currentSortState.ToString() : SortState.None.ToString());

        if (priceSortButton != null) priceSortButton.GetComponent<Image>().sprite = GetSortSprite(currentSortField == SortField.Price ? currentSortState.ToString() : SortState.None.ToString());

    }


    public void ItemDisplay()
    {
        if (isStorage) DisplayStorage();
        if (isBuy) DisplayBuy();


    }
    public void DisplayBuy()
    {
        // 清空原有 UI
        foreach (Transform child in scrollRect.content)
        {
            Destroy(child.gameObject);
        }
        products.Clear();

        // 随机生成 10 个商品
        for (int i = 0; i < 10; i++)
        {
            int randomIndex = Random.Range(0, GameValue.Instance.GetStoreCanSellItem().Count);
            ItemBase product = GameValue.Instance.GetStoreCanSellItem()[randomIndex];
            GameObject newProduct = Instantiate(itemPrefab, scrollRect.content);
            ProductPreFabControl control = newProduct.GetComponent<ProductPreFabControl>();
            if (control != null)
            {
                control.SetProduct(product, 10, storeBuyControl);
                products.Add(control);
            }
            GameValue.Instance.AddSellItem(product);

        }

        RefreshBuyItems();
    }

    void DisplayStorage()
    {
        foreach (Transform child in scrollRect.content)
        {
            Destroy(child.gameObject);
        }

        items.Clear();

        foreach (ItemBase item in GameValue.Instance.GetPlayerItems())
        {
            GameObject newItem = Instantiate(itemPrefab, scrollRect.content);
            ItemPrefabControl control = newItem.GetComponent<ItemPrefabControl>();
            if (control != null)
            {
                control.SetItem(item, itemPanel, itemPanelManger);
                items.Add(control);
            }
        }
        RefreshItems();
    }

    void RefreshItems()
    {
        if (isStorage) RefreshStorageItems();
        if (isBuy) RefreshBuyItems();

    }
    public void RefreshStorageItems()
    {
        IEnumerable<ItemPrefabControl> filtered = items;

        if (isStar)
            filtered = filtered.Where(i => i.GetItem().IsStar);
        filtered = (currentSortField, currentSortState) switch
        {
            (SortField.Num, SortState.Ascending) => filtered.OrderBy(i => i.GetItem().GetPlayerHasCount()),
            (SortField.Num, SortState.Descending) => filtered.OrderByDescending(i => i.GetItem().GetPlayerHasCount()),

            (SortField.RemainingNum, SortState.Ascending) => filtered.OrderBy(i => i.GetItem().GetRemainingNum()),
            (SortField.RemainingNum, SortState.Descending) => filtered.OrderByDescending(i => i.GetItem().GetRemainingNum()),

            (SortField.Rare, SortState.Ascending) => filtered.OrderBy(i => i.GetItem().rare),
            (SortField.Rare, SortState.Descending) => filtered.OrderByDescending(i => i.GetItem().rare),

            (SortField.Price, SortState.Ascending) => filtered.OrderBy(i => i.GetItem().price),
            (SortField.Price, SortState.Descending) => filtered.OrderByDescending(i => i.GetItem().price),

            _ => filtered
        };

        int index = 0;
        foreach (var control in filtered)
        {
            control.gameObject.SetActive(true);
            control.transform.SetSiblingIndex(index++);
        }

        foreach (var control in items.Except(filtered))
        {
            control.gameObject.SetActive(false);
        }

        UpdateSortIcons();
    }

    public void RefreshBuyItems()
    {
        IEnumerable<ProductPreFabControl> filtered = products;

        if (isStar)
            filtered = filtered.Where(i => i.GetProductIsStar());
        filtered = (currentSortField, currentSortState) switch
        {
            (SortField.Name, SortState.Ascending) => filtered.OrderByDescending(i => i.GetProductName()), 
            (SortField.Name, SortState.Descending) => filtered.OrderBy(i => i.GetProductName()),

            (SortField.Rare, SortState.Ascending) => filtered.OrderBy(i => i.GetRare()),
            (SortField.Rare, SortState.Descending) => filtered.OrderByDescending(i => i.GetRare()),

            (SortField.Price, SortState.Ascending) => filtered.OrderBy(i => i.GetPrice()),
            (SortField.Price, SortState.Descending) => filtered.OrderByDescending(i => i.GetPrice()),

            _ => filtered
        };

        int index = 0;
        foreach (var control in filtered)
        {
            control.gameObject.SetActive(true);
            control.transform.SetSiblingIndex(index++);
        }

        foreach (var control in products.Except(filtered))
        {
            control.gameObject.SetActive(false);
        }

        UpdateSortIcons();
    }
}
