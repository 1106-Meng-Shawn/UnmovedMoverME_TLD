using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class InputPanelControl : MonoBehaviour
{
    public GameObject InputPanel;
    public TMP_InputField inputField;
    public Button CanceButton;
    public Button CheckButton;
    public Button ClearButton;
    public Button ResetButton;


    private int type; // 1 is saveOrload note change, 2 is player Name, 3 is setting change name
   // public SaveAndLoadButtonControl saveAndLoadButtonControl;
    private SaveData saveData; 
    private string savePath;

    private string originalString;
    private bool allowNullString = false;
    //   private TextMeshProUGUI TextAfterChange;


    void Start()
    {
        if (inputField == null)
        {
            inputField = GetComponentInChildren<TMP_InputField>();
        }

        inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
       // inputField.text = null;

        CanceButton.onClick.AddListener(OnCanceButtonClick);
        CheckButton.onClick.AddListener(OnCheckButtonClick);
        ClearButton.onClick.AddListener(OnClearButtonClick);
        ResetButton.onClick.AddListener(OnResetButtonClick);


    }


    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            HideInputPanel();
        }   
    }

    public string GetInputText()
    {
       
        return inputField.text;
    }


    void OnCheckButtonClick()
    {

    }


    void OnCanceButtonClick()
    {
        HideInputPanel();
    }

    void OnClearButtonClick()
    {
        inputField.text = null;
    }

    void OnResetButtonClick()
    {
        inputField.text = originalString;
    }


    public void ShowInputPanel(Action onCheckButtonClick, bool allowNullString,string originalString = null)
    {
        InputPanel.SetActive(true);
        this.allowNullString = allowNullString;

        this.originalString = originalString;
        inputField.text = originalString;

        CheckButton.onClick.RemoveAllListeners();
        CheckButton.onClick.AddListener(() => HandleCheck(onCheckButtonClick));
    }

    private void HandleCheck(Action onCheckButtonClick)
    {
        if (IsInputValid())
        {
            onCheckButtonClick?.Invoke();
            HideInputPanel();
        }
        else
        {
            // NotificationManage.Instance.ShowAtTop("Input is empty but not allowed.");
            NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Input_Empty);
        }
    }

    private bool IsInputValid()
    {
        if (allowNullString)return true;
        return !string.IsNullOrWhiteSpace(inputField.text);
    }



    public void HideInputPanel()
    {
        if (!InputPanel.activeSelf) return;
        originalString = null;
        inputField.text = null;
        saveData = null;
        InputPanel.SetActive(false);
    }

}
