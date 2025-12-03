using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SoundVolumeType
{
    Global,
    Background,
    SoundEffect,
    Character,
    Ambient
}

public class SoundVolumeControl : MonoBehaviour
{
    [Header("Volume Type")]
    [SerializeField] private SoundVolumeType soundVolumeType = SoundVolumeType.Global;

    [Header("UI References")]
    //[SerializeField] private TMP_Text volumeTypeName;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button volumeButton;
    [SerializeField] private TMP_Text volumeNumber;

    [Header("Test Audio Settings")]
    [SerializeField] private AudioClip testAudioClip; // 测试音频

    private AudioSource testAudioSource; // 测试用的AudioSource

    // 事件：当音量改变时通知外部
    public event Action<float> OnVolumeChanged;

    /// <summary>
    /// 初始化音量控制
    /// </summary>
    public void Initialize(AudioSource testAudioSource)
    {
        this.testAudioSource = testAudioSource;

        float currentVolume = GetCurrentVolume();
        volumeSlider.value = currentVolume;
        UpdateVolumeNumber(currentVolume);

        ApplyVolumeToAudioSource(currentVolume);
        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);

        if (volumeButton != null)
        {
            volumeButton.onClick.AddListener(OnButtonClick);
        }
    }

    private float GetCurrentVolume()
    {
        SoundSettingValue settings = SettingValue.Instance.GetSoundSettingValue();

        return soundVolumeType switch
        {
            SoundVolumeType.Global => settings.globalVolume,
            SoundVolumeType.Background => settings.backgroundVolume,
            SoundVolumeType.SoundEffect => settings.soundEffectsVolume,
            SoundVolumeType.Character => settings.characterVolume,
            SoundVolumeType.Ambient => settings.ambientVolume,
            _ => 0.5f
        };
    }

    private void SaveVolume(float volume)
    {
        SoundSettingValue settings = SettingValue.Instance.GetSoundSettingValue();

        switch (soundVolumeType)
        {
            case SoundVolumeType.Global:
                settings.globalVolume = volume;
                break;
            case SoundVolumeType.Background:
                settings.backgroundVolume = volume;
                break;
            case SoundVolumeType.SoundEffect:
                settings.soundEffectsVolume = volume;
                break;
            case SoundVolumeType.Character:
                settings.characterVolume = volume;
                break;
            case SoundVolumeType.Ambient:
                settings.ambientVolume = volume;
                break;
        }
    }

    public void SetSoundVolumeType(SoundVolumeType soundVolumeType)
    {
        this.soundVolumeType = soundVolumeType;
    }

    private void OnSliderValueChanged(float value)
    {
        UpdateVolumeNumber(value);
        SaveVolume(value);
        ApplyVolumeToAudioSource(value);
        OnVolumeChanged?.Invoke(value);
    }

    private void UpdateVolumeNumber(float value)
    {
        if (volumeNumber != null)
        {
            volumeNumber.text = (value * 100).ToString("F0") + "%";
        }
    }

    private void ApplyVolumeToAudioSource(float volume)
    {
        if (SoundManager.Instance == null) return;

        float actualVolume = CalculateActualVolume(volume);
        switch (soundVolumeType)
        {
            case SoundVolumeType.Global:
                UpdateAllAudioSources();
                break;

            case SoundVolumeType.Background:
                if (SoundManager.Instance.bgmSource != null)
                {
                    SoundManager.Instance.bgmSource.volume = actualVolume;
                }
                break;

            case SoundVolumeType.SoundEffect:
                if (SoundManager.Instance.sfxSource != null)
                {
                    SoundManager.Instance.sfxSource.volume = actualVolume;
                }
                break;

            case SoundVolumeType.Character:
                if (SoundManager.Instance.characterSource != null)
                {
                    SoundManager.Instance.characterSource.volume = actualVolume;
                }
                break;

            case SoundVolumeType.Ambient:
                if (SoundManager.Instance.ambientSource != null)
                {
                    SoundManager.Instance.ambientSource.volume = actualVolume;
                }
                break;
        }
    }

    /// <summary>
    /// 更新所有AudioSource（当全局音量改变时）
    /// </summary>
    private void UpdateAllAudioSources()
    {
        if (SoundManager.Instance == null) return;

        SoundSettingValue settings = SettingValue.Instance.GetSoundSettingValue();
        float globalVolume = settings.globalVolume;

        // 更新所有AudioSource
        if (SoundManager.Instance.bgmSource != null)
        {
            SoundManager.Instance.bgmSource.volume = settings.backgroundVolume * globalVolume;
        }

        if (SoundManager.Instance.sfxSource != null)
        {
            SoundManager.Instance.sfxSource.volume = settings.soundEffectsVolume * globalVolume;
        }

        if (SoundManager.Instance.characterSource != null)
        {
            SoundManager.Instance.characterSource.volume = settings.characterVolume * globalVolume;
        }

        if (SoundManager.Instance.ambientSource != null)
        {
            SoundManager.Instance.ambientSource.volume = settings.ambientVolume * globalVolume;
        }
    }

    /// <summary>
    /// 按钮点击事件 - 播放测试音效
    /// </summary>
    private void OnButtonClick()
    {
        PlayTestSound();
    }

    /// <summary>
    /// 播放测试音效
    /// </summary>
    public void PlayTestSound()
    {
        if (testAudioClip == null)
        {
            Debug.LogWarning($"[{soundVolumeType}] No test audio clip assigned!");
            return;
        }

        PlayTestSoundOnSource();
    }

    /// <summary>
    /// 在测试AudioSource上播放测试音效
    /// </summary>
    private void PlayTestSoundOnSource()
    {
        if (testAudioSource == null)
        {
            Debug.LogWarning($"[{soundVolumeType}] Test audio source is null!");
            return;
        }

        float testVolume = CalculateActualVolume(volumeSlider.value);
        testAudioSource.volume = testVolume;
        testAudioSource.PlayOneShot(testAudioClip);
    }

    /// <summary>
    /// 计算实际音量（考虑全局音量）
    /// </summary>
    private float CalculateActualVolume(float currentVolume)
    {
        if (soundVolumeType == SoundVolumeType.Global)
        {
            return currentVolume;
        }

        SoundSettingValue settings = SettingValue.Instance.GetSoundSettingValue();
        return currentVolume * settings.globalVolume;
    }

    /// <summary>
    /// 设置音量值
    /// </summary>
    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        volumeSlider.value = volume;
    }

    private void OnDestroy()
    {
        volumeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        if (volumeButton != null)
        {
            volumeButton.onClick.RemoveListener(OnButtonClick);
        }
    }
}