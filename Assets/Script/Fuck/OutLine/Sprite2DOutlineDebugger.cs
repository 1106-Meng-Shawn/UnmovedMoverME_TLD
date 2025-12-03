using UnityEngine;

public class Sprite2DOutlineDebugger : MonoBehaviour
{
    void Update()
    {
        // Press D to print debug info
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("=== 2D Sprite Outline Debug Info ===");

            // Check Outliner
            var outliner = Camera.main?.GetComponent<Sprite2DOutliner>();
            if (outliner == null)
            {
                Debug.LogError("? No Sprite2DOutliner component found on the main camera");
            }
            else
            {
                Debug.Log("? Sprite2DOutliner found");
                Debug.Log($"  Outline Color: {outliner.outlineColor}");
                Debug.Log($"  Outline Width: {outliner.outlineWidth}");
            }

            // Check Shader
            Shader shader = Shader.Find("Hidden/Sprite2DOutline");
            if (shader == null)
            {
                Debug.LogError("? Shader not found: Hidden/Sprite2DOutline");
            }
            else
            {
                Debug.Log("? Shader found");
            }

            // Check Sprites
            var outlinables = Sprite2DOutlinable.GetAllOutlinables();
            Debug.Log($"? Found {outlinables.Count} sprites:");

            foreach (var obj in outlinables)
            {
                var spriteRenderer = obj.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    Debug.LogWarning($"  ? {obj.gameObject.name} has no SpriteRenderer!");
                }
                else if (spriteRenderer.sprite == null)
                {
                    Debug.LogWarning($"  ? {obj.gameObject.name} has no Sprite assigned!");
                }
                else
                {
                    Debug.Log($"  ? {obj.gameObject.name} - Sprite: {spriteRenderer.sprite.name}");
                }
            }

            Debug.Log("==================================");
        }

        // Press Space to toggle all outlines
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var outlinables = Sprite2DOutlinable.GetAllOutlinables();

            // Iterate over a copy to avoid collection modification issues
            foreach (var obj in new System.Collections.Generic.List<Sprite2DOutlinable>(outlinables))
            {
                obj.ToggleOutline();
            }

            Debug.Log("All outlines toggled");
        }
    }
}
