using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static ExcelReader;
using static ItemBase;
using static EnumHelper;

#region Value

public static class ItemConstants
{
    public const int NoneItem = -1;
    public const int WoodenSwordID = 0;
    public const int WoodenShieldID = 1;
    public const int BootsID = 2;
    public const int FoodBookID = 3;
    public const int TornFoodBookID = 4;
    public const int KronungsevangeliarID = 5;
    public const int GlovesID = 6;
    public const int ReichsapfelID = 7;
    public const int ReichskroneID = 8;
    public const int ReichssehwertID = 9;
    public const int ReichszepterID = 10;
    public const int ZeremonienschwertID = 11;
    public const int MaidOutfitID = 12;
    public const int HairTieID = 13;
    public const int BrokenHairTieID = 14;

    public const int NoneID = -1;
}


public enum ItemType
{
    Equipment, OneTime, UIChange
}

#endregion


public class ItemBase
{
    private int ID;
    private Dictionary<string, string> itemName = new();
    private bool isStar;
    public bool IsStar
    {
        get => isStar;
        set => SetProperty(ref isStar, value);
    }

    private Dictionary<string, string> effectDescribe = new();
    private Dictionary<string, string> itemDescribe = new();
    public int maxInStore;
    public int rare;
    public float price;
    public Sprite icon;
    public List<ItemOwnerInfo> Owners = new List<ItemOwnerInfo>();
    private ItemType itemType;
    public event Action OnItemValueChange;


    public class ItemOwnerInfo
    {
        public string OwnerCountry;
        public string EquippedByCharacterKey;

        public ItemOwnerInfo(string Country)
        {
            OwnerCountry = Country;
            EquippedByCharacterKey = CharacterConstants.NoneKey;
        }
    }



