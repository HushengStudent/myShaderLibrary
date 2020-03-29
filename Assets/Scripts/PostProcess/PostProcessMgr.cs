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

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            for (int i = 0; i < _postProcessList.Count; i++)
            {
                var target = _postProcessList[i];
                if (target.MatLoaded)
                {
                    target.Render(interval);
                }
            }
        }

        public void AddPostProcess(string path, PostProcessType type = PostProcessType.Common)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            for (int i = 0; i < _postProcessList.Count; i++)
            {
                if (_postProcessList[i].MatPath == path)
                {
                    return;
                }
            }
            AbsPostProcess post = null;
            switch (type)
            {
                case PostProcessType.Common:
                    post = new PostProcessCommon();
                    post.OnInitialize(_mainCamera, path);
                    _postProcessList.Add(post);
                    break;
                default:
                    break;
            }
        }

        public void ReleasePostProcess(string path)
        {
            for (int i = 0; i < _postProcessList.Count; i++)
            {
                var post = _postProcessList[i];
                if (post.MatPath == path)
                {
                    post.Release();
                    _postProcessList.Remove(post);
                }
            }
        }
    }
}