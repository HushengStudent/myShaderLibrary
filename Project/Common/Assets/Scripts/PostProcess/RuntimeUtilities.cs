using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using LoadAction = UnityEngine.Rendering.RenderBufferLoadAction;
using StoreAction = UnityEngine.Rendering.RenderBufferStoreAction;

namespace Framework
{
    public static class RuntimeUtilities
    {
        #region Rendering

        static Mesh s_FullscreenTriangle;
        public static Mesh fullscreenTriangle
        {
            get
            {
                if (s_FullscreenTriangle != null)
                {
                    return s_FullscreenTriangle;
                }

                s_FullscreenTriangle = new Mesh { name = "Fullscreen Triangle" };

                // Because we have to support older platforms (GLES2/3, DX9 etc) we can't do all of
                // this directly in the vertex shader using vertex ids :(
                s_FullscreenTriangle.SetVertices(new List<Vector3>
                {
                    new Vector3(-1f, -1f, 0f),
                    new Vector3(-1f,  3f, 0f),
                    new Vector3( 3f, -1f, 0f)
                });
                s_FullscreenTriangle.SetIndices(new[] { 0, 1, 2 }, MeshTopology.Triangles, 0, false);
                s_FullscreenTriangle.UploadMeshData(false);

                return s_FullscreenTriangle;
            }
        }

        /// <summary>
        /// Sets the current render target using specified <see cref="RenderBufferLoadAction"/>.
        /// </summary>
        /// <param name="cmd">The command buffer to set the render target on</param>
        /// <param name="rt">The render target to set</param>
        /// <param name="loadAction">The load action</param>
        /// <param name="storeAction">The store action</param>
        /// <remarks>
        /// <see cref="RenderBufferLoadAction"/> are only used on Unity 2018.2 or newer.
        /// </remarks>
        private static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd
            , RenderTargetIdentifier rt, LoadAction loadAction, StoreAction storeAction)
        {
#if UNITY_2018_2_OR_NEWER
            cmd.SetRenderTarget(rt, loadAction, storeAction);
#else
            cmd.SetRenderTarget(rt);
#endif
        }

        /// <summary>
        /// Blits a fullscreen triangle using a given material.
        /// </summary>
        /// <param name="cmd">The command buffer to use</param>
        /// <param name="source">The source render target</param>
        /// <param name="destination">The destination render target</param>
        /// <param name="propertySheet">The property sheet to use</param>
        /// <param name="pass">The pass from the material to use</param>
        /// <param name="loadAction">The load action for this blit</param>
        /// <param name="viewport">An optional viewport to consider for the blit</param>
        public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination
            , Material mat, int pass, MaterialPropertyBlock properties, LoadAction loadAction)
        {
            cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
            bool clear = false;
#if UNITY_2018_2_OR_NEWER
            if (loadAction == LoadAction.Clear)
            {
                loadAction = LoadAction.DontCare;
                clear = true;
            }
#endif
            cmd.SetRenderTargetWithLoadStoreAction(destination, loadAction, StoreAction.Store);

            if (clear)
            {
                cmd.ClearRenderTarget(true, true, Color.clear);
            }

            cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, mat, 0, pass, properties);
        }

        /// <summary>
        /// Blits a fullscreen triangle using a given material.
        /// </summary>
        /// <param name="cmd">The command buffer to use</param>
        /// <param name="source">The source render target</param>
        /// <param name="destination">The destination render target</param>
        /// <param name="propertySheet">The property sheet to use</param>
        /// <param name="pass">The pass from the material to use</param>
        /// <param name="clear">Should the destination target be cleared?</param>
        /// <param name="viewport">An optional viewport to consider for the blit</param>
        public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination
            , Material mat, int pass, MaterialPropertyBlock properties, bool clear = false)
        {
            cmd.BlitFullscreenTriangle(source, destination, mat, pass, properties, clear ? LoadAction.Clear : LoadAction.DontCare);
        }

        /// <summary>
        /// Does a copy of source to destination using the builtin blit command.
        /// </summary>
        /// <param name="cmd">The command buffer to use</param>
        /// <param name="source">The source render target</param>
        /// <param name="destination">The destination render target</param>
        public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
#if UNITY_2018_2_OR_NEWER
            cmd.SetRenderTarget(destination, LoadAction.DontCare, StoreAction.Store);
            destination = BuiltinRenderTextureType.CurrentActive;
#endif
            cmd.Blit(source, destination);
        }

        /// <summary>
        /// Blits a fullscreen quad using the builtin blit command and a given material.
        /// </summary>
        /// <param name="cmd">The command buffer to use</param>
        /// <param name="source">The source render target</param>
        /// <param name="destination">The destination render target</param>
        /// <param name="mat">The material to use for the blit</param>
        /// <param name="pass">The pass from the material to use</param>
        public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material mat, int pass = 0)
        {
#if UNITY_2018_2_OR_NEWER
            cmd.SetRenderTarget(destination, LoadAction.DontCare, StoreAction.Store);
            destination = BuiltinRenderTextureType.CurrentActive;
#endif
            cmd.Blit(source, destination, mat, pass);
        }

        #endregion

        #region Unity specifics & misc methods

        /// <summary>
        /// Returns <c>true</c> if a scriptable render pipeline is currently in use, <c>false</c>
        /// otherwise.
        /// </summary>
        public static bool scriptableRenderPipelineActive
        {
            get { return GraphicsSettings.renderPipelineAsset != null; } // 5.6+ only
        }

        #endregion
    }
}
