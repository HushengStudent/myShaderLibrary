﻿using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    public abstract class AbsPostProcess
    {
        protected Camera _camera;
        protected MainCameraTargetTexture _cameraTargetTexture;
        protected CommandBuffer _commandBuffer;
        protected Material _mat;

        protected virtual CameraEvent CameraEvent { get; set; } = CameraEvent.AfterEverything;
        protected abstract PostProcessType PostProcessType { get; }

        public string MatPath { get; private set; }
        public bool MatLoaded { get; private set; }

        public void Render(float interval)
        {
            if (_commandBuffer != null && _cameraTargetTexture)
            {
                //var tex = _cameraTargetTexture.CameraRT;
                //_commandBuffer.SetGlobalTexture(ShaderIDs.MainTex, new RenderTargetIdentifier(tex));

                OnRendering(interval);
            }
        }

        public abstract void OnRendering(float interval);

        public void OnInitialize(Camera camera, string matPath)
        {
            _camera = camera;
            _cameraTargetTexture = camera.GetComponent<MainCameraTargetTexture>();
            MatPath = matPath;
            MatLoaded = false;
            var request = Resources.LoadAsync<Material>(matPath);
            request.completed += (operation) =>
            {
                _mat = request.asset as Material;
                if (_mat)
                {
                    MatLoadFinish();
                }
            };
        }

        private void MatLoadFinish()
        {
            if (_cameraTargetTexture)
            {
                _commandBuffer = new CommandBuffer { name = MatPath };
                _commandBuffer.Clear();
                _commandBuffer.ClearRenderTarget(true, true, Color.black);
                _camera.AddCommandBuffer(CameraEvent, _commandBuffer);

                var tex = _cameraTargetTexture.CameraRT;

                //var renderTargetIdentifier = new RenderTargetIdentifier(tex);
                //_commandBuffer.SetGlobalTexture(ShaderIDs.MainTex, renderTargetIdentifier);

                _commandBuffer.Blit(tex, BuiltinRenderTextureType.CameraTarget, _mat);
                MatLoaded = true;
            }
        }

        public void Release()
        {
            _camera.RemoveCommandBuffer(CameraEvent, _commandBuffer);
            _commandBuffer.Release();
            _camera = null;
            _cameraTargetTexture = null;
            MatPath = null;
            MatLoaded = false;
            Resources.UnloadAsset(_mat);
            _mat = null;
        }
    }
}