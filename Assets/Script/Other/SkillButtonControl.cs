using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GetColor;

public class SkillButtonControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Skill skill;
    bool isPersonBattle = false;
    public GameObject SkillPanelPrefab;
    public bool isFollow = false;

    private GameObject currentSkillPanel;
    public Vector3 panelTransform = new Vector3(20f, -20f, 0f);

    public TextMeshProUGUI skillText;
    public Image skillShadow;
    public Button skillButton;
    private Canvas uiCanvas;

    void Start()
    {
        if (CityConnetManage.Instance != null) uiCanvas = GeneralManager.Instance.uiCanvas;
        if (uiCanvas == null) uiCanvas = GeneralManager.Instance.uiCanvas;
    }


    public void InitSkillButton(bool isPersonBattle, Skill skill, SkillPanelControl skillPanelControl)
    {
        this.isPersonBattle = isPersonBattle;
        isFollow = false;
        SetSkill(skill);
        currentSkillPanel = skillPanelControl.gameObject;
        HideSkillPanel();

    }

    public void SetSkill(Skill skill)
    {
        this.skill = skill;

        if (skill != null) { 
            skillText.text = skill.GetSkillName();
            skillText.color = GetSkillRareColor(skill.skillRare);
            if (skillShadow != null)
            {
                skillShadow.gameObject.SetActive(true);
                skillShadow.color = GetSkillFunctionTypeColor(skill.functionType);
            }
        }
        else {
            skillText.text = "NONE";
            skillText.color = GetSkillRareColor(-1);
            if (skillShadow != null) skillShadow.gameObject.SetActive(false);
        };


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowSkillPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideSkillPanel();
    }

    void Update()
    {
        if (isFollow && currentSkillPanel != null)
        {
            RectTransform parentRect = transform.parent as RectTransform;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                Input.mousePosition,
                uiCanvas.worldCamera, // <--- 这里必须传入摄像机
                out localPoint
            );
            currentSkillPanel.GetComponent<RectTransform>().localPosition = localPoint + (Vector2)panelTransform;
        }
    }



    private void ShowSkillPanel()
    {
        if (isFollow)
        {
            CreateSkillPanel();
        } else
        {
            SetPanel();
        }
    }

    void SetPanel()
    {
        currentSkillPanel.gameObject.SetActive(true);
        currentSkillPanel.GetComponent<SkillPanelControl>().ShowSkillPanel(isPersonBattle, skill);
    }

    void CreateSkillPanel()
    {
        HideSkillPanel();
        Transform parentTransform = transform.parent;

        currentSkillPanel = Instantiate(SkillPanelPrefab, parentTransform, false);
        currentSkillPanel.GetComponent<SkillPanelControl>().ShowSkillPanel(isPersonBattle, skill);

        RectTransform parentRect = parentTransform as RectTransform;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            Input.mousePosition,
            uiCanvas.worldCamera,  // <--- 这里也传入摄像机
            out localPoint
        );
        if (isFollow)
        {
            currentSkillPanel.GetComponent<RectTransform>().localPosition = localPoint + (Vector2)panelTransform;

        }
        else
        {
            currentSkillPanel.GetComponent<RectTransform>().localPosition = (Vector2)panelTransform;

        }

    }

    void HideSkillPanel()
    {
        if (isFollow)
        {
            DestroyPanel();
        } else
        {
            ClosePanel();
        }
    }

    void ClosePanel()
    {
        currentSkillPanel.gameObject.SetActive(false);
    }

    void DestroyPanel()
    {
        if (currentSkillPanel != null)
        {
            Destroy(currentSkillPanel);
            currentSkillPanel = null;
        }
    }


    public void SetActive(bool active)
    {
        if (active)
        {
            gameObject.SetActive(true);
        } else
        {
            HideSkillPanel();
            gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        HideSkillPanel();
    }
}
