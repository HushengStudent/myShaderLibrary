using Framework;
using UnityEngine;

public class Main : MonoBehaviour
{
    private readonly string _blur = "PostProcess/Blur";
    private readonly string _greyScale = "PostProcess/GreyScale";
    private readonly string _glitch = "PostProcess/Glitch";

    private bool _isBlur = false;
    private bool _isGreyScale = false;
    private bool _isGlitch = false;

    private void Awake()
    {
        PostProcessMgr.singleton.Launch();
    }

    private void OnGUI()
    {
        var w = GUILayout.Width(300);
        var h = GUILayout.Height(100);
        GUIStyle style = new GUIStyle(GUI.skin.button)
        {
            fontSize = 30
        };
        var color = GUI.backgroundColor;
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(_isBlur ? "关闭模糊" : "开启模糊", style, w, h))
        {
            if (_isBlur)
            {
                PostProcessMgr.singleton.ReleaseMainCameraPostProcess(_blur);
            }
            else
            {
                PostProcessMgr.singleton.AddMainCameraPostProcess(_blur, PostProcessType.Common);
            }
            _isBlur = !_isBlur;
        }

        if (GUILayout.Button(_isGreyScale ? "关闭置灰" : "开启置灰", style, w, h))
        {
            if (_isGreyScale)
            {
                PostProcessMgr.singleton.ReleaseMainCameraPostProcess(_greyScale);
            }
            else
            {
                PostProcessMgr.singleton.AddMainCameraPostProcess(_greyScale, PostProcessType.Common);
            }
            _isGreyScale = !_isGreyScale;
        }

        if (GUILayout.Button(_isGlitch ? "关闭故障" : "开启故障", style, w, h))
        {
            if (_isGlitch)
            {
                PostProcessMgr.singleton.ReleaseMainCameraPostProcess(_glitch);
            }
            else
            {
                PostProcessMgr.singleton.AddMainCameraPostProcess(_glitch, PostProcessType.Common);
            }
            _isGlitch = !_isGlitch;
        }

        GUILayout.EndHorizontal();
    }
}
