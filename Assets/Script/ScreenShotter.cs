using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotter : MonoBehaviour
{
    public static ScreenShotter Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    public Texture2D CaptureScreenshot()
    {
        int width = Screen.width;
        int height = Screen.height;

        // Create a new RenderTexture to hold the camera's render output
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("Main Camera not found");
            return null;
        }

        // Set the camera's target texture to the render texture
        mainCamera.targetTexture = rt;

        // Render the camera view to the RenderTexture
        mainCamera.Render();

        // Set the active RenderTexture to the created render texture
        RenderTexture.active = rt;

        // Create a Texture2D to capture the content of the RenderTexture
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        // Reset the camera's target texture and the active RenderTexture
        mainCamera.targetTexture = null;
        RenderTexture.active = null;

        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(rt);

        // Resize the screenshot if necessary (optional)
        Texture2D resizedScreenshot = ResizeTexture(screenshot, width / 6, height / 6);

        // Clean up the original screenshot texture
        Destroy(screenshot);

        // Return the resized screenshot
        return resizedScreenshot;
    }

    private Texture2D ResizeTexture(Texture2D original, int newWidth, int newHeight)
    {
        // Create a temporary RenderTexture to resize the screenshot
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight, 24);
        RenderTexture.active = rt;

        // Blit the original texture to the temporary RenderTexture
        Graphics.Blit(original, rt);

        // Create a new Texture2D to hold the resized content
        Texture2D resized = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
        resized.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resized.Apply();

        // Reset the active RenderTexture and release the temporary one
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        // Return the resized texture
        return resized;
    }
}

