using UnityEngine;

namespace Framework
{
    static class ShaderIDs
    {
        internal static readonly int MainTex = Shader.PropertyToID("_MainTex");
        internal static readonly int MeltStrength = Shader.PropertyToID("_MeltStrength");
        internal static readonly int SourceTex = Shader.PropertyToID("_SourceTex");
        internal static readonly int BlomTex = Shader.PropertyToID("_BlomTex");
    }
}
