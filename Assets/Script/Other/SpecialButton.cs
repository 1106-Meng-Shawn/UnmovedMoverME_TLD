using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class SpecialButtonConstants
{
    public const int InitStep = 1;
    public const bool EnableCtrlMultiplier = true;
    public const int CtrlMultiplier = 10;

}

[System.Serializable]
public class SpecialButtonData
{
    public ValueType valueType;
    public int step = 1;
}

public class SpecialButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private SpecialButtonData data;

    [Header("Repeat Settings")]
    public float initialDelay = 0.5f;     // ??????
    public float repeatInterval = 0.2f;   // ??????

    private UnityAction<SpecialButtonData, int> onStepAction;   // ??????
    private UnityAction onCtrlAction;                           // Ctrl ????
    private Dictionary<KeyCode, Func<int, int>> modifierFuncs = new Dictionary<KeyCode, Func<int, int>>();

    private bool isPressed = false;
    private float pressTime = 0f;
    private float nextTriggerTime = 0f;

    public void Setup(UnityAction<SpecialButtonData, int> action, SpecialButtonData specialButtonData)
    {
        onStepAction = action;
        data = specialButtonData;
    }

    public void RegisterCtrlAlternativeAction(UnityAction ctrlAction)
    {
        onCtrlAction = ctrlAction;
    }

    public void RegisterModifier(KeyCode key, Func<int, int> modifierFunc)
    {
        modifierFuncs[key] = modifierFunc;
    }

    public void RegisterBothCtrlModifier(Func<int, int> modifierFunc)
    {
        modifierFuncs[KeyCode.LeftControl] = modifierFunc;
        modifierFuncs[KeyCode.RightControl] = modifierFunc;
    }

    public void ClearModifiers()
    {
        modifierFuncs.Clear();
        onCtrlAction = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        pressTime = Time.time;
        nextTriggerTime = Time.time;
        Trigger(); // ??????
    }

    public void OnPointerUp(PointerEventData eventData) => isPressed = false;
    public void OnPointerExit(PointerEventData eventData) => isPressed = false;
    public void OnDisable() => isPressed = false;

    private void Update()
    {
        if (!isPressed) return;

        if (Time.time >= nextTriggerTime && Time.time - pressTime >= initialDelay)
        {
            Trigger();
            nextTriggerTime = Time.time + repeatInterval;
        }
    }

    /// <summary>
    /// ?????????????Ctrl ???
    /// </summary>
    private void Trigger()
    {
        bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        // Ctrl ????
        if (ctrlPressed && onCtrlAction != null)
        {
            onCtrlAction.Invoke();
            return;
        }

        // ????
        if (data == null || onStepAction == null) return;

        int delta = data.step;

        foreach (var kvp in modifierFuncs)
        {
            if (Input.GetKey(kvp.Key))
            {
                delta = kvp.Value(delta);
                break;
            }
        }

        onStepAction.Invoke(data, delta);
    }
}
