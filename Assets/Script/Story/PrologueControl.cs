using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text dialogueText;

    private List<DialogueEntry> dialogues;
    private int currentDialogueIndex = 0;
    private bool isTextFullyDisplayed = false;
    private Coroutine currentCoroutine = null;
    public float textSpeed;
    public float waitTime;

    private bool isSkipping = false; // ???????????

    [System.Serializable]
    public class DialogueEntry
    {
        public int ID;
        public string Text;
        public int NextID;
    }

    void Start()
    {
        string filePath = "Assets/Resource/Story/Prologue.csv";
        dialogues = ReadCSV(filePath);
        DisplayDialogue(currentDialogueIndex);
    }

    List<DialogueEntry> ReadCSV(string filePath)
    {
        List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();

        try
        {
            using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                bool isFirstLine = true;

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }

                    string[] values = ParseCsvLine(line);

                    if (values.Length == 3)
                    {
                        DialogueEntry entry = new DialogueEntry();

                        if (!int.TryParse(values[0], out entry.ID))
                        {
                            Debug.LogWarning($"Invalid ID format in line: {line}");
                            continue;
                        }

                        entry.Text = values[1];

                        if (values[2] == "END" || values[2] == "-1")
                        {
                            entry.NextID = -1;
                        }
                        else if (!int.TryParse(values[2], out entry.NextID))
                        {
                            Debug.LogWarning($"Invalid NextID format in line: {line}");
                            continue;
                        }

                        dialogueEntries.Add(entry);
                    }
                    else
                    {
                        Debug.LogWarning("Invalid CSV format detected. Skipping line: " + line);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error reading CSV file: " + ex.Message);
        }

        return dialogueEntries;
    }

    private string[] ParseCsvLine(string line)
    {
        List<string> values = new List<string>();
        bool insideQuotes = false;
        string currentValue = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                insideQuotes = !insideQuotes;
            }
            else if (c == ',' && !insideQuotes)
            {
                values.Add(currentValue);
                currentValue = "";
            }
            else
            {
                currentValue += c;
            }
        }

        if (currentValue.Length > 0)
        {
            values.Add(currentValue);
        }

        return values.ToArray();
    }

    void DisplayDialogue(int index)
    {
        if (index < 0 || index >= dialogues.Count)
            return;

        DialogueEntry entry = dialogues[index];
        isTextFullyDisplayed = false;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(DisplayText(entry.Text, entry.NextID));
    }

    private IEnumerator DisplayText(string text, int nextID)
    {
        dialogueText.text = ""; // ????
        isTextFullyDisplayed = false;
        isSkipping = false; // ??????

        // ????????
        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text += text[i];
            yield return new WaitForSeconds(textSpeed);

            // ?????????????????
            if (isSkipping)
            {
                dialogueText.text = text; // ????????
                break;
            }
        }

        isTextFullyDisplayed = true;

        // ??????????????
        if (nextID == -1)
        {
            Debug.Log("End of Dialogue");
        }
        else
        {
            // ?????????
            yield return new WaitForSeconds(waitTime);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isTextFullyDisplayed)
            {
                // ?????????????
                isSkipping = true; // ??????? true

                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }

                DialogueEntry entry = dialogues[currentDialogueIndex];
                dialogueText.text = entry.Text; // ????????
                isTextFullyDisplayed = true; // ????????

                // ??????????????
                StartCoroutine(WaitAndContinue(waitTime));
            }
            else
            {
                // ???????????????????????
                if (currentDialogueIndex + 1 < dialogues.Count)
                {
                    currentDialogueIndex++;
                    DisplayDialogue(currentDialogueIndex);
                }
            }
        }
    }

    private IEnumerator WaitAndContinue(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (currentDialogueIndex + 1 < dialogues.Count && isTextFullyDisplayed)
        {
            currentDialogueIndex++;
            DisplayDialogue(currentDialogueIndex);
        }
    }
}
