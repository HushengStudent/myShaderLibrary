namespace Framework
{
    public class PostProcessMelt : AbsPostProcessBase
    {
        public PostProcessMelt(string matPath) : base(matPath) { }

        private readonly float _minValue = 0;
        private readonly float _maxValue = 1;
        private float _meltStrength = 0;

        protected override void OnPreRenderInternal()
        {
            base.OnPreRenderInternal();
            if (_meltStrength < _minValue)
            {
                _meltStrength = _minValue;
            }
            if (_meltStrength > _maxValue)
            {
                _meltStrength = _minValue;
            }
            _meltStrength = _meltStrength + 0.01f;
            ReBuildCommandBuffer();
        }

        protected override void OnBuildCommandBuffer()
        {
            base.OnBuildCommandBuffer();
            //_commandBuffer.SetGlobalFloat(ShaderIDs.MeltStrength, _meltStrength);
            _materialPropertyBlock.SetFloat(ShaderIDs.MeltStrength, _meltStrength);
        }
    }
}