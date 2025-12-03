using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static BattleCharacterInfo;
using static GetColor;
using static GetSprite;
using static PositionAtBattle;
using static GeneralAnimation;
using static UnityEngine.GraphicsBuffer;


public class BattleCharacterInfo : MonoBehaviour
{
    private BattleCharacterValue battleCharacterValue;
    public TMP_Text characterName;

    public Scrollbar ForceRate;
    public TMP_Text force;
    public Image handleImage;

    public Image icon;
    public Image roleIcon;
    public Image effectImage;
    public bool isEnemy;

    public Image battleImage;
    public PositionAtBattle positionAtBattle;

    public GameObject Corners;

    private Coroutine cornerPulseCoroutine;
    private bool isPulsing = false;

    [Header("Effect")]
    public Effect effect;
    [System.Serializable]
    public class Effect
    {
        private Vector2 battleImageOriginalPos;

        public DamageTextControl damageTextPrefab;
        public RectTransform damageTextParent;

        public CanvasGroup battleImageCanvas;

        public CanvasGroup iconImageCanvas;

        public Vector2 GetBattleImageOriginalPos()
        {
            return battleImageOriginalPos;
        }

        public void SetBattleImageOriginalPos(Vector2 battleImageOriginalPos)
        {
            this.battleImageOriginalPos = battleImageOriginalPos;
        }


    }


    public Sprite[] skillEffectSprites;

    private class ActiveTextInfo
    {
        public TMP_Text text;
        public int valueIndex;
        public GameObject effectObj;     // ????????
        public Sequence effectSequence;  // ???????Sequence
    }
    private List<ActiveTextInfo> activeTexts = new List<ActiveTextInfo>();

    private void Awake()
    {
        effect.SetBattleImageOriginalPos(battleImage.rectTransform.anchoredPosition);
    }


    public void SetPositionAtBattle(PositionAtBattle positionAtBattle)
    {
        if (this.positionAtBattle != null) this.positionAtBattle.StopCornersPulse();
        this.positionAtBattle = positionAtBattle;
        SetCharacter(positionAtBattle.GetBattleCharacterValue());
        StopCornersPulse();
        StartCornersPulse();

    }


    void SetCharacter(BattleCharacterValue character)
    {
        if (battleCharacterValue == character) return;

        if (battleCharacterValue != null)
        {
            battleCharacterValue.OnValueChanged -= SetCharacterInfo;
        }



        battleCharacterValue = character;

        battleCharacterValue.OnValueChanged += SetCharacterInfo;
        battleCharacterValue.characterValue = character.characterValue;
        SetCharacterInfo();
    }

    private void OnDestroy()
    {
        if (battleCharacterValue != null)
        {
            battleCharacterValue.OnValueChanged -= SetCharacterInfo;
        }

        StopCornersPulse();
    }



    public BattleCharacterValue GetBattleCharacterValue()
    {
        return battleCharacterValue;
    }

    void SetCharacterInfo()
    {
        Character character = battleCharacterValue.characterValue;
        characterName.text = battleCharacterValue.GetBattleCharacterName();
        if (battleCharacterValue.IsPersonBattle())
        {
            force.text = character.GetHealthAndMaxHealthString();
        } else
        {
            force.text = GetValueColorString(battleCharacterValue.characterValue.Force.ToString("N0") + " / " + battleCharacterValue.characterValue.MaxForce.ToString("N0"), ValueColorType.Pop);
        }
        icon.sprite = battleCharacterValue.characterValue.icon;
        roleIcon.sprite = GetRoleSprite(battleCharacterValue.characterValue.RoleClass);
        battleImage.sprite = battleCharacterValue.characterValue.image;
        SetForceScrollbar();
        SetCharacterAlpha();
    }


    void SetForceScrollbar()
    {
        Character character = battleCharacterValue.characterValue;
            if (battleCharacterValue.IsPersonBattle())
            {
                ForceRate.size = (float)(character.Health) / character.GetMaxHealth();
                
            } else
            {
                ForceRate.size = (float)(character.Force) / character.MaxForce;
            }
            UpdateScrollbarHandleColor();
    }


