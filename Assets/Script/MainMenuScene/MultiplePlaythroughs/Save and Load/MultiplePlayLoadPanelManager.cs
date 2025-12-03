using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultiplePlayLoadPanelManager : MonoBehaviour
{
    public static MultiplePlayLoadPanelManager Instance;

    [SerializeField] private GameObject LoadPanel;

    [SerializeField] private MultiplePlayDetailInfoControl MultiplePlayDetailInfoControl;
    [SerializeField] private MultiplePlaySaveInfoControl MultiplePlaySaveInfoControl;
    [SerializeField] private MultiplePlaySaveButtonsListControl MultiplePlaySaveButtonsListControl;

    [SerializeField] private InputPanelControl inputNotePanel;

    [SerializeField] private Button BackButton;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // ClosePanel();
        InitButtons();
    }

    void InitButtons()
    {
        BackButton.onClick.AddListener(ClosePanel);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ClosePanel();
        }
    }



    public void ShowPanel()
    {
        LoadPanel.gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        if (!LoadPanel.activeSelf) return;
        LoadPanel.gameObject.SetActive(false);

    }


}
