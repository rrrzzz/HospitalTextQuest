using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(WaveRenderer), PostProcessEvent.AfterStack, "Custom/Wave")]
public sealed class Wave : PostProcessEffectSettings
{
    [Range(0, 500), Tooltip("Wave sinus intensity.")]
    public FloatParameter divisor = new FloatParameter { value = 125 };
    [Range(0, 500), Tooltip("Wave speed.")]
    public FloatParameter speed = new FloatParameter { value = 2 };
}
 
public sealed class WaveRenderer : PostProcessEffectRenderer<Wave>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Wave"));
        sheet.properties.SetFloat("_Divisor", settings.divisor);
        sheet.properties.SetFloat("_Speed", settings.speed);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}