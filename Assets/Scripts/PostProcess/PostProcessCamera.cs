using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class PostProcessCamera : MonoBehaviour
    {
        public Camera Camera { get; private set; }
        private List<AbsPostProcessBase> _postProcessList;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            _postProcessList = new List<AbsPostProcessBase>();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _postProcessList.Count; i++)
            {
                var target = _postProcessList[i];
                RemovePostProcess(target);
            }
            _postProcessList.Clear();
        }

        public bool IsContains(string matPath)
        {
            foreach (var post in _postProcessList)
            {
                if (post.MatPath == matPath)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddPostProcess(AbsPostProcessBase absPostProcessBase)
        {
            if (!IsContains(absPostProcessBase.MatPath))
            {
                _postProcessList.Add(absPostProcessBase);
            }
        }

        public void RemovePostProcess(AbsPostProcessBase absPostProcessBase)
        {
            _postProcessList.Remove(absPostProcessBase);
            absPostProcessBase.Release();
        }

        public void RemovePostProcess(string matPath)
        {
            foreach (var post in _postProcessList)
            {
                if (post.MatPath == matPath)
                {
                    RemovePostProcess(post);
                    return;
                }
            }
        }

        //OnPreRender is called before a camera starts rendering the scene.
        private void OnPreRender()
        {
            for (int i = 0; i < _postProcessList.Count; i++)
            {
                var target = _postProcessList[i];
                if (target.IsEnabled())
                {
                    target.OnPreRender();
                }
            }
        }

        //OnPostRender is called after a camera has finished rendering the scene.
        private void OnPostRender()
        {
            for (int i = 0; i < _postProcessList.Count; i++)
            {
                var target = _postProcessList[i];
                if (target.IsEnabled())
                {
                    target.OnPostRender();
                }
            }
        }

        //OnPreCull is called before a camera culls the scene.
        private void OnPreCull()
        {
            for (int i = 0; i < _postProcessList.Count; i++)
            {
                var target = _postProcessList[i];
                if (target.IsEnabled())
                {
                    target.OnPreCull();
                }
            }
        }

        //OnRenderObject is called after camera has rendered the scene.
        private void OnRenderObject()
        {
            for (int i = 0; i < _postProcessList.Count; i++)
            {
                var target = _postProcessList[i];
                if (target.IsEnabled())
                {
                    target.OnRenderObject();
                }
            }
        }

        //OnWillRenderObject is called for each camera if the object is visible.
        private void OnWillRenderObject()
        {
            for (int i = 0; i < _postProcessList.Count; i++)
            {
                var target = _postProcessList[i];
                if (target.IsEnabled())
                {
                    target.OnWillRenderObject();
                }
            }
        }

        public void OnUpdate(float interval)
        {
            for (int i = 0; i < _postProcessList.Count; i++)
            {
                var target = _postProcessList[i];
                if (target.Deprecated)
                {
                    RemovePostProcess(target);
                    continue;
                }
                if (target.IsEnabled())
                {
                    target.OnUpdate();
                }
            }
        }
    }
}