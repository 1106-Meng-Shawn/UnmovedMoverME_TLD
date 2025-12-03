using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static ExprParser;

#region Constants

    public static class EffectCommands
    {
        public const string GO_TO_SHEET = "GoToSheet";
        public const string GO_TO_FILE = "GoToFile";
        public const string ADD_FAVORABILITY = "AddFavorability";
        public const string CHANGE_FAVORABILITY_LEVEL = "ChangeFavorabilityLevel";
        public const string ADD_ITEM = "AddItem";
        public const string SET_PLAYER_COUNTRY = "SetPlayerCountry";
        public const string CHARACTER_TYPE_CHANGE = "CharacterTypeChange";
        public const string END = "END";
        public const string CHOICE = "Choice";
        public const string CHOICE_IMPORTANT = "Choice_Important";
        public const string CHANGE_REGION_COUNTRY = "ChangeRegionCountry";
        public const string CHANGE_CITY_COUNTRY = "ChangeCityCountry";
        public const string GO_TO_SCENE = "GoToScene";
        public const string SHOW_RESULT_PANEL = "ShowResultPanel";
        public const string SHOW_IF = "ShowIf";
    }

    // ==================== Object Names ====================
    /// <summary>
    /// Defines object names used in condition expressions
    /// </summary>
    public static class ObjectNames
    {
        public const string GAME_VALUE = "GameValue";
        // Add more object names as needed
    }

    // ==================== GameValue Attributes ====================
    /// <summary>
    /// Defines all GameValue attribute names
    /// </summary>
    public static class GameValueAttributes
    {
        public const string CURRENT_YEAR = "CurrentYear";
        // Add more GameValue attributes as needed
    }

    // ==================== Character Attributes ====================
    /// <summary>
    /// Defines all Character attribute names
    /// </summary>
    public static class CharacterAttributes
    {
        public const string FAVORABILITY_LEVEL = "FavorabilityLevel";
        public const string FAVORABILITY = "Favorability";
        // Add more Character attributes as needed
    }

    // ==================== Condition Functions ====================
    /// <summary>
    /// Defines all condition function names
    /// </summary>
    public static class ConditionFunctions
    {
        public const string HAS_SHEET = "HasSheet";
        // Add more condition functions as needed
    }



#endregion






/// <summary>
/// Handles story text effects execution and parsing.
/// Uses dictionary-based command routing for extensibility.
/// Delegates parameter parsing to ExprParser for cleaner code.
/// </summary>
public class StoryTextEffect : MonoBehaviour
{
    [SerializeField] private TotalStoryManager TotalStoryManager;
    private TypewriterEffect TypewriterEffect;

    // Dictionary-based command handlers for easy extensibility
    private Dictionary<string, Func<string, Action>> effectCommandHandlers;
    private Dictionary<string, Func<int>> gameValueAttributeGetters;
    private Dictionary<string, Func<Character, int>> characterAttributeGetters;
    private Dictionary<string, Func<string, bool>> conditionFunctionEvaluators;

    private void Start()
    {
        TypewriterEffect = TypewriterEffect.Instance;
        InitializeHandlers();
    }

