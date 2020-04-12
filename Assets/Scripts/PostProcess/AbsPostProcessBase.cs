using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    public abstract class AbsPostProcessBase
    {
        private readonly HashSet<int> _rtHashSet = new HashSet<int>();
        private bool _isDirty = false;

        protected PostProcessCamera _postProcessCamera { get; private set; }
        protected CommandBuffer _commandBuffer { get; private set; }
        protected Material _mat { get; private set; }
        protected MaterialPropertyBlock _materialPropertyBlock { get; private set; }

        protected virtual CameraEvent _cameraEvent { get; set; } = CameraEvent.AfterEverything;

        public string MatPath { get; private set; }
        public bool IsActive { get; private set; }
        public bool Deprecated { get; private set; }

        public void OnPreRender()
        {
            if (IsEnabled())
            {
                OnPreRenderInternal();
                if (_isDirty)
                {
                    BuildCommandBuffer();
                }
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

        /// <summary>
        /// 执行渲染前.可更新CommandBuffer参数;
        /// </summary>
        protected virtual void OnPreRenderInternal() { }
        protected virtual void OnPreCullInternal() { }
        protected virtual void OnRenderObjectInternal() { }
        protected virtual void OnWillRenderObjectInternal() { }
        protected virtual void OnPostRenderInternal() { }
        protected virtual void OnUpdateInternal() { }

        /// <summary>
        /// 构建CommandBuffer;
        /// </summary>
        protected virtual void OnBuildCommandBuffer() { }
        /// <summary>
        /// 释放CommandBuffer;
        /// </summary>
        protected virtual void OnReleaseCommandBuffer() { }
        /// <summary>
        /// 释放该后处理并卸载相关资源;
        /// </summary>
        protected virtual void OnReleasePostProcess() { }


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
            _materialPropertyBlock = new MaterialPropertyBlock();
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

            OnBuildCommandBuffer();//Command be execute every frame.

            var mesh = PostProcessMgr.singleton.FullscreenTriangle;
            //_commandBuffer.DrawMesh(mesh, Matrix4x4.identity, _mat);
            _commandBuffer.DrawMesh(mesh, Matrix4x4.identity, _mat, 0, -1, _materialPropertyBlock);
            _materialPropertyBlock.Clear();
            ReleaseTemporaryRT(ShaderIDs.MainTex);
            IsActive = true;
            _isDirty = false;
        }

        private void ReleaseCommandBuffer()
        {
            if (_commandBuffer != null)
            {
                ReleaseAllTemporaryRT();
                _commandBuffer.Clear();
            }
            OnReleaseCommandBuffer();
        }

        public void ReleasePostProcess()
        {
            Deprecated = true;
            IsActive = false;
            MatPath = null;
            ReleaseCommandBuffer();
            RemoveCommandBuffer();
            OnReleasePostProcess();
            if (_commandBuffer != null)
            {
                _commandBuffer.Release();
            }
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
                if (temp.name == _commandBuffer.name)
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
                if (temp.name == _commandBuffer.name)
                {
                    _postProcessCamera.Camera.RemoveCommandBuffer(_cameraEvent, _commandBuffer);
                    return;
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