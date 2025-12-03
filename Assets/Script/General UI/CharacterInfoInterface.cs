using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BaseCharacterMultiplePlaythroughsPanel;
public abstract class CharacterInfoInterface : MonoBehaviour
{
    [Header("Character Info")]
    [SerializeField] protected CharacterInfoGroup characterInfo;

    public struct CharacterInfoGroup
    {
        public TextMeshProUGUI nameText;
        public Image favorabilityImage;
        public TextMeshProUGUI favorabilityText;
        public TextMeshProUGUI moveText;
    }
}
