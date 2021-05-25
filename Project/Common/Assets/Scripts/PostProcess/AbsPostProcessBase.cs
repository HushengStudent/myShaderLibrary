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
        protected MaterialPropertyBlock _properties { get; private set; }

        protected virtual CameraEvent _cameraEvent { get; set; } = CameraEvent.BeforeImageEffects;

        protected RenderTargetIdentifier CameraTarget = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
        protected RenderTargetIdentifier CurrentActive = new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive);

        public string MatPath { get; private set; }
        public bool IsActive { get; private set; }
        public bool Deprecated { get; private set; }

        public bool IsEnabled
        {
            get
            {
                return !Deprecated && IsActive;
            }
        }

        public void OnPreRender()
        {
            OnPreRenderInternal();
            if (_isDirty)
            {
                BuildCommandBuffer();
            }
        }
        public void OnPreCull()
        {
            OnPreCullInternal();
        }
        public void OnRenderObject()
        {
            OnRenderObjectInternal();
        }
        public void OnWillRenderObject()
        {
            OnWillRenderObjectInternal();
        }
        public void OnPostRender()
        {
            OnPostRenderInternal();
        }
        public void OnUpdate()
        {
            OnUpdateInternal();
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

        protected virtual void OnAddCommandBuffer() { }
        protected virtual void OnRemoveCommandBuffer() { }
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
            if (!postProcessCamera.Camera || !PostProcessMgr.singleton.PostProcessResource
                || string.IsNullOrWhiteSpace(MatPath))
            {
                Deprecated = true;
                return;
            }
            _properties = new MaterialPropertyBlock();
            _commandBuffer = new CommandBuffer { name = MatPath };

            _mat = PostProcessMgr.singleton.PostProcessResource.GetMaterial(MatPath);

            AddCommandBuffer();
            BuildCommandBuffer();
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

            //RequiresInitialBlit
            GetTemporaryRT(ShaderIDs.MainTex);
            _commandBuffer.BuiltinBlit(CameraTarget, ShaderIDs.MainTex);

            OnBuildCommandBuffer();//Command be execute every frame.

            //BlitFullscreenTriangle
            _commandBuffer.BlitFullscreenTriangle(ShaderIDs.MainTex, CameraTarget, _mat, 0, _properties);

            _properties.Clear();
            ReleaseAllTemporaryRT();

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
            if (_commandBuffer == null || !_postProcessCamera)
            {
                return;
            }
            var camera = _postProcessCamera.Camera;
            if (!camera)
            {
                return;
            }
            foreach (var temp in camera.GetCommandBuffers(_cameraEvent))
            {
                if (temp.name == _commandBuffer.name)
                {
                    return;
                }
            }
            camera.AddCommandBuffer(_cameraEvent, _commandBuffer);
            OnAddCommandBuffer();
        }

        private void RemoveCommandBuffer()
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
            foreach (var temp in camera.GetCommandBuffers(_cameraEvent))
            {
                if (temp.name == _commandBuffer.name)
                {
                    _postProcessCamera.Camera.RemoveCommandBuffer(_cameraEvent, _commandBuffer);
                    RemoveCommandBuffer();
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
            ReleaseTemporaryRT(nameID);
            _commandBuffer.GetTemporaryRT(nameID, w, h);
            _rtHashSet.Add(nameID);
        }

        protected void ReleaseAllTemporaryRT()
        {
            var hashSet = new HashSet<int>(_rtHashSet);
            foreach (var nameID in hashSet)
            {
                ReleaseTemporaryRT(nameID);
            }
            hashSet.Clear();
            _rtHashSet.Clear();
        }

        protected void ReleaseTemporaryRT(int nameID)
        {
            if (_commandBuffer == null)
            {
                return;
            }
            if (!_rtHashSet.Contains(nameID))
            {
                return;
            }
            _commandBuffer.ReleaseTemporaryRT(nameID);
            _rtHashSet.Remove(nameID);
        }
    }
}