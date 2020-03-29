using UnityEngine;
using UnityEngine.UI;

public class MainCameraTargetTexture : MonoBehaviour
{
    private Camera _camera;
    private int _pixelWidth;
    private int _pixelHeight;
    private RawImage _rawImage;

    public RenderTexture CameraRT { get; private set; }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera)
        {
            _pixelWidth = _camera.pixelWidth;
            _pixelHeight = _camera.pixelHeight;
        }
        GetTemporary();
    }

    private void OnPreRender()
    {
        //GetTemporary();
    }

    private void OnPostRender()
    {
        //ReleaseTemporary();   
    }

    private void OnDisable()
    {
        ReleaseTemporary();
    }

    private void OnDestroy()
    {
        ReleaseTemporary();
    }

    public void SetRawImage(RawImage rawImage)
    {
        if (rawImage)
        {
            _rawImage = rawImage;
            _rawImage.texture = CameraRT;
        }
    }

    public void GetTemporary()
    {
        ReleaseTemporary();

        CameraRT = RenderTexture.GetTemporary(_pixelWidth, _pixelHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 4);
        CameraRT.name = "MainCameraTargetTexture";
        if (_rawImage)
        {
            _rawImage.texture = CameraRT;
        }
        if (_camera)
        {
            _camera.targetTexture = CameraRT;
        }
    }

    private void ReleaseTemporary()
    {
        if (_camera)
        {
            _camera.targetTexture = null;
        }
        if (_rawImage)
        {
            _rawImage.texture = null;
        }
        if (CameraRT)
        {
            CameraRT.DiscardContents();
            RenderTexture.ReleaseTemporary(CameraRT);
            CameraRT = null;
        }
    }
}
