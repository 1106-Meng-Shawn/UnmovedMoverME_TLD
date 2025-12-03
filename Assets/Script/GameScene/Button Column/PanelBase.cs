using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelBase : MonoBehaviour
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


    public virtual bool IsActive()
    {
        return panel.activeSelf;    
    }


    public abstract void SetPanelSaveData(PanelSaveData panelSaveData);

    public abstract PanelSaveData GetPanelSaveData();
}
