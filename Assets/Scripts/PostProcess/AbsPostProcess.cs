using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    public abstract class AbsPostProcess
    {
        private Camera _attachedCamera;

        protected abstract CameraEvent CameraEvent { get; }

        public abstract void OnRendering();
    }
}