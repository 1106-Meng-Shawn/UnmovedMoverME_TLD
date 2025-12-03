using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterExtrasPanelControl : BaseCharacterMultiplePlaythroughsPanel
{
    [Header("Character Info")]
    [SerializeField] protected CharacterExtrasInfoGroup characterExtrasInfo;

    [Header("Panel Bottom Row")]
    [SerializeField] protected CharacterBottomRow characterBottomRow;
    [SerializeField] protected PlayerBottomRow playerBottomRow;

    [Header("Bottom Row")]
    [SerializeField] private CharacterExtrasScrollRow characterExtrasScrollRow;

    #region Structs
    [System.Serializable]
    public struct CharacterExtrasInfoGroup
    {
        public Button TypeChangeLeftButton;
        public Button TypeChangeRightButton;
    }

    [System.Serializable]
    public struct CharacterBottomRow
    {
        public GameObject ENDRow;
        public CharacterExtrasENDControl TEControl;
        public ScrollRect GEScrollRect;
        public ScrollRect BEScrollRect;
        public CharacterExtrasENDControl ENDPrefab;
    }

    [System.Serializable]
    public struct PlayerBottomRow
    {
        public GameObject ENDRow;
        public List<CharacterExtrasENDControl> TEControls; 
        public TextMeshProUGUI GENumText;
        public TextMeshProUGUI BENumText;
    }

    #endregion

    #region Public Methods
    public void ShowPanel() {

        gameObject.SetActive(true);
        SetCharacterExtrasSaveDataList(ExtrasValue.Instance.GetCharacterExtrasSaveDatas());
    } 

    public void SetCharacterExtrasSave(CharacterExtrasSaveData characterExtrasSave)
    {
        if (characterExtrasSave == null) return;

        base.SetExtraData(characterExtrasSave);
        SetBottomRow(characterExtrasSave);
    }

    void SetCharacterExtrasSaveDataList(List<CharacterExtrasSaveData> characterExtrasSaveList)
    {
        characterExtrasScrollRow.SetCharacterExtrasSaveDataList(characterExtrasSaveList);
    }
    #endregion

    #region Bottom Row Logic
    private void SetBottomRow(CharacterExtrasSaveData characterExtrasSave)
    {
        bool isPlayer = characterExtrasSave.CharacterKey == CharacterConstants.PlayerKey;

        playerBottomRow.ENDRow.SetActive(isPlayer);
        characterBottomRow.ENDRow.SetActive(!isPlayer && characterExtrasSave.HasEND());

        if (isPlayer)
            SetPlayerBottomRow(characterExtrasSave);
        else if (characterExtrasSave.HasEND())
            SetCharacterENDBottomRow(characterExtrasSave);
    }

    private void SetPlayerBottomRow(CharacterExtrasSaveData characterExtrasSave)
    {
        foreach (var teControl in playerBottomRow.TEControls)
            teControl.SetCharacterENDData(false, -1);

        for (int i = 0; i < characterExtrasSave.CharacterTECount.Count && i < playerBottomRow.TEControls.Count; i++)
        {
            playerBottomRow.TEControls[i].SetCharacterENDData(false, characterExtrasSave.CharacterTECount[i]);
        }

        playerBottomRow.GENumText.text = characterExtrasSave.GECount.ToString();
        playerBottomRow.BENumText.text = characterExtrasSave.BECount.ToString();
    }

    private void SetCharacterENDBottomRow(CharacterExtrasSaveData characterExtrasSave)
    {
        int teCount = characterExtrasSave.CharacterTECount.Count;
        characterBottomRow.TEControl.SetCharacterENDData(false, teCount > 0 ? characterExtrasSave.CharacterTECount[0] : -1);

        foreach (Transform child in characterBottomRow.GEScrollRect.content)
            Destroy(child.gameObject);

        foreach (var geID in characterExtrasSave.CharacterGECount)
        {
            GameObject instance = Instantiate(characterBottomRow.ENDPrefab.gameObject, characterBottomRow.GEScrollRect.content);
            instance.GetComponent<CharacterExtrasENDControl>().SetCharacterENDData(false, geID);
        }

        foreach (Transform child in characterBottomRow.BEScrollRect.content)
            Destroy(child.gameObject);

        foreach (var beID in characterExtrasSave.CharacterBECount)
        {
            GameObject instance = Instantiate(characterBottomRow.ENDPrefab.gameObject, characterBottomRow.BEScrollRect.content);
            instance.GetComponent<CharacterExtrasENDControl>().SetCharacterENDData(false, beID);
        }
    }
    #endregion
}
