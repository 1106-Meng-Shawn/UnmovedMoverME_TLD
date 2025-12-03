using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static GetColor;

public static class EffectConstans
{
    public static Vector3 xFlipVectorOne = new Vector3(-Vector3.one.x, Vector3.one.y, Vector3.one.z);
}

public class EffectControl : MonoBehaviour
{
    [Header("UI References")]
    public Image characterImage;
    public Image BackgroundImage;
    public Transform EffectArrows;
    public GameObject ArrowPrefab;
    public TextMeshProUGUI criticalOrBlockText;
    public TextMeshProUGUI skillNameText;

    private List<GameObject> activeArrows = new List<GameObject>();

    [Header("Critical Animation Settings")]
    float enterDuration = 0.3f;
    float shakeDuration = 0.25f;
    float exitDuration = 0.25f;

    private void Awake()
    {
        CheckReferences();
    }

    // =========================
    // ???? UI ??
    // =========================
    public void CheckReferences()
    {
        Debug.Assert(characterImage != null, "EffectControl.characterImage ???!");
        Debug.Assert(BackgroundImage != null, "EffectControl.BackgroundImage ???!");
        Debug.Assert(criticalOrBlockText != null, "EffectControl.criticalOrBlockText ???!");
        Debug.Assert(skillNameText != null, "EffectControl.skillNameText ???!");
    }

    // =========================
    // ??????
    // =========================
    public void ShowBlockAnimation(BattleCharacterValue targeter, EffectControl criticalEffectControl, Action onComplete = null)
    {
        gameObject.SetActive(true);
        characterImage.sprite = targeter.characterValue.icon;
        characterImage.color = new Color(1, 1, 1, 0);
        characterImage.gameObject.SetActive(true);

        criticalOrBlockText.text = "BLOCK";
        skillNameText.gameObject.SetActive(false);

        Vector3 baseScale = targeter.isEnemy ? Vector3.one : EffectConstans.xFlipVectorOne;
        BattleRightCol.Instance.BlockRecord(targeter);
        StartCoroutine(PlayCriticalAnimation(targeter, baseScale, criticalEffectControl,false,onComplete));
    }

    // =========================
    // ??????
    // =========================
    public void ShowCriticalAnimation(BattleCharacterValue user, BattleCharacterValue targeted, Skill skill,EffectControl blockEffControl, bool isBlock = false, Action onComplete = null)
    {
        gameObject.SetActive(true);
        characterImage.sprite = user.characterValue.icon;
        characterImage.color = new Color(1, 1, 1, 0);
        characterImage.gameObject.SetActive(true);

        criticalOrBlockText.text = "CRITICAL";
        skillNameText.gameObject.SetActive(true);
        skillNameText.color = GetSkillFunctionTypeColor(skill.functionType);
        skillNameText.text = skill.GetSkillName();

        Vector3 baseScale = user.isEnemy ? Vector3.one : EffectConstans.xFlipVectorOne;
        BattleRightCol.Instance.CriticalRecord(user);
        StartCoroutine(PlayCriticalAnimation(targeted, baseScale, blockEffControl, isBlock, onComplete));
    }

    // =========================
    // ??????
    // =========================
    private IEnumerator PlayCriticalAnimation(
        BattleCharacterValue targeted,
        Vector3 baseScale,
        EffectControl effectControl,
        bool isBlock = false,
        Action onComplete = null)
    {
        transform.SetAsLastSibling();
        float bgWidth = BackgroundImage.rectTransform.rect.width;
        Vector3 leftPos = new Vector3(bgWidth / 2, 0, 0);
        Vector3 centerPos = Vector3.zero;
        Vector3 rightPos = new Vector3(-bgWidth / 2, 0, 0);

        // ????????
        characterImage.rectTransform.localPosition = leftPos;
        characterImage.rectTransform.localScale = baseScale * 0.8f;

        // ????
        characterImage.rectTransform.DOLocalMove(centerPos, enterDuration).SetEase(Ease.OutQuad);
        characterImage.rectTransform.DOScale(baseScale * 1.1f, enterDuration).SetEase(Ease.OutBack);
        characterImage.DOFade(1f, enterDuration);

        AnimateSkillNameText(baseScale);
        AnimateCriticalOrBlockText(baseScale);

        yield return new WaitForSeconds(enterDuration);

        // ??/????
        Sequence critSeq = DOTween.Sequence();
        critSeq.Append(characterImage.rectTransform
            .DOShakePosition(shakeDuration, strength: new Vector3(15, 10, 0), vibrato: 25, randomness: 90))
               .Join(characterImage.rectTransform
            .DOScale(baseScale * 1.3f, shakeDuration * 0.5f)
            .SetLoops(2, LoopType.Yoyo));

        yield return critSeq.WaitForCompletion();

        yield return new WaitForSeconds(0.15f);

        // ????
        Sequence exitSeq = DOTween.Sequence();

        if (isBlock)
        {
            exitSeq.Append(characterImage.rectTransform.DOLocalMove(leftPos, exitDuration).SetEase(Ease.InBack))
                   .Join(characterImage.rectTransform.DOScale(baseScale * 0.3f, exitDuration).SetEase(Ease.InBack))
                   .Join(characterImage.DOFade(0f, exitDuration));

            yield return exitSeq.WaitForCompletion();
            effectControl.ShowBlockAnimation(targeted,this,onComplete);
            // onComplete?.Invoke(); // ??????
            yield break;
        }

        exitSeq.Append(characterImage.rectTransform.DOLocalMove(rightPos, exitDuration).SetEase(Ease.InBack))
               .Join(characterImage.rectTransform.DOScale(baseScale * 0.3f, exitDuration).SetEase(Ease.InBack))
               .Join(characterImage.DOFade(0f, exitDuration));

        yield return exitSeq.WaitForCompletion();

        characterImage.gameObject.SetActive(false);

        // ????
        //foreach (GameObject arrow in activeArrows)
        //{
        //    if (arrow != null) Destroy(arrow);
        //}
        //activeArrows.Clear();

        effectControl?.Close();
        Close();

        onComplete?.Invoke(); // ??????
    }

    // =========================
    // ??????
    // =========================
    private void AnimateSkillNameText(Vector3 baseScale)
    {
        if (skillNameText == null || !skillNameText.gameObject.activeSelf) return;

        RectTransform rt = skillNameText.rectTransform;
        rt.localScale = baseScale;

        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOLocalMoveX(0, 0.4f).SetEase(Ease.OutBack))
           .Join(rt.DOScale(baseScale * 1.2f, 0.3f).SetLoops(2, LoopType.Yoyo));
    }

    private void AnimateCriticalOrBlockText(Vector3 baseScale)
    {
        if (criticalOrBlockText == null) return;

        RectTransform rt = criticalOrBlockText.rectTransform;
        rt.localScale = baseScale;

        criticalOrBlockText.color = new Color(
            criticalOrBlockText.color.r,
            criticalOrBlockText.color.g,
            criticalOrBlockText.color.b,
            1
        );

        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOScale(baseScale * 1.8f, 0.25f).SetEase(Ease.OutBack))
           .Append(rt.DOScale(baseScale * 1.2f, 0.2f).SetEase(Ease.OutQuad))
           .AppendInterval(0.3f)
           .Append(criticalOrBlockText.DOFade(0, 0.4f));
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}

