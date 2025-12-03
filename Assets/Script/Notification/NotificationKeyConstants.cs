using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class NotificationKeyConstants
{
    public struct NotificationKey
    {
        public string Key;
        public int ParamCount;
        public NotificationKey(string key, int paramCount)
        {
            Key = key;
            ParamCount = paramCount;
        }
    }

    /// <summary>{Resource} Add {amount} - 2 parameter</summary>
    public static readonly NotificationKey Resource_Add = new NotificationKey("Resource_Add", 2);

    /// <summary>{PositionName} don't have Character - 1 parameter</summary>
    public static readonly NotificationKey Position_NoCharacter = new NotificationKey("Position_NoCharacter", 1);

    /// <summary>{CharacterName} force are full - 1 parameter</summary>
    public static readonly NotificationKey Character_ForceFull = new NotificationKey("Character_ForceFull", 1);

    /// <summary>{Character} Add {available} - 2 parameters</summary>
    public static readonly NotificationKey Character_AddForce = new NotificationKey("Character_AddForce", 2);

    /// <summary>You should have at least one character in the position - 0 parameters</summary>
    public static readonly NotificationKey Position_AtLeastOneCharacter = new NotificationKey("Position_AtLeastOneCharacter", 0);

    /// <summary>Not enough {resource}! - 1 parameter</summary>
    public static readonly NotificationKey NotEnoughResource = new NotificationKey("NotEnoughResource", 1);

    /// <summary>AT WAR WITH {Country}. - 1 parameter</summary>
    public static readonly NotificationKey War_WithCountry = new NotificationKey("War_WithCountry", 1);

    /// <summary>Unlocked {Level} - 1 parameter</summary>
    public static readonly NotificationKey Level_Unlocked = new NotificationKey("Level_Unlocked", 1);

    /// <summary>Recruited Population +{returnPop}, Gold +{returnGold}. - 2 parameters</summary>
    public static readonly NotificationKey Recruited_PopGold = new NotificationKey("Recruited_PopGold", 2);

    /// <summary>{RegionName} Don't have lord - 1 parameter</summary>
    public static readonly NotificationKey Region_NoLord = new NotificationKey("Region_NoLord", 1);

    /// <summary>Minimum Force is 1! - 0 parameters</summary>
    public static readonly NotificationKey Force_Minimum = new NotificationKey("Force_Minimum", 0);

    /// <summary>{Character} Force is Maximum! - 1 parameter</summary>
    public static readonly NotificationKey Force_Max = new NotificationKey("Force_Max", 1);

    /// <summary>Restored total {totalRestored} Force. - 1 parameter</summary>
    public static readonly NotificationKey Force_Restored = new NotificationKey("Force_Restored", 1);

    /// <summary>Don’t have cities connected to {cityName} - 1 parameter</summary>
    public static readonly NotificationKey City_NoConnection = new NotificationKey("City_NoConnection", 1);

    /// <summary>All characters are at full Force! - 0 parameters</summary>
    public static readonly NotificationKey Force_AllFull = new NotificationKey("Force_AllFull", 0);

    /// <summary>No Force to restore! - 0 parameters</summary>
    public static readonly NotificationKey Force_NoneToRestore = new NotificationKey("Force_NoneToRestore", 0);

    /// <summary>Input is empty but not allowed. - 0 parameters</summary>
    public static readonly NotificationKey Input_Empty = new NotificationKey("Input_Empty", 0);

    /// <summary>This save is star - 0 parameters</summary>
    public static readonly NotificationKey Save_Starred = new NotificationKey("Save_Starred", 0);

    /// <summary>Has {num} event{s} must be resolved before you can end the season - 1 parameter</summary>
    public static readonly NotificationKey Event_MustResolve = new NotificationKey("Event_MustResolve", 1);

    /// <summary>Game Saved Successfully - 0 parameters</summary>
    public static readonly NotificationKey Game_SaveSuccess = new NotificationKey("Game_SaveSuccess", 0);

    /// <summary>Save Failed! - 0 parameters</summary>
    public static readonly NotificationKey Game_SaveFail = new NotificationKey("Game_SaveFail", 0);

    /// <summary>Quick Save Successful - 0 parameters</summary>
    public static readonly NotificationKey QuickSave_Success = new NotificationKey("QuickSave_Success", 0);

    /// <summary>Quick Save Failed! - 0 parameters</summary>
    public static readonly NotificationKey QuickSave_Fail = new NotificationKey("QuickSave_Fail", 0);

    /// <summary>No UnStar Save to delete - 0 parameters</summary>
    public static readonly NotificationKey Save_NoUnstar = new NotificationKey("Save_NoUnstar", 0);

    /// <summary>Deleted All UnStar Save - 0 parameters</summary>
    public static readonly NotificationKey Save_DeletedAll = new NotificationKey("Save_DeletedAll", 0);

    /// <summary>Load Success - 0 parameters</summary>
    public static readonly NotificationKey Load_Success = new NotificationKey("Load_Success", 0);




    /// <summary>{character} has acted - 1 parameter</summary>
    public static readonly NotificationKey Character_Moved = new NotificationKey("Character_Moved", 1);

    /// <summary>The {character}'s force has reached its limit! - 1 parameter</summary>
    public static readonly NotificationKey Character_LimitReached = new NotificationKey("Character_LimitReached", 1);

    /// <summary>Exploration can only be performed solo - 0 parameters</summary>
    public static readonly NotificationKey Exploration_SingleOnly = new NotificationKey("Exploration_SingleOnly", 0);

    /// <summary>You’ve already scouted all levels! - 0 parameters</summary>
    public static readonly NotificationKey Scout_AllLevel = new NotificationKey("Scout_AllLevel", 0);

    /// <summary>Scout successful! - 0 parameters</summary>
    public static readonly NotificationKey Scout_Success = new NotificationKey("Scout_Success", 0);

    /// <summary>{Character} is the lord - 1 parameter</summary>
    public static readonly NotificationKey Character_IsLord = new NotificationKey("Character_IsLord", 1);

    /// <summary>There is no character at the position! - 0 parameters</summary>
    public static readonly NotificationKey AllPosition_NoCharacter = new NotificationKey("AllPosition_NoCharacter", 0);

    /// <summary>You have unlocked this level! - 0 parameters</summary>
    public static readonly NotificationKey ScoutLevel_Unlocked_Already = new NotificationKey("ScoutLevel_Unlocked_Already", 0);


}

