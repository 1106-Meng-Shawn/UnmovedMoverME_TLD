using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ArcButtonController : MonoBehaviour
{
    public RectTransform centerPoint; // Anchor point at bottom-right (empty RectTransform)
    public List<RectTransform> buttons; // List of buttons
    public float radius = 200f; // Radius of the arc

    [Tooltip("Center angle for the middle button or center between two middle buttons")]
    public float centerAngle = 155f; // 10:30 direction

    [Tooltip("Angle interval between buttons in degrees")]
    public float angleInterval = 15f;

    [Tooltip("Maximum allowed angle for leftmost button (e.g. 180 = 9 o'clock)")]
    public float maxAngle = 180f;

    public float offsetAmount = 10f; // Floating offset amount
    public float animationDuration = 0.5f; // Base duration of animation

    private int selectedIndex = -1;
    bool isAnimating = false;

    void Start()
    {
        ArrangeButtons();
        AnimateIdleOffset();
    }

    void ArrangeButtons()
    {
        List<int> activeIndices = new List<int>();
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] != null && buttons[i].gameObject.activeSelf)
            {
                activeIndices.Add(i);
            }
        }

        int count = activeIndices.Count;
        if (count == 0) return;

        float usedAngleInterval = angleInterval;

        if (count > 1)
        {
            float theoreticalStartAngle;

            if (count % 2 == 1)
            {
                int midIndex = count / 2;
                theoreticalStartAngle = centerAngle + usedAngleInterval * midIndex;
            }
            else
            {
                theoreticalStartAngle = centerAngle + usedAngleInterval / 2f + usedAngleInterval * (count / 2 - 1);
            }

            if (theoreticalStartAngle > maxAngle) // ???????????????
            {
                if (count % 2 == 1)
                {
                    int midIndex = count / 2;
                    usedAngleInterval = (maxAngle - centerAngle) / midIndex;  // ???? maxAngle - centerAngle
                }
                else
                {
                    usedAngleInterval = (maxAngle - centerAngle) / (count / 2 - 0.5f);
                }
            }
        }

        float startAngle;

        if (count % 2 == 1)
        {
            int midIndex = count / 2;
            startAngle = centerAngle + usedAngleInterval * midIndex;
            for (int i = 0; i < count; i++)
            {
                float angle = startAngle - usedAngleInterval * i; // ??????12??????9?
                SetButtonPositionAndRotation(activeIndices[i], angle);
            }
        }
        else
        {
            startAngle = centerAngle + usedAngleInterval / 2f + usedAngleInterval * (count / 2 - 1);
            for (int i = 0; i < count; i++)
            {
                float angle = startAngle - usedAngleInterval * i; // ??????12??????
                SetButtonPositionAndRotation(activeIndices[i], angle);
            }
        }
    }


    void SetButtonPositionAndRotation(int btnIdx, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        buttons[btnIdx].anchoredPosition = pos;

        float rotationZ = angle - 180f;
        buttons[btnIdx].localRotation = Quaternion.Euler(0, 0, rotationZ);
    }

    void AnimateIdleOffset()
    {
        if (isAnimating) return;
        isAnimating = true;

        List<int> activeIndices = new List<int>();
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].gameObject.activeSelf)
                activeIndices.Add(i);
        }

        foreach (int i in activeIndices)
        {
            if (i == selectedIndex) continue;

            RectTransform btn = buttons[i];
            Vector2 originalPos = btn.anchoredPosition;

            Vector2 tangent = new Vector2(-originalPos.y, originalPos.x).normalized;
            float randomOffset = offsetAmount * Random.Range(0.6f, 1.2f);
            float randomDuration = animationDuration * Random.Range(0.8f, 1.5f);
            Vector2 targetPos = originalPos + tangent * randomOffset;

            btn.DOKill();
            btn.DOAnchorPos(targetPos, randomDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(Random.Range(0f, 0.5f));
        }
    }

    public void SetButtons(List<SkillButtonControl> skillButtons)
    {
        buttons.Clear();
        if (skillButtons != null){
            foreach (var btn in skillButtons)
            {
                if (btn != null)
                    buttons.Add(btn.GetComponent<RectTransform>());
            }
        }

        selectedIndex = -1;
        ArrangeButtons();
        AnimateIdleOffset();
    }




    public void SelectButton(int index)
    {
        if (selectedIndex >= 0 && selectedIndex < buttons.Count && buttons[selectedIndex].gameObject.activeSelf)
        {
            buttons[selectedIndex].DOKill();
            Vector2 originalPos = buttons[selectedIndex].anchoredPosition;
            Vector2 offsetDir = originalPos.normalized;
            Vector2 targetPos = originalPos + offsetDir * offsetAmount;

            buttons[selectedIndex].DOAnchorPos(targetPos, animationDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }

        selectedIndex = index;

        if (selectedIndex >= 0 && selectedIndex < buttons.Count && buttons[selectedIndex].gameObject.activeSelf)
        {
            buttons[selectedIndex].DOKill();
            Vector2 origin = GetOriginalPosition(selectedIndex);
            buttons[selectedIndex].DOAnchorPos(origin, 0.2f).SetEase(Ease.OutSine);
        }
    }

    public void ShowAllButtons()
    {
        HideAllButtons();
        
        foreach (var btn in buttons)
        {
            if (btn != null)
            {
                btn.gameObject.SetActive(true);
            }
        }

        ArrangeButtons();      // ??????
        AnimateIdleOffset();   // ????????
    }

    public void HideAllButtons()
    {
        foreach (var btn in buttons)
        {
            if (btn != null)
            {
                btn.DOKill(); // ????
                btn.GetComponent<SkillButtonControl>().SetActive(false);
            }
        }

        selectedIndex = -1;
        isAnimating = false;
    }



    Vector2 GetOriginalPosition(int index)
    {
        List<int> activeIndices = new List<int>();
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].gameObject.activeSelf)
                activeIndices.Add(i);
        }

        int count = activeIndices.Count;
        if (count == 0) return Vector2.zero;

        int activePos = activeIndices.IndexOf(index);
        if (activePos == -1) return Vector2.zero;

        float usedAngleInterval = angleInterval;

        if (count > 1)
        {
            float theoreticalStartAngle;

            if (count % 2 == 1)
            {
                int midIndex = count / 2;
                theoreticalStartAngle = centerAngle - usedAngleInterval * midIndex;
            }
            else
            {
                theoreticalStartAngle = centerAngle - usedAngleInterval / 2f - usedAngleInterval * (count / 2 - 1);
            }

            if (theoreticalStartAngle < maxAngle)
            {
                if (count % 2 == 1)
                {
                    int midIndex = count / 2;
                    usedAngleInterval = (centerAngle - maxAngle) / midIndex;
                }
                else
                {
                    usedAngleInterval = (centerAngle - maxAngle) / (count / 2 - 0.5f);
                }
            }
        }

        float startAngle;

        if (count % 2 == 1)
        {
            int midIndex = count / 2;
            startAngle = centerAngle - usedAngleInterval * midIndex;
            float angle = startAngle + usedAngleInterval * activePos;
            float rad = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        }
        else
        {
            startAngle = centerAngle - usedAngleInterval / 2f - usedAngleInterval * (count / 2 - 1);
            float angle = startAngle + usedAngleInterval * activePos;
            float rad = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        }
    }

    public void Cleanup()
    {
        foreach (var btn in buttons)
        {
            if (btn != null)
            {
                btn.DOKill(); // ?? DOTween ??
            }
        }
    }

    private void OnDestroy()
    {
        Cleanup();
    }


}
