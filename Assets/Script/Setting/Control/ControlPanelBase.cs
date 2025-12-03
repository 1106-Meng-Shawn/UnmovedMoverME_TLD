using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all control panels
/// </summary>
public abstract class ControlPanelBase : MonoBehaviour
{
    [Header("Panel GameObject")]
    [SerializeField] protected GameObject panel;

    public virtual void OpenPanel()
    {
        RestorePanel();           
        panel.SetActive(true);   
    }

    /// <summary>
    /// Close the panel. Default behavior: hide panel
    /// </summary>
    public virtual void ClosePanel()
    {
        panel.SetActive(false);  // 隐藏面板
    }

    /// <summary>
    /// Restore panel-specific data, must be implemented by child classes
    /// </summary>
    public abstract void RestorePanel();
    public abstract void OnDefaultButtonClick();

}
