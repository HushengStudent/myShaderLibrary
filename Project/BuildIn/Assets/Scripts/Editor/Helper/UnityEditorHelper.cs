using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UnityEditorHelper
{
    public static List<T> GetSelectObjects<T>(string extension) where T : Object
    {
        var list = new List<T>();
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
                else if (Path.GetExtension(path) == $".{extension}" && target is T)
                {
                    list.Add(target as T);
                }
            }
            foreach (var path in pathList)
            {
                string[] allFile = Directory.GetFiles(path, $"*.{extension}", SearchOption.AllDirectories);
                foreach (var filePath in allFile)
                {
                    var temp = filePath.Replace("\\", "/");
                    var target = AssetDatabase.LoadAssetAtPath<T>(filePath);
                    if (target)
                    {
                        list.Add(target);
                    }
                }
            }
        }
        return list;
    }

    public static List<Object> GetSelectObjects()
    {
        var list = new List<Object>();
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
                else
                {
                    list.Add(target);
                }
            }
            foreach (var path in pathList)
            {
                string[] allFile = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                foreach (var filePath in allFile)
                {
                    var temp = filePath.Replace("\\", "/");
                    var target = AssetDatabase.LoadAssetAtPath<Object>(filePath);
                    if (target)
                    {
                        list.Add(target);
                    }
                }
            }
        }
        return list;
    }

    public static List<T> GetAllObjects<T>(string extension, string path = "Assets/") where T : Object
    {
        var list = new List<T>();
        string[] allFile = Directory.GetFiles(path, $"*.{extension}", SearchOption.AllDirectories);
        foreach (var filePath in allFile)
        {
            var temp = filePath.Replace("\\", "/");
            var target = AssetDatabase.LoadAssetAtPath<T>(filePath);
            if (target)
            {
                list.Add(target);
            }
        }
        return list;
    }

    public static List<Object> GetAllObjects(string path = "Assets/")
    {
        var list = new List<Object>();
        string[] allFile = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        foreach (var filePath in allFile)
        {
            var temp = filePath.Replace("\\", "/");
            var target = AssetDatabase.LoadAssetAtPath<Object>(filePath);
            if (target)
            {
                list.Add(target);
            }
        }
        return list;
    }
}
