using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HintControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Image HintImage;
    public TextMeshProUGUI HintText;
    public Button checkButton;

    private float timer = 0f;
    private float autoDestroyTime = 5f;
    private bool hasCheckButton = false;




    public SpriteRenderer spriteRenderer;
    public SpriteRenderer Subscript;

    public List<Sprite> ReduceHintSprites;
    // food = 0, science = 1, politics = 2, gold = 3, faith = 4
    // population = 5 , support rate = 6



    public Animation animationComponent;

    public Sprite exclamationMark;
    public Sprite reduceSprite;


    void Start()
    {
        if (checkButton != null) { 
            checkButton.onClick.AddListener(OnCheckButtonClick);
             hasCheckButton = true;
        }
    }

    void OnCheckButtonClick()
    {
        if (animationComponent.IsPlaying("TipShow")) animationComponent.Stop();
        DestroyHint();
    }

    public void ShowFavorabilityAdd(Character character,int changesValue)
    {
        HintImage.sprite = character.icon;
        //character.AddFavorabilityValue(changesValue); // maybe need chaneg

        string sign = changesValue >= 0 ? "+" : "-";
        int absValue = Mathf.Abs(changesValue);

        string spriteTag = character.FavorabilityLevel switch
        {
            FavorabilityLevel.Self => "<sprite=\"Self\" index=0>",
            FavorabilityLevel.Normal => "<sprite=\"NormalFavorability\" index=0>",
            FavorabilityLevel.Romance => "<sprite=\"RomanceFavorability\" index=0>",
            _ => "BUG"
        };

        HintText.text = $"  {sign}{absValue} {spriteTag}";

        animationComponent.Play("TipShow");
    }

    public void ShowItemAdd(ItemBase itemBase, int changesValue)
    {
        HintImage.sprite = itemBase.icon;
        string sign = changesValue >= 0 ? "+" : "-";
        int absValue = Mathf.Abs(changesValue);

        HintText.text = $"  {sign}{absValue}";
        animationComponent.Play("TipShow");
    }




    /*  Sprite GetSprite(string name, List<Sprite> ReduceHintSprites){
          switch (name)
          {
              case "food" : return ReduceHintSprites[0];
              case "science" : return ReduceHintSprites[1]; 
              case "politics" : return ReduceHintSprites[2];
              case "gold" : return ReduceHintSprites[3];
              case "faith" : return ReduceHintSprites[4]; 
              case "population" : return ReduceHintSprites[5];
              case "support rate" : return ReduceHintSprites[6];

              default: return null;
          }

      }*/


    Sprite GetSprite(int type){
        switch (type)
        {
            case 1 : return exclamationMark;

            default: return null;
        }

    }

    void Update()
    {
        if (!animationComponent.IsPlaying("HintJump")){
            if (animationComponent != null && animationComponent["HintJump"] != null)
            {
                if (!animationComponent.IsPlaying("HintJump") && Random.Range(0f, 1f) <= 0.005f)
                {
                    animationComponent.Play("HintJump");
                }
            }
        }

        if (hasCheckButton)
        {
            timer += Time.deltaTime;
            if (timer >= autoDestroyTime)
            {
                DestroyHint();
            }
        }

    }

    void DestroyHint()
    {
        TipPanelManager.Instance.RemoveHint(this.gameObject);
        Destroy(gameObject);

    }

    /*  public void GenerateReductionTip(string name){
          spriteRenderer.sprite = GetSprite(name,ReduceHintSprites);
      }*/

    public void GenerateReductionTip(int num){
        spriteRenderer.sprite = GetSprite(num);
        Subscript.sprite = reduceSprite;
    }

    

}
