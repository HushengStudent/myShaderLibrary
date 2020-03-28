using UnityEngine.Rendering;

namespace Framework
{
    public class PostProcessBlur : AbsPostProcess
    {
        protected override CameraEvent CameraEvent => CameraEvent.AfterImageEffects;

        protected override string EffectName => "PostProcessBlur";

        protected override PostProcessType PostProcessType => PostProcessType.Blur;

        public override void OnRendering()
        {

        }
    }
}