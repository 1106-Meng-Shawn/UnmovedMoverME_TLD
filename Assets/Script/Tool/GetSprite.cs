using System;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static GetSprite;
public class GetSprite
{
    public static Sprite GetValueTypeSprite(int index)
    {
        switch (index)
        {
            case 0: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/BattleValueIcon");
            case 1: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/ParameterValueIcon");
            case 2: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/HelpValueIcon");
        }

        return null;
    }

    public static Sprite GetCharacterIcon(string characterKey, string type)
    {
        string characterNameNoSpace = type.Replace(" ", "");
        return GameValue.Instance.GetCharacterIcon(characterKey, type);
    }

    public static Sprite GetCharacterImage(string characterKey, string type)
    {
        string characterNameNoSpace = type.Replace(" ", "");
        return GameValue.Instance.GetCharacterImage(characterKey, type);
    }

    public static Sprite GetForceSprite(int index)
    {
        switch (index)
        {
            case 0: return Resources.Load<Sprite>($"MyDraw/UI/CharacterValueIcon/Force");
            case 1: return Resources.Load<Sprite>($"MyDraw/UI/CharacterValueIcon/Health");
            case 2: return Resources.Load<Sprite>($"MyDraw/UI/CharacterValueIcon/ForceLimit");
        }

        return null;
    }

