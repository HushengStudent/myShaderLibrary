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

    private readonly float _fpsUpdateInterval = 0.5f;
    private float _fps = 0;
    private int _frames = 0;
    private float _accumulator;
    private float _timeLeft;

    private void Awake()
    {
        PostProcessMgr.singleton.Launch();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.update += () =>
        {
            if (UnityEditor.EditorApplication.isPlaying && UnityEditor.EditorApplication.isCompiling)
            {
                LogHelper.PrintError("script update.");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        };
#endif
    }

    private void Update()
    {
        _frames++;
        _accumulator += Time.unscaledDeltaTime;
        _timeLeft -= Time.unscaledDeltaTime;

        if (_timeLeft <= 0f)
        {
            _fps = _accumulator > 0f ? _frames / _accumulator : 0f;
            _frames = 0;
            _accumulator = 0f;
            _timeLeft += _fpsUpdateInterval;
        }
    }

    private void OnGUI()
    {
        var w = GUILayout.Width(250);
        var h = GUILayout.Height(75);
        GUIStyle btnStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 30,
        };
        GUIStyle redTextStyle = new GUIStyle()
        {
            fontSize = 30,
        };
        redTextStyle.normal.textColor = Color.red;

        var backgroundColor = GUI.backgroundColor;
        var color = GUI.color;
        var contentColor = GUI.contentColor;

        GUILayout.BeginVertical();

        GUILayout.Label($" FPS：{(int)_fps}", redTextStyle);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(_isBlur ? "关闭模糊" : "开启模糊", btnStyle, w, h))
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

        if (GUILayout.Button(_isGreyScale ? "关闭置灰" : "开启置灰", btnStyle, w, h))
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

        if (GUILayout.Button(_isGlitch ? "关闭故障" : "开启故障", btnStyle, w, h))
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
        GUILayout.EndVertical();
    }
}
