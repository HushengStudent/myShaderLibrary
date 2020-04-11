public class UIDefaultExtensionShaderGUI : AbsShaderGUI
{
    protected override void OnGUIEx()
    {
        ShaderProperty("_Color");
        ShaderProperty("_StencilComp");
        ShaderProperty("_Stencil");
        ShaderProperty("_StencilOp");
        ShaderProperty("_StencilWriteMask");
        ShaderProperty("_StencilReadMask");
        ShaderProperty("_ColorMask");
        //ShaderProperty("_UseUIAlphaClip");
        ShaderFeature("UI_CLIP_RECT_ON", "UI_CLIP_RECT_ON", "支持RectMask2D");
        _matEditor.RenderQueueField();
        _matEditor.DoubleSidedGIField();

        ShaderFeature("GREYSCALE_ON", "GREYSCALE_ON(置灰)", "置灰",
            new string[]
            {
                "_GrayScaleLuminosity"
            });

        ShaderFeature("BLUR_ON", "BLUR_ON(模糊)", "模糊",
            new string[]
            {
                "_BlurStrength"
            });

        ShaderFeature("Glitch_ON", "Glitch_ON(故障)", "故障",
            new string[]
            {
                "_ScanLineJitter",
                "_VerticalJumpRange",
                "_VerticalJumpSpeed",
                "_HorizontalShake",
                "_ColorDrift"
            });

        ShaderFeature("GLOW_ON", "GLOW_ON(发光)", "发光",
            new string[]
            {
                "_GlowColor",
                "_GlowIntensity"
            });

        ShaderFeature("MELT_ON", "MELT_ON(消融)", "消融",
            new string[]
            {
                "_MeltNoiseTex",
                "_MeltStrength",
                "_MeltAddColor",
                "_MeltAddColorStrength",
                "_MeltAddColorLength",
                "_MeltAdditionalTex"
            },
            new string[]
            {
                "MELT_TEX_ON",
            });
    }
}