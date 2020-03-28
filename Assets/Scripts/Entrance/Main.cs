using Framework;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private readonly string _uiRoot = "UIRoot";
    private readonly string _rawImage = "MainUICamera/RawImage";

    private void Awake()
    {
        var camera = GetComponent<Camera>();
        var uiRoot = GameObject.Find(_uiRoot);
        var ui = uiRoot.transform.Find(_rawImage);
        var rawImage = ui.gameObject.GetComponent<RawImage>();

        var mainCameraTargetTexture = gameObject.AddComponent<MainCameraTargetTexture>();
        mainCameraTargetTexture.SetRawImage(rawImage);

        PostProcessMgr.singleton.Launch();
    }
}