    /// <summary>
    /// Initialize all dictionary mappings for commands, attributes, and condition functions.
    /// To add new functionality, simply add entries to the appropriate dictionary.
    /// </summary>
    private void InitializeHandlers()
    {
        // Initialize effect command handlers
        effectCommandHandlers = new Dictionary<string, Func<string, Action>>
        {
            { EffectCommands.GO_TO_SHEET, ParseGoToSheetEffect },
            { EffectCommands.GO_TO_FILE, ParseGoToFileEffect },
            { EffectCommands.ADD_FAVORABILITY, ParseAddFavorabilityEffect },
            { EffectCommands.CHANGE_FAVORABILITY_LEVEL, ParseChangeFavorabilityLevelEffect },
            { EffectCommands.ADD_ITEM, ParseAddItemEffect },
            { EffectCommands.SET_PLAYER_COUNTRY, ParseSetPlayerEffect },
            { EffectCommands.CHARACTER_TYPE_CHANGE, ParseChangeTypeChange },
            { EffectCommands.END, ParseENDEffect },
            { EffectCommands.CHOICE, ParseChoiceEffect },
            { EffectCommands.CHOICE_IMPORTANT, ParseChoiceEffect },
            { EffectCommands.CHANGE_REGION_COUNTRY, ParseChangeRegionCountryEffect },
            { EffectCommands.CHANGE_CITY_COUNTRY, ParseChangeCityCountryEffect },
            { EffectCommands.GO_TO_SCENE, ParseGoToSceneEffect },
            { EffectCommands.SHOW_RESULT_PANEL, ParseShowResultPanelEffect },
            { EffectCommands.SHOW_IF, ParseShowIfEffect }
        };

        // Initialize GameValue attribute getters
        gameValueAttributeGetters = new Dictionary<string, Func<int>>
        {
            { GameValueAttributes.CURRENT_YEAR, () => GameValue.Instance.GetCurrentYear() }
            // Add new GameValue attributes here
        };

        // Initialize Character attribute getters
        characterAttributeGetters = new Dictionary<string, Func<Character, int>>
        {
            { CharacterAttributes.FAVORABILITY_LEVEL, c => (int)c.FavorabilityLevel },
            { CharacterAttributes.FAVORABILITY, c => c.Favorability }
            // Add new Character attributes here
        };

        // Initialize condition function evaluators
        conditionFunctionEvaluators = new Dictionary<string, Func<string, bool>>
        {
            { ConditionFunctions.HAS_SHEET, EvaluateHasSheet }
            // Add new condition functions here
        };
    }

    /// <summary>
    /// Execute all effects in the effect string
    /// </summary>
    public void DoEffect(string effectString)
    {
        foreach (var action in ParseEffectActions(effectString))
            action?.Invoke();
    }