    private void UpdateScrollbarHandleColor()
    {
        float forcePercentage = ForceRate.size;
        if (battleCharacterValue.IsPersonBattle())
        {
            SetHealthScrollbarHandleColor(forcePercentage);
        } else
        {
            SetForceScrollbarHandleColor();
        }
    }

    void SetHealthScrollbarHandleColor(float forcePercentage)
    {
        if (forcePercentage <= 0.1f)
        {
            handleImage.color = GetValueColor(ValueColorType.Decrease);
        }
        else if (forcePercentage <= 0.5f)
        {
            handleImage.color = Color.yellow;
        }
        else
        {
            handleImage.color = GetValueColor(ValueColorType.Increase);
        }
    }


    void SetForceScrollbarHandleColor()
    {
            if (isEnemy)
            {
                handleImage.color = GetValueColor(ValueColorType.Enemy);

            }
            else
            {
                handleImage.color = GetValueColor(ValueColorType.Player);

            }
    }
    public void StartCornersPulse()
    {
        if (Corners == null || isPulsing) return;

        isPulsing = true;
        Corners.SetActive(true);

        if (cornerPulseCoroutine != null)
            StopCoroutine(cornerPulseCoroutine);

        cornerPulseCoroutine = StartCoroutine(CornersPulseSequence());
    }

    public void StopCornersPulse()
    {
        if (!isPulsing) return;

        isPulsing = false;

        if (cornerPulseCoroutine != null)
            StopCoroutine(cornerPulseCoroutine);

        if (Corners != null)
        {
            Corners.transform.localScale = Vector3.one;
            Corners.SetActive(false);
        }
    }

    private IEnumerator CornersPulseSequence()
    {
        yield return StartCoroutine(InitialSelectEffect());

        // ??????????????
        yield return new WaitForEndOfFrame();

        Vector3 fromScale = Corners.transform.localScale;
        cornerPulseCoroutine = StartCoroutine(CornersPulseRoutine());
    }

    private IEnumerator InitialSelectEffect()
    {
        yield return StartCoroutine(LerpScaleWithEase(Corners.transform, Vector3.one * 1.1f, Vector3.one * 0.9f, 0.15f));
    }

    private IEnumerator CornersPulseRoutine()
    {
        Vector3 minScale = Vector3.one * 0.9f;
        Vector3 maxScale = Vector3.one * 1.1f;
        float duration = 0.5f;

        while (isPulsing)
        {
            float t = Mathf.PingPong(Time.time, duration) / duration;
            Corners.transform.localScale = Vector3.Lerp(minScale, maxScale, t);
            yield return null;
        }
    }

