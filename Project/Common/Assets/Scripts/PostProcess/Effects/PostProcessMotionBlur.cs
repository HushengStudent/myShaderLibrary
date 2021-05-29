using UnityEngine;

namespace Framework
{
    public class PostProcessMotionBlur : AbsPostProcessBase
    {
        public PostProcessMotionBlur(string matPath) : base(matPath) { }

        protected override bool NeedsFinalPass => false;

        private RenderTexture _renderTexture;

        protected override void OnAddCommandBuffer()
        {
            base.OnAddCommandBuffer();
            var camera = _postProcessCamera.Camera;
            if (!camera)
            {
                return;
            }
            var w = camera.pixelWidth;
            var h = camera.pixelHeight;
            _renderTexture = RenderTextureHelper.GetRT(w, h);
        }

        protected override void OnRemoveCommandBuffer()
        {
            base.OnRemoveCommandBuffer();
            RenderTextureHelper.ReleaseRT(_renderTexture);
        }

        protected override void OnBuildCommandBuffer()
        {
            if (!_renderTexture)
            {
                return;
            }
            _renderTexture.MarkRestoreExpected();
            var source = ShaderIDs.MainTex;
            var dest = _renderTexture;
            _commandBuffer.BlitFullscreenTriangle(source, dest, _mat, -1, null);
            _commandBuffer.BlitFullscreenTriangle(dest, CameraTarget, CopyMat, 0, null);
        }
    }
}
