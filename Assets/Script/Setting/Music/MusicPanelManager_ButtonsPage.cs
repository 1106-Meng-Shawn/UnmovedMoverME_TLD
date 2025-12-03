using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MusicPanelManager_ButtonsPage : MonoBehaviour
{
    [SerializeField] List<Button> MusicButtons = new List<Button> ();

    [SerializeField] BottomBottons bottomBottons;

    [Serializable]
    public struct BottomBottons
    {
        public Button FirstPageButton;
        public Button Previous5PageButton;
        public Button PreviousPageButton;
        public TMP_InputField InputField;
        public Button NextPageButton;
        public Button Next5PageButton;
        public Button LastPageButton;

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetMusicButtons(List<BGMScriptableObject> musicDatas)
    {
        for (int i = 0; i < MusicButtons.Count; i++)
        {
            if (i < musicDatas.Count && musicDatas[i] != null)
            {
                MusicButtons[i].gameObject.SetActive(true);

                TMP_Text buttonText = MusicButtons[i].GetComponentInChildren<TMP_Text>();

                if (buttonText != null)
                {
                   buttonText.text = musicDatas[i].GetButtonTitle();
                }
            }
            else
            {
                MusicButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
