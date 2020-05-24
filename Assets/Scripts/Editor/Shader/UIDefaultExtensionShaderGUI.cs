using System.Collections.Generic;

public class UIDefaultExtensionShaderGUI : AbsShaderGUI
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
            "_MeltStrength",
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
            "_AberrationAmount",
            "_AberrationAlpha"
        },

        ["SHADOW_ON"] = new string[] {
            "_ShadowLength",
            "_ShadowStrength"
        },

        ["TORSION_ON"] = new string[] {
            "_TorsionStrength",
            "_TorsionSpeed"
        },

        ["SHAKE_ON"] = new string[] {
            "_ShakeStrength",
            "_ShakeSpeed"
        },
    };

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

        ShaderFeature("GREYSCALE_ON", "GREYSCALE_ON(置灰)", "置灰", _keysDict["GREYSCALE_ON"]);

        ShaderFeature("BLUR_ON", "BLUR_ON(模糊)", "模糊", _keysDict["BLUR_ON"]);

        ShaderFeature("GLITCH_ON", "GLITCH_ON(故障)", "故障", _keysDict["GLITCH_ON"]);

        ShaderFeature("GLOW_ON", "GLOW_ON(发光)", "发光", _keysDict["GLOW_ON"]);

        ShaderFeature("MELT_ON", "MELT_ON(消融)", "消融", _keysDict["MELT_ON"], _keysDict["MELT_TEX_ON"]);

        ShaderFeature("NEGATIVE_ON", "NEGATIVE_ON(负片)", "负片", null);

        ShaderFeature("PIXELATE_ON", "PIXELATE_ON(像素)", "像素", _keysDict["PIXELATE_ON"]);

        ShaderFeature("ABERRATION_ON", "ABERRATION_ON(色差)", "色差", _keysDict["ABERRATION_ON"]);

        ShaderFeature("SHADOW_ON", "SHADOW_ON(阴影)", "阴影", _keysDict["SHADOW_ON"]);

        ShaderFeature("TORSION_ON", "TORSION_ON(扭曲)", "扭曲", _keysDict["TORSION_ON"]);

        ShaderFeature("SHAKE_ON", "SHAKE_ON(摇晃)", "摇晃", _keysDict["SHAKE_ON"]);
    }
}