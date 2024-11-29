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

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject)
        {
            _virtualCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.Follow = playerObject.GetComponent<Transform>();
        }
    }

    protected override void OnDestroy()
    {

    }
}
