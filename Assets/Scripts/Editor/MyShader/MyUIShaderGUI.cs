using System;
using UnityEditor;
using UnityEngine;

public class MyUIShaderGUI : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);

        Material targetMat = materialEditor.target as Material;

        bool MyTestBool = Array.IndexOf(targetMat.shaderKeywords, "MyTestBool") != -1;

        EditorGUI.BeginChangeCheck();
        MyTestBool = EditorGUILayout.Toggle("MyTestBool", MyTestBool);

        if (EditorGUI.EndChangeCheck())
        {
            if (MyTestBool)
            {
                targetMat.EnableKeyword("MyTestBool");
            }
            else
            {
                targetMat.DisableKeyword("MyTestBool");
            }
        }
    }
}
