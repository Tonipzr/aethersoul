using Unity.Burst;
using Unity.Entities;
using UnityEngine.SceneManagement;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct GameOverSystem : ISystem
{
    private EntityQuery playerDeathQuery;

    public void OnCreate(ref SystemState state)
    {
        playerDeathQuery = state.GetEntityQuery(
            ComponentType.ReadOnly<PlayerComponent>(),
            ComponentType.ReadOnly<DeathComponent>()
        );
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!playerDeathQuery.IsEmpty)
        {
            UnityEngine.Debug.Log("Game Over");
            SceneToLoadGameObject.FromSceneToScene("MainScene", "MainMenuScene");
            SceneManager.LoadScene("LoadingScene");
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
