using System.Collections.Generic;

public class AllInOnePostProcessShaderGUI : AbsShaderGUI
{
    private readonly Dictionary<string, string[]> _keysDict = new Dictionary<string, string[]>()
    {
        ["GREYSCALE_ON"] = new string[] {
            "_GrayScaleLuminosity"
        },

        ["BLUR_ON"] = new string[] {
            "_BlurStrength"
        },

        ["GLITCH_ON"] = new string[] {
            "_ScanLineJitter",
            "_VerticalJumpRange",
            "_VerticalJumpSpeed",
            "_HorizontalShake",
            "_ColorDrift"
        },

        ["GLOW_ON"] = new string[] {
            "_GlowColor",
            "_GlowIntensity"
        },

        ["MELT_ON"] = new string[] {
            "_MeltNoiseTex",
            "_MeltAddColor",
            "_MeltAddColorStrength",
            "_MeltAddColorLength",
            "_MeltAdditionalTex"
        },
        ["MELT_TEX_ON"] = new string[] {
            "MELT_TEX_ON"
        },

        ["PIXELATE_ON"] = new string[] {
            "_PixelateIntensity"
        },

        ["ABERRATION_ON"] = new string[] {
            "_AberrationColor",
            "_AberrationAmount",
            "_AberrationAlpha"
        },

        ["DISTORT_ON"] = new string[] {
            "_DistortNoiseTex",
            "_DistortSpeedX",
            "_DistortSpeedY",
            "_DistortStrength"
        },
    };

    protected override void OnGUIEx()
    {
        //ShaderProperty("_MainTex");
        _matEditor.RenderQueueField();
        _matEditor.DoubleSidedGIField();

        ShaderFeature("GREYSCALE_ON", "GREYSCALE_ON(置灰)", "置灰", _keysDict["GREYSCALE_ON"]);

        ShaderFeature("BLUR_ON", "BLUR_ON(模糊)", "模糊", _keysDict["BLUR_ON"]);

        ShaderFeature("GLITCH_ON", "GLITCH_ON(故障)", "故障", _keysDict["GLITCH_ON"]);

        ShaderFeature("GLOW_ON", "GLOW_ON(发光)", "发光", _keysDict["GLOW_ON"]);

        ShaderFeature("MELT_ON", "MELT_ON(消融)", "消融", _keysDict["MELT_ON"], _keysDict["MELT_TEX_ON"]);

        ShaderFeature("NEGATIVE_ON", "NEGATIVE_ON(负片)", "负片", null);

        ShaderFeature("PIXELATE_ON", "PIXELATE_ON(像素)", "像素", _keysDict["PIXELATE_ON"]);

        ShaderFeature("ABERRATION_ON", "ABERRATION_ON(色差)", "色差", _keysDict["ABERRATION_ON"]);

        ShaderFeature("DISTORT_ON", "DISTORT_ON(变形)", "变形", _keysDict["DISTORT_ON"]);
    }
}
