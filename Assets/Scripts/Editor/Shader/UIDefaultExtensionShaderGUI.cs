using System;
using UnityEditor;
using UnityEngine;

public class UIDefaultExtensionShaderGUI : ShaderGUI
{
    private MaterialEditor _matEditor;
    private MaterialProperty[] _properties;
    private Material _targetMat;
    private Color _originalColor;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        _matEditor = materialEditor;
        _properties = properties;
        _targetMat = materialEditor.target as Material;
        _originalColor = GUI.backgroundColor;

        ShaderProperty("_Color");
        ShaderProperty("_StencilComp");
        ShaderProperty("_Stencil");
        ShaderProperty("_StencilOp");
        ShaderProperty("_StencilWriteMask");
        ShaderProperty("_StencilReadMask");
        ShaderProperty("_ColorMask");
        //ShaderProperty("_UseUIAlphaClip");
        ShaderFeature("UI_CLIP_RECT_ON", "UI_CLIP_RECT_ON", "支持RectMask2D", new string[] { });
        _matEditor.RenderQueueField();
        _matEditor.DoubleSidedGIField();

        ShaderFeature("GREYSCALE_ON", "GREYSCALE_ON(置灰)", "置灰", new string[] { "_GrayScaleLuminosity" });
        ShaderFeature("BLUR_ON", "BLUR_ON(模糊)", "模糊", new string[] { "_BlurStrength" });
    }

    private void ShaderFeature(string shaderKeywords, string title, string annotation
        , params string[] propertiesName)
    {
        bool enable = Array.IndexOf(_targetMat.shaderKeywords, shaderKeywords) != -1;
        enable = EditorGUILayout.BeginToggleGroup(title, enable);
        if (enable)
        {
            _targetMat.EnableKeyword(shaderKeywords);
            GUILayout.Space(10);
            GUI.backgroundColor = Color.yellow;
            EditorGUILayout.HelpBox(annotation, MessageType.Info);
            if (propertiesName != null && propertiesName.Length > 0)
            {
                GUI.backgroundColor = Color.green;
                EditorGUILayout.BeginVertical(GUI.skin.box);
                for (int i = 0; i < propertiesName.Length; i++)
                {
                    var propertyName = propertiesName[i];
                    var property = FindProperty(propertyName, _properties);
                    _matEditor.ShaderProperty(property, property.displayName);
                }
                EditorGUILayout.EndVertical();
                GUI.backgroundColor = _originalColor;
            }
        }
        else
        {
            _targetMat.DisableKeyword(shaderKeywords);
        }
        EditorGUILayout.EndToggleGroup();
    }

    public void ShaderProperty(string propertyName)
    {
        var property = FindProperty(propertyName, _properties);
        _matEditor.ShaderProperty(property, property.displayName);
    }
}
