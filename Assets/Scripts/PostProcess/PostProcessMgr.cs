using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum PostProcessType : int
    {
        Common = 0,
    }

    public class PostProcessMgr : MonoSingleton<PostProcessMgr>
    {
        private readonly string _mainCameraTag = "MainCamera";
        private List<PostProcessCamera> _postProcessCameraList;

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

        public Camera MainCamera { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var go = GameObject.FindWithTag(_mainCameraTag);
            MainCamera = go.GetComponent<Camera>();
            _postProcessCameraList = new List<PostProcessCamera>();
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            for (int i = 0; i < _postProcessCameraList.Count; i++)
            {
                var target = _postProcessCameraList[i];
                if (target)
                {
                    target.OnUpdate(interval);
                }
            }
        }

        public void AddMainCameraPostProcess(string matPath, PostProcessType type = PostProcessType.Common)
        {
            AddPostProcess(MainCamera, matPath, type);
        }

        public void AddPostProcess(Camera camera, string matPath, PostProcessType type = PostProcessType.Common)
        {
            if (string.IsNullOrWhiteSpace(matPath) || !camera)
            {
                return;
            }
            PostProcessCamera postProcessCamera = null;
            for (int i = 0; i < _postProcessCameraList.Count; i++)
            {
                var target = _postProcessCameraList[i];
                if (target.Camera == camera)
                {
                    postProcessCamera = target;
                    break;
                }
            }
            if (!postProcessCamera)
            {
                postProcessCamera = camera.gameObject.AddComponent<PostProcessCamera>();
                _postProcessCameraList.Add(postProcessCamera);
            }
            if (!postProcessCamera.IsContains(matPath))
            {
                AbsPostProcessBase target = null;
                switch (type)
                {
                    case PostProcessType.Common:
                        target = new PostProcessCommon();
                        break;
                    default:
                        break;
                }
                if (target != null)
                {
                    target.OnInitialize(postProcessCamera, matPath);
                    postProcessCamera.AddPostProcess(target);
                }
            }
        }

        public void ReleaseMainCameraPostProcess(string matPath)
        {
            ReleasePostProcess(MainCamera, matPath);
        }

        public void ReleasePostProcess(Camera camera, string matPath)
        {
            for (int i = 0; i < _postProcessCameraList.Count; i++)
            {
                var target = _postProcessCameraList[i];
                if (target.Camera == camera)
                {
                    target.RemovePostProcess(matPath);
                    return;
                }
            }
        }
    }
}