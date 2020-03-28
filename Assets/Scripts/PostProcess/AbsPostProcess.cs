using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    public abstract class AbsPostProcess : IPool
    {
        private Material _material;
        protected Camera _camera;
        protected MainCameraTargetTexture _cameraTargetTexture;
        protected CommandBuffer _commandBuffer;
        protected bool MatLoaded;

        protected abstract CameraEvent CameraEvent { get; }
        protected abstract string EffectName { get; }
        protected abstract PostProcessType PostProcessType { get; }

        public abstract void OnRendering();

        public void OnInitialize(Camera camera, string matPath)
        {
            _camera = camera;
            _cameraTargetTexture = camera.GetComponent<MainCameraTargetTexture>();
            var req = Resources.LoadAsync<Material>(matPath);
            req.completed += (operation) => {
                _material = req.asset as Material;
                if (_material)
                {
                    MatLoadFinish();
                }
            };
        }

        private void MatLoadFinish()
        {
            _commandBuffer = new CommandBuffer { name = EffectName };
            _camera.AddCommandBuffer(CameraEvent, _commandBuffer);

            var tex = _cameraTargetTexture.RenderTexture;
            RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(tex);
            _commandBuffer.SetGlobalTexture(ShaderIDs.MainTex, renderTargetIdentifier);
            _commandBuffer.Blit(renderTargetIdentifier, renderTargetIdentifier, _material);
            MatLoaded = true;
        }

        public void OnGet(params object[] args)
        {
            MatLoaded = false;
        }

        public void OnRelease()
        {
            _camera.RemoveCommandBuffer(CameraEvent, _commandBuffer);
            MatLoaded = false;
        }
    }
}