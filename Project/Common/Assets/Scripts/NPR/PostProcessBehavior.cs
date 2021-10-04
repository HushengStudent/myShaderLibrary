using Framework;
using UnityEngine;
using UnityEngine.UI;

public class PostProcessBehavior : MonoBehaviour
{
    private bool _isBlur = false;
    private bool _isGreyScale = false;
    private bool _isGlitch = false;
    private bool _isGlow = false;
    private bool _isMelt = false;
    private bool _isNegative = false;
    private bool _isPixelate = false;
    private bool _isAberration = false;
    private bool _isDistort = false;
    private bool _isBloom = false;
    private bool _isMotionBlur = false;
    private bool _isEdgeDetection = false;
    private bool _isRadialBlur = false;
    private bool _isEdgeDetection2 = false;

    [SerializeField]
    public bool IsTargetTexture;
    [SerializeField]
    public RawImage RawImage;

    [SerializeField]
    public bool IsBlur = false;
    [SerializeField]
    private bool IsGreyScale = false;
    [SerializeField]
    private bool IsGlitch = false;
    [SerializeField]
    private bool IsGlow = false;
    [SerializeField]
    private bool IsMelt = false;
    [SerializeField]
    private bool IsNegative = false;
    [SerializeField]
    private bool IsPixelate = false;
    [SerializeField]
    private bool IsAberration = false;
    [SerializeField]
    private bool IsDistort = false;
    [SerializeField]
    private bool IsBloom = false;
    [SerializeField]
    private bool IsMotionBlur = false;
    [SerializeField]
    private bool IsEdgeDetection = false;
    [SerializeField]
    private bool IsRadialBlur = false;
    [SerializeField]
    private bool IsEdgeDetection2 = false;

    private Camera Camera
    {
        get
        {
            return gameObject.GetComponent<Camera>();
        }
    }

    private void Awake()
    {
        //TODO;
        PostProcessMgr.singleton.Launch();

        if (IsTargetTexture)
        {
            var descriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.ARGB32, 24);
            var rt = RenderTexture.GetTemporary(descriptor);
            Camera.targetTexture = rt;
            RawImage.texture = rt;
            RawImage.SetNativeSize();
        }
        else
        {
            Camera.targetTexture = null;
        }
    }

    private void Update()
    {
        Process<PostProcessCommon>(IsBlur, "Blur", ref _isBlur);

        Process<PostProcessCommon>(IsGreyScale, "GreyScale", ref _isGreyScale);

        Process<PostProcessCommon>(IsGlitch, "Glitch", ref _isGlitch);

        Process<PostProcessCommon>(IsGlow, "Glow", ref _isGlow);

        Process<PostProcessMelt>(IsMelt, "Melt", ref _isMelt);

        Process<PostProcessCommon>(IsNegative, "Negative", ref _isNegative);

        Process<PostProcessCommon>(IsPixelate, "Pixelate", ref _isPixelate);

        Process<PostProcessCommon>(IsAberration, "Aberration", ref _isAberration);

        Process<PostProcessCommon>(IsDistort, "Distort", ref _isDistort);

        Process<PostProcessBloom>(IsBloom, "Bloom", ref _isBloom);

        Process<PostProcessMotionBlur>(IsMotionBlur, "MotionBlur", ref _isMotionBlur);

        Process<PostProcessCommon>(IsEdgeDetection, "EdgeDetection", ref _isEdgeDetection);

        Process<PostProcessRadialBlur>(IsRadialBlur, "RadialBlur", ref _isRadialBlur);

        Process<PostProcessCommon>(IsEdgeDetection2, "EdgeDetection2", ref _isEdgeDetection2);
    }

    private void Process<T>(bool state, string path, ref bool value) where T : AbsPostProcessBase
    {
        if (state)
        {
            if (!value)
            {
                PostProcessMgr.singleton.AddPostProcess<T>(Camera, path);
                value = true;
            }
        }
        else
        {
            if (value)
            {
                PostProcessMgr.singleton.ReleasePostProcess(Camera, path);
                value = false;
            }
        }
    }
}
