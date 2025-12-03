using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using System;
using System.Linq;
using UnityEngine.TextCore.Text;
using static EnumHelper;
using static GetString;

public class CharacterAssistRowControl : PanelBase
{
    private GameValue gameValue;
    public GameObject CharacterAssist;
    public Transform scrollContent;


    public TMP_Text value;

    public List<Character> SelectedCharacters = new List<Character>();

    public SBNType type = SBNType.None; // Negotiation is 3, bulid is 2, scout is 1;
    public Sprite negotiationSprite;
    public Sprite buildSprite;
    public Sprite scoutSprite;

    public Scrollbar scrollbar;

    public Button checkButton;
    public Button closeButton;


   // public GameObject feedbackPrefab;
    public GameObject valuePrefab;


    public Transform negotiationText;
    public Transform buildText;
    public Transform scoutText;

    public static event Action afterClose;
    private RecruitPanelManage recruitPanelManage;

        private float haveValue = 0;
    private float needValue = 0;
    private float charactersValue = 0;


    public static CharacterAssistRowControl Instance { get; private set; }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }



    private void Start()
    {
        gameValue = GameValue.Instance;
        checkButton.onClick.AddListener(() => CheckValue());
        closeButton.onClick.AddListener(() => ToggleRow(SBNType.None));

    }


    void Update()
    {
        if (Input.GetMouseButtonDown(1)) ToggleRow(SBNType.None);


        if (Input.GetKeyDown(KeyCode.Alpha7)) ToggleRow(SBNType.Scout);
        if (Input.GetKeyDown(KeyCode.Alpha8)) ToggleRow(SBNType.Build);
        if (Input.GetKeyDown(KeyCode.Alpha9)) ToggleRow(SBNType.Negotiation);
    }


    public void ValueUpdate()
    {
        if (!IsActive()) return;

        haveValue = 0;
        charactersValue = 0;

        if (type == SBNType.Scout)
        {
            haveValue += gameValue.GetResourceValue().Scout;


        } else if (type == SBNType.Build)
        {
            haveValue += gameValue.GetResourceValue().Build;

        } else if (type == SBNType.Negotiation)
        {
            haveValue += gameValue.GetResourceValue().Negotiation;
        }



        if (SelectedCharacters != null && SelectedCharacters.Count > 0)
        {
            foreach (var character in SelectedCharacters)
            {
                if (type == SBNType.Scout)
                {
                    charactersValue += (character.GetValue(2, 1) + (float)character.GetValue(2, 4) / 10);
                }

                else if (type == SBNType.Build)
                {
                    charactersValue += ( character.GetValue(2, 2) + (float)character.GetValue(2, 4) / 10);
                }

                else if (type == SBNType.Negotiation)
                {
                    charactersValue += ( character.GetValue(2, 3) + (float)character.GetValue(2, 4) / 10);

                }
            }

            haveValue += charactersValue;
        }

        UpdateValueText();
    }




    void UpdateValueText()
    {
        if (needValue > 0)
        {
            value.text = $"{haveValue}/{needValue}";
        }
        else
        {
            value.text = haveValue.ToString();
        }

        if (haveValue >= needValue)
        {
            value.color = Color.green;
        }
        else if (haveValue < needValue)
        {
            value.color = Color.red;
        }
    }

    public void ToggleRow(SBNType type, float needValue,RecruitPanelManage recruitPanelManage)
    {
        this.recruitPanelManage = recruitPanelManage;
        ToggleRow(type, needValue);
    }



    public void ToggleRow(SBNType type,float needValue)
    {
        if (this.type == type)
        {
            Debug.Log("ToggleRow(SBNType type,float needValue) work");
            this.type = SBNType.None;
            ClearSelection();
            panel.gameObject.SetActive(false);
            return;
        }

        if (IsActive() && this.type != type && type != SBNType.None)
        {
            ClearSelection();
            panel.gameObject.SetActive(true);
        }

        if (!IsActive())
        {
            this.type = type;
            this.needValue = needValue;
            SetTheCharacterAssistPreFab(type);
            panel.gameObject.SetActive(true);
            scrollbar.value = 1f;
            ValueUpdate();

        }
        else
        {
            ClearSelection();
            panel.gameObject.SetActive(false);
        }
    }

    public void ToggleBuildRow()
    {
        ToggleRow(SBNType.Build);
    }


    public void ToggleRow(SBNType type)
    {
        if (this.type == type)
        {
            this.type = SBNType.None;
            ClearSelection();
            panel.gameObject.SetActive(false);
            return;
        }


        if (type == SBNType.None)
        {
            ClearSelection();
            panel.gameObject.SetActive(false);
            return;
        }

        if (IsActive() && this.type != type)
        {
            ClearSelection();
            panel.gameObject.SetActive(false);
        }

        if (!IsActive())
        {
            this.type = type;
            SetTheCharacterAssistPreFab(type);
            panel.gameObject.SetActive(true);
            scrollbar.value = 1f;

          //  if (type == 1) UpCharacterRecruitCost();
            ValueUpdate();
        }
    }

    private void ClearSelection()
    {
        SelectedCharacters.Clear();
        haveValue = 0;
        charactersValue = 0;
    }

    public void SetTheCharacterAssistPreFab(SBNType type)
    {
        gameValue = GameValue.Instance;
        foreach (Transform child in scrollContent)
        {
            Destroy(child.gameObject);
        }

        SelectedCharacters.Clear();


        Func<Character, double> getValueFunc = c =>
        {
            if (type == SBNType.Scout) return c.GetValue(2, 1) + (double)c.GetValue(2, 4) / 10;
            if (type == SBNType.Build) return c.GetValue(2, 2) + (double)c.GetValue(2, 4) / 10;
            if (type == SBNType.Negotiation) return c.GetValue(2, 3) + (double)c.GetValue(2, 4) / 10;
            return 0;
        };

        // ??CanMove?true?false?
        if (gameValue == null)
        {
            Debug.LogError("gameValue is null!");
            return;
        }
        if (gameValue.GetPlayerCharacters() == null)
        {
            Debug.LogError("gameValue.PlayerCharacters is null!");
            return;
        }

        foreach (var c in gameValue.GetPlayerCharacters())
        {
            if (c == null)
            {
                Debug.LogError("Found null character in PlayerCharacters list!");
            }
        }

        List<Character> playerCharacters = gameValue.GetPlayerCharacters();
        var canMoveList = playerCharacters.Where(c => c.CanMove()).OrderByDescending(getValueFunc).ToList();
        var cannotMoveList = playerCharacters.Where(c => !c.CanMove()).OrderByDescending(getValueFunc).ToList();

        // ??
        List<Character> characters = canMoveList.Concat(cannotMoveList).ToList();

        foreach (var character in characters)
        {
            GameObject characterAssistPrefab = Instantiate(CharacterAssist, scrollContent);

            CharacterAssistPreFabControl characterAssistPreFabControl = characterAssistPrefab.GetComponent<CharacterAssistPreFabControl>();

            if (characterAssistPreFabControl != null)
            {
                if (type == SBNType.Negotiation) characterAssistPreFabControl.SetTheNegotiation(character);
                else if (type == SBNType.Build) characterAssistPreFabControl.SetTheBuild(character);
                else if (type == SBNType.Scout) characterAssistPreFabControl.SetTheScout(character);

                characterAssistPreFabControl.characterAssistRowControl = this;
            }
        }
    }




    void CheckValue()
    {
        if (haveValue >= needValue)
        {

            Transform targetText = negotiationText;
            Sprite targetSprite = negotiationSprite;
            if (type == SBNType.Scout)
            {
                gameValue.GetResourceValue().Scout = haveValue - needValue;
                targetText = scoutText;
                targetSprite = scoutSprite;

            } else if (type == SBNType.Build)
            {
                gameValue.GetResourceValue().Build = haveValue - needValue;
                targetText = buildText;
                targetSprite = buildSprite;
            } else if (type == SBNType.Negotiation)
            {
                gameValue.GetResourceValue().Negotiation = haveValue - needValue;
                targetText = negotiationText;
                targetSprite = negotiationSprite;

            }

            GenerateValuePrefabs(targetText, targetSprite);


            if (recruitPanelManage != null)
            {
                 recruitPanelManage.RecruitCharacter();
                recruitPanelManage = null;

             }

                foreach (var character in SelectedCharacters)
                {
                    character.IsMoved = true;
                }
                SelectedCharacters.Clear();


        //            gameValue.GetCurrentCharactersInGame();

            ToggleRow(SBNType.None);
        } else
        {
            if (type == SBNType.Negotiation) {
                //NotificationManage.Instance.ShowToTop("You don't have enough negotiating points!"); 
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource,GetResourceStringWithSprite(ValueType.Negotiation));

            }
            else if (type == SBNType.Build) {
                //NotificationManage.Instance.ShowToTop("You don't have enough build points!");
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Build));

            }
            else if (type == SBNType.Scout) {
                //NotificationManage.Instance.ShowToTop("You don't have enough scout points!");
                NotificationManage.Instance.ShowToTopByKey(NotificationKeyConstants.NotEnoughResource, GetResourceStringWithSprite(ValueType.Scout));

            }

        }

        if (afterClose != null) afterClose.Invoke();
    }


    void GenerateValuePrefabs(Transform targetText,Sprite targetSprite)
    {
        if (value == null || valuePrefab == null) return;

        int numPrefabs;

        if (needValue != 0)
        {
            numPrefabs  = (int)(charactersValue - needValue);
        } else
        {
            numPrefabs = (int)charactersValue;

        }


        RectTransform checkButtonRectTransform = checkButton.GetComponent<RectTransform>();
        float width = checkButtonRectTransform.position.x;
        float height = checkButtonRectTransform.position.y;
        Vector3 checkButtonCenter = checkButtonRectTransform.position;


        for (int i = 0; i < numPrefabs; i++)
        {
            GameObject valuePrefabInstance = Instantiate(valuePrefab);
            RectTransform valuePrefabRectTransform = valuePrefabInstance.GetComponent<RectTransform>();
            Image valuePrefabImage = valuePrefabInstance.GetComponent<Image>();
            valuePrefabImage.sprite = targetSprite;

            valuePrefabInstance.transform.SetParent(targetText.parent, false);

            float randomX = UnityEngine.Random.Range(checkButtonCenter.x - width / 8, checkButtonCenter.x + width / 8);
            float randomY = UnityEngine.Random.Range(checkButtonCenter.y - height / 8, checkButtonCenter.y + height / 8);
            Vector3 randomPosition = new Vector3(randomX, randomY, checkButtonCenter.z);
            valuePrefabRectTransform.position = randomPosition;

            valuePrefabRectTransform.localScale = Vector3.zero;

            valuePrefabRectTransform.DOScale(Vector3.one, 0.3f)
                .OnComplete(() =>
                {
                    Vector3 targetPos = targetText.localPosition;
                    valuePrefabRectTransform.DOLocalMove(targetPos, 1f).OnKill(() =>
                    {
                        Destroy(valuePrefabInstance);
                    });
                });
        }
    }

    public override void OpenPanel()
    {
        ToggleRow(type);
    }

    public override void ClosePanel()
    {
        ToggleRow(SBNType.None);
    }



    public override void SetPanelSaveData(PanelSaveData panelSaveData)
    {
        if (panelSaveData.customData.TryGetValue(CustomDataType.PanelType, out string panelTypeValue))
        {
            type = ParseEnumOrDefault<SBNType>(panelTypeValue);
            if (panelSaveData.isActive) OpenPanel();
        }
    }

    public override PanelSaveData GetPanelSaveData()
    {
        PanelSaveData panelSaveData = new PanelSaveData();
        panelSaveData.isActive = IsActive();
        panelSaveData.customData.Add(CustomDataType.PanelType,type.ToString());
        return panelSaveData;
    }

}
