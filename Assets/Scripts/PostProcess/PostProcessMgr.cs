using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum PostProcessType : int
    {
        Blur = 1,
    }

    public class PostProcessMgr : MonoSingleton<PostProcessMgr>
    {
        private readonly string _mainCameraTag = "MainCamera";
        private Camera _mainCamera;
        private MainCameraTargetTexture _mainCameraTargetTexture;
        private List<AbsPostProcess> _postProcessList;

        private Mesh _fullscreenTriangle;

        public Mesh FullscreenTriangle
        {
            get
            {
                if (_fullscreenTriangle != null)
                {
                    return _fullscreenTriangle;
                }
                _fullscreenTriangle = new Mesh { name = "Fullscreen Triangle" };
                _fullscreenTriangle.SetVertices(new List<Vector3>
                {
                    new Vector3(-1f, -1f, 0f),
                    new Vector3(-1f,  3f, 0f),
                    new Vector3( 3f, -1f, 0f)
                });
                _fullscreenTriangle.SetIndices(new[] { 0, 1, 2 }, MeshTopology.Triangles, 0, false);
                _fullscreenTriangle.UploadMeshData(false);
                return _fullscreenTriangle;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var go = GameObject.FindWithTag(_mainCameraTag);
            _mainCamera = go.GetComponent<Camera>();
            _mainCameraTargetTexture = go.GetComponent<MainCameraTargetTexture>();
            _postProcessList = new List<AbsPostProcess>();
        }
    }
}