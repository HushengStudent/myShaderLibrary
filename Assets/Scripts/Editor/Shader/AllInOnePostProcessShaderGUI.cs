public class AllInOnePostProcessShaderGUI : AbsShaderGUI
{
    private readonly string[] _grayScaleKeys = new string[] { "_GrayScaleLuminosity" };

    private readonly string[] _blurKeys = new string[] { "_BlurStrength" };

    private readonly string[] _glitchKeys = new string[] { "_ScanLineJitter",
        "_VerticalJumpRange", "_VerticalJumpSpeed", "_HorizontalShake", "_ColorDrift" };

    private readonly string[] _glowKeys = new string[] { "_GlowColor", "_GlowIntensity" };

    private readonly string[] _meltKeys = new string[] { "_MeltNoiseTex", "_MeltAddColor",
        "_MeltAddColorStrength", "_MeltAddColorLength", "_MeltAdditionalTex" };

    private readonly string[] _meltTexKeys = new string[] { "MELT_TEX_ON" };

    private readonly string[] _pixelateKeys = new string[] { "_PixelateIntensity" };

    private readonly string[] _aberrationKeys = new string[] { "_AberrationColor",
        "_AberrationAmount", "_AberrationAlpha" };

    protected override void OnGUIEx()
    {
        //ShaderProperty("_MainTex");
        _matEditor.RenderQueueField();
        _matEditor.DoubleSidedGIField();

        ShaderFeature("GREYSCALE_ON", "GREYSCALE_ON(置灰)", "置灰", _grayScaleKeys);

        ShaderFeature("BLUR_ON", "BLUR_ON(模糊)", "模糊", _blurKeys);

        ShaderFeature("GLITCH_ON", "GLITCH_ON(故障)", "故障", _glitchKeys);

        ShaderFeature("GLOW_ON", "GLOW_ON(发光)", "发光", _glowKeys);

        ShaderFeature("MELT_ON", "MELT_ON(消融)", "消融", _meltKeys, _meltTexKeys);

        ShaderFeature("NEGATIVE_ON", "NEGATIVE_ON(负片)", "负片", null);

        ShaderFeature("PIXELATE_ON", "PIXELATE_ON(像素)", "像素", _pixelateKeys);

        ShaderFeature("ABERRATION_ON", "ABERRATION_ON(色差)", "色差", _aberrationKeys);
    }
}
