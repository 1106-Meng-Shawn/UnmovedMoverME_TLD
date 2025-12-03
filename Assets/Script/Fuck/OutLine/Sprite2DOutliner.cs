using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class Sprite2DOutliner : MonoBehaviour
{
    [Header("????")]
    [Tooltip("????")]
    public Color outlineColor = Color.yellow;

    [Tooltip("????????")]
    [Range(1, 20)]
    public float outlineWidth = 5f;

    [Tooltip("?????")]
    [Range(0, 10)]
    public int smoothness = 3;

    private Camera mainCamera;
    private Material outlineMaterial;
    private RenderTexture spriteTexture;
    private RenderTexture outlineTexture;

    void OnEnable()
    {
        mainCamera = GetComponent<Camera>();
        CreateMaterial();

        Debug.Log("[Sprite2DOutliner] ??? - 2D Sprite ????");
    }

    void OnDisable()
    {
        CleanupTextures();
        if (outlineMaterial != null)
        {
            DestroyImmediate(outlineMaterial);
        }
    }

    void CreateMaterial()
    {
        Shader shader = Shader.Find("Hidden/Sprite2DOutline");

        if (shader == null)
        {
            Debug.LogError("[Sprite2DOutliner] ??? 'Hidden/Sprite2DOutline' Shader?");
            Debug.LogError("??? Sprite2DOutline.shader ????");
            return;
        }

        outlineMaterial = new Material(shader);
        outlineMaterial.hideFlags = HideFlags.HideAndDontSave;

        Debug.Log("[Sprite2DOutliner] ??????");
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (outlineMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // ????????? Sprite
        List<Sprite2DOutlinable> outlinables = Sprite2DOutlinable.GetAllOutlinables();

        if (outlinables.Count == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // ??????
        int width = source.width;
        int height = source.height;

        if (spriteTexture == null || spriteTexture.width != width)
        {
            CleanupTextures();
            spriteTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            outlineTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        }

        // === ??1: ?? Sprite ? Alpha ?? ===
        Graphics.SetRenderTarget(spriteTexture);
        GL.Clear(true, true, new Color(0, 0, 0, 0)); // ????

        // ?????? Sprite
        foreach (var outlinable in outlinables)
        {
            if (!outlinable.enabled || !outlinable.gameObject.activeInHierarchy)
                continue;

            SpriteRenderer spriteRenderer = outlinable.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null || !spriteRenderer.enabled || spriteRenderer.sprite == null)
                continue;

            // ?? Sprite??? Alpha ???
            RenderSprite(spriteRenderer);
        }

        Graphics.SetRenderTarget(null);

        // === ??2: ?? + ?? ===
        outlineMaterial.SetFloat("_OutlineWidth", outlineWidth);
        outlineMaterial.SetColor("_OutlineColor", outlineColor);

        RenderTexture temp = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);

        // ????
        for (int i = 0; i < smoothness; i++)
        {
            Graphics.Blit(spriteTexture, temp, outlineMaterial, 0); // Pass 0: ??
            Graphics.Blit(temp, spriteTexture);
        }

        RenderTexture.ReleaseTemporary(temp);

        // === ??3: ????????? ===
        outlineMaterial.SetTexture("_SpriteMask", spriteTexture);
        Graphics.Blit(source, destination, outlineMaterial, 1); // Pass 1: ??

        Debug.Log($"[Sprite2DOutliner] ??? {outlinables.Count} ? Sprite ??");
    }

    void RenderSprite(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer.sprite == null) return;

        // ?? Sprite ???? UV
        Sprite sprite = spriteRenderer.sprite;
        Texture2D texture = sprite.texture;

        // ??????????????
        Vector3 worldPos = spriteRenderer.transform.position;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        // ?? Sprite ???????
        Bounds bounds = spriteRenderer.bounds;
        Vector3 bottomLeft = mainCamera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, worldPos.z));
        Vector3 topRight = mainCamera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, worldPos.z));

        float screenWidth = topRight.x - bottomLeft.x;
        float screenHeight = topRight.y - bottomLeft.y;

        // ????
        Material mat = new Material(Shader.Find("Hidden/SpriteToAlpha"));
        mat.mainTexture = texture;

        // ?? UV
        Rect textureRect = sprite.textureRect;
        Vector2 uvMin = new Vector2(textureRect.x / texture.width, textureRect.y / texture.height);
        Vector2 uvMax = new Vector2((textureRect.x + textureRect.width) / texture.width,
                                     (textureRect.y + textureRect.height) / texture.height);

        // ?????
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();

        GL.Begin(GL.QUADS);
        GL.Color(spriteRenderer.color); // ?? Sprite ???

        // ??
        GL.TexCoord2(uvMin.x, uvMin.y);
        GL.Vertex3(bottomLeft.x / Screen.width, bottomLeft.y / Screen.height, 0);

        // ??
        GL.TexCoord2(uvMax.x, uvMin.y);
        GL.Vertex3(topRight.x / Screen.width, bottomLeft.y / Screen.height, 0);

        // ??
        GL.TexCoord2(uvMax.x, uvMax.y);
        GL.Vertex3(topRight.x / Screen.width, topRight.y / Screen.height, 0);

        // ??
        GL.TexCoord2(uvMin.x, uvMax.y);
        GL.Vertex3(bottomLeft.x / Screen.width, topRight.y / Screen.height, 0);

        GL.End();
        GL.PopMatrix();

        DestroyImmediate(mat);
    }

    void CleanupTextures()
    {
        if (spriteTexture != null)
        {
            RenderTexture.ReleaseTemporary(spriteTexture);
            spriteTexture = null;
        }

        if (outlineTexture != null)
        {
            RenderTexture.ReleaseTemporary(outlineTexture);
            outlineTexture = null;
        }
    }
}

