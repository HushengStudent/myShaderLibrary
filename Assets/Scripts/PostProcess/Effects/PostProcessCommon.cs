namespace Framework
{
    public class PostProcessCommon : AbsPostProcess
    {
        protected override PostProcessType PostProcessType => PostProcessType.Common;

        public override void OnRendering(float interval)
        {
        }
    }
}