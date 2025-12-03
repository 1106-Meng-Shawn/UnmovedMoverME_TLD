using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static GetSprite;

public class CharacterENDControl : MonoBehaviour
{
    public GameObject ENDDetail;
    public Image characterIcon;
  //  public GameObject isTEImage;
    public TextMeshProUGUI ENDTypeText;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI characterENDContent;

    private GameValue gameValue;

    public GameObject CharacterENDPrefab;
    private GameObject PlayerEND;

    public GameObject GEContent;
    public GameObject BEContent;

    public Image favorabilityType;
    List<CharacterENDSaveData> characterENDSaveDatas = new List<CharacterENDSaveData>();

    private void Awake()
    {
        gameValue = GameValue.Instance;
    }

    public void ShowCharacterENDPanel()
    {
        gameObject.SetActive(true);

    }

    public void HideCharacterENDPanel()
    {
        gameObject.SetActive(false);

    }

    public void InitCharacterENDPanel()
    {
        List<Character> characters = gameValue.GetAllCharacters();

        foreach (Character character in characters) {

            if (character.IsHaveEND() && character.GetCharacterKey() != CharacterConstants.PlayerKey) CreatEND(character); 
        }

        characterENDSaveDatas = new List<CharacterENDSaveData>();
    }

    void CreatEND(Character character)
    {
        CharacterEND characterEND = character.GetCharacterEND(gameValue);
        if (characterEND == null) return;
        GameObject instance = null;
        if (characterEND.IsGE())
        {
            instance = Instantiate(CharacterENDPrefab, GEContent.transform);

        }
        else {
            instance = Instantiate(CharacterENDPrefab, BEContent.transform);
        }

        var saveData = new CharacterENDSaveData(
            character.GetCharacterKey(),
            GameValue.Instance.IsLastJudgmentHappen(),
            characterEND.IsTE() ? characterEND.GetENDID() : -1,
            characterEND.IsGE() ? characterEND.GetENDID() : -1,
            !characterEND.IsGE() ? characterEND.GetENDID() : -1
        );

        characterENDSaveDatas.Add(saveData);

        instance.GetComponent<CharacterENDPrefabControl>().SetCharacter(character, characterEND, this);

    }


    public void ShowENDDetail(Sprite characterIconSprite, Character character, CharacterEND characterEND)
    {
        characterIcon.sprite = characterIconSprite;
        favorabilityType.sprite = GetFavorabilitySprite(character.FavorabilityLevel);
        characterNameText.text = character.GetCharacterName();
        if (characterEND.IsTE())
        {
            ENDTypeText.text = "Ture End";
        }
        else if (characterEND.IsGE()) {
            ENDTypeText.text = "Good End";

        } else
        {
            ENDTypeText.text = "Bad End";
        }
        characterENDContent.text = characterEND.GetContent();
        ENDDetail.SetActive(true);

    }

    public void HideENDDetail()
    {
        ENDDetail.SetActive(false);
        characterIcon.sprite = null;
        characterNameText.text = null;
        characterENDContent.text = null;
    }


    public void RemovePlayerEND()
    {
        if (PlayerEND != null)
        {
            Destroy(PlayerEND);
            PlayerEND = null;
        }

        characterENDSaveDatas.RemoveAll(END => END.characterKey == CharacterConstants.PlayerKey);
    }


    public void AddPlayerEND(bool isGE)
    {
        Transform contentParent = isGE ? GEContent.transform : BEContent.transform;
        GameObject instance = Instantiate(CharacterENDPrefab, contentParent);

        Character player = gameValue.GetCharacterByKey(CharacterConstants.PlayerKey);

        instance.GetComponent<CharacterENDPrefabControl>().SetCharacter(player, player.GetPlayerEND(isGE), this);

        instance.transform.SetSiblingIndex(0);
        PlayerEND = instance;

        var saveData = new CharacterENDSaveData(
            player.GetCharacterKey(),
            false,                  
            0,
            isGE ? 1 : -1,          
            isGE ? -1 : 1           
        );

        characterENDSaveDatas.RemoveAll(END => END.characterKey == CharacterConstants.PlayerKey);
        characterENDSaveDatas.Add(saveData);
    }



    public List<CharacterENDSaveData> GetCharacterENDSaveDatas()
    {
        return characterENDSaveDatas;
    }

}

public struct CharacterENDSaveData
{
    public string characterKey;
    public bool IsLastJudgmentHappen;
    public int TEID;
    public int GEID;
    public int BEID;

    public CharacterENDSaveData(string characterKey, bool isLastJudgmentHappen, int teid, int geid, int beid)
    {
        this.characterKey = characterKey;
        this.IsLastJudgmentHappen = isLastJudgmentHappen;
        this.TEID = teid;
        this.GEID = geid;
        this.BEID = beid;
    }

    public bool HasValue()
    {
        return characterKey != string.Empty;
    }
}
