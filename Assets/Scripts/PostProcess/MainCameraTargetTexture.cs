using UnityEngine;
using UnityEngine.UI;

public class MainCameraTargetTexture : MonoBehaviour
{
    private Camera _camera;

    public RenderTexture RenderTexture { get; private set; }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera)
        {
            int w = _camera.pixelWidth;
            int h = _camera.pixelHeight;
            RenderTexture = RenderTexture.GetTemporary(w, h, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 4);
        }
    }

    private void OnPreRender()
    {
        if (_camera)
        {
            _camera.targetTexture = RenderTexture;
        }
    }
    private void OnPostRender()
    {
        if (_camera)
        {
            _camera.targetTexture = null;
        }
    }

    private void OnDisable()
    {
        if (_camera)
        {
            _camera.targetTexture = null;
        }
    }

    private void OnDestroy()
    {
        if (_camera)
        {
            _camera.targetTexture = null;
        }
        if (RenderTexture)
        {
            RenderTexture.ReleaseTemporary(RenderTexture);
        }
    }

    public void SetRawImage(RawImage rawImage)
    {
        if (rawImage)
        {
            rawImage.texture = RenderTexture;
        }
    }
}
