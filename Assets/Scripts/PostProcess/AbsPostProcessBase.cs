using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    public abstract class AbsPostProcessBase
    {
        private readonly HashSet<int> _rtHashSet = new HashSet<int>();
        private bool _isDirty = false;

        protected PostProcessCamera _postProcessCamera;
        protected CommandBuffer _commandBuffer;
        protected Material _mat;

        protected virtual CameraEvent _cameraEvent { get; set; } = CameraEvent.AfterEverything;
        protected abstract PostProcessType _postProcessType { get; }

        public string MatPath { get; private set; }
        public bool IsActive { get; private set; }
        public bool Deprecated { get; private set; }

        public void OnPreRender()
        {
            if (IsEnabled())
            {
                if (_isDirty)
                {
                    BuildCommandBuffer();
                }
                OnPreRenderInternal();
            }
        }
        public void OnPreCull()
        {
            if (IsEnabled())
            {
                OnPreCullInternal();
            }
        }
        public void OnRenderObject()
        {
            if (IsEnabled())
            {
                OnRenderObjectInternal();
            }
        }
        public void OnWillRenderObject()
        {
            if (IsEnabled())
            {
                OnWillRenderObjectInternal();
            }
        }
        public void OnPostRender()
        {
            if (IsEnabled())
            {
                OnPostRenderInternal();
            }
        }
        public void OnUpdate()
        {
            if (IsEnabled())
            {
                OnUpdateInternal();
            }
        }

        protected virtual void OnPreRenderInternal() { }
        protected virtual void OnPreCullInternal() { }
        protected virtual void OnRenderObjectInternal() { }
        protected virtual void OnWillRenderObjectInternal() { }
        protected virtual void OnPostRenderInternal() { }
        protected virtual void OnUpdateInternal() { }
        protected virtual void BuildCommandBufferInternal() { }
        protected virtual void ReleaseCommandBufferInternal() { }
        protected virtual void ReleasePostProcessInternal() { }


        public bool IsEnabled()
        {
            return !Deprecated && IsActive;
        }

        protected void ReBuildCommandBuffer()
        {
            _isDirty = true;
        }

        public AbsPostProcessBase(string matPath)
        {
            MatPath = matPath;
        }

        public void InitializePostProcess(PostProcessCamera postProcessCamera)
        {
            Deprecated = false;
            IsActive = false;
            _postProcessCamera = postProcessCamera;
            if (!postProcessCamera.Camera || string.IsNullOrWhiteSpace(MatPath))
            {
                Deprecated = true;
                return;
            }

            _commandBuffer = new CommandBuffer { name = MatPath };
            AddCommandBuffer();

            var request = Resources.LoadAsync<Material>(MatPath);
            request.completed += (async) =>
            {
                _mat = request.asset as Material;
                BuildCommandBuffer();
            };
        }

        private void BuildCommandBuffer()
        {
            ReleaseCommandBuffer();
            if (!_mat || Deprecated)
            {
                ReleasePostProcess();
                return;
            }

            _commandBuffer.Clear(); //Clear all commands in the buffer.

            GetTemporaryRT(ShaderIDs.MainTex);
            _commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, ShaderIDs.MainTex);

            BuildCommandBufferInternal();

            var mesh = PostProcessMgr.singleton.FullscreenTriangle;
            _commandBuffer.DrawMesh(mesh, Matrix4x4.identity, _mat);
            ReleaseTemporaryRT(ShaderIDs.MainTex);
            IsActive = true;
            _isDirty = true;
        }

        private void ReleaseCommandBuffer()
        {
            if (_commandBuffer != null)
            {
                ReleaseAllTemporaryRT();
                _commandBuffer.Clear();
            }
            ReleaseCommandBufferInternal();
        }

        public void ReleasePostProcess()
        {
            Deprecated = true;
            IsActive = false;
            MatPath = null;
            ReleaseCommandBuffer();
            RemoveCommandBuffer();
            ReleasePostProcessInternal();
            _commandBuffer.Release();
            _commandBuffer = null;
            _postProcessCamera = null;
            if (_mat)
            {
                Resources.UnloadAsset(_mat);
            }
            _mat = null;
        }

        private void AddCommandBuffer()
        {
            if (_commandBuffer == null || !_postProcessCamera || !_postProcessCamera.Camera)
            {
                return;
            }
            var allCommandBuffer = _postProcessCamera.Camera.GetCommandBuffers(_cameraEvent);
            foreach (var temp in allCommandBuffer)
            {
                if (temp == _commandBuffer)
                {
                    return;
                }
            }
            _postProcessCamera.Camera.AddCommandBuffer(_cameraEvent, _commandBuffer);
        }

        private void RemoveCommandBuffer()
        {
            if (_commandBuffer == null || !_postProcessCamera || !_postProcessCamera.Camera)
            {
                return;
            }
            var allCommandBuffer = _postProcessCamera.Camera.GetCommandBuffers(_cameraEvent);
            foreach (var temp in allCommandBuffer)
            {
                if (temp == _commandBuffer)
                {
                    _postProcessCamera.Camera.RemoveCommandBuffer(_cameraEvent, _commandBuffer);
                }
            }
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

        protected void ReleaseAllTemporaryRT()
        {
            foreach (var nameID in _rtHashSet)
            {
                ReleaseTemporaryRT(nameID);
            }
            _rtHashSet.Clear();
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