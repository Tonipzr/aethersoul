using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

partial struct MousePositionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var mousePosition in SystemAPI.Query<RefRW<MousePositionComponent>>())
        {
            Camera mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.ValueRW.Position = new float2(worldPosition.x, worldPosition.y);
        }
    }

    public void OnDestroy(ref SystemState state)
    {

    }
}
