using System;
using UnityEditor;
using UnityEngine;

public abstract class AbsShaderGUI : ShaderGUI
{
    protected MaterialEditor _matEditor;
    protected MaterialProperty[] _properties;
    protected Material _targetMat;
    protected Color _originalColor;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        _matEditor = materialEditor;
        _properties = properties;
        _targetMat = materialEditor.target as Material;
        _originalColor = GUI.backgroundColor;

        OnGUIEx();
    }

    protected abstract void OnGUIEx();

    protected void ShaderFeature(string shaderKeywords, string title, string annotation
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

    protected void ShaderProperty(string propertyName)
    {
        var property = FindProperty(propertyName, _properties);
        _matEditor.ShaderProperty(property, property.displayName);
    }
}
