using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class MultiplePlaythroughsManager : MonoBehaviour
{
    public static MultiplePlaythroughsManager Instance;

    [SerializeField] private Button CloseButton;
    [SerializeField] private GameObject MultiplePlaythroughsPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitButton();
    }

    void InitButton()
    {
        CloseButton.onClick.AddListener(ClosePanel);
    }




    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && MultiplePlaythroughsPanel.activeSelf)
        {
            ClosePanel();
        }
    }

    public void ShowPanel()
    {
        MultiplePlaythroughsPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        MultiplePlaythroughsPanel.SetActive(false);
    }
}

