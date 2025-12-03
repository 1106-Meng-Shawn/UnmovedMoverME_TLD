using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ExploreBottomControl : MonoBehaviour
{
    public GameObject charactersPanel;
    public GameObject itemsPanel;

    public Button charactersButton;
    public Button itemsButton;

    void Start()
    {
        if (charactersButton != null)
        {
            charactersButton.onClick.AddListener(() => TogglePanel(charactersPanel));
        }
        if (itemsButton != null)
        {
            itemsButton.onClick.AddListener(() => TogglePanel(itemsPanel));
        }

    }

    void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            if (panel.activeSelf)
            {
                panel.SetActive(false);
            }
            else
            {
                panel.SetActive(true);
            }
        }
    }
}
