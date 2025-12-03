using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultiplePlaySaveButtonsListControl : MonoBehaviour
{

    [SerializeField] Image Achievement;
    [SerializeField] Button AchievementButton;
    [SerializeField] List<Button> AchievementButtons;

    [SerializeField] ScrollRect scrollRect;
    [SerializeField] MultiplePlaythroughsGameCharacterRowControl MultiplePlaythroughsGameCharacterRowControlPrefab;
    private List<MultiplePlaythroughsGameCharacterRowControl> MultiplePlaythroughsGameCharacterRowControls = new List<MultiplePlaythroughsGameCharacterRowControl>();

    public enum SortField
    {
        Name, Favorability, Force, Health, Limit, Attack, Food, Leadership, Defense, Science, Scout, Magic, Politics, Build, Speed, Gold, Negotiation, Lucky, Faith, Charm, None
    }

    public enum SortDirection
    {
        None, Ascending, Descending
    }

    public struct SortStatus
    {
        public SortField Field;
        public SortDirection Direction;

        public bool IsSorted => Field != SortField.None && Direction != SortDirection.None;
    }

    List<SortField> activeSortFields = new List<SortField>();
    private SortStatus currentSort;




}
