using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    public abstract class AbsPostProcess
    {
        protected Camera _camera;
        protected CommandBuffer _commandBuffer;
        protected Material _mat;

        protected virtual CameraEvent CameraEvent { get; set; } = CameraEvent.AfterEverything;
        protected abstract PostProcessType PostProcessType { get; }

        public string MatPath { get; private set; }
        public bool MatLoaded { get; private set; }

        public void Render(float interval)
        {
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
            MatPath = matPath;
            MatLoaded = false;
            var request = Resources.LoadAsync<Material>(matPath);
            request.completed += (async) =>
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
            _commandBuffer = new CommandBuffer { name = MatPath };
            _camera.AddCommandBuffer(CameraEvent, _commandBuffer);


            //var renderTargetIdentifier = new RenderTargetIdentifier(tex);
            //_commandBuffer.SetGlobalTexture(ShaderIDs.MainTex, renderTargetIdentifier);

            _commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, ShaderIDs.MainTex, _mat);
            MatLoaded = true;
        }

        public void Release()
        {
            _camera.RemoveCommandBuffer(CameraEvent, _commandBuffer);
            _commandBuffer.Release();
            _camera = null;
            MatPath = null;
            MatLoaded = false;
            Resources.UnloadAsset(_mat);
            _mat = null;
        }
    }
}