    protected bool SetProperty<T>(ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnItemValueChange?.Invoke();
            return true;
        }
        return false;
    }

    public ItemBase(ExcelItemData excelItemData)
    {
        IsStar = false;
        ID = excelItemData.ID;
        maxInStore = excelItemData.maxInStore;
        rare = excelItemData.rare;
        price = excelItemData.price;

        Owners = new List<ItemOwnerInfo>();
        Owners = new List<ItemOwnerInfo>();
        for (int i = 0; i < excelItemData.num; i++)
        {
            Owners.Add(new ItemOwnerInfo(GameValue.Instance.GetPlayerCountryENName())); 
        }
        itemType = ParseEnumOrDefault<ItemType>(excelItemData.itemType);
        itemName = excelItemData.itemNames;
        effectDescribe = excelItemData.effectDescribe;
        itemDescribe = excelItemData.itemDescribe;

        string itemNameNoSpace = GetItemName().Replace(" ", "");
        icon = Resources.Load<Sprite>($"MyDraw/item/{rare}/{itemNameNoSpace}");

    }

    
    public void SetSaveData(ItemSaveData itemSaveData)
    {
        isStar = itemSaveData.star;
        Owners = itemSaveData.Owners;
    }

    public void UseEffect(Character character)
    {
        switch (GetID())
        {
            case ItemConstants.MaidOutfitID: MaidOutfit(character); break;

        }

    }

    public void UseRemoveEffect(Character character)
    {
        switch (GetID())
        {
            case ItemConstants.MaidOutfitID: MaidOutfitRemove(character); break;

        }

    }

    public void UseOneTimeEffect(Character character)
    {
        bool success = false;
        switch (GetID())
        {
            case ItemConstants.TornFoodBookID: success = TornFoodBook(character); break;
            case ItemConstants.HairTieID: success = HairTie(character); break;
            case ItemConstants.BrokenHairTieID: success = BrokenHairTie(character);break;
        }
        if (success) ItemNumDecrease(1);
    }

    public void RecordCharacter(Character character)
    {

        if (character.GetCountryENName() == GameValue.Instance.GetPlayerCountryENName())
        {
            var owner = Owners.FirstOrDefault(o => o.OwnerCountry == GameValue.Instance.GetPlayerCountryENName());
            if (owner != null)
            {
                owner.EquippedByCharacterKey = character.GetCharacterKey();
            }
            else
            {
                var newOwner = new ItemOwnerInfo(character.GetCountryENName());
                newOwner.EquippedByCharacterKey = character.GetCharacterKey();
                Owners.Add(newOwner);
            }
        }

        OnItemValueChange?.Invoke();
    }
    public void RemoveCharacter(Character character)
    {
        if (character.GetCountryENName() == GameValue.Instance.GetPlayerCountryENName())
        {
            var owner = Owners.FirstOrDefault(o => o.OwnerCountry == GameValue.Instance.GetPlayerCountryENName() && o.EquippedByCharacterKey == character.GetCharacterKey());
            if (owner != null)
            {
                if (owner.EquippedByCharacterKey == character.GetCharacterKey())
                {
                    if(itemType != ItemType.OneTime) owner.EquippedByCharacterKey = CharacterConstants.NoneKey;
                }
            }
        }
        UseRemoveEffect(character);
        OnItemValueChange?.Invoke();
    }

    public bool TornFoodBook(Character character)
    {
        character.AddValue(1, 0, 1);
        return true;
    }
    public int UseItemValue(int type, int index)
    {
        switch (GetID())
        {
            case ItemConstants.WoodenSwordID: return WoodSword(type, index);
            case ItemConstants.WoodenShieldID: return WoodenShield(type, index);
            case ItemConstants.BootsID: return Boots(type, index);
            case ItemConstants.FoodBookID: return FoodBook(type, index);
        }
        return 0;
    }
   /* public void UseUIChangeEffect()
    {
        /*   switch (GetItemENName())
           {
               case "Kronungsevangeliar": Kronungsevangeliar(); break;
               case "Gloves": Gloves(); break;
               case "Reichsapfel": Reichsapfel(); break;
               case "Reichskrone": Reichskrone();break;
               case "Reichssehwert": Reichssehwert(); break;
               case "Zeremonienschwert": Zeremonienschwert(); break;
               case "Reichszepter": Reichszepter(); break;
           }
    }*/


    private int WoodSword(int type, int index)
    {
        if (type == 0 && index == 0) return 1;
        return 0;
    }
    private int WoodenShield(int type, int index)
    {
        if (type == 0 && index == 1) return 1;
        return 0;
    }
    private int Boots(int type, int index)
    {
        if (type == 0 && index == 3) return 1;
        return 0;
    }
    private int FoodBook(int type, int index)
    {
        if (type == 1 && index == 0) return 1;
        return 0;
    }

    public bool TriggerEffect(ItemType itemType)
    {
        return this.itemType == itemType;
    }

    #region Get

    public int GetID()
    {
        return ID;
    }

    public string GetItemENName()
    {
        return itemName[LanguageCode.EN];
    }

    public string GetItemName()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return itemName.TryGetValue(currentLanguage, out var text) ? text : itemName[LanguageCode.EN];
    }

    public string GetItemNameWithColorString()
    {
        string currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
        return itemName.TryGetValue(currentLanguage, out var text) ? text : itemName[LanguageCode.EN];
    }



    public bool CanSell()
    {
        List<int> storeItems = GameValue.Instance.GetStoreSellItems();
        int count = storeItems.Count(itemId => itemId == ID);
        return count < maxInStore;
    }
    public int GetPlayerUsed()
    {
        return Owners.Count(owner =>
            owner.OwnerCountry == GameValue.Instance.GetPlayerCountryENName() && owner.EquippedByCharacterKey != CharacterConstants.NoneKey);
    }

    public int GetRemainingNum()
    {
        return GetPlayerHasCount() - GetPlayerUsed();
    }


    public string GetEffectDescription()
    {
        string langCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        return effectDescribe.TryGetValue(langCode, out var text) ? text : effectDescribe[LanguageCode.EN];
    }

    public string GetItemDescription()
    {
        string langCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        return itemDescribe.TryGetValue(langCode, out var text) ? text : itemDescribe[LanguageCode.EN];
    }

    #endregion

    public bool PlayerHas()
    {
        return Owners.Any(owner => owner.OwnerCountry == GameValue.Instance.GetPlayerCountryENName());
    }

    public int GetPlayerHasCount()
    {
        return Owners.Count(o => o.OwnerCountry == GameValue.Instance.GetPlayerCountryENName());
    }

    public void ItemNumAdd(int count)
    {
        for (int i = 0; i < count; i++)
        {
            ItemOwnerInfo ownerInfo = new ItemOwnerInfo(GameValue.Instance.GetPlayerCountryENName());
            Owners.Add(ownerInfo);
        }
        GameValue.Instance.OnItemsChange();
        /* if (itemType == ItemType.UIChange)
             UseUIChangeEffect();*/
    }

    public void ItemNumDecrease(int count)
    {
        string playerCountry = GameValue.Instance.GetPlayerCountryENName();

        for (int i = 0; i < count; i++)
        {
            var owner = Owners.FirstOrDefault(o =>o.OwnerCountry == playerCountry && o.EquippedByCharacterKey == CharacterConstants.NoneKey);
            if (owner != null)
            {
                Owners.Remove(owner);
            }
            else
            {
                break; 
            }
        }
        GameValue.Instance.OnItemsChange();
        ;
    }


    #region Effect
    void MaidOutfit(Character character)
    {
        switch (character.GetCharacterID())
        {
            case 0:
            case 27:
                character.SetCharacterType(character.Type + "Maid");
                Debug.Log($"{character.Type}");
                break;
        }
    }

    void MaidOutfitRemove(Character character)
    {
        switch (character.GetCharacterID())
        {
            case 0:
            case 27:
                var type = character.Type;
                if (type.EndsWith("_Maid"))
                {
                    type = type.Substring(0, type.Length - "Maid".Length);
                }
                character.SetCharacterType(type);
                break;
        }
    }

    bool HairTie(Character character)
    {
        switch (character.GetCharacterID())
        {
            case 0:
            case 27:
                var type = character.Type;
                if (type.Contains("_Ponytail"))
                    return false;

                int underscoreIndex = type.IndexOf('_');
                if (underscoreIndex >= 0)
                {
                    type = type.Insert(underscoreIndex, "_Ponytail");
                }
                else
                {
                    type += "_Ponytail";
                }

                character.SetCharacterType(type);
                GameValue.Instance.GetItem(14).ItemNumAdd(1);
            return true;
                break;
        }

        return false;
    }

    bool BrokenHairTie(Character character)
    {
        switch (character.GetCharacterID())
        {
            case 0:
            case 27:
                var type = character.Type;
                if (!type.Contains("_Ponytail"))
                    return false;
                int ponytailIndex = type.IndexOf("_Ponytail");
                if (ponytailIndex >= 0)
                {
                    type = type.Remove(ponytailIndex, "_Ponytail".Length);
                    character.SetCharacterType(type);
                    GameValue.Instance.GetItem(13).ItemNumAdd(1);
                    return true;
                }
                break;
        }

        return false;
    }

    #endregion
}

public class ItemSaveData
{
    public int ID;
    public bool star;
    public List<ItemOwnerInfo> Owners = new List<ItemOwnerInfo>();

    public ItemSaveData(ItemBase itemBase)
    {
        if (itemBase == null)
        {
            return;
        }
        this.ID = itemBase.GetID();
        this.star = itemBase.IsStar;
        Owners = itemBase.Owners;
    }
}
