using System;
using UnityEditor;
using UnityEngine;

public abstract class AbsShaderGUI : ShaderGUI
{
    protected MaterialEditor _matEditor;
    protected MaterialProperty[] _properties;
    protected Material _targetMat;
    protected Shader _targetShader;
    protected Color _backgroundColor;
    protected Color _contentColor;
    protected Color _color;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        _matEditor = materialEditor;
        _properties = properties;
        _targetMat = materialEditor.target as Material;
        _targetShader = _targetMat.shader;
        _backgroundColor = GUI.backgroundColor;
        _contentColor = GUI.contentColor;
        _color = GUI.color;

        OnGUIEx();
    }

    protected abstract void OnGUIEx();

    protected void ShaderFeature(string shaderKeywords, string title, string annotation
        , string[] propertiesName = null, string[] internalShaderKeywords = null)
    {
        var enable = Array.IndexOf(_targetMat.shaderKeywords, shaderKeywords) != -1;
        enable = EditorGUILayout.BeginToggleGroup(title, enable);
        if (enable)
        {
            _targetMat.EnableKeyword(shaderKeywords);
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUI.backgroundColor = Color.yellow;
            EditorGUILayout.HelpBox(annotation, MessageType.Info);
            GUI.backgroundColor = _backgroundColor;

            if (internalShaderKeywords != null && internalShaderKeywords.Length > 0)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                for (int i = 0; i < internalShaderKeywords.Length; i++)
                {
                    var keyword = internalShaderKeywords[i];
                    var state = Array.IndexOf(_targetMat.shaderKeywords, keyword) != -1;
                    state = EditorGUILayout.BeginToggleGroup(keyword, state);
                    if (state)
                    {
                        _targetMat.EnableKeyword(keyword);
                    }
                    else
                    {
                        _targetMat.DisableKeyword(keyword);
                    }
                    EditorGUILayout.EndToggleGroup();
                }
                EditorGUILayout.EndVertical();
            }

            if (propertiesName != null && propertiesName.Length > 0)
            {
                GUI.backgroundColor = Color.green;
                EditorGUILayout.BeginVertical(GUI.skin.box);
                for (int i = 0; i < propertiesName.Length; i++)
                {
                    var propertyName = propertiesName[i];
                    ShaderProperty(propertyName);
                }
                EditorGUILayout.EndVertical();
                GUI.backgroundColor = _backgroundColor;
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            _targetMat.DisableKeyword(shaderKeywords);
            if (propertiesName != null && propertiesName.Length > 0)
            {
                for (int i = 0; i < propertiesName.Length; i++)
                {
                    var propertyName = propertiesName[i];
                    var propertyCount = ShaderUtil.GetPropertyCount(_targetShader);
                    var propertyIdx = -1;
                    for (int j = 0; j < propertyCount; j++)
                    {
                        if (ShaderUtil.GetPropertyName(_targetShader, j) == propertyName)
                        {
                            propertyIdx = j;
                            break;
                        }
                    }
                    if (propertyIdx < 0)
                    {
                        continue;
                    }
                    var propertyType = ShaderUtil.GetPropertyType(_targetShader, propertyIdx);
                    if (propertyType == ShaderUtil.ShaderPropertyType.TexEnv)
                    {
                        _targetMat.SetTexture(propertyName, null);
                    }
                }
            }
            if (internalShaderKeywords != null && internalShaderKeywords.Length > 0)
            {
                for (int i = 0; i < internalShaderKeywords.Length; i++)
                {
                    var keyword = internalShaderKeywords[i];
                    _targetMat.DisableKeyword(keyword);
                }
            }
        }
        EditorGUILayout.EndToggleGroup();
    }

    protected void ShaderProperty(string propertyName)
    {
        var property = FindProperty(propertyName, _properties);
        _matEditor.ShaderProperty(property, property.displayName);
    }
}
