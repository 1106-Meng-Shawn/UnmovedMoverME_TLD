using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SpriteButton : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Action OnClick;
    private void Awake()
    {
        if (!TryGetComponent<BoxCollider2D>(out var collider))
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
    }


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void OnMouseDown()
    {
        OnClick?.Invoke(); // ? ??????
    }

    void OnMouseEnter()
    {
        Darken();
    }

    void OnMouseExit()
    {
        RestoreColor();
    }

    public void SetAction(Action action)
    {
        OnClick = action;
    }


    private void Darken()
    {
        if (spriteRenderer != null)
        {
            Color c = originalColor; // ?????????
            float darkenFactor = 0.7f;
            spriteRenderer.color = new Color(c.r * darkenFactor, c.g * darkenFactor, c.b * darkenFactor, c.a);
        }
    }

    private void RestoreColor()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}
