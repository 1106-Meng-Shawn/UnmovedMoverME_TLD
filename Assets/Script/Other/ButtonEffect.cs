using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isTriggerEffect = true;
    [Header("Color Settings")]
    public bool isChangeColor = false;
    public Color32 selectedImageColor = Color.yellow;
    public Color32 unselectedImageColor = Color.white;

    [Header("Text Color Settings")]

    public Color32 selectedTextColor = new Color32(255,216,52,255);
    public Color32 unselectedTextColor = Color.white;

    [Header("Sprite Settings")]
    public bool isChangeSprite = true;
    public Sprite unSelectedSprite;
    public Sprite selectedSprite;

    [Header("Animation Settings")]
    public bool isAnimation = false;
    public Animation hoverAnimation;
    public string animationName;

    [Header("Move To Top")]

    public bool isMoveToTop;
    private int originalSiblingIndex;


    [Header("References")]
    public Image targetImage;
    public TextMeshProUGUI text;

    

    private void Awake()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (text == null) text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        ApplyVisualState(false); // ????????
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Button>().interactable == false) return;
        ApplyVisualState(true); // ????
        if (isAnimation) PlayHoverAnimation();
        if (isMoveToTop) MoveToTop(true); // ????
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Button>().interactable == false) return;

        if (isTriggerEffect)
        {
            ApplyVisualState(false); // ??????
        }
        else
        {
            ApplyVisualState(true); // ????
        }
        if (isAnimation) StopHoverAnimation();
        if (isMoveToTop) MoveToTop(false);
    }

    void MoveToTop(bool isNotMove)
    {
        if (isNotMove) {
            originalSiblingIndex = transform.GetSiblingIndex();
            transform.SetAsLastSibling();
        } else
        {
            transform.SetSiblingIndex(originalSiblingIndex);
        }
    }
    private void ApplyVisualState(bool selected)
    {
        if (targetImage == null) return;

        if (isChangeSprite)
        {
            targetImage.sprite = selected ? selectedSprite : unSelectedSprite;
        }

        if (isChangeColor)
        {
            targetImage.color = selected ? selectedImageColor : unselectedImageColor;
        }

        if (text != null)
        {
            text.color = selected ? selectedTextColor : unselectedTextColor;
        }
    }

    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    public void SetButtonUnInteractable (bool isSel)
    {
        gameObject.GetComponent<Button>().interactable = false;

        ApplyVisualState(isSel);
    }


    private void PlayHoverAnimation()
    {
        if (hoverAnimation != null && !string.IsNullOrEmpty(animationName) && !hoverAnimation.isPlaying)
        {
            hoverAnimation.Play(animationName);
        }
    }

    private void StopHoverAnimation()
    {
        if (hoverAnimation != null && !string.IsNullOrEmpty(animationName))
        {
            hoverAnimation.Stop(animationName);
            hoverAnimation[animationName].time = 0f;
            hoverAnimation.Sample();
        }
    }

    public void SetChangeSprite(Sprite selected, Sprite unselected)
    {
        selectedSprite = selected;
        unSelectedSprite = unselected;
        if (targetImage != null)
            targetImage.sprite = unSelectedSprite;
    }


    public void SetIsTriggerEffect(bool isTriggerEffect)
    {
        this.isTriggerEffect = isTriggerEffect;
        ApplyVisualState(!isTriggerEffect);
    }
}
