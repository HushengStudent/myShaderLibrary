﻿using System;
using UnityEditor;
using UnityEngine;

public class MyUIShaderGUI : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material targetMat = materialEditor.target as Material;

        ShaderProperty(materialEditor, properties, "_Color");
        ShaderProperty(materialEditor, properties, "_StencilComp");
        ShaderProperty(materialEditor, properties, "_Stencil");
        ShaderProperty(materialEditor, properties, "_StencilOp");
        ShaderProperty(materialEditor, properties, "_StencilWriteMask");
        ShaderProperty(materialEditor, properties, "_StencilReadMask");
        ShaderProperty(materialEditor, properties, "_ColorMask");
        ShaderProperty(materialEditor, properties, "_UseUIAlphaClip");
        materialEditor.RenderQueueField();
        materialEditor.DoubleSidedGIField();

        {
            bool toggle = Array.IndexOf(targetMat.shaderKeywords, "DOODLE_ON") != -1;
            toggle = EditorGUILayout.BeginToggleGroup("DOODLE_ON", toggle);
            if (toggle)
            {
                targetMat.EnableKeyword("DOODLE_ON");
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Space(10);
                    EditorGUILayout.HelpBox("DOODLE_ON", MessageType.Info);
                    ShaderProperty(materialEditor, properties, "_HandDrawnAmount");
                    ShaderProperty(materialEditor, properties, "_HandDrawnSpeed");
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                targetMat.DisableKeyword("DOODLE_ON");
            }
        }
    }

    public void ShaderProperty(MaterialEditor materialEditor, MaterialProperty[] properties, string propertyName)
    {
        var property = FindProperty(propertyName, properties);
        materialEditor.ShaderProperty(property, property.displayName);
    }
}
