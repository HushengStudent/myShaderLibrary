using Framework;
using UnityEngine;

public class Main : MonoBehaviour
{
    private readonly float _fpsUpdateInterval = 0.5f;
    private float _fps = 0;
    private int _frames = 0;
    private float _accumulator;
    private float _timeLeft;

    [SerializeField]
    public GameObject Capsule;
    private bool _direction = false;

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

        Tween();
    }

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

    private void OnGUI()
    {
        var redTextStyle = new GUIStyle()
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

        ShowButton<PostProcessCommon>("全屏模糊", "Blur", ref _isBlur);
        ShowButton<PostProcessCommon>("全屏置灰", "GreyScale", ref _isGreyScale);
        ShowButton<PostProcessCommon>("全屏故障", "Glitch", ref _isGlitch);
        ShowButton<PostProcessCommon>("全屏发光", "Glow", ref _isGlow);
        ShowButton<PostProcessMelt>("消融", "Melt", ref _isMelt);

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        ShowButton<PostProcessCommon>("全屏负片", "Negative", ref _isNegative);
        ShowButton<PostProcessCommon>("像素化", "Pixelate", ref _isPixelate);
        ShowButton<PostProcessCommon>("色差", "Aberration", ref _isAberration);
        ShowButton<PostProcessCommon>("变形", "Distort", ref _isDistort);

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        ShowButton<PostProcessBloom>("Bloom", "Bloom", ref _isBloom);

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void ShowButton<T>(string effectName, string matPath, ref bool state) where T : AbsPostProcessBase
    {
        var btnStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 30,
        };
        var w = GUILayout.Width(300);
        var h = GUILayout.Height(50);

        if (GUILayout.Button(state ? $"关闭{effectName}" : $"开启{effectName}", btnStyle, w, h))
        {
            var path = $"{matPath}";
            if (state)
            {
                PostProcessMgr.singleton.ReleaseMainCameraPostProcess(path);
            }
            else
            {
                PostProcessMgr.singleton.AddMainCameraPostProcess<T>(path);
            }
            state = !state;
        }
    }

    private void Tween()
    {
        if (!Capsule)
        {
            return;
        }
        var trans = Capsule.transform;
        var pos = trans.position;

        if ((_direction && pos.z > 58f) || (!_direction && pos.z < 48f))
        {
            _direction = !_direction;
            return;
        }
        var dis = 0.05f;
        trans.position = new Vector3(pos.x, pos.y, pos.z + (_direction ? dis : -dis));
    }
}
