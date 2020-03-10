using System;
using UnityEditor;
using UnityEngine;

public class UIDefaultExtensionShaderGUI : ShaderGUI
{
    private MaterialEditor _matEditor;
    private MaterialProperty[] _properties;
    private Material _targetMat;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        _matEditor = materialEditor;
        _properties = properties;
        _targetMat = materialEditor.target as Material;

        ShaderProperty("_Color");
        ShaderProperty("_StencilComp");
        ShaderProperty("_Stencil");
        ShaderProperty("_StencilOp");
        ShaderProperty("_StencilWriteMask");
        ShaderProperty("_StencilReadMask");
        ShaderProperty("_ColorMask");
        //ShaderProperty("_UseUIAlphaClip");
        SerializeShaderFeature("UI_CLIP_RECT_ON", "UI_CLIP_RECT_ON", "支持RectMask2D", new string[] { });
        _matEditor.RenderQueueField();
        _matEditor.DoubleSidedGIField();

        SerializeShaderFeature("GREYSCALE_ON", "GREYSCALE_ON(置灰)", "置灰", "_GrayScale");
    }

    private void SerializeShaderFeature(string shaderKeywords, string title, string annotation
        , params string[] propertiesName)
    {
        var targetMat = _matEditor.target as Material;
        bool enable = Array.IndexOf(targetMat.shaderKeywords, shaderKeywords) != -1;
        enable = EditorGUILayout.BeginToggleGroup(title, enable);
        if (!enable)
        {
            targetMat.DisableKeyword(shaderKeywords);
            return;
        }
        targetMat.EnableKeyword(shaderKeywords);
        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);
        EditorGUILayout.HelpBox(annotation, MessageType.Info);
        if (propertiesName != null && propertiesName.Length > 0)
        {
            for (int i = 0; i < propertiesName.Length; i++)
            {
                var propertyName = propertiesName[i];
                var property = FindProperty(propertyName, _properties);
                _matEditor.ShaderProperty(property, property.displayName);
            }
        }
        EditorGUILayout.EndVertical();
    }

    public void ShaderProperty(string propertyName)
    {
        var property = FindProperty(propertyName, _properties);
        _matEditor.ShaderProperty(property, property.displayName);
    }
}
