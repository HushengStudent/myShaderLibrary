using UnityEditor;
using UnityEngine;

public class MaterialPropertyCleanHelper
{
    [MenuItem("Assets/myShaderLibrary/Helper/Clean MaterialProperty", false, 1)]
    public static void MaterialPropertyClean()
    {
        var materials = UnityEditorHelper.GetSelectObjects("mat");
        if (materials.Length > 0)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                var mat = materials[i] as Material;
                if (!mat)
                {
                    continue;
                }

                SerializedObject serializedObject = new SerializedObject(mat);
                SerializedProperty savedProperties = serializedObject.FindProperty("m_SavedProperties");
                SerializedProperty texEnvs = savedProperties.FindPropertyRelative("m_TexEnvs");
                SerializedProperty floats = savedProperties.FindPropertyRelative("m_Floats");
                SerializedProperty colors = savedProperties.FindPropertyRelative("m_Colors");

                if (CleanSerializedProperty(texEnvs, mat) || CleanSerializedProperty(floats, mat)
                    || CleanSerializedProperty(colors, mat))
                {
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(mat);
                }
            }
        }
    }

    private static bool CleanSerializedProperty(SerializedProperty property, Material mat)
    {
        bool dirty = false;
        for (int i = property.arraySize - 1; i >= 0; i--)
        {
            var targetProperty = property.GetArrayElementAtIndex(i);
            string propertyName = targetProperty.displayName;
            if (!mat.HasProperty(propertyName))
            {
                property.DeleteArrayElementAtIndex(i);
                dirty = true;
            }
        }
        return dirty;
    }
}
