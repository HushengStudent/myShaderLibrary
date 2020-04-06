public class AllInOnePostProcessShaderGUI : AbsShaderGUI
{
    protected override void OnGUIEx()
    {
        //ShaderProperty("_MainTex");
        _matEditor.RenderQueueField();
        _matEditor.DoubleSidedGIField();

        ShaderFeature("GREYSCALE_ON", "GREYSCALE_ON(置灰)", "置灰", "_GrayScaleLuminosity");
        ShaderFeature("BLUR_ON", "BLUR_ON(模糊)", "模糊", "_BlurStrength");
        ShaderFeature("Glitch_ON", "Glitch_ON(故障)", "故障",
            "_ScanLineJitter",
            "_VerticalJumpRange", "_VerticalJumpSpeed",
            "_HorizontalShake",
            "_ColorDrift");
        ShaderFeature("GLOW_ON", "GLOW_ON(发光)", "发光", "_GlowColor", "_GlowIntensity");
    }
}
