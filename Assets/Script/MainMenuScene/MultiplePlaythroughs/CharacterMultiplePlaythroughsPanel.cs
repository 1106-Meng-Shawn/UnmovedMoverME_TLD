using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMultiplePlaythroughsPanel : BaseCharacterMultiplePlaythroughsPanel
{
    [SerializeField] private Button BackButton;
    [SerializeField] private TextMeshProUGUI AchievementCoefficientText;
    [SerializeField] private GameObject CharacterTypeRow;


    private void Start()
    {
        BackButton.onClick.AddListener(ClosePanel);
    }

    public void ShowPanel(MultiplePlaythroughsGameCharacterRowControlData data)
    {
        base.SetData(data);
        gameObject.SetActive(true);
        CharacterTypeRow.SetActive(!data.IsImportant());
    }

    public void ClosePanel()
    {
        base.SetData(null);
        gameObject.SetActive(false);
    }


}