    private IEnumerator LerpScaleWithEase(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            if (target == null) yield break;
            float t = time / duration;
            float eased = EaseOutBack(t);
            target.localScale = Vector3.LerpUnclamped(from, to, eased);
            time += Time.deltaTime;
            yield return null;
        }
        target.localScale = to;
    }

    // ???????????? EaseOutBack?
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f; 
        float c3 = c1 + 1f;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    #region Heal Animation
    public void PlayHealReaction(int healNum, Skill skill, Action onComplete)
    {
        StartCoroutine(PlayHealCoroutine(healNum, skill, onComplete));
    }

    private IEnumerator PlayHealCoroutine(int healNum, Skill skill, Action onComplete)
    {
        var seq = CreateHealTextSequence(healNum);
        if (seq != null) yield return seq.WaitForCompletion();

        onComplete?.Invoke();
    }

    private Sequence CreateHealTextSequence(int healNum)
    {
        return GeneralAnimation.CreateHealTextSequence(
            healNum,
            effect.damageTextPrefab.gameObject,
            effect.damageTextParent,
            new AnimationConfig()
            {
                TotalDuration = EffectConstants.totalEffectDuration,
                StartOffsetX = 25f,
                StartOffsetY = 25f,
                EndOffsetX = 75f,
                EndOffsetY = 75f,
                FadeRatio = 0.6f
            }
        );
    }


    #endregion

    #region Hit Reaction Animation

    public void PlayHitReaction(DamageResult result, BattleCharacterValue attacker, Skill skill, Action onComplete)
    {
            StartCoroutine(PlayHitReactionCoroutine(result, attacker, skill, onComplete));
    }

    private IEnumerator PlayHitReactionCoroutine(
        DamageResult result,
        BattleCharacterValue attacker,
        Skill skill,
        Action onComplete)
    {
        if (battleImage == null || !battleCharacterValue.IsAlive())
        {
            onComplete?.Invoke();
            yield break;
        }

        Vector3 originalPos = battleImage.rectTransform.anchoredPosition;

        float direction = isEnemy ? 1f : -1f;

        // ? ?????????
        yield return PlayCriticalOrBlockEffect(result, attacker, skill);

        PlayDamageText(result, direction);
        positionAtBattle.PlayHitReaction(result);

        // ? ??????
        yield return PlayShakeReaction(originalPos, result.IsCritical,direction);

        // ? ????????

        onComplete?.Invoke();
        if (!battleCharacterValue.IsAlive()) CharacterDieAnimation();
    }

    private IEnumerator PlayCriticalOrBlockEffect(DamageResult result, BattleCharacterValue attacker, Skill skill)
    {
        if (!(result.IsCritical || result.IsBlock))
            yield break;

        bool effectFinished = false;

        EffectManager.instance.StartCriticalOrBlockAnimationInternal(
            new DamageAnimationRequest(GetBattleCharacterValue(), attacker, skill, new List<DamageResult> { result }),
            () => effectFinished = true
        );

        yield return new WaitUntil(() => effectFinished);
    }


    private void PlayDamageText(DamageResult result, float direction)
    {

        GeneralAnimation.CreateDamageTextSequence(
            result,
            effect.damageTextPrefab.gameObject,   // prefab
            effect.damageTextParent,              // parent
           // effect.GetBattleImageOriginalPos(),   // ????
            direction,
            new AnimationConfig()                
            {
                TotalDuration = EffectConstants.totalEffectDuration,
                StartOffsetX = 25f,
                StartOffsetY = 25f,
                EndOffsetX = 75f,
                EndOffsetY = 75f,
                FadeRatio = 0.6f
            }
        );
    }


    private IEnumerator PlayShakeReaction(Vector3 originalPos, bool isCritical,float direction)
    {
        float intensity = isCritical ? 22.5f : 15f; // ?????
        int shakeCount = 2;
        float duration = 0.15f;

        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < shakeCount; i++)
        {
            float singleDuration = duration / shakeCount;
            float forwardTime = singleDuration * EffectConstants.forwardTimeRate;
            float backwardTime = singleDuration * EffectConstants.backwardTimeRate;
            float forwardDist = intensity;
            float backwardDist = intensity * 0.5f;
            Vector3 forwardOffset = new Vector3(direction * forwardDist, direction * forwardDist, 0);

            seq.Append(battleImage.rectTransform
                .DOAnchorPos(originalPos + forwardOffset, forwardTime)
                .SetEase(Ease.OutQuad));
            seq.Append(battleImage.rectTransform
                .DOAnchorPos(originalPos, backwardTime)
                .SetEase(Ease.InOutSine));
        }

        seq.Append(battleImage.rectTransform
            .DOAnchorPosX(originalPos.x, 0.15f)
            .SetEase(Ease.OutQuad));

        yield return seq.WaitForCompletion();
    }


    #endregion


    #region value change animation
    public IEnumerator ValueChangeAnimation(BattleCharacterValue target, List<(BattleValue valueIndex, int delta)> values)
    {
        if (target != battleCharacterValue) yield break;
        if (values == null || values.Count == 0) yield break;

        float totalDuration = EffectConstants.totalEffectDuration;
        int count = values.Count;
        // float eachDuration = totalDuration / count;
        float eachDuration = totalDuration;
        // ?? yield return ??????????
        yield return StartCoroutine(PlayValueQueue(values, eachDuration));
        BattleRightCol.Instance.ValueChangeRecord(battleCharacterValue, values);

    }

    private IEnumerator PlayValueQueue(List<(BattleValue valueIndex, int delta)> values, float eachDuration)
    {
        foreach (var (valueIndex, delta) in values)
        {
            if (delta == 0)
            {
                yield return new WaitForSeconds(eachDuration);
                continue;
            }

            TMP_Text tmpText = CreateValueText((int)valueIndex, delta);
            Image effectImage = CreateValueEffectImage(valueIndex);

            // ????????????
            StartCoroutine(PlayEffectImageFrames(effectImage, eachDuration));

            int current = battleCharacterValue.GetValueAt((BattleValue)valueIndex);
            battleCharacterValue.SetValueAt((BattleValue)valueIndex, current + delta);
            // ??????
            yield return StartCoroutine(PlayValueTextAnimation(tmpText, delta, eachDuration));

            // ?????????????
            if (tmpText != null) Destroy(tmpText.gameObject);
            if (effectImage != null) Destroy(effectImage.gameObject);
        }
    }


    private IEnumerator PlayValueTextAnimation(TMP_Text text, int delta, float duration)
    {
        RectTransform rect = text.rectTransform;
        text.alpha = 1f;

        float originalScale = 1f;
        float enlargedScale = 1.3f;
        float moveDistance = 50f;

        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos;

        // ???????
        if (delta > 0)
        {
            endPos = startPos + new Vector2(0, moveDistance);
        }
        else
        {
            startPos += new Vector2(0, moveDistance);
            rect.anchoredPosition = startPos;
            endPos = startPos - new Vector2(0, moveDistance);
        }

        Sequence seq = DOTween.Sequence();

        // Step 1????????????
        seq.Append(rect.DOScale(enlargedScale, duration * 0.4f).SetEase(Ease.OutBack))
           .Join(text.DOFade(1f, duration * 0.4f));

        // Step 2???????????????
        seq.Append(rect.DOScale(originalScale, duration * 0.6f).SetEase(Ease.InOutBack))
           .Join(rect.DOAnchorPosY(endPos.y, duration * 0.6f).SetEase(Ease.OutCubic))
           .Join(text.DOFade(0f, duration * 0.6f).SetEase(Ease.InQuad));

        seq.Play();

        yield return seq.WaitForCompletion();
    }




    private IEnumerator PlayEffectImageFrames(Image effectImage, float duration)
    {
        int frameCount = skillEffectSprites.Length;
        float frameDuration = duration / frameCount;
        effectImage.gameObject.SetActive(true);

        for (int i = 0; i < frameCount; i++)
        {
           if (effectImage != null) effectImage.sprite = skillEffectSprites[i];
            yield return new WaitForSeconds(frameDuration);
        }
    }


    private TMP_Text CreateValueText(int valueIndex, int delta)
    {
        GameObject textObj = new GameObject("StatChangeText");
        textObj.transform.SetParent(battleImage.transform.parent, false);

        TMP_Text tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.fontSize = 72;
        tmpText.spriteAsset = GetSprite.LoadBattleValueSpriteAsset();

        string sign = delta > 0 ? "+" : "";
        tmpText.text = $"<sprite={valueIndex}> {sign}{delta}";
        tmpText.color = GetBattleValueColor((BattleValue)valueIndex);
        tmpText.alpha = 0;

        RectTransform rt = tmpText.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, 300);

        Canvas canvas = textObj.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = EffectConstants.baseSortingOrder;

        return tmpText;
    }

    private Image CreateValueEffectImage(BattleValue valueIndex)
    {
        GameObject effectObj = Instantiate(effectImage.gameObject, effectImage.transform.parent, false);
        Image img = effectObj.GetComponent<Image>();
        img.color = GetBattleValueColor(valueIndex);
        img.gameObject.SetActive(false);

        RectTransform rt = effectObj.GetComponent<RectTransform>();
        RectTransform template = effectImage.GetComponent<RectTransform>();
        rt.position = template.position;
        rt.anchorMin = template.anchorMin;
        rt.anchorMax = template.anchorMax;
        rt.pivot = template.pivot;
        rt.sizeDelta = template.sizeDelta;

        return img;
    }


    void SetCharacterAlpha()
    {
        if (battleCharacterValue.IsAlive())
        {
            effect.battleImageCanvas.alpha = EffectConstants.aliveAlpha;
            effect.iconImageCanvas.alpha = EffectConstants.aliveAlpha;
        } else
        {
            effect.battleImageCanvas.alpha = EffectConstants.deadAlpha;
            effect.iconImageCanvas.alpha = EffectConstants.deadAlpha;
        }
    }

    public void CharacterDieAnimation()
    {
        StartCoroutine(FadeOutCoroutine(effect.battleImageCanvas));
        StartCoroutine(FadeOutCoroutine(effect.iconImageCanvas));
    }


    #endregion
}
