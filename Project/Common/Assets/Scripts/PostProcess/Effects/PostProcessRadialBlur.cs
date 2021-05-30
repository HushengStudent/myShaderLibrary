using UnityEngine;

namespace Framework
{
    public class PostProcessRadialBlur : AbsPostProcessBase
    {
        public PostProcessRadialBlur(string matPath) : base(matPath) { }

        private float _radialRadius = 0.6f;
        private int _radialIteration = 10;
        private float _radialCenterX = 0.6f;
        private float _radialCenterY = 0.6f;

        protected override void OnBuildCommandBuffer()
        {
            var value = new Vector4(_radialRadius * 0.02f, _radialIteration, _radialCenterX, _radialCenterY);
            _properties.SetVector("_Params", value);
        }
    }
}