    public static Sprite[] GetBattleValueSpriteSheet()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("MyDraw/Effects/BattleValue_Sheet");
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("? cant find Sprite GetValueSpriteSheet()");
        }
        return sprites;
    }


    public static TMP_SpriteAsset LoadBattleValueSpriteAsset()
    {
        TMP_SpriteAsset spriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/BattleValue_Sheet");
        if (spriteAsset == null)
        {
            Debug.LogError($"? cant find TMP Sprite Asset: Assets/Value_Sheet");
        }
        return spriteAsset;
    }


    public static Sprite[] GetValueSpriteSheet()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("MyDraw/Effects/Value_Sheet");
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("? cant find Sprite GetValueSpriteSheet()");
        }
        return sprites;
    }


    public static TMP_SpriteAsset LoadValueSpriteAsset()
    {
        TMP_SpriteAsset spriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Value_Sheet");
        if (spriteAsset == null)
        {
            Debug.LogError($"? cant find TMP Sprite Asset: Assets/Value_Sheet");
        }
        return spriteAsset;
    }


    public static Sprite GetMuralSprite(string muralName)
    {
        Sprite muralSprite = Resources.Load<Sprite>($"MyDraw/UI/Mural/{muralName}");
        if (muralSprite == null)
        {
            Debug.Log($"{muralName}");
        }
        return muralSprite;
    }



    public static Sprite GetRegionValueSprite(int valueType)
    {
        //   valueType = valueType.ToLower();
        //  string iconPath = $"MyDraw/UI/Region/Build/";
        switch (valueType)
        {
            case 0: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Food/Food");
            case 1: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Science/Science");
            case 2: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Politics/Politics");
            case 3: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Gold/Gold");
            case 4: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Faith/Faith");
            case 5: return Resources.Load<Sprite>($"MyDraw/UI/Character/Population/RegionCurrentPopulation");

        }

        return null;
    }


    public static Sprite GetCharacterStateSprite(string type)
    {
        string iconPath = $"MyDraw/UI/Other/";
        type = type.ToLower();
        switch (type)
        {
            case "move": return Resources.Load<Sprite>($"{iconPath}MovePoint");
            case "cantmove": return Resources.Load<Sprite>($"{iconPath}MovePointCant");
            case "lord": return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/LordHint");
        }

        return null;
    }

    public static Sprite GetCharacterTag(CharacterTag tagType)
    {
        string iconPath = $"MyDraw/UI/Character/CharacterValueIcon/";

        switch (tagType)
        {
            case CharacterTag.None: return Resources.Load<Sprite>(iconPath + "CharacterIcon");
            case CharacterTag.Battle: return Resources.Load<Sprite>(iconPath + "BattleIcon");
            case CharacterTag.Explore: return Resources.Load<Sprite>($"MyDraw/UI/Other/StartExplore");
            case CharacterTag.Lord: return GetCharacterStateSprite("lord");
            case CharacterTag.Help: return Resources.Load<Sprite>(iconPath + "HelpIcon");

        }
        return null;

    }


    public static Sprite GetMark(string name)
    {
        string iconPath = $"MyDraw/UI/Other/";

        name = name.ToLower();
        switch (name)
        {
            case "exclamation": return Resources.Load<Sprite>(iconPath+ "RedExclamationMark");
            case "redpoint": return Resources.Load<Sprite>(iconPath + "RedPointMark");
        }
        return null;

    }

    public static Sprite GetLastResourceTaxIcon(int index)
    {
        string name = null;
        switch (index)
        {
            case 0: name = "Food"; break;
            case 1: name = "Science"; break;
            case 2: name = "Politics"; break;
            case 3: name = "Gold"; break;
            case 4: name = "Faith"; break;

        }

        string iconPath = $"MyDraw/UI/Region/Value/{name}/{name}Take";
        return Resources.Load<Sprite>(iconPath);
    }

    public static Sprite GetSortSprite(string type)
    {
        string iconPath = $"MyDraw/UI/Other/";
        type = type.ToLower();
        switch (type) {
            case "none": return Resources.Load<Sprite>(iconPath+"SortNone");
            case "descending": return Resources.Load<Sprite>(iconPath + "SortDescending");
            case "ascending": return Resources.Load<Sprite>(iconPath + "SortAscending");
        }

        return null;
    }


    public static Sprite GetBattleForceType(string type)
    {
        type = type.ToLower();

        switch (type)
        {
            case "max": return Resources.Load<Sprite>($"MyDraw/UI/Character/Population/MaxForce");
            case "health": return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Health");
            case "force": return Resources.Load<Sprite>($"MyDraw/UI/Character/Population/RecruitmentPopulation");
        }

        return null;
    }



    public static Sprite GetCitySprite(int cityIndex)
    {
        string iconPath = $"MyDraw/UI/Region/Build/";
        if (cityIndex == 0)
        {
            return Resources.Load<Sprite>(iconPath + "CityStarMain");

        }
        else
        {
            return Resources.Load<Sprite>(iconPath + "CityStar");

        }
    }


    public static Sprite GetCurrentPopulationIcon()
    {
        string iconPath = $"MyDraw/UI/Character/Population/RegionCurrentPopulation";
        return Resources.Load<Sprite>(iconPath);
    }


    public static Sprite GetCharacterStorySprite(string characterName,string iconName)
    {
        string iconPath = $"MyDraw/Character/{characterName}/{iconName}StandIcon";
        return Resources.Load<Sprite>(iconPath);
    }


    public static Sprite GetSupportRateSprite()
    {
        string iconPath = $"MyDraw/UI/Character/Population/SupportRate";
        return Resources.Load<Sprite>(iconPath);

    }



    public static Sprite GetRecruitmentPopulationSprite()
    {
        string iconPath = $"MyDraw/UI/Character/Population/RecruitmentPopulation";
        return Resources.Load<Sprite>(iconPath);

    }



    public static Sprite UpStarButtonSprite(bool isStar)
    {
        string iconPath = $"MyDraw/UI/Other/";
        if (isStar) { return Resources.Load<Sprite>(iconPath + "Star"); }
        else { return Resources.Load<Sprite>(iconPath + "Unstar"); }
    }

    public static Sprite UpDeleteButtonSprite(bool isStar)
    {
        string iconPath = $"MyDraw/UI/Other/";
        if (isStar) { return Resources.Load<Sprite>(iconPath + "Star"); }
        else { return Resources.Load<Sprite>(iconPath + "Unstar"); }
    }




    public static Sprite UpBattleButtonSprite(bool isPerson)
    {
        string iconPath = $"MyDraw/UI/Character/";
        if (isPerson) { 
            return Resources.Load<Sprite>(iconPath + "CharacterValueIcon/Health"); 
        }else { 

            return Resources.Load<Sprite>(iconPath + "Population/RecruitmentPopulation");
        }
    }



    public static Sprite GetSpriteAtUIOther(string name)
    {
        string iconPath = $"MyDraw/UI/Other/";
        return Resources.Load<Sprite>(iconPath + name);
    }


    public static Sprite GetCharacterValueSprite(int index)
    {
        string iconPath = $"MyDraw/UI/Character/CharacterValueIcon/";
        string name = null;
        switch (index)
        {
            case 0:name = "BattleValueIcon"; break;
            case 1: name = "ParameterValueIcon"; break;
            case 2: name = "HelpValueIcon"; break;

        }


        return Resources.Load<Sprite>(iconPath + name);
    }


    public static Sprite GetBuildingSprite(string buildType)
    {
        string iconPath = $"MyDraw/UI/Region/Build/";
        return Resources.Load<Sprite>(iconPath + buildType);
    }

    public static Sprite GetNoLordSprite()
    {
        string iconPath = $"MyDraw/UI/GameUI/";
        return Resources.Load<Sprite>(iconPath + "emptyLord");
    }

    public static Sprite GetCharacterIconSprite()
    {
        string iconPath = $"MyDraw/UI/Character/CharacterValueIcon/";
        return Resources.Load<Sprite>(iconPath + "CharacterIcon");
    }


    public static Sprite GetValueSprite(ValueType type)
    {
        string iconPath = $"MyDraw/UI/";

        switch (type)
        {
            case ValueType.Food:return Resources.Load<Sprite>(iconPath + "Region/Value/Food/Food");
            case ValueType.Science:return Resources.Load<Sprite>(iconPath + "Region/Value/Science/Science");
            case ValueType.Politics:return Resources.Load<Sprite>(iconPath + "Region/Value/Politics/Politics");
            case ValueType.Gold:return Resources.Load<Sprite>(iconPath + "Region/Value/Gold/Gold");
            case ValueType.Faith:return Resources.Load<Sprite>(iconPath + "Region/Value/Faith/Faith");

            case ValueType.Attack: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Attack");
            case ValueType.Defense: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Defense");
            case ValueType.Magic: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Magic");
            case ValueType.Speed: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Speed");
            case ValueType.Lucky: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Lucky");

            case ValueType.FoodP: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Food/FoodP");
            case ValueType.ScienceP: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Science/ScienceP");
            case ValueType.PoliticsP: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Politics/PoliticsP");
            case ValueType.GoldP: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Gold/GoldP");
            case ValueType.FaithP: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Faith/FaithP");

            case ValueType.Leadership: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Leadership");
            case ValueType.Negotiation: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Negotiation");
            case ValueType.Build: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Build");
            case ValueType.Scout: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Scout");
            case ValueType.Charm: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Charm");

            default: return null;

        }
    }

    public static Sprite GetValueSprite(string valueName)
    {
        string iconPath = $"MyDraw/UI/";
        string value = valueName.ToLower() ;

        switch (value) {
            case "food":
                return Resources.Load<Sprite>(iconPath + "Region/Value/Food/Food");
            case "science":
                return Resources.Load<Sprite>(iconPath + "Region/Value/Science/Science");
            case "politics":
                return Resources.Load<Sprite>(iconPath + "Region/Value/Politics/Politics");
            case "gold":
                return Resources.Load<Sprite>(iconPath + "Region/Value/Gold/Gold");
            case "faith":
                return Resources.Load<Sprite>(iconPath + "Region/Value/Faith/Faith");
            case "build":
                return Resources.Load<Sprite>(iconPath + "Character/CharacterValueIcon/Build");
            case "negotitation":
                return Resources.Load<Sprite>(iconPath + "Character/CharacterValueIcon/Negotitation");
            case "scout":
                return Resources.Load<Sprite>(iconPath + "Character/CharacterValueIcon/scout");

            default: return null;
        }
    }


    public static Sprite GetHelpValueSpirte(int index)
    {
        string iconPath = $"MyDraw/UI/";
        switch (index)
        {
            case 1:
                return Resources.Load<Sprite>(iconPath + "Character/CharacterValueIcon/Scout");
            case 2:
                return Resources.Load<Sprite>(iconPath + "Character/CharacterValueIcon/Build");
            case 3:
                return Resources.Load<Sprite>(iconPath + "Character/CharacterValueIcon/Negotiation");

        }

        return null;

    }


    public static Sprite GetRoleSprite(CharacterRole role)
    {
        switch (role)
        {
            case CharacterRole.Swordsman: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Swordsman");
            case CharacterRole.Lancer: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Lancer");
            case CharacterRole.Shieldbearer: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Shieldbearer");
            case CharacterRole.Archer: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Archer");
            case CharacterRole.Alchemist: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Alchemist");
            case CharacterRole.Assassin: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Assassin");
            case CharacterRole.Priest: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Priest");
            case CharacterRole.Commander: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Commander");
            case CharacterRole.Cavalry: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Cavalry");
            case CharacterRole.Knight: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Knight");
            case CharacterRole.Magician: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Magic");
            case CharacterRole.Berserker: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Berserker");
            case CharacterRole.Commoner: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/None");
            case CharacterRole.Monster: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Monster");
            case CharacterRole.Bard: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/bard");
            case CharacterRole.Brawler: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Brawler");
            case CharacterRole.Special: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Special");

            case CharacterRole.All: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/CharacterClass");
            case CharacterRole.Custom: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Custom");
            case CharacterRole.Cance: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterClassIcon/Cance");


        }

        return null;
    }





    public static Sprite GetFavorabilitySprite(FavorabilityLevel FavorabilityLevel)
    {
        if (FavorabilityLevel == FavorabilityLevel.Self) return Resources.Load<Sprite>("MyDraw/UI/CharacterValueIcon/Self"); 
        if (FavorabilityLevel == FavorabilityLevel.Normal) return Resources.Load<Sprite>("MyDraw/UI/CharacterValueIcon/NormalFavorability");
        if (FavorabilityLevel == FavorabilityLevel.Romance) return Resources.Load<Sprite>("MyDraw/UI/CharacterValueIcon/RomanceFavorability");

        return null;
    }

    public static Sprite GetCharacterValueSpritesData(int valueType, int index)
    {
        switch (valueType)
        {
            case 0:
                switch (index)
                {
                    case 0: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Attack");
                    case 1: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Defense");
                    case 2: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Magic");
                    case 3: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Speed");
                    case 4: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Lucky");
                }
                break;
            case 1:
                switch (index)
                {
                    case 0: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Food/FoodP");
                    case 1: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Science/ScienceP");
                    case 2: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Politics/PoliticsP");
                    case 3: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Gold/GoldP");
                    case 4: return Resources.Load<Sprite>($"MyDraw/UI/Region/Value/Faith/FaithP");
                }
                break;
            case 2:
                switch (index)
                {
                    case 0: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Leadership");
                    case 1: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Negotiation");
                    case 2: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Build");
                    case 3: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Scout");
                    case 4: return Resources.Load<Sprite>($"MyDraw/UI/Character/CharacterValueIcon/Charm");
                }
                break;
        }

        return null;
    }

    public static Sprite GetCardTypeSprite(int valueType)
    {
        //Nothing = 0,Trap = 1,Heal = 2,Battle = 3,Item = 4,Escape = 5,Pass = 6
        switch (valueType)
        {
            case -2: return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardNodeBack");
            case -1: return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardNode");
            case 0: return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardNothing");
            case 1: return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardTrap");
            case 2: return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardHeal");
            case 3: return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardMonster");
            case 4: return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardItem");
            case 5: return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardExit");
            case 6: return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardPass");

        }

        return null;
    }

    public static Sprite GetCardTypeSprite(string valueType)
    {
        valueType = valueType.ToLower();
        switch (valueType)
        {
           // case "openleft": return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardNodeOpenLeft");
            //case "openright": return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardNodeOpenreft");
            case "nothing": return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardNothing");
            case "trap": return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardTrap");
            case "heal": return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardHeal");
            case "battle": return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardMonster");
            case "item": return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardItem");
            case "escape": return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardExit");
            case "pass": return Resources.Load<Sprite>($"MyDraw/UI/Explore/CardPass");


        }

        return null;
    }

    public static Sprite GetSeasonSprite(Season season)
    {
        GetSeasonSprite((int)season);
        return null;

    }


    public static Sprite GetSeasonSprite(int seasonIndex)
    {
        string iconPath = $"MyDraw/UI/";
        switch (seasonIndex)
        {
            case 0:
                return Resources.Load<Sprite>(iconPath + "GameUI/SpringSeason");
            case 1:
                return Resources.Load<Sprite>(iconPath + "GameUI/SummerSeason");
            case 2:
                return Resources.Load<Sprite>(iconPath + "GameUI/FallSeason");
            case 3:
                return Resources.Load<Sprite>(iconPath + "GameUI/WinterSeason");

        }

        return null;

    }


    public static Sprite GetSeasonSpriteBox(Season season)
    {
        GetSeasonSpriteBox((int)season);
        return null;

    }

    public static Sprite GetSeasonSpriteBox(int seasonIndex)
    {
        string iconPath = $"MyDraw/UI/";
        switch (seasonIndex)
        {
            case 0:
                return Resources.Load<Sprite>(iconPath + "GameUI/SpringTurn");
            case 1:
                return Resources.Load<Sprite>(iconPath + "GameUI/SummerTurn");
            case 2:
                return Resources.Load<Sprite>(iconPath + "GameUI/FallTurn");
            case 3:
                return Resources.Load<Sprite>(iconPath + "GameUI/WinterTurn");

        }

        return null;

    }


    public static Sprite GetSeasonSpriteCol(Season season)
    {
        return GetSeasonSpriteCol((int)season);
    }

    public static Sprite GetSeasonSpriteCol(int seasonIndex)
    {
        string iconPath = $"MyDraw/UI/";
        switch (seasonIndex)
        {
            case 0:
                return Resources.Load<Sprite>(iconPath + "GameUI/SpringRightColumn");
            case 1:
                return Resources.Load<Sprite>(iconPath + "GameUI/SummerRightColumn");
            case 2:
                return Resources.Load<Sprite>(iconPath + "GameUI/FallRightColumn");
            case 3:
                return Resources.Load<Sprite>(iconPath + "GameUI/WinterRightColumn");

        }

        Debug.Log($"Cant found GetSeasonSpriteCol(int seasonIndex) bug {seasonIndex}");
        return null;

    }

    public static Sprite GetClockSprite(bool isRed)
    {
        string iconPath = $"MyDraw/UI/Other/";

        if (isRed)
        {
            return Resources.Load<Sprite>(iconPath + "ClockRed");
        }
        else
        {
            return Resources.Load<Sprite>(iconPath + "ClockBlack");

        }
        return null;

    }





}
