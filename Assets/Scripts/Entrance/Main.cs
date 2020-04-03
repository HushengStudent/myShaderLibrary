using Framework;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        PostProcessMgr.singleton.Launch();
    }
}
