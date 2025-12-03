using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Sprite2DOutlinable : MonoBehaviour
{
    private static List<Sprite2DOutlinable> allOutlinables = new List<Sprite2DOutlinable>();

    [Header("????")]
    public bool enableOutline = true;

    void OnEnable()
    {
        if (!allOutlinables.Contains(this))
        {
            allOutlinables.Add(this);
            Debug.Log($"[Sprite2DOutlinable] ?? Sprite: {gameObject.name}");
        }
    }

    void OnDisable()
    {
        allOutlinables.Remove(this);
    }

    void OnDestroy()
    {
        allOutlinables.Remove(this);
    }

    public static List<Sprite2DOutlinable> GetAllOutlinables()
    {
        allOutlinables.RemoveAll(x => x == null);
        return allOutlinables;
    }

    public void ToggleOutline()
    {
        enableOutline = !enableOutline;
        enabled = enableOutline;
    }
}
