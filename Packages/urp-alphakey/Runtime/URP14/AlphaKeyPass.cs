#if URP14

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public partial class AlphaKeyPass : ScriptableRenderPass
{
    public RenderTexture renderTexture;
    public Material material;

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!renderTexture)
            return;

        if (!material)
            return;

        if (renderingData.cameraData is { isSceneViewCamera: true } or { isPreviewCamera: true })
            return;
        
        if (!renderingData.cameraData.camera.CompareTag("MainCamera"))
            return;

        var cmd = CommandBufferPool.Get(ProfilerTag);

        cmd.Blit(renderingData.cameraData.renderer.cameraColorTargetHandle, renderTexture, material);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }
}

#endif
