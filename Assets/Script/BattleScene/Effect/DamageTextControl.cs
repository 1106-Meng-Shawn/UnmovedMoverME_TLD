using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextControl : MonoBehaviour
{
    public class DamageTextConstants
    {
        public static readonly Color CriticalColor = Color.yellow;
        public static readonly Color BlockColor = Color.gray;
        public static readonly Color DamageColor = Color.red;
        public static readonly Color DodgedColor = Color.white;
    }


    [Header("Text")]
    public TextMeshProUGUI DamageText;  // ???? / MISS
    public TextMeshProUGUI CritText;    // ????
    public TextMeshProUGUI BlockText;   // ????

    private void Awake()
    {
        ResetTexts();
    }

    public void SetDamageResult(DamageResult result)
    {
        SetDamageText(result);
        SetCritText(result);
        SetBlockText(result);
    }

    private void SetDamageText(DamageResult result)
    {
        if (DamageText == null) return;

        DamageText.text = result.IsDodged ? "MISS" : result.Damage.ToString();
        DamageText.color = GetDamageTextColor(result);
    }

    private void SetCritText(DamageResult result)
    {
        if (CritText == null) return;

        if (result.IsCritical)
        {
            CritText.gameObject.SetActive(true);
            CritText.text = "CRITICAL";
        }
        else
        {
            CritText.gameObject.SetActive(false);
            CritText.text = "";
        }
    }

    // =========================
    // Step 3: ??????
    // =========================
    private void SetBlockText(DamageResult result)
    {
        if (BlockText == null) return;

        if (result.IsBlock)
        {
            BlockText.gameObject.SetActive(true);
            BlockText.text = "BLOCK";
        }
        else
        {
            BlockText.gameObject.SetActive(false);
            BlockText.text = "";
        }
    }
    private Color GetDamageTextColor(DamageResult result)
    {
        if (result.IsCritical) return DamageTextConstants.CriticalColor;
        if (result.IsBlock) return DamageTextConstants.BlockColor;
        if (result.IsDodged) return DamageTextConstants.DodgedColor;
        return DamageTextConstants.DamageColor;
    }

    /// <summary>
    /// ???????????
    /// </summary>
    public void ResetTexts()
    {
        ResetDamageText();
        ResetCritText();
        ResetBlockText();
    }

    private void ResetDamageText()
    {
        if (DamageText == null) return;
        DamageText.text = "";
        DamageText.color = DamageTextConstants.DamageColor;
    }

    private void ResetCritText()
    {
        if (CritText == null) return;
        CritText.gameObject.SetActive(false);
        CritText.text = "CRITICAL";
        CritText.color = DamageTextConstants.CriticalColor;
    }

    private void ResetBlockText()
    {
        if (BlockText == null) return;
        BlockText.gameObject.SetActive(false);
        BlockText.text = "BLOCK";
        BlockText.color = DamageTextConstants.BlockColor;
    }

}
