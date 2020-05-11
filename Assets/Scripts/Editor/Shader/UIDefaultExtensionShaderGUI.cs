﻿public class UIDefaultExtensionShaderGUI : AbsShaderGUI
{
    private readonly string[] _grayScaleKeys = new string[] { "_GrayScaleLuminosity" };

    private readonly string[] _blurKeys = new string[] { "_BlurStrength" };

    private readonly string[] _glitchKeys = new string[] { "_ScanLineJitter",
        "_VerticalJumpRange", "_VerticalJumpSpeed", "_HorizontalShake", "_ColorDrift" };

    private readonly string[] _glowKeys = new string[] { "_GlowColor", "_GlowIntensity" };

    private readonly string[] _meltKeys = new string[] { "_MeltNoiseTex", "_MeltStrength",
        "_MeltAddColor", "_MeltAddColorStrength", "_MeltAddColorLength", "_MeltAdditionalTex" };

    private readonly string[] _meltTexKeys = new string[] { "MELT_TEX_ON" };

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

        ShaderFeature("GREYSCALE_ON", "GREYSCALE_ON(置灰)", "置灰", _grayScaleKeys);

        ShaderFeature("BLUR_ON", "BLUR_ON(模糊)", "模糊", _blurKeys);

        ShaderFeature("Glitch_ON", "Glitch_ON(故障)", "故障", _glitchKeys);

        ShaderFeature("GLOW_ON", "GLOW_ON(发光)", "发光", _glowKeys);

        ShaderFeature("MELT_ON", "MELT_ON(消融)", "消融", _meltKeys, _meltTexKeys);
    }
}