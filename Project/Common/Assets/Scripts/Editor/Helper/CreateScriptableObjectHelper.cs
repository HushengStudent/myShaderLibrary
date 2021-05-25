using Framework;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateScriptableObjectHelper
{
    private static void CreateScriptableObject<T>() where T : ScriptableObject
    {
        var objects = Selection.objects;
        if (objects.Length < 1)
        {
            return;
        }
        var target = objects[0];
        if (!(target as DefaultAsset))
        {
            return;
        }
        var path = AssetDatabase.GetAssetPath(target);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var assetPath = $"{path}/new{typeof(T).ToString()}";
        for (var i = 1; ; i++)
        {
            var tempPath = $"{assetPath}{i.ToString()}.asset";
            if (!File.Exists(tempPath))
            {
                assetPath = tempPath;
                break;
            }
        }
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), assetPath);
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/myShaderLibrary/Helper/CreateScriptableObject/PostProcessResource", false, 1)]
    public static void Create()
    {
        CreateScriptableObject<PostProcessResource>();
    }
}