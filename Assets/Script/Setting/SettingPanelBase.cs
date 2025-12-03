using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SettingPanelBase : MonoBehaviour
{
    [Header("Panel GameObject")]
    [SerializeField] protected GameObject panel;

    public virtual void OpenPanel()
    {
        panel.SetActive(true);
    }

    public virtual void ClosePanel()
    {
        panel.SetActive(false); 
    }
    public abstract void Init();
    public abstract void OnDefaultButtonClick();
}
