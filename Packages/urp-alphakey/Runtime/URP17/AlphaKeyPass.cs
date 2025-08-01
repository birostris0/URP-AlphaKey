#if URP17

using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public partial class AlphaKeyPass : ScriptableRenderPass
{
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    private class PassData
    {
        public TextureHandle source;
        public TextureHandle target;
        public Material material;
    }

    public RenderTexture renderTexture;
    public Material material;

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        var cameraData = frameData.Get<UniversalCameraData>();
        var resourceData = frameData.Get<UniversalResourceData>();
        
        if (!renderTexture || renderTexture is not { depthStencilFormat: GraphicsFormat.None })
            return;
        
        if (!material)
            return;
        
        if (cameraData is { isSceneViewCamera: true } or { isPreviewCamera: true })
            return;
        
        if (!cameraData.camera.CompareTag("MainCamera"))
            return;

        using (var builder = renderGraph.AddUnsafePass<PassData>(ProfilerTag, out var passData))
        {
            var target = renderGraph.ImportTexture(RTHandles.Alloc(renderTexture));

            passData.source = resourceData.activeColorTexture;
            passData.target = target;
            passData.material = material;

            builder.UseTexture(resourceData.activeColorTexture, AccessFlags.Read);
            builder.UseTexture(target, AccessFlags.Write);

            builder.SetRenderFunc(static (PassData passData, UnsafeGraphContext context) =>
            {
                var cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

                var property = new MaterialPropertyBlock();
                property.SetTexture(MainTex, passData.source);

                context.cmd.SetRenderTarget(passData.target);
                cmd.DrawProcedural(Matrix4x4.identity, passData.material, 0, MeshTopology.Triangles, 3, 1, property);
            });
        }
    }
}

#endif
