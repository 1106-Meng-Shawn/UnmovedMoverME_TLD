using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.U2D;
using UnityEngine.UI;
using static GetSprite;


public class SideImageControl : MonoBehaviour
{
    public Image leftImage;
    public Image rightImage;

    private int itemType = 0; // 0 is storage. 1 buy store, 2 sell store

    public Button StarButton; 
    private bool isStar = false;

    public List<Button> maidButtons;
    private List<string> maidIDs = new List<string>{
        CharacterConstants.EmiliaKey,
        CharacterConstants.FriederikeKey,
        CharacterConstants.PurpleMaidKey,
        CharacterConstants.PinkMaidKey,
        CharacterConstants.PinkMaid2Key,
        CharacterConstants.BuleMaidKey,
        CharacterConstants.BuleMaid2key,
        CharacterConstants.BlackMaidKey,
        CharacterConstants.BlackMaid2Key,
        CharacterConstants.GreenMaidKey,
        CharacterConstants.GreenMaid2Key,
        CharacterConstants.MageMaidKey,
        CharacterConstants.BerserkerMaidKey,
        CharacterConstants.HiyoriKey,
        CharacterConstants.LiyaKey
    };

    private int maidType = 0;

    private List<Sprite> maidLeftSprite = new List<Sprite>();
    private List<Sprite> maidRightSprite = new List<Sprite>();

    private List<Sprite> storeSprite = new List<Sprite>();



    public GameObject leftImageGameObject;
    public GameObject rightImageGameObject;

    public Button LockButton;
    private bool isLock = false;
    public Animation buttonsAnim;
    public GameObject bottomButtons;
    private float mouseNotOverInButtonTimer = 0f;
    private bool isHidden =false;

    private Action maidLiyaHandler;
    private Canvas uiCanvas;


    private void Start()
    {
       // GameValue.Instance.PlayerCharacters.OnListChanged += ShowOrHideMaidButtons;
        GameValue.Instance.RegisterPlayerCharacterChanged(ShowOrHideMaidButtons);

        InitMaid();
        InitStar();
        InitLock();
        InitializeBottomButtons();
        uiCanvas = GeneralManager.Instance.uiCanvas;


    }

    private void OnDestroy()
    {
        //GameValue.Instance.PlayerCharacters.OnListChanged -= ShowOrHideMaidButtons;
        GameValue.Instance.UnRegisterPlayerCharacterChanged(ShowOrHideMaidButtons);
        for (int i = 0; i < maidIDs.Count; i++)
        {
            Character maid = GameValue.Instance.GetCharacterByKey(maidIDs[i]);
            if (maid != null && maid.GetCharacterKey() == CharacterConstants.LiyaKey && maidLiyaHandler != null)
            {
                maid.OnCharacterChanged -= maidLiyaHandler;
            }
        }
    }

    private void Update()
    {
        if(itemType == 0) ShowOrHideoButtons();

    }

