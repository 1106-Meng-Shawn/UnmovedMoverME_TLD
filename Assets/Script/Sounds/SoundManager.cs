using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource sfxSource;
    public AudioSource bgmSource;
    public AudioSource vocalSource;
    public AudioSource ambientSource;
    public AudioSource characterSource;

    private Dictionary<string, AudioClip> clipCache = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    AudioClip LoadClip(string path)
    {

        if (clipCache.TryGetValue(path, out var clip))
            return clip;

        clip = Resources.Load<AudioClip>(path);
        if (clip != null)
        {
            clipCache[path] = clip;
        }
        else
        {
            Debug.LogWarning($"? Missing Audio: {path}");
        }

        return clip;
    }


    public void PlaySelSFX() { string path = "Sounds/UI/RowSel"; PlaySFX(path); }


    public void PlaySFX(string path)
    {
        var clip = LoadClip(path);
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(string title, float fadeTime = 1f)
    {
        BGMScriptableObject BGMScriptableObject = SettingValue.Instance.GetBGM(title);
        ExtrasValue.Instance.UnlockBGM(BGMScriptableObject.ID);
        StartCoroutine(FadeToNewClip(bgmSource, BGMScriptableObject.musicClip, fadeTime, true));
    }


    public void PlayerBGM(int BGMID, float fadeTime = 1f)
    {
        BGMScriptableObject BGMScriptableObject = SettingValue.Instance.GetBGM(BGMID);
        ExtrasValue.Instance.UnlockBGM(BGMID);
        StartCoroutine(FadeToNewClip(bgmSource, BGMScriptableObject.musicClip, fadeTime, true));
    }






    public void PlayVocal(string path, float fadeTime = 0.5f)
    {
        StartCoroutine(FadeToNewClip(vocalSource, LoadClip(path), fadeTime, false));
    }

    public void PlayAmbient(string path, float fadeTime = 1f)
    {
        StartCoroutine(FadeToNewClip(ambientSource, LoadClip(path), fadeTime, true));
    }

    IEnumerator FadeToNewClip(AudioSource source, AudioClip newClip, float fadeTime, bool loop)
    {
        if (newClip == null) yield break;

        float startVol = source.volume;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
            yield return null;
        }
        source.clip = newClip;
        source.loop = loop;
        source.Play();

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0f, startVol, t / fadeTime);
            yield return null;
        }
        source.volume = startVol;
    }

    public void StopAllSound()
    {
        bgmSource.Stop();
        vocalSource.Stop();
        ambientSource.Stop();
        sfxSource.Stop();
    }
}
