using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class PostProcessResource : ScriptableObject
    {
        [SerializeField]
        public List<Material> MaterialsList = new List<Material>();

        public Material GetMaterial(string matName)
        {
            foreach (var mat in MaterialsList)
            {
                if (mat && mat.name == matName)
                {
                    return mat;
                }
            }
            return null;
        }
    }
}