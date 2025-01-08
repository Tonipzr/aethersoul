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

            Entity mapEntity = state.EntityManager.CreateEntityQuery(typeof(MapEntityComponent)).GetSingletonEntity();
            MapEntityComponent mapEntityComponent = state.EntityManager.GetComponentData<MapEntityComponent>(mapEntity);

            GameStatsManager.Instance.SetStat(GameStatType.TotalSpellsUsed, mapEntityComponent.TotalSpellsUsed);
            GameStatsManager.Instance.SetStat(GameStatType.TotalGoldCollected, mapEntityComponent.TotalGoldCollected);
            GameStatsManager.Instance.SetStat(GameStatType.TotalGoldUsed, mapEntityComponent.TotalGoldUsed);
            GameStatsManager.Instance.SetStat(GameStatType.TotalEnemiesKilled, mapEntityComponent.TotalEnemiesKilled);
            GameStatsManager.Instance.SetStat(GameStatType.TotalEnemiesKilledNoDamage, mapEntityComponent.TotalEnemiesKilledNoDamage);
            GameStatsManager.Instance.SetStat(GameStatType.TotalTraveledDistance, mapEntityComponent.TotalTraveledDistance);
            GameStatsManager.Instance.SetStat(GameStatType.TotalPOIsVisited, mapEntityComponent.TotalPOIsVisited);
            GameStatsManager.Instance.SetStat(GameStatType.TotalPOIsCleared, mapEntityComponent.TotalPOIsCleared);
            GameStatsManager.Instance.SetStat(GameStatType.TotalBuffsCollected, mapEntityComponent.TotalBuffsCollected);
            GameStatsManager.Instance.SetStat(GameStatType.TotalCheckpointsReached, mapEntityComponent.TotalCheckpointsReached);
            GameStatsManager.Instance.SetStat(GameStatType.TotalLevelsUp, mapEntityComponent.TotalLevelsUp);
            GameStatsManager.Instance.SetStat(GameStatType.TotalSpellsUnlocked, mapEntityComponent.TotalSpellsUnlocked);

            SaveData gameSave = new SaveData(
            DreamCityStatsGameObject.FireBuff,
            DreamCityStatsGameObject.WaterBuff,
            DreamCityStatsGameObject.EarthBuff,
            DreamCityStatsGameObject.AirBuff,
            DreamCityStatsGameObject.CurrentCoins,
            new SaveSettingsData(
                PlayerPrefsManager.Instance.GetSpellsVolume(),
                PlayerPrefsManager.Instance.GetMusicVolume(),
                PlayerPrefsManager.Instance.GetSFXVolume(),
                PlayerPrefsManager.Instance.GetMonsterSpeed(),
                PlayerPrefsManager.Instance.GetLanguage()
            ),
            new SaveAchievements(
                AchievementsManager.Instance.GetUnlockedAchievements().ToArray()
            ),
            new SaveStats(
                GameStatsManager.Instance.TotalSpellsUsed,
                GameStatsManager.Instance.TotalGoldCollected,
                GameStatsManager.Instance.TotalGoldUsed,
                GameStatsManager.Instance.TotalEnemiesKilled,
                GameStatsManager.Instance.TotalEnemiesKilledNoDamage,
                GameStatsManager.Instance.TotalTraveledDistance,
                GameStatsManager.Instance.TotalPOIsVisited,
                GameStatsManager.Instance.TotalPOIsCleared,
                GameStatsManager.Instance.TotalBuffsCollected,
                GameStatsManager.Instance.TotalCheckpointsReached,
                GameStatsManager.Instance.TotalLevelsUp,
                GameStatsManager.Instance.TotalSpellsUnlocked
            )
        );

            SaveGame.Save(gameSave);

            SceneToLoadGameObject.FromSceneToScene("MainScene", "MainMenuScene");
            SceneManager.LoadScene("LoadingScene");
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
