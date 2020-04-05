using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    public abstract class AbsPostProcessBase
    {
        private readonly HashSet<int> _rtHashSet = new HashSet<int>();

        protected PostProcessCamera _postProcessCamera;
        protected CommandBuffer _commandBuffer;
        protected Material _mat;

        protected virtual CameraEvent _cameraEvent { get; set; } = CameraEvent.AfterEverything;
        protected abstract PostProcessType _postProcessType { get; }

        public string MatPath { get; private set; }
        public bool IsActive { get; private set; }
        public bool Deprecated { get; private set; }

        public virtual void OnPreRender() { }
        public virtual void OnPostRender() { }
        public virtual void OnPreCull() { }
        public virtual void OnRenderObject() { }
        public virtual void OnWillRenderObject() { }
        public virtual void OnUpdate() { }
        public virtual void OnMatLoadFinish() { }

        public bool IsEnabled()
        {
            return !Deprecated && IsActive;
        }

        public void OnInitialize(PostProcessCamera postProcessCamera, string matPath)
        {
            Deprecated = false;
            IsActive = false;
            MatPath = matPath;
            _postProcessCamera = postProcessCamera;
            if (!postProcessCamera.Camera)
            {
                Deprecated = true;
                return;
            }
            var request = Resources.LoadAsync<Material>(matPath);
            request.completed += (async) =>
            {
                _mat = request.asset as Material;
                MatLoadFinish();
            };
        }

        private void MatLoadFinish()
        {
            if (!_mat || Deprecated)
            {
                Release();
                return;
            }
            _commandBuffer = new CommandBuffer { name = MatPath };
            _commandBuffer.Clear();
            _postProcessCamera.Camera.AddCommandBuffer(_cameraEvent, _commandBuffer);
            GetTemporaryRT(ShaderIDs.MainTex);
            _commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, ShaderIDs.MainTex);
            var mesh = PostProcessMgr.singleton.FullscreenTriangle;
            _commandBuffer.DrawMesh(mesh, Matrix4x4.identity, _mat);
            ReleaseTemporaryRT(ShaderIDs.MainTex);
            OnMatLoadFinish();
            IsActive = true;
        }

        public void Release()
        {
            Deprecated = true;
            IsActive = false;
            MatPath = null;
            if (_commandBuffer != null)
            {
                _postProcessCamera.Camera.RemoveCommandBuffer(_cameraEvent, _commandBuffer);
                foreach (var nameID in _rtHashSet)
                {
                    _commandBuffer.ReleaseTemporaryRT(nameID);
                }
                _commandBuffer.Release();
            }
            _postProcessCamera = null;
            if (_mat)
            {
                Resources.UnloadAsset(_mat);
            }
            _mat = null;
        }

        protected void GetTemporaryRT(int nameID, float scale = 1f)
        {
            if (_commandBuffer == null || !_postProcessCamera)
            {
                return;
            }
            var camera = _postProcessCamera.Camera;
            if (!camera)
            {
                return;
            }
            int w = (int)(camera.pixelWidth * scale);
            int h = (int)(camera.pixelHeight * scale);
            _commandBuffer.GetTemporaryRT(nameID, w, h);
            _rtHashSet.Add(nameID);
        }

        protected void ReleaseTemporaryRT(int nameID)
        {
            if (_commandBuffer == null)
            {
                return;
            }
            _commandBuffer.ReleaseTemporaryRT(nameID);
            _rtHashSet.Remove(nameID);
        }
    }
}