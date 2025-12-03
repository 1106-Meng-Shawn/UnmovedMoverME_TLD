using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PanelExtensions;

public class ElectorPanelManage : PanelBase
{
    public static ElectorPanelManage Instance { get; private set; }
    public Button closeButton;
    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    void Start()
    {
        closeButton.onClick.AddListener(ClosePanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override PanelSaveData GetPanelSaveData()
    {
        PanelSaveData saveData = GetSaveData(this,panel,PanelType.Elector);
        return saveData;
    }

    public override void SetPanelSaveData(PanelSaveData panelSaveData)
    {
        SetSaveData(this,panel,panelSaveData);
    }


}
