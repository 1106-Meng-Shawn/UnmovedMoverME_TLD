using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplePlaythroughsPlayerPanelControl: BaseCharacterMultiplePlaythroughsPanel
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        base.SetData(MultiplePlaythroughsGameCIInheritance.Instance.GetCharacterData(CharacterConstants.PlayerKey));
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public string GetPlayerName()
    {
        string PlayerName = "";
        return PlayerName;
    }

    public void Default()
    {

    }

}
