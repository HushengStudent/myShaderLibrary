using UnityEditor;
using UnityEngine;

public class MaterialPropertyCleanHelper
{
    [MenuItem("Assets/myShaderLibrary/Helper/Clean MaterialProperty", false, 1)]
    public static void MaterialPropertyClean()
    {
        var matList = UnityEditorHelper.GetSelectObjects<Material>("mat");
        if (matList.Count > 0)
        {
            for (int i = 0; i < matList.Count; i++)
            {
                var mat = matList[i] as Material;
                if (!mat)
                {
                    continue;
                }

                var serializedObject = new SerializedObject(mat);
                var savedProperties = serializedObject.FindProperty("m_SavedProperties");
                var texEnvs = savedProperties.FindPropertyRelative("m_TexEnvs");
                var floats = savedProperties.FindPropertyRelative("m_Floats");
                var colors = savedProperties.FindPropertyRelative("m_Colors");

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
