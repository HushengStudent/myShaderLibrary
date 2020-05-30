using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ShaderVariantCollection;

public class CreateShaderVariantsHelper
{
    [MenuItem("Tools/Create ShaderVariants _F2")]
    public static void CreateShaderVariants()
    {
        var shaderVariantPath = "Assets/Shaders/ShaderVariants/";
        var shaderPath = "Assets/Shaders/";
        if (Directory.Exists(shaderVariantPath))
        {
            Directory.Delete(shaderVariantPath, true);
        }
        var shaderList = UnityEditorHelper.GetAllObjects<Shader>("shader", shaderPath);
        var matList = UnityEditorHelper.GetAllObjects<Material>("mat");
        var shaderVariantDict = new Dictionary<string, HashSet<ShaderVariant>>();
        foreach (var mat in matList)
        {
            if (!mat)
            {
                continue;
            }
            var shader = mat.shader;
            if (shaderList.Contains(shader))
            {
                var shaderVariantCollectionName = $"{Path.GetFileNameWithoutExtension(shader.name)}_ShaderVariants";
                var shaderVariantCollectionPath = $"{shaderVariantPath}{shaderVariantCollectionName}.shadervariants";
                if (!shaderVariantDict.TryGetValue(shaderVariantCollectionPath, out var hashSet))
                {
                    shaderVariantDict[shaderVariantCollectionPath] = new HashSet<ShaderVariant>();
                }
                var shaderVariant = new ShaderVariant
                {
                    shader = shader,
                    keywords = mat.shaderKeywords
                };
                shaderVariantDict[shaderVariantCollectionPath].Add(shaderVariant);
            }
        }
        foreach (var shaderVariantCollection in shaderVariantDict)
        {
            var path = shaderVariantCollection.Key;
            if (File.Exists(path))
            {
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.Refresh();
            }
            var dir = path.Replace(Path.GetFileName(path), "");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            AssetDatabase.CreateAsset(new ShaderVariantCollection(), path);
            var shaderVariantCollectionAsset = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(path);
            foreach (var shaderVariant in shaderVariantCollection.Value)
            {
                shaderVariantCollectionAsset.Add(shaderVariant);
            }
            EditorUtility.SetDirty(shaderVariantCollectionAsset);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}
