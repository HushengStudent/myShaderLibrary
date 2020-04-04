namespace Framework
{
    public class PostProcessCommon : AbsPostProcessBase
    {
        protected override PostProcessType _postProcessType => PostProcessType.Common;

        public override void OnPreRender(float interval)
        {
        }
    }
}