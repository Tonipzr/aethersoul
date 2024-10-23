using Cinemachine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

partial class CameraSystem : SystemBase
{
    private CinemachineVirtualCamera _virtualCamera;

    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        if (_virtualCamera != null) return;

        _virtualCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>();
        _virtualCamera.Follow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    protected override void OnDestroy()
    {

    }
}
