using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class PostProcessMgr : MonoSingleton<PostProcessMgr>
    {
        private readonly string _mainCameraTag = "MainCamera";
        private List<PostProcessCamera> _postProcessCameraList;

        public Camera MainCamera { get; private set; }

        public PostProcessResource PostProcessResource { get; private set; }

        protected override void AwakeEx()
        {
            base.AwakeEx();
            var request = Resources.LoadAsync<PostProcessResource>("PostProcessResource");
            request.completed += (async) =>
            {
                PostProcessResource = request.asset as PostProcessResource;
            };
        }

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
            for (var i = 0; i < _postProcessCameraList.Count; i++)
            {
                var target = _postProcessCameraList[i];
                if (target)
                {
                    target.OnUpdate(interval);
                }
            }
        }

        public void AddMainCameraPostProcess<T>(string matPath) where T : AbsPostProcessBase
        {
            AddPostProcess<T>(MainCamera, matPath);
        }

        public void AddPostProcess<T>(Camera camera, string matPath) where T : AbsPostProcessBase
        {
            if (string.IsNullOrWhiteSpace(matPath) || !camera)
            {
                return;
            }
            PostProcessCamera postProcessCamera = null;
            for (var i = 0; i < _postProcessCameraList.Count; i++)
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
                var target = Activator.CreateInstance(typeof(T), matPath) as AbsPostProcessBase;
                if (target != null)
                {
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
            for (var i = 0; i < _postProcessCameraList.Count; i++)
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