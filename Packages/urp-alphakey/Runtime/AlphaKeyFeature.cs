using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AlphaKeyFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader shader;
    [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    [SerializeField] private RenderTexture renderTexture;

    private AlphaKeyPass pass;

    private AlphaKeyPass CreatePass()
    {
        return new()
        {
            renderTexture = renderTexture,
            renderPassEvent = renderPassEvent,
            material = shader ? new(shader) : null
        };
    }

    public override void Create()
    {
        pass = CreatePass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass ?? CreatePass());
    }
}

public partial class AlphaKeyPass
{
    private const string ProfilerTag = nameof(AlphaKeyPass);
}
