using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineTextureScroller : MonoBehaviour
{
    [Range(0.1f, 2f)]
    public float scrollSpeed = 0.3f;
    [Range(0.1f, 2f)]
    public float stripeWidth = 0.8f;

    private LineRenderer lr;
    private Material mat;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        if (lr != null)
        {
            mat = new Material(Shader.Find("Unlit/Texture"));
            Texture2D stripeTex = Resources.Load<Texture2D>("MyDraw/UI/Other/StripePattern");
            if (stripeTex != null)
            {
                mat.mainTexture = stripeTex;
                mat.mainTextureScale = new Vector2(stripeWidth, 1f);
                mat.mainTextureOffset = Vector2.zero;
                lr.material = mat;
            }
            else
            {
                Debug.LogWarning("Stripe texture not found! Please add to Resources/Textures/Stripe.png");
            }

            lr.textureMode = LineTextureMode.Tile;
        }
    }

    void Update()
    {
        if (mat != null)
        {
            float offset = Time.time * scrollSpeed;
            mat.mainTextureOffset = new Vector2(-offset, 0);
        }
    }
}
