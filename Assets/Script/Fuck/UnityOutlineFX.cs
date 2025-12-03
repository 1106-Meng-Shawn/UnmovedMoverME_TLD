// UnityOutlineFX.cs
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class UnityOutlineFX : MonoBehaviour
{
    [Header("Outline Settings")]
    public Color OutlineColor = new Color(1, 0, 0, 1);
    public CameraEvent BufferDrawEvent = CameraEvent.AfterEverything;

    [Header("Blur Settings")]
    [Range(0, 1)]
    public int Downsample = 1;
    [Range(0.0f, 3.0f)]
    public float BlurSize = 1.0f;

    private CommandBuffer _commandBuffer;
    private int _outlineRTID, _blurredRTID, _temporaryRTID, _depthRTID, _idRTID;
    private List<List<Renderer>> _objectRenderers;
    private Material _outlineMaterial;
    private Camera _camera;
    private int _RTWidth, _RTHeight;

    public void AddRenderers(List<Renderer> renderers)
    {
        if (renderers == null || renderers.Count == 0)
        {
            Debug.LogWarning("No renderers to add");
            return;
        }
        _objectRenderers.Add(renderers);
        Debug.Log("Added " + renderers.Count + " renderers, total groups: " + _objectRenderers.Count);
        RecreateCommandBuffer();
    }

    public void ClearOutlineData()
    {
        _objectRenderers.Clear();
        RecreateCommandBuffer();
    }

    private void Awake()
    {
        _objectRenderers = new List<List<Renderer>>();

        _commandBuffer = new CommandBuffer { name = "UnityOutlineFX Command Buffer" };
        _depthRTID = Shader.PropertyToID("_DepthRT");
        _outlineRTID = Shader.PropertyToID("_OutlineRT");
        _blurredRTID = Shader.PropertyToID("_BlurredRT");
        _temporaryRTID = Shader.PropertyToID("_TemporaryRT");
        _idRTID = Shader.PropertyToID("_idRT");

        _RTWidth = Screen.width;
        _RTHeight = Screen.height;

        _outlineMaterial = new Material(Shader.Find("Hidden/UnityOutline"));
        if (_outlineMaterial == null)
            Debug.LogError("Shader Hidden/UnityOutline not found");

        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;
        _camera.AddCommandBuffer(BufferDrawEvent, _commandBuffer);
    }

    private void RecreateCommandBuffer()
    {
        _commandBuffer.Clear();
        if (_objectRenderers.Count == 0)
        {
            Debug.Log("No renderers to outline, skipping command buffer.");
            return;
        }

        _commandBuffer.GetTemporaryRT(_depthRTID, _RTWidth, _RTHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
        _commandBuffer.SetRenderTarget(_depthRTID, BuiltinRenderTextureType.CurrentActive);
        _commandBuffer.ClearRenderTarget(false, true, Color.clear);

        float id = 0f;
        foreach (var group in _objectRenderers)
        {
            id += 0.25f;
            _commandBuffer.SetGlobalFloat("_ObjectId", id);

            foreach (var rend in group)
            {
                if (rend != null)
                {
                    Debug.Log("DrawRenderer called on " + rend.name);
                    _commandBuffer.DrawRenderer(rend, _outlineMaterial, 0, 1);
                    _commandBuffer.DrawRenderer(rend, _outlineMaterial, 0, 0);
                }
            }
        }

        _commandBuffer.GetTemporaryRT(_idRTID, _RTWidth, _RTHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
        _commandBuffer.Blit(_depthRTID, _idRTID, _outlineMaterial, 3);

        int rtW = _RTWidth >> Downsample;
        int rtH = _RTHeight >> Downsample;

        _commandBuffer.GetTemporaryRT(_temporaryRTID, rtW, rtH, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
        _commandBuffer.GetTemporaryRT(_blurredRTID, rtW, rtH, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

        _commandBuffer.Blit(_idRTID, _blurredRTID);
        _commandBuffer.SetGlobalVector("_BlurDirection", new Vector2(BlurSize, 0));
        _commandBuffer.Blit(_blurredRTID, _temporaryRTID, _outlineMaterial, 2);
        _commandBuffer.SetGlobalVector("_BlurDirection", new Vector2(0, BlurSize));
        _commandBuffer.Blit(_temporaryRTID, _blurredRTID, _outlineMaterial, 2);

        _commandBuffer.SetGlobalColor("_OutlineColor", OutlineColor);
        _commandBuffer.Blit(_blurredRTID, BuiltinRenderTextureType.CameraTarget, _outlineMaterial, 4);

        _commandBuffer.ReleaseTemporaryRT(_blurredRTID);
        _commandBuffer.ReleaseTemporaryRT(_outlineRTID);
        _commandBuffer.ReleaseTemporaryRT(_temporaryRTID);
        _commandBuffer.ReleaseTemporaryRT(_depthRTID);
    }
}
