using Framework;
using UnityEngine;

public class Main : MonoBehaviour
{
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

    private bool _isBlur = false;
    private bool _isGreyScale = false;
    private bool _isGlitch = false;
    private bool _isGlow = false;

    private void OnGUI()
    {
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

        ShowButton("模糊", "Blur", ref _isBlur);
        ShowButton("置灰", "GreyScale", ref _isGreyScale);
        ShowButton("故障", "Glitch", ref _isGlitch);
        ShowButton("发光", "Glow", ref _isGlow);

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void ShowButton(string effectName, string matPath, ref bool state
        , PostProcessType postProcessType = PostProcessType.Common)
    {
        GUIStyle btnStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 30,
        };
        var w = GUILayout.Width(150);
        var h = GUILayout.Height(50);

        if (GUILayout.Button(state ? $"关闭{effectName}" : $"开启{effectName}", btnStyle, w, h))
        {
            var path = $"PostProcess/{matPath}";
            if (state)
            {
                PostProcessMgr.singleton.ReleaseMainCameraPostProcess(path);
            }
            else
            {
                PostProcessMgr.singleton.AddMainCameraPostProcess(path, postProcessType);
            }
            state = !state;
        }
    }
}