    /// <summary>
    /// Parse effect string into a sequence of actions.
    /// Handles both conditional (if/else) and sequential (&&) effects.
    /// </summary>
    IEnumerable<Action> ParseEffectActions(string effectString)
    {
        if (effectString.Contains("if") || effectString.Contains("else"))
        {
            foreach (var act in ParseConditionalActions(effectString))
                yield return act;
            yield break;
        }

        // Split by && for sequential effects
        string[] effects = effectString.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string rawEffect in effects)
        {
            var (command, argument) = ExprParser.ParseCommandWithArguments(rawEffect);
            if (string.IsNullOrEmpty(command))
                continue;

            yield return ParseEffectCommand(command, argument);
        }
    }

    /// <summary>
    /// Parse conditional effect branches (if/else if/else)
    /// </summary>
    IEnumerable<Action> ParseConditionalActions(string fullCode)
    {
        var actions = new List<Action>();

        var branches = Regex.Matches(fullCode,
            @"(?:(if|else\s*if|else)\s*(?:\(\s*(?:condition\((.*?)\)|(.*?))\s*\))?)\s*\{\s*(.*?)\s*\}",
            RegexOptions.Singleline);

        foreach (Match branch in branches)
        {
            string type = branch.Groups[1].Value.Trim();
            string condition = !string.IsNullOrWhiteSpace(branch.Groups[2].Value)
                ? branch.Groups[2].Value.Trim()
                : branch.Groups[3].Value.Trim();
            string statement = branch.Groups[4].Value.Trim();

            // Check if this branch should execute
            bool ShouldExecute()
            {
                if (type == "if" || type == "else if")
                    return EvaluateCondition(condition);
                return true; // else branch always executes if reached
            }

            if (ShouldExecute())
            {
                // Parse and execute all statements in this branch
                foreach (var line in statement.Split(new[] { ';', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string trimmed = line.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        var match = Regex.Match(trimmed, @"(\w+)\((.*?)\)");
                        if (match.Success)
                        {
                            string command = match.Groups[1].Value;
                            string argument = match.Groups[2].Value.Trim().Trim('"');
                            var action = ParseEffectCommand(command, argument);
                            if (action != null)
                                actions.Add(action);
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid effect action: {trimmed}");
                        }
                    }
                }
                break;
            }
        }

        return actions;
    }

    /// <summary>
    /// Evaluate a condition string.
    /// Supports: function calls (HasSheet), logical OR (||), logical AND (&&), comparisons (==, !=, <, >, <=, >=).
    /// </summary>
    bool EvaluateCondition(string condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return false;

        condition = condition.Trim();

        // Try to evaluate as function call (e.g., HasSheet(2))
        if (TryEvaluateFunction(condition, out bool functionResult))
            return functionResult;

        // Parse logical OR (||)
        if (TrySplitByOr(condition, out var orParts))
            return orParts.Any(part => EvaluateCondition(part.Trim()));

        // Parse logical AND (&&)
        if (TrySplitByAnd(condition, out var andParts))
            return andParts.All(part => EvaluateCondition(part.Trim()));

        // Parse simple comparison
        return EvaluateSimpleComparison(condition);
    }

    /// <summary>
    /// Evaluate a function-style condition using dictionary lookup
    /// Example: HasSheet(2)
    /// </summary>
    bool TryEvaluateFunction(string condition, out bool result)
    {
        result = false;

        // Match function call format: FunctionName(args)
        var match = Regex.Match(condition, @"^(\w+)\((.*?)\)$");
        if (!match.Success)
            return false;

        string functionName = match.Groups[1].Value;
        string args = match.Groups[2].Value.Trim();

        // Lookup and execute using dictionary
        if (conditionFunctionEvaluators.TryGetValue(functionName, out var evaluator))
        {
            result = evaluator(args);
            return true;
        }

        Debug.LogWarning($"Unknown condition function: {functionName}");
        return false;
    }

    /// <summary>
    /// HasSheet condition evaluator - checks if a sheet index has been read
    /// </summary>
    private bool EvaluateHasSheet(string args)
    {
        if (int.TryParse(args, out int index))
        {
            return TotalStoryManager.Instance.GetReadSheetIndex().Contains(index);
        }
        Debug.LogWarning($"Invalid HasSheet argument: {args}");
        return false;
    }

    /// <summary>
    /// Evaluate simple comparison expressions like "gameValue.currentYear == 2024"
    /// </summary>
    bool EvaluateSimpleComparison(string expr)
    {
        if (!ExprParser.TryParseComparison(expr, out var objName, out var attrName, out var op, out var value))
        {
            Debug.LogWarning($"Invalid comparison expression: {expr}");
            return false;
        }

        int actual = GetAttributeValue(objName, attrName);
        return ExprParser.Compare(actual, op, value);
    }

    /// <summary>
    /// Get attribute value from an object using dictionary lookup
    /// </summary>
    int GetAttributeValue(string objectName, string attribute)
    {
        if (objectName == ObjectNames.GAME_VALUE)
        {
            return GetGameValueAttribute(attribute);
        }
        else
        {
            return GetCharacterAttribute(objectName, attribute);
        }
    }

    /// <summary>
    /// Get GameValue attributes using dictionary lookup
    /// </summary>
    int GetGameValueAttribute(string attribute)
    {
        if (gameValueAttributeGetters.TryGetValue(attribute, out var getter))
        {
            return getter();
        }

        Debug.LogWarning($"Unknown GameValue attribute: {attribute}");
        return -1;
    }

    /// <summary>
    /// Get Character attributes using dictionary lookup
    /// </summary>
    int GetCharacterAttribute(string characterName, string attribute)
    {
        Character character = GameValue.Instance.GetCharacterByName(characterName);
        if (character == null)
        {
            Debug.LogWarning($"Character not found: {characterName}");
            return -1;
        }

        if (characterAttributeGetters.TryGetValue(attribute, out var getter))
        {
            return getter(character);
        }

        Debug.LogWarning($"Unknown character attribute: {attribute}");
        return -1;
    }

    /// <summary>
    /// Parse and dispatch effect commands using dictionary lookup.
    /// To add a new command: 
    /// 1. Add constant to Constants.EffectCommands
    /// 2. Add handler to effectCommandHandlers dictionary in InitializeHandlers()
    /// 3. Implement Parse method
    /// </summary>
    Action ParseEffectCommand(string command, string argument)
    {
        if (effectCommandHandlers.TryGetValue(command, out var handler))
        {
            return handler(argument);
        }

        Debug.LogWarning($"Unknown effect command: {command}");
        return null;
    }

    // ==================== Effect Command Parsers ====================

    Action ParseSetPlayerEffect(string arg)
    {
        if (string.IsNullOrWhiteSpace(arg))
        {
            Debug.LogWarning("SetPlayerCountry: Argument is null or empty.");
            return null;
        }

        GameValue.Instance.SetPlayerCountry(arg);
        return null;
    }

    Action ParseShowIfEffect(string arg)
    {
        if (string.IsNullOrWhiteSpace(arg))
        {
            Debug.LogWarning("ShowIf: Argument is null or empty.");
            return null;
        }

        try
        {
            return EvaluateCondition(arg)
                ? TotalStoryManager.DisplayThisLine
                : TotalStoryManager.SkipThisLine;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"ShowIf: Failed to evaluate condition '{arg}'. Error: {ex.Message}");
            return null;
        }
    }

    Action ParseGoToSheetEffect(string arg)
    {
        TotalStoryManager.SaveProgress();

        if (string.IsNullOrWhiteSpace(arg))
        {
            Debug.LogWarning("GoToSheet: Argument is null or empty.");
            return null;
        }

        // Try parsing two integers (sheetIndex, startLine)
        if (TryParseTwoInts(arg, out int sheetIndex, out int startLine))
        {
            return () => TotalStoryManager.GoToSheet(sheetIndex, startLine);
        }

        // Try parsing single integer (sheetIndex only)
        if (ExprParser.TryParseInt(arg, out sheetIndex))
        {
            return () => TotalStoryManager.GoToSheet(sheetIndex, Constants.DEFAULT_START_LINE);
        }

        Debug.LogWarning($"GoToSheet: Invalid argument format: {arg}");
        return null;
    }

    Action ParseChoiceEffect(string arg)
    {
        if (!ExprParser.TryParseInt(arg, out int index))
        {
            Debug.LogError($"Choice: Cannot parse '{arg}' to integer.");
            return null;
        }

        if (TotalStoryManager.IsSkipAll())
        {
            int currentLine = TotalStoryManager.GetCurrentLine();
            TotalStoryManager.SetCurrentLine(currentLine += index);
            DoEffect(TotalStoryManager.GetStoryData()[currentLine].effect);
        }
        else
        {
            TotalStoryManager.ShowChoices(index);
            TotalStoryManager.SaveProgress();
            TotalStoryManager.SetChoiceTriggered(true);
        }

        return null;
    }

    Action ParseENDEffect(string arg)
    {
        string currentStoryFileName = TotalStoryManager.GetCurrentStoryFileName();

        if (!TypewriterEffect.IsTyping())
        {
            if (gameObject.activeSelf)
                StartCoroutine(DisableAfterDelay(0.5f));
            TotalStoryManager.TriggerNoStoryGameObejcts(true);
        }
        else
        {
            TypewriterEffect.CompleteLine();
        }

        TotalStoryManager.SaveProgress();
        TotalStoryManager.MarkStoryAsCompleted(currentStoryFileName);

        if (!string.IsNullOrWhiteSpace(arg))
        {
            ENDPanelManager.Instance.ShowENDPanel(arg);
        }

        return null;
    }

    Action ParseShowResultPanelEffect(string arg)
    {
        TotalStoryManager totalStoryManager = TotalStoryManager.Instance;
        string currentStoryFileName = totalStoryManager.GetCurrentStoryFileName();

        totalStoryManager.SetIsSkipAll(false);
        if (!TypewriterEffect.Instance.IsTyping())
        {
            StartCoroutine(DisableAfterDelay(0.5f));
            totalStoryManager.TriggerNoStoryGameObejcts(true);
        }
        else
        {
            TypewriterEffect.Instance.CompleteLine();
        }

        totalStoryManager.SaveProgress();
        CloseStoryPanel();

        if (!string.IsNullOrWhiteSpace(arg))
        {
            if (ExprParser.TryParseStringInt(arg, out var endKey, out var sheetIndex))
            {
                Debug.Log($"ShowResultPanel: {endKey}, {currentStoryFileName}, {sheetIndex}");
                ENDPanelManager.Instance.ShowENDPanel(endKey, currentStoryFileName, sheetIndex);
            }
            else
            {
                Debug.LogWarning($"ShowResultPanel: Failed to parse argument '{arg}'");
            }
        }

        return null;
    }

    Action ParseGoToSceneEffect(string arg)
    {
        if (!TypewriterEffect.Instance.IsTyping())
        {
            StartCoroutine(DisableAfterDelay(0.5f));
            TotalStoryManager.Instance.TriggerNoStoryGameObejcts(true);
        }
        else
        {
            TypewriterEffect.Instance.CompleteLine();
        }

        TotalStoryManager.Instance.SaveProgress();

        if (!string.IsNullOrWhiteSpace(arg))
        {
            if (ExprParser.TryParseTwoStrings(arg, out var sceneName, out var extraParam))
            {
                Debug.Log($"GoToScene: {sceneName}, Extra: {extraParam}");
                GameValue.Instance.SetEND(extraParam);
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning($"GoToScene: Failed to parse argument '{arg}'");
            }
        }

        return null;
    }

    Action ParseGoToFileEffect(string fileName)
    {
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            return () => TotalStoryManager.Instance.InitializeAndLoadStory(fileName, Constants.DEFAULT_START_LINE, Constants.DEFAULT_SHEET_INDEX);
        }
        else
        {
            Debug.LogWarning("GoToFile: Empty file name.");
            return null;
        }
    }

    Action ParseAddFavorabilityEffect(string arg)
    {
        if (!ExprParser.TryParseCharacterIntEffect(arg, out var charName, out var value))
        {
            Debug.LogWarning($"AddFavorability: Invalid format '{arg}'");
            return null;
        }

        return () => TipPanelManager.Instance.AddFavorability(charName, value);
    }

    Action ParseChangeFavorabilityLevelEffect(string arg)
    {
        if (!ExprParser.TryParseCharacterIntEffect(arg, out var charName, out var value))
        {
            Debug.LogWarning($"ChangeFavorabilityLevel: Invalid format '{arg}'");
            return null;
        }

        return () => TipPanelManager.Instance.ChangeFavorabilityLevel(charName, (FavorabilityLevel)value);
    }

    Action ParseAddItemEffect(string arg)
    {
        if (!ExprParser.TryParseCharacterIntEffect(arg, out var itemName, out var value))
        {
            Debug.LogWarning($"AddItem: Invalid format '{arg}'");
            return null;
        }

        return () => TipPanelManager.Instance.AddItem(itemName, value);
    }

    Action ParseChangeTypeChange(string arg)
    {
        if (!ExprParser.TryParseCharacterStringEffect(arg, out var charName, out var type))
        {
            Debug.LogWarning($"CharacterTypeChange: Invalid format '{arg}'");
            return null;
        }

        return () => TipPanelManager.Instance.CharacterTypeChange(charName, type);
    }

    Action ParseChangeRegionCountryEffect(string arg)
    {
        if (!ExprParser.TryParseRegionCountry(arg, out var region, out var country))
        {
            Debug.LogWarning($"ChangeRegionCountry: Invalid format '{arg}'");
            return null;
        }

        return () => TipPanelManager.Instance.ChangeRegionCountry(region, country);
    }

    Action ParseChangeCityCountryEffect(string arg)
    {
        if (!ExprParser.TryParseCityCountry(arg, out var region, out var cityIndex, out var country))
        {
            Debug.LogWarning($"ChangeCityCountry: Invalid format '{arg}'");
            return null;
        }

        return () => TipPanelManager.Instance.ChangeCityCountry(region, cityIndex.ToString(), country);
    }

    // ==================== Helper Methods ====================

    IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseStoryPanel();
    }

    void CloseStoryPanel()
    {
        TotalStoryManager.Instance.CloseStoryPanel();
    }
}