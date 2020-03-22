using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UnityEditorHelper
{
    public static Object[] GetSelectObjects(string extension)
    {
        var objectList = new List<Object>();
        var objects = Selection.objects;
        if (objects.Length > 0)
        {
            var pathList = new List<string>();
            for (int i = 0; i < objects.Length; i++)
            {
                var target = objects[i];
                var path = AssetDatabase.GetAssetPath(target);
                if (target as DefaultAsset)
                {
                    pathList.Add(path);
                }
                else if (Path.GetExtension(path) == $".{extension}")
                {
                    objectList.Add(target);
                }
            }
            foreach (var path in pathList)
            {
                string[] allFile = Directory.GetFiles(path, $"*.{extension}", SearchOption.AllDirectories);
                foreach (var filePath in allFile)
                {
                    var temp = filePath.Replace("\\", "/");
                    objectList.Add(AssetDatabase.LoadAssetAtPath(filePath, typeof(Object)));
                }
            }
        }
        return objectList.ToArray();
    }
}
