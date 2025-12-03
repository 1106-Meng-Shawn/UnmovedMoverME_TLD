using UnityEngine;
using static GetColor;

[RequireComponent(typeof(SpriteRenderer))]
public class OutlineSprite : MonoBehaviour
{
    public Material outlineMaterial;       // 使用带描边的Shader的材质
    public SpriteRenderer InitialSprite;   // 拿到原始Sprite贴图来源
    public Region region;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (InitialSprite.sprite == null) Debug.Log($"{InitialSprite.gameObject.name} bug");
        if (spriteRenderer == null || outlineMaterial == null || InitialSprite == null || InitialSprite.sprite == null) return;

        spriteRenderer.sprite = InitialSprite.sprite;
        spriteRenderer.material = outlineMaterial;


    }

    private void Start()
    {
     //   SetOutLine(Color.gray);
    }

    public void SetOutLine(Color32 countryColor)
    {

        gameObject.SetActive(true);
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(block); 

        block.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        block.SetColor("_OutlineColor", countryColor); 
        block.SetFloat("_OutlineSize", 4.0f);
        // block.SetFloat("_AlphaThreshold", 0.1f);     
        spriteRenderer.SetPropertyBlock(block);
    }

    public void CloseOutLine()
    {
        gameObject.SetActive(false);
    }

}


