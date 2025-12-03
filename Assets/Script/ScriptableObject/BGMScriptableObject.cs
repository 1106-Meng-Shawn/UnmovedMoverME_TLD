using UnityEngine;
using System.Collections.Generic;
using static GetString;

[CreateAssetMenu(fileName = "NewBGMScriptableObject", menuName = "Music/BGM ScriptableObject")]
public class BGMScriptableObject : ScriptableObject
{
    public bool isStar = false;
    public int ID = -1;
    public AudioClip musicClip;
    public float musicTime = -1f;

    [System.NonSerialized]
    public Dictionary<string, string> titles = new Dictionary<string, string>();

    [System.NonSerialized]
    public Dictionary<string, string> authors = new Dictionary<string, string>();

    [SerializeField, HideInInspector]
    private string titlesJson;

    [SerializeField, HideInInspector]
    private string authorsJson;

    public string title
    {
        get => GetTitle(LanguageCode.EN);
        set
        {
            if (titles == null) titles = new Dictionary<string, string>();
            titles[LanguageCode.EN] = value;
        }
    }

    public string authorName
    {
        get => GetAuthorName(LanguageCode.EN);
        set
        {
            if (authors == null) authors = new Dictionary<string, string>();
            authors[LanguageCode.EN] = value;
        }
    }

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(titlesJson))
        {
            titles = JsonUtility.FromJson<SerializableDict>(titlesJson)?.ToDict() ?? new Dictionary<string, string>();
        }
        if (!string.IsNullOrEmpty(authorsJson))
        {
            authors = JsonUtility.FromJson<SerializableDict>(authorsJson)?.ToDict() ?? new Dictionary<string, string>();
        }
    }

    private void OnDisable()
    {
        titlesJson = JsonUtility.ToJson(SerializableDict.FromDict(titles));
        authorsJson = JsonUtility.ToJson(SerializableDict.FromDict(authors));
    }

    public string GetTitle(string languageCode = null)
    {
        if (titles == null) titles = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(languageCode))
        {
            languageCode = LanguageCode.EN;
        }

        if (titles.ContainsKey(languageCode))
        {
            return titles[languageCode];
        }

        if (titles.ContainsKey(LanguageCode.EN))
        {
            return titles[LanguageCode.EN];
        }

        return "Unknown Title";
    }

    public string GetAuthorName(string languageCode = null)
    {
        if (authors == null) authors = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(languageCode))
        {
            languageCode = LanguageCode.EN;
        }

        if (authors.ContainsKey(languageCode))
        {
            return authors[languageCode];
        }

        if (authors.ContainsKey(LanguageCode.EN))
        {
            return authors[LanguageCode.EN];
        }

        return "Unknown Author";
    }

    public void SetTitles(Dictionary<string, string> titleDict)
    {
        if (titles == null) titles = new Dictionary<string, string>();

        if (titleDict != null)
        {
            titles.Clear();
            foreach (var kvp in titleDict)
            {
                titles[kvp.Key] = kvp.Value;
            }
        }
    }

    public void SetAuthorNames(Dictionary<string, string> authorDict)
    {
        if (authors == null) authors = new Dictionary<string, string>();

        if (authorDict != null)
        {
            authors.Clear();
            foreach (var kvp in authorDict)
            {
                authors[kvp.Key] = kvp.Value;
            }
        }
    }

    public float GetMusicTime()
    {
        if (musicTime < 0 && musicClip != null)
        {
            return musicClip.length;
        }
        return musicTime;
    }

    public string GetMusicTimeString()
    {
        string timeString = GetTimeString(musicTime);
        return timeString;
    }

    public string GetButtonTitle(string languageCode = null)
    {
        string localizedTitle = GetTitle(languageCode);
        string ButtonTitle = $"NO.{ID} {localizedTitle} {GetMusicTimeString()}";
        return ButtonTitle;
    }

    public string GetButtonTitle()
    {
        return GetButtonTitle(null);
    }

    // 辅助类用于JSON序列化
    [System.Serializable]
    private class SerializableDict
    {
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>();

        public static SerializableDict FromDict(Dictionary<string, string> dict)
        {
            var sd = new SerializableDict();
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    sd.keys.Add(kvp.Key);
                    sd.values.Add(kvp.Value);
                }
            }
            return sd;
        }

        public Dictionary<string, string> ToDict()
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < keys.Count && i < values.Count; i++)
            {
                dict[keys[i]] = values[i];
            }
            return dict;
        }
    }
}