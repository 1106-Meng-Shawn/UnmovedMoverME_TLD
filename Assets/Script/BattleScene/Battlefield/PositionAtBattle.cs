using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static GetColor;
using static GetSprite;
using static PositionAtBattle;
using static GeneralAnimation;


public class PositionAtBattle : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text forceText;
    public TMP_Text battleMoveNumText;
    public List<TextMeshProUGUI> valuesText;
    private List<InitialAttribute> valuesInitialAttributes = new List<InitialAttribute>();

    private BattleCharacterValue battleCharacterValue;

    public Image classImage;
    public Image profileIcon;

    public bool isEnemy;
    public BattleCharacterInfo battleCharacterInfo;

    public GameObject Corners;
    public Button SetButton;
    public Button PositionButton;
    public GameObject moveOj;

    private Coroutine cornerPulseCoroutine;
    private bool isPulsing = false;

    public GameObject HighLightImage;
    private Coroutine alphaFlashCoroutine;


    [Header("Effect")]
    public Effect effect;
    [System.Serializable]
    public class Effect
    {
        public List<Image> activeEffectImages = new List<Image>();
        public Image valueEffectImage;

        private Vector2 profileIconOriginalPos;

        public DamageTextControl damageTextPrefab;
        public RectTransform damageTextParent;

        public CanvasGroup canvasGroup;

        public Vector2 GetProfileIconOriginalPos()
        {
            return profileIconOriginalPos;
        }

        public void SetProfileIconOriginalPos(Vector2 profileIconOriginalPos)
        {
             this.profileIconOriginalPos = profileIconOriginalPos;
        }


    }

    class InitialAttribute
    {
        public Vector3 baseScale;
        public Vector3 basePos;
        public Color baseColor;
        public float baseFontSize;
        public FontStyles baseFontStyle;

        public InitialAttribute(TextMeshProUGUI targetText)
        {
            RectTransform rt = targetText.rectTransform;
            baseScale = rt.localScale;
            basePos = rt.localPosition;
            baseColor = targetText.color;
            baseFontSize = targetText.fontSize;
            baseFontStyle = targetText.fontStyle;

        }

    }

    private float effectDelayOffset = 0f;
    private const float effectDelayStep = 0.1f;

    private class ActiveImageInfo
    {
        public Image image;
        public Sprite valueSprite;
        public GameObject effectObj;     // 保存技能特效对象
        public Sequence effectSequence;  // 保存技能特效的Sequence
    }
    private List<ActiveImageInfo> activeImages = new List<ActiveImageInfo>();



    private void Awake()
    {
        effect.SetProfileIconOriginalPos(profileIcon.rectTransform.anchoredPosition);
    }

    private void Start()
    {
        PositionButton.onClick.AddListener(OnPositionButtonClick);
        StopHighlightFlash();
    }


    void OnPositionButtonClick()
    {
        SetBattleValueToInfo();
    }


    public void SetCharacterToPosition(BattleCharacterValue battleCharacter)
    {

        if (battleCharacterValue != null)
        {
            battleCharacterValue.OnValueChanged -= OnBattleCharacterValueChanged;
        }

        this.battleCharacterValue = battleCharacter;

        if (battleCharacter != null)
        {
            battleCharacter.OnValueChanged += OnBattleCharacterValueChanged;
            UpCharacterValue();
            InitValuesInitialAttributes();
        }
        else
        {
            profileIcon.sprite = null;
            nameText.text = "NONE";
            forceText.text = "0/0";
            classImage.sprite = null;
            battleMoveNumText.text = "NONE";
            gameObject.SetActive(false);
        }
    }

    private void OnBattleCharacterValueChanged()
    {
        UpCharacterValue();
    }


    public BattleCharacterValue GetBattleCharacterValue()
    {
        return battleCharacterValue;
    }

    void UpCharacterValue()
    {
        Character character = battleCharacterValue.characterValue;
        profileIcon.sprite = character.icon;
        nameText.text = battleCharacterValue.GetBattleCharacterName();
        classImage.sprite = GetRoleSprite(character.RoleClass);

        battleMoveNumText.text = GetValueColorString(" * " + battleCharacterValue.MoveNum.ToString(), ValueColorType.CanMove);

        if (battleCharacterValue.IsPersonBattle())
        {
            forceText.text = character.GetHealthAndMaxHealthString();
            moveOj.SetActive(false);
        }
        else
        {
            forceText.text = GetValueColorString(character.Force.ToString("N0") + " / " + character.MaxForce.ToString("N0"), ValueColorType.Pop);
            moveOj.SetActive(true);
        }

        for (int i = 0; i < 5; i++)
        {
            valuesText[i].color = GetValueColor(0, i);
            valuesText[i].text = battleCharacterValue.GetValueAt((BattleValue)i).ToString();
        }
    }

    public void SetBattleValueToInfo()
    {
        if (battleCharacterValue == null)
        {
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Position_NoCharacter, GetPositionName());
            return;
        }

        battleCharacterInfo.SetPositionAtBattle(this);
        StartCornersPulse();
        PlayButtonPressFeedback(true);
    }

    public string GetPositionName()
    {
        return "Position";
    }


    public void PlayButtonPressFeedback(bool restoreColor)
    {
        if (SetButton != null && SetButton.gameObject.activeSelf)
        {
            if (SetButton != null && SetButton.gameObject.activeSelf)
            {
                DOTween.Kill(SetButton.gameObject, true);

                Transform btnTransform = SetButton.transform;
                Image image = SetButton.GetComponent<Image>();
                ButtonEffect effect = SetButton.GetComponent<ButtonEffect>();
                TextMeshProUGUI text = SetButton.GetComponentInChildren<TextMeshProUGUI>();

                Color pressedImageColor = effect != null ? effect.selectedImageColor : Color.gray;
                Color pressedTextColor = effect != null ? effect.selectedTextColor : Color.black;

                Sequence seq = DOTween.Sequence().SetId("tabPress");

                // ??????
                if (image != null)
                    seq.Append(image.DOColor(pressedImageColor, 0.2f));
                if (text != null)
                    seq.Join(text.DOColor(pressedTextColor, 0.2f));

                // ????
                seq.Join(btnTransform.DOScale(0.85f, 0.1f).SetEase(Ease.OutQuad));
                seq.Append(btnTransform.DOScale(1.0f, 0.1f).SetEase(Ease.OutBack));

                // ???????
                if (restoreColor)
                {
                    if (image != null)
                        seq.Join(image.DOColor(effect.unselectedImageColor, 0.1f));
                    if (text != null)
                        seq.Join(text.DOColor(effect.unselectedTextColor, 0.1f));
                }
            }
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


    public void SetPositionButton(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            SetButton.gameObject.SetActive(true);
            SetButton.GetComponentInChildren<TextMeshProUGUI>().text = input;
        } else
        {
            SetButton.gameObject.SetActive(false);
        }
    }


    private void OnDestroy()
    {
        if (battleCharacterValue != null)
        {
            battleCharacterValue.OnValueChanged -= OnBattleCharacterValueChanged;
        }
        StopCornersPulse();
    }



    public void SetHighLight(bool isSet,Skill skill)
    {
        if (battleCharacterValue == null) return;
        if (!gameObject.activeSelf) return;
        if (isSet) { HighLight(skill); }
        else { StopHighlightFlash(); };
    }


    void HighLight(Skill skill)
    {
        HighLightImage.GetComponent<Image>().color = GetSkillFunctionTypeColor(skill.functionType);
        StartHighlightFlash();
    }



    void StartHighlightFlash()
    {
        if (alphaFlashCoroutine != null)
            StopCoroutine(alphaFlashCoroutine);
        HighLightImage.SetActive(true);

        Image img = HighLightImage.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0f); // ??????

        alphaFlashCoroutine = StartCoroutine(FlashImageAlphaCoroutine(img, 0.2f, 0.8f, 2f));
    }

    public void StopHighlightFlash()
    {
        if (alphaFlashCoroutine != null)
        {
            StopCoroutine(alphaFlashCoroutine);
            alphaFlashCoroutine = null;
        }

        Image img = HighLightImage.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1f); // ??????
        HighLightImage.SetActive(false);
    }



    private IEnumerator FlashImageAlphaCoroutine(Image image, float minAlpha, float maxAlpha, float speed)
    {
        float t = 0f;
        bool increasing = true;

        Color baseColor = image.color;

        while (true)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            image.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

            t += (increasing ? 1 : -1) * Time.deltaTime * speed;

            if (t >= 1f)
            {
                t = 1f;
                increasing = false;
            }
            else if (t <= 0f)
            {
                t = 0f;
                increasing = true;
            }

            yield return null;
        }
    }

    #region Heal Animation
    public void PlayHealReaction(int healNum)
    {
        StartCoroutine(PlayHealCoroutine(healNum));
    }

    private IEnumerator PlayHealCoroutine(int healNum)
    {

        var seq = CreateHealTextSequence(healNum);
        if (seq != null) yield return seq.WaitForCompletion();
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

    #region value Change Animation
    public IEnumerator ValueChangeAnimation(List<(BattleValue valueIndex, int delta)> values)
    {
        if (values == null || values.Count == 0) yield break;

        float totalDuration = EffectConstants.totalEffectDuration;

        // 1. 文字动画
        foreach (var (valueIndex, delta) in values)
        {
            AnimateSingleValue(valueIndex, delta, totalDuration);
        }

        // 2. 准备 Image 动画
        List<Sprite> valueSprites = new List<Sprite>();
        List<int> deltas = new List<int>();
        foreach (var (valueIndex, delta) in values)
        {
            Sprite sprite = GetBattleValueSpriteSheet()[(int)valueIndex]; // 获取对应 Sprite
            valueSprites.Add(sprite);
            deltas.Add(delta);
        }

        // 3. 播放 Image 动画
        PlayEffectImageAnimation(valueSprites, deltas, totalDuration);

        // 4. 等待动画结束
        yield return new WaitForSeconds(totalDuration);
    }

    /// <summary>
    /// 单个数值文字动画
    /// </summary>
    private void AnimateSingleValue(BattleValue battleValue, int delta, float duration)
    {
        int valueIndex = (int)battleValue;
        if (delta == 0) return;
        if (valueIndex < 0 || valueIndex >= valuesText.Count) return;

        TextMeshProUGUI targetText = valuesText[valueIndex];
        RectTransform rt = targetText.rectTransform;
        InitialAttribute initAttr = valuesInitialAttributes[valueIndex];

        // 重置 Text
        rt.localPosition = initAttr.basePos;
        rt.localScale = Vector3.zero;
        targetText.color = new Color(initAttr.baseColor.r, initAttr.baseColor.g, initAttr.baseColor.b, 0f);
        targetText.fontSize = initAttr.baseFontSize * 2.5f;
        targetText.fontStyle = FontStyles.Bold;

        // Canvas 排序
        Canvas textCanvas = targetText.gameObject.AddComponent<Canvas>();
        textCanvas.overrideSorting = true;
        textCanvas.sortingOrder = EffectConstants.baseSortingOrder;

        // 根据 delta 设置颜色
        Color effectColor = delta > 0 ? GetSkillFunctionTypeColor(SkillFunctionType.Buff) :
                            delta < 0 ? GetSkillFunctionTypeColor(SkillFunctionType.Debuff) :
                            initAttr.baseColor;

        // 并行动画
        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOScale(initAttr.baseScale * 2f, duration * 0.4f).SetEase(Ease.OutBack))
           .Join(targetText.DOFade(1f, duration * 0.4f))
           .Join(targetText.DOColor(effectColor, duration * 0.4f))
           .Append(rt.DOScale(initAttr.baseScale * 1.2f, duration * 0.3f).SetEase(Ease.InOutBack))
           .Append(rt.DOScale(initAttr.baseScale, duration * 0.3f).SetEase(Ease.InBack))
           .Join(targetText.DOFade(0f, duration * 0.3f))
           .Join(targetText.DOColor(initAttr.baseColor, duration * 0.3f))
           .OnComplete(() =>
           {
               Destroy(textCanvas);
               rt.localScale = initAttr.baseScale;
               targetText.color = initAttr.baseColor;
               targetText.fontSize = initAttr.baseFontSize;
               targetText.fontStyle = initAttr.baseFontStyle;
           });
    }




    private void PlayEffectImageAnimation(List<Sprite> valueSprites, List<int> deltas, float totalDuration)
    {
        if (valueSprites == null || valueSprites.Count == 0) return;
        if (deltas == null || deltas.Count != valueSprites.Count)
            throw new ArgumentException("deltas count must match valueSprites count");

        int count = valueSprites.Count;
        float eachDuration = totalDuration;
        for (int i = 0; i < count; i++)
        {
            Sprite sprite = valueSprites[i];
            int delta = deltas[i];

            if (delta == 0)
            {
                DOTween.Sequence().AppendInterval(eachDuration).Play();
                continue;
            }

            if (sprite == null) continue;

            Image img = Instantiate(effect.valueEffectImage, effect.valueEffectImage.transform.parent);
            img.gameObject.SetActive(true);
            img.sprite = sprite;
            img.raycastTarget = false;

            Canvas canvas = img.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = EffectConstants.baseSortingOrder;

            RectTransform rt = img.rectTransform;
            RectTransform templateRt = effect.valueEffectImage.rectTransform;
            rt.position = templateRt.position;
            rt.anchorMin = templateRt.anchorMin;
            rt.anchorMax = templateRt.anchorMax;
            rt.pivot = templateRt.pivot;
            rt.sizeDelta = templateRt.sizeDelta;

            rt.localScale = Vector3.zero;
            img.color = new Color(1f, 1f, 1f, 0f);

            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(i * eachDuration);
            seq.Append(rt.DOScale(1.5f, eachDuration * 0.5f).SetEase(Ease.OutBack))
               .Join(img.DOFade(1f, eachDuration * 0.5f))
               .Append(rt.DOScale(1f, eachDuration * 0.5f).SetEase(Ease.InBack))
               .Join(img.DOFade(0f, eachDuration * 0.5f))
               .OnComplete(() => Destroy(img.gameObject));

            seq.Play();
        }
    }
    #endregion

    #region damage Animation
    public void PlayHitReaction(DamageResult result)
    {
        if (battleCharacterValue.IsAlive())
        {
            StartCoroutine(PlayHitReactionCoroutine(result));
        }
    }

    private IEnumerator PlayHitReactionCoroutine(DamageResult result)
    {
        // 同步播放震动 + 飞出伤害数字
        Sequence masterSeq = DOTween.Sequence();

        float direction = isEnemy ? -1f : 1f;

        // Step 1: 角色震动动画
        Sequence shakeSeq = CreateProfileShakeSequence(result, direction);
        masterSeq.Join(shakeSeq);

        // Step 2: 飞出伤害数字
        Sequence dmgSeq = CreateDamageTextSequence(result, direction);
        if (dmgSeq != null)
            masterSeq.Join(dmgSeq);

        if (!result.IsDodged)battleCharacterValue.ApplyDamage(result);
        // 等待动画完成
        BattleRightCol.Instance.DamageTakenRecord(battleCharacterValue, result);
        yield return masterSeq.WaitForCompletion();

        // 还原位置
        profileIcon.rectTransform.anchoredPosition = effect.GetProfileIconOriginalPos();
        if (!battleCharacterValue.IsAlive()) CharacterDieAnimation();
    }

    // =========================
    // Step 1: 角色震动动画
    // =========================
    private Sequence CreateProfileShakeSequence(DamageResult result, float direction)
    {
        float intensity = result.IsCritical ? 15f * 1.5f : 15f;
        Vector2 originalPos = effect.GetProfileIconOriginalPos();

        // X 和 Y 都根据 direction 变化，形成对角方向的震动
        Vector2 forwardOffset = new Vector2(direction * intensity, direction * intensity * 0.6f);

        float singleDuration = EffectConstants.totalEffectDuration;
        float forwardTime = singleDuration * EffectConstants.forwardTimeRate;
        float backwardTime = singleDuration * EffectConstants.backwardTimeRate;

        Sequence seq = DOTween.Sequence();

        seq.Append(profileIcon.rectTransform
            .DOAnchorPos(originalPos + forwardOffset, forwardTime)
            .SetEase(Ease.OutQuad));

        seq.Append(profileIcon.rectTransform
            .DOAnchorPos(originalPos, backwardTime)
            .SetEase(Ease.InOutSine));

        return seq;
    }

    // =========================
    // Step 2: 飞出伤害数字动画
    // =========================
    public Sequence CreateDamageTextSequence(DamageResult result, float direction)
    {

        // 2️⃣ 调用通用动画方法
        Sequence seq = GeneralAnimation.CreateDamageTextSequence(
            result,
            effect.damageTextPrefab.gameObject,   // prefab
            effect.damageTextParent,              // parent
           // effect.GetProfileIconOriginalPos(),   // 原始位置
            direction,
            new AnimationConfig()                 // 可选：传入自定义参数
            {
                TotalDuration = EffectConstants.totalEffectDuration,
                StartOffsetX = 25f,
                StartOffsetY = 25f,
                EndOffsetX = 75f,
                EndOffsetY = 75f,
                FadeRatio = 0.6f
            }
        );

        return seq;
    }

    #endregion

    void InitValuesInitialAttributes()
    {
        foreach (var textAttribute in valuesText)
        {
            InitialAttribute initAttribut = new InitialAttribute(textAttribute);
            valuesInitialAttributes.Add(initAttribut);
        }

    }

    public void CharacterDieAnimation()
    {
        StartCoroutine(FadeOutCoroutine(effect.canvasGroup));
    }

}
