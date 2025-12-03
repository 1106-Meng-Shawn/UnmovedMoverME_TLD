using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine.TextCore.Text;
using static GetSprite;
using static GetColor;


public enum TurnState
{
    Next, Event
}
public class TurnManager : MonoBehaviour
{
    #region Singleton
    public static TurnManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    #region UI References
    public TMP_Text GameTimeText;
    public Button nextTurnButton;

    public Image TurnImageShow;
    public Image TurnImageHide;
    public Image TurnImage;
    public List<Sprite> SeasonSprite;
    public List<Sprite> TurnSprites;

    public List<Image> RightColBackGroundImages;
    public Image PlayerCountryImage;

    public GameObject pointer;
    public GameObject nextTurnpointer;

    public GameValue gameValue;
    public UITotalControl UITotalControl;

    public TurnState turnState = TurnState.Next;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        if (gameValue == null) gameValue = GameValue.Instance;

        // ????
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        GameValue.Instance.RegisterPlayerStateChanged(OnPlayerCountryChange);

        UpdateTurnUI();
        nextTurnButton.onClick.AddListener(NextTurn);

        InitializePointers();
        InitializeTurnImages();
        OnPlayerCountryChange();
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        GameValue.Instance.UnRegisterPlayerStateChanged(OnPlayerCountryChange);
    }
    #endregion

    #region Player / Locale Updates
    private void OnPlayerCountryChange()
    {
        string playerCountry = gameValue.GetPlayerCountryENName();
        var countryManager = gameValue.GetCountryManager();

        PlayerCountryImage.sprite = countryManager.GetCountryIcon(playerCountry);
        Color32 countryColor = countryManager.GetCountryColor(playerCountry);
        string CountryName = GetColorString(countryManager.GetCountry(playerCountry).GetCountryName(), countryColor);
        PlayerCountryImage.gameObject.GetComponent<IntroPanelShow>().SetIntroName(CountryName);
    }

    private void OnLocaleChanged(Locale locale)
    {
        UpdateTurnUI();
    }
    #endregion

    #region Turn UI Updates
    private void UpdateTurnUI()
    {
        GameTimeText.text = gameValue.GetTurnString();
    }

    private void InitializeTurnImages()
    {
        SetTurnImage(TurnImageShow);
        SetTurnImage(TurnImageHide);
        TurnImage.sprite = TurnSprites[(int)gameValue.GetCurrentSeason()];
        TurnImageHide.gameObject.SetActive(false);

        RightColBackGroundImages[0].sprite = GetSeasonSpriteCol(gameValue.GetCurrentSeason());
        RightColBackGroundImages[1].gameObject.SetActive(false);
    }

    private void SetTurnImage(Image image)
    {
        image.sprite = SeasonSprite[(int)gameValue.GetCurrentSeason()];
        image.fillMethod = Image.FillMethod.Radial90;
    }

    private void InitializePointers()
    {
        pointer.transform.rotation = Quaternion.Euler(0f, 0f, GetRotationForSeason((int)gameValue.GetCurrentSeason()));
        nextTurnpointer.transform.rotation = Quaternion.Euler(0f, 0f, GetRotationForTurnSeason((int)gameValue.GetCurrentSeason()));
        pointer.SetActive(false);
        nextTurnpointer.SetActive(false);
    }
    #endregion

    #region Turn Handling
    private void NextTurn()
    {
        if (turnState != TurnState.Next)
        {
            TriggerTurnRemind();
            return;
        }

        if (StoryRemindPanelManager.Instance.CheckStoryTrigger()) return;

        int currentSeason = (int)gameValue.GetCurrentSeason();
        int nextSeason = (currentSeason + 1) % 4;

        gameValue.GetPlayerState().AddSeason();

        if (gameValue.GetPlayerState().IsEndGame())
        {
            EndGame();
        }
        else
        {
            StartCoroutine(RotatePointer(currentSeason, nextSeason));
        }

        StoryRemindPanelManager.Instance.CheckStoryTrigger();
    }

    private void EndGame()
    {
        nextTurnButton.interactable = false;
        ENDPanelManager.Instance.ShowENDPanel(null);
    }
    #endregion

    #region Rotation / Animation
    private IEnumerator RotatePointer(int currentSeason, int nextSeason)
    {
        LoadPanelManage.Instance.AutoSaveGameTurnEnd();

        pointer.SetActive(true);
        nextTurnpointer.SetActive(true);
        yield return new WaitForSeconds(0.25f);

        TurnImage.sprite = TurnSprites[nextSeason];
        yield return new WaitForSeconds(0.25f);

        float duration = 0.2f;
        float timeElapsed = 0f;

        float startRotation = GetRotationForSeason(currentSeason);
        float endRotation = GetRotationForSeason(nextSeason);

        float nextTurnStartRotation = GetRotationForTurnSeason(currentSeason);
        float nextTurnEndRotation = GetRotationForTurnSeason(nextSeason);

        float rotationDiff = Mathf.DeltaAngle(startRotation, endRotation);
        float nextTurnRotationDiff = Mathf.DeltaAngle(nextTurnStartRotation, nextTurnEndRotation);

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            pointer.transform.rotation = Quaternion.Euler(0f, 0f, startRotation + rotationDiff * t);
            nextTurnpointer.transform.rotation = Quaternion.Euler(0f, 0f, nextTurnStartRotation + nextTurnRotationDiff * t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        pointer.transform.rotation = Quaternion.Euler(0f, 0f, endRotation);
        nextTurnpointer.transform.rotation = Quaternion.Euler(0f, 0f, nextTurnEndRotation);

        StartCoroutine(RotateImage());

        foreach (var character in gameValue.GetPlayerCharacters())
        {
            character.IsMoved = false;
        }

        UpdateTurnUI();
    }

    private IEnumerator RotateImage()
    {
        TurnImageHide.sprite = TurnImageShow.sprite;
        TurnImageHide.gameObject.SetActive(true);
        TurnImageShow.sprite = SeasonSprite[(int)gameValue.GetCurrentSeason()];
        yield return new WaitForSeconds(0.1f);

        RightColBackGroundImages[1].sprite = RightColBackGroundImages[0].sprite;
        RightColBackGroundImages[0].sprite = GetSeasonSpriteCol(gameValue.GetCurrentSeason());
        RightColBackGroundImages[1].gameObject.SetActive(true);

        float duration = 0.5f;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            TurnImageHide.fillClockwise = gameValue.GetCurrentSeason() != 0;
            TurnImageHide.fillAmount = Mathf.Lerp(1, 0, timeElapsed / duration);
            RightColBackGroundImages[1].fillAmount = Mathf.Lerp(1, 0, timeElapsed / duration);
            yield return null;
        }

        TurnImageHide.gameObject.SetActive(false);
        RightColBackGroundImages[1].gameObject.SetActive(false);
        yield return new WaitForSeconds(0.15f);

        pointer.SetActive(false);
        nextTurnpointer.SetActive(false);
        TurnImageHide.fillAmount = 1.0f;

        yield return new WaitForEndOfFrame();
        LoadPanelManage.Instance.AutoSaveGameTurnBegin();
    }
    #endregion

    #region TurnType Management
    public void SetTurnState(TurnState type)
    {
        turnState = type;
        switch (type)
        {
            case TurnState.Next: SetNextTurnState(); break;
            case TurnState.Event: SetEventTurnState(); break;
        }
    }

    private void SetNextTurnState()
    {
        Sprite unSel = Resources.Load<Sprite>("MyDraw/UI/GameUI/NextTurnArrowUnSel");
        Sprite sel = Resources.Load<Sprite>("MyDraw/UI/GameUI/NextTurnArrowSel");
        ApplyTurnButtonSprite(unSel, sel, "Next Season");
    }

    private void SetEventTurnState()
    {
        Sprite unSel = Resources.Load<Sprite>("MyDraw/UI/GameUI/NextTurnMakeUnSel");
        Sprite sel = Resources.Load<Sprite>("MyDraw/UI/GameUI/NextTurnMakeSel");
        ApplyTurnButtonSprite(unSel, sel, "Event Pending");
    }

    private void ApplyTurnButtonSprite(Sprite unSel, Sprite sel, string introName)
    {
        nextTurnButton.image.sprite = unSel;
        nextTurnButton.GetComponent<ButtonEffect>().SetChangeSprite(sel, unSel);
        nextTurnButton.GetComponent<IntroPanelShow>().SetIntroName(introName);
    }

    private void TriggerTurnRemind()
    {
        if (turnState == TurnState.Event) TriggerEventTurnType();
    }

    private void TriggerEventTurnType()
    {
        int eventNum = RightColumnManage.Instance.CountTriggerableEvents();
        RightColumnManage.Instance.ToggleEventCol();
        NotificationManage.Instance.ShowAtTopByKey(NotificationKeyConstants.Event_MustResolve, eventNum.ToString());
    }
    #endregion

    #region Utility Methods
    private float GetRotationForSeason(int season)
    {
        switch (season)
        {
            case 3: return 95f;
            case 2: return 60f;
            case 1: return 30f;
            case 0: return 0f;
            default: return 0f;
        }
    }

    private float GetRotationForTurnSeason(int season)
    {
        switch (season)
        {
            case 3: return -80f;
            case 2: return -110f;
            case 1: return -140f;
            case 0: return -180f;
            default: return 0f;
        }
    }




    //private Sprite GetRightColBackGroundSprite(int season)
    //{
    //    string path = "MyDraw/UI/GameUI/";
    //    return Resources.Load<Sprite>(season switch
    //    {
    //        0 => path + "SpringRightColumn",
    //        1 => path + "SummerRightColumn",
    //        2 => path + "FallRightColumn",
    //        3 => path + "WinterRightColumn",
    //        _ => null
    //    });
    //}
    #endregion
}
