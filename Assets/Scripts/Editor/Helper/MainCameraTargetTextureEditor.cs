using Framework;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MainCameraTargetTexture))]
public class MainCameraTargetTextureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("开启模糊"))
        {
            PostProcessMgr.singleton.AddPostProcess("PostProcess/Blur", PostProcessType.Common);

        }
        if (GUILayout.Button("关闭模糊"))
        {
            PostProcessMgr.singleton.ReleasePostProcess("PostProcess/Blur");

        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("开启置灰"))
        {
            PostProcessMgr.singleton.AddPostProcess("PostProcess/GreyScale", PostProcessType.Common);

        }
        if (GUILayout.Button("关闭置灰"))
        {
            PostProcessMgr.singleton.ReleasePostProcess("PostProcess/GreyScale");

        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}
