using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;


public class HistoryManager : MonoBehaviour
{
    public static HistoryManager Instance { get; private set; }
    public HistoryLineControl historyLinePrefab;
    private List<HistoryLineControl> currentHistoryLines = new List<HistoryLineControl>();
    public ScrollRect historyScrollRect;
    public Button closeButton;

    private LinkedList<HistoryData> historyRecords = new LinkedList<HistoryData>();

    public static event Action OnShowPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        closeButton.onClick.AddListener(CloseHistory);
    }

    public void ShowHistory()
    {
        LinkedListNode<HistoryData> currentNode = historyRecords.Last;
        DestroyHistoryContent();
        while (currentNode != null)
        {
            AddHistoryItem(currentNode.Value);
            currentNode = currentNode.Previous;
        }
        historyScrollRect.GetComponent<RectTransform>().localPosition = Vector3.zero;

        historyScrollRect.gameObject.SetActive(true);

        ScrollToBottom();
        LayoutRebuilder.ForceRebuildLayoutImmediate(historyScrollRect.content.GetComponent<RectTransform>());

    }

    void ScrollToBottom()
    {
        StartCoroutine(ScrollToBottomCoroutine());
    }

    IEnumerator ScrollToBottomCoroutine()
    {
        yield return null;

        historyScrollRect.verticalNormalizedPosition = 0f;
    }


    public void InitializeHistory()
    {
        DestroyHistoryContent();
        historyRecords = new LinkedList<HistoryData>(); 
    }

    void DestroyHistoryContent()
    {
        foreach (var child in currentHistoryLines)
        {
            Destroy(child.gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && historyScrollRect.gameObject.activeSelf)
        {
           CloseHistory();
        }
     }


    void CloseHistory()
    {
        historyScrollRect.gameObject.SetActive(false);
        TotalStoryManager.Instance.OpenUI();

    }

    private void AddHistoryItem(HistoryData HistoryData)
    {
        GameObject historyItem = Instantiate(historyLinePrefab.gameObject, historyScrollRect.content);
            historyItem.GetComponent<HistoryLineControl>().SetTheLine(HistoryData);
            historyItem.transform.SetAsFirstSibling();
        currentHistoryLines.Add(historyItem.GetComponent<HistoryLineControl>());
    }


    public void RecordHistory(HistoryData data)
    {
        historyRecords.AddLast(data);
    }

    public LinkedList<HistoryData> GetHistoryRecords()
    {
        return historyRecords;
    }

    public void SetHistoryRecords(LinkedList<HistoryData> historyRecords)
    {
       this.historyRecords = historyRecords;
    }

}