    bool IsHittingBottomButtons()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            bottomButtons.GetComponent<RectTransform>(),
            Input.mousePosition,
            uiCanvas.worldCamera
            // ? ??????Screen Space - Overlay ? null
            );
    }




    void InitializeBottomButtons()
    {

        StartCoroutine(WaitForSecondsShowBottomButtons(0.1f));
    }

    IEnumerator WaitForSecondsShowBottomButtons(float delay)
    {
        yield return new WaitForSeconds(delay);

        mouseNotOverInButtonTimer = 0f;
        ShowBottomButtons();

    }

    void ShowOrHideoButtons()
    {
        if (isLock) return;

        bool mouseOver = IsHittingBottomButtons();

        if (mouseOver && isHidden)
        {
            mouseNotOverInButtonTimer = 0f;
            ShowBottomButtons();
        }
        else if (!mouseOver && !isHidden)
        {
            mouseNotOverInButtonTimer += Time.deltaTime;
            if (!isHidden && mouseNotOverInButtonTimer >= 1f)
            {
                HideBottomButtons();
            }
        }

    }

    void ShowBottomButtons()
    {
        if (isLock) return;
        if (isHidden)
        {
            buttonsAnim.Play("ShowItemBottomButton");
            isHidden = false;

        }

    }

    void HideBottomButtons()
    {
        if (isLock) return;
        if (!isHidden)
        {
            /*StartCoroutine(WaitForAnimation("HideBottomButton"));
            isHidden = true;*/

            buttonsAnim.Play("HideItemBottomButton");
            isHidden = true;

        }
    }


    void InitStar()
    {
        UpStarButtonSprite();
        StarButton.onClick.AddListener(OnStarClick);
    }


    void OnStarClick()
    {
        isStar = !isStar;
        UpStarButtonSprite();
    }

    void UpStarButtonSprite()
    {
        string imagePath = "MyDraw/UI/Other/";
        if (isStar){StarButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath + "Star");}
        else { StarButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath + "Unstar"); }

    }


    void ShowOrHideMaidButtons()
    {
        for (int i = 0; i < maidButtons.Count; i++)
        {
            Character maid = GameValue.Instance.GetCharacterByKey(maidIDs[i]);
            if (maid != null)
            {
                string type = maid.Type.ToLower();
                bool isMaid = true;
                switch (maid.GetCharacterKey())
                {
                    case CharacterConstants.LiyaKey:
                     isMaid = type.Contains("maid");
                     break;
                }
                maidButtons[i].image.gameObject.SetActive(isMaid && maid.GetCountryENName() == GameValue.Instance.GetPlayerCountryENName());
            }
        }
    }

    void InitMaid()
    {
        maidLeftSprite.Clear();
        maidRightSprite.Clear();
        string imagePath = "MyDraw/Character/";

        int count = Mathf.Min(maidButtons.Count, maidIDs.Count);

        for (int i = 0; i < count; i++)
        {
            Character maid = GameValue.Instance.GetCharacterByKey(maidIDs[i]);
            Sprite leftSprite = null;
            Sprite rightSprite = null;
            Sprite iconSprite = null;
            if ( maid != null)
            {

                if (maid != null && maid.GetCharacterKey() == CharacterConstants.LiyaKey)
                {
                    int maidIndex = i;
                    maidLiyaHandler = () => OnMaidTypeChanged(maid, maidIndex);
                    maid.OnCharacterChanged += maidLiyaHandler;
                }

                switch (maid.GetCharacterKey())
                {
                    case CharacterConstants.LiyaKey:
                        iconSprite = Resources.Load<Sprite>(imagePath + $"{maid.GetCharacterENName()}/{maid.GetCharacterENName()}MaidStandIcon");
                        leftSprite = Resources.Load<Sprite>(imagePath + $"{maid.GetCharacterENName()}/{maid.GetCharacterENName()}MaidStand");
                        break;

                    default:
                        iconSprite = maid.icon;
                        leftSprite = maid.image;
                        break;

                }
                rightSprite = Resources.Load<Sprite>(imagePath + $"{maid.GetCharacterENName()}/{maid.GetCharacterENName()}Item");
            }
            int index = i;
            maidButtons[i].onClick.AddListener(() => ChangeMaidByType(index));
            maidButtons[i].image.sprite = iconSprite;
            maidLeftSprite.Add(leftSprite);
            maidRightSprite.Add(rightSprite);
        }

        if (!isStar) maidType = UnityEngine.Random.Range(0, maidLeftSprite.Count);
        ChangeMaidSprite();
    }


    private void OnMaidTypeChanged(Character maid, int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= maidButtons.Count)
        {
            Debug.LogError(
                $"[OnMaidTypeChanged] buttonIndex out number: {buttonIndex} / maidButtons.Count={maidButtons.Count} " +
                $"maidLeftSprite.Count={maidLeftSprite.Count}, maidIDs.Count={maidIDs.Count}, " +
                $"maid={maid?.GetCharacterENName()} ({maid?.GetCharacterID()})"
            );
        }

        bool isMaid = maid.Type != null && maid.Type.ToLower().Contains("maid");
        maidButtons[buttonIndex].gameObject.SetActive(isMaid && maid.GetCountryENName() == GameValue.Instance.GetPlayerCountryENName());

        if (isMaid)
        {
            maidButtons[buttonIndex].image.sprite = maid.icon;
            if (maidLeftSprite.Count > buttonIndex)
                maidLeftSprite[buttonIndex] = maid.image;
        }
    }

    void InitLock()
    {
        UpLockButtonSprite();
        LockButton.onClick.AddListener(OnLockClick);
    }

    void OnLockClick()
    {
        isLock = !isLock;
        isHidden = false;
        mouseNotOverInButtonTimer = 0f;
        UpLockButtonSprite();
    }

    void UpLockButtonSprite()
    {
        string imagePath = "MyDraw/UI/Other/";
        if (isLock) {
            LockButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath + "Lock");
            LockButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(imagePath + "LockSel"), Resources.Load<Sprite>(imagePath + "Lock"));
        }
        else {
            LockButton.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath + "Unlock");
            LockButton.gameObject.GetComponent<ButtonEffect>().SetChangeSprite(Resources.Load<Sprite>(imagePath + "UnlockSel"), Resources.Load<Sprite>(imagePath + "Unlock"));
        }

    }


    void ChangeMaidByType(int maidType)
    {
        this.maidType = maidType;
        leftImage.sprite = maidLeftSprite[maidType];
        rightImage.sprite = maidRightSprite[maidType];
    }

    public void ChangeMaidSprite()
   {
        ShowOrHideMaidButtons();
        // if (maidLeftSprite.Count == 0) InitMaid();
        itemType = 0;
        StarButton.gameObject.SetActive(true);
        bottomButtons.gameObject.SetActive(true);
        if (!isStar)
        {
            do
            {
                maidType = UnityEngine.Random.Range(0, maidIDs.Count);
            }
            while (!maidButtons[maidType].gameObject.activeSelf);
        }
        //InitializeBottomButtons();
        mouseNotOverInButtonTimer = 0f;
       // isHidden = false;
        ShowBottomButtons();
        // Debug.Log($"maid type is {maidType} and {maidLeftSprite.Count}");

        if (maidLeftSprite.Count == 0) InitMaid();

        ChangeMaidByType(maidType);
    }


    public void ChangeStoreSprite()
    {
        itemType = 1;
        string imagePath = "MyDraw/Character/Hanna/";
        StarButton.gameObject.SetActive(false);
        bottomButtons.gameObject.SetActive(false);
        leftImage.sprite = Resources.Load<Sprite>(imagePath + "HannaItem");
    }


    public void ShowLeftGameObeject()
    {
        rightImageGameObject.SetActive(false);
        leftImageGameObject.SetActive(true);
        mouseNotOverInButtonTimer = 0f;
        if (!isStar) ShowBottomButtons();
    }

    public void ShowRightGameObeject()
    {
        if (!isLock) PlayToEnd(buttonsAnim, "HideItemBottomButton");
        isHidden = true;
        leftImageGameObject.SetActive(false);
        rightImageGameObject.SetActive(true);
    }

    public static void PlayToEnd(Animation anim, string clipName)
    {
        anim.Play(clipName);
        AnimationState state = anim[clipName];
        state.time = state.length;
        anim.Sample();
        anim.Stop();
    }

}
