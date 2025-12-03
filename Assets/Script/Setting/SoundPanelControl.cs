using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SoundPanelControl : SettingPanelBase
{
    [SerializeField] AudioSource testAudioSource;
    [Header("Volume Controls")]
    public SoundVolumeControl globalVolumeControl;
    public SoundVolumeControl backgroundVolumeControl;
    public SoundVolumeControl soundEffectsVolumeControl;
    public SoundVolumeControl characterVolumeControl;
    public SoundVolumeControl ambientVolumeControl;

    [SerializeField] Button musicButton;

    private List<SoundVolumeControl> allVolumeControls = new List<SoundVolumeControl>();

    public override void Init()
    {
        CollectVolumeControls();

        foreach (var control in allVolumeControls)
        {
            if (control != null)
            {
                control.Initialize(testAudioSource);
            }
        }

        InitButtons();

    }


    void InitButtons()
    {
        musicButton.onClick.AddListener(OnMusicButtonClick);
    }

    private void CollectVolumeControls()
    {
        allVolumeControls.Clear();

        allVolumeControls.Add(globalVolumeControl);
        globalVolumeControl.SetSoundVolumeType(SoundVolumeType.Global);

        allVolumeControls.Add(backgroundVolumeControl);
        backgroundVolumeControl.SetSoundVolumeType(SoundVolumeType.Background);

        allVolumeControls.Add(soundEffectsVolumeControl);
        soundEffectsVolumeControl.SetSoundVolumeType(SoundVolumeType.SoundEffect);


        allVolumeControls.Add(characterVolumeControl);
        characterVolumeControl.SetSoundVolumeType(SoundVolumeType.Character);


        allVolumeControls.Add(ambientVolumeControl);
        ambientVolumeControl.SetSoundVolumeType(SoundVolumeType.Ambient);


    }

    public override void OnDefaultButtonClick()
    {
        SoundSettingValue soundSettingValue = new SoundSettingValue();
        SetSettingValue(soundSettingValue);
    }

    void SetSettingValue(SoundSettingValue soundSettingValue)
    {
        if (globalVolumeControl != null)
            globalVolumeControl.SetVolume(soundSettingValue.globalVolume);

        if (backgroundVolumeControl != null)
            backgroundVolumeControl.SetVolume(soundSettingValue.backgroundVolume);

        if (soundEffectsVolumeControl != null)
            soundEffectsVolumeControl.SetVolume(soundSettingValue.soundEffectsVolume);

        if (characterVolumeControl != null)
            characterVolumeControl.SetVolume(soundSettingValue.characterVolume);

        if (ambientVolumeControl != null)
            ambientVolumeControl.SetVolume(soundSettingValue.ambientVolume);

    }



    void OnMusicButtonClick()
    {
        MusicPanelManager.Instance.OpenPanel();
        SettingsManager.Instance.ClosePanel();
    }


}
public class SoundSettingValue
{
    public float globalVolume { get; set; } = 0.5f;
    public float backgroundVolume { get; set; } = 0.5f;
    public float soundEffectsVolume { get; set; } = 0.5f;
    public float characterVolume { get; set; } = 0.5f;
    public float ambientVolume { get; set; } = 0.5f;

    public SoundSettingValue() { }


}
