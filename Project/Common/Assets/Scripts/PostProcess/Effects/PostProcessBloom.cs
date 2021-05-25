using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class PostProcessBloom : AbsPostProcessBase
    {
        public PostProcessBloom(string matPath) : base(matPath) { }

        private readonly int _iteration = 4;
        private readonly string _tempRTName = "_Bloom_TempRT";

        private List<int> _rtList = new List<int>();

        protected override void OnAddCommandBuffer()
        {
            base.OnAddCommandBuffer();
            for (var i = 0; i < _iteration; i++)
            {
                _rtList.Add(Shader.PropertyToID($"{_tempRTName}{i}"));
            }
        }

        protected override void OnBuildCommandBuffer()
        {
            GetTemporaryRT(ShaderIDs.BlomTex);

            //亮度提取;
            _commandBuffer.BlitFullscreenTriangle(ShaderIDs.MainTex, ShaderIDs.BlomTex, _mat, 1, null);

            for (var i = 0; i < _iteration; i++)
            {
                var source = i == 0 ? ShaderIDs.BlomTex : _rtList[i - 1];
                var dest = _rtList[i];
                GetTemporaryRT(dest, (float)(1f / Math.Pow(2, i)));
                _commandBuffer.BlitFullscreenTriangle(source, dest, _mat, 2, null);
            }
            for (var i = _iteration - 1; i >= 0; i--)
            {
                var source = _rtList[i];
                var dest = i == 0 ? ShaderIDs.BlomTex : _rtList[i - 1];
                _commandBuffer.BlitFullscreenTriangle(source, dest, _mat, 2, null, true);
            }

            _commandBuffer.BlitFullscreenTriangle(CameraTarget, ShaderIDs.MainTex, _mat, 3, null, true);
        }
    }
}
