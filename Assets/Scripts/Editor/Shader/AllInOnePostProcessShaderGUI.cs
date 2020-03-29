public class AllInOnePostProcessShaderGUI : AbsShaderGUI
{
    protected override void OnGUIEx()
    {
        ShaderProperty("_MainTex");
        _matEditor.RenderQueueField();
        _matEditor.DoubleSidedGIField();

        ShaderFeature("GREYSCALE_ON", "GREYSCALE_ON(置灰)", "置灰", new string[] { "_GrayScaleLuminosity" });
        ShaderFeature("BLUR_ON", "BLUR_ON(模糊)", "模糊", new string[] { "_BlurStrength" });
    }
}
