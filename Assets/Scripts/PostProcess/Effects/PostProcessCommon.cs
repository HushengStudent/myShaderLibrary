namespace Framework
{
    public class PostProcessCommon : AbsPostProcessBase
    {
        protected override PostProcessType _postProcessType => PostProcessType.Common;

        public PostProcessCommon(string matPath) : base(matPath) { }

    }
}