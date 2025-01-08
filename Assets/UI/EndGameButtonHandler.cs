
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject buttonExplanationPanel;

    private bool textShown = false;

    // Start is called before the first frame update
    void Start()
    {
        button.gameObject.SetActive(false);
        buttonExplanationPanel.gameObject.SetActive(false);
        textShown = false;
    }

    // Update is called once per frame
    void Update()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity mapEntity = entityManager.CreateEntityQuery(typeof(MapEntityComponent)).GetSingletonEntity();

        if (entityManager.HasComponent<MapEntityGameStateComponent>(mapEntity))
        {
            MapEntityGameStateComponent gameState = entityManager.GetComponentData<MapEntityGameStateComponent>(mapEntity);

            if (gameState.GamePhase == GamePhase.AfterBoss)
            {
                button.gameObject.SetActive(true);

                if (!textShown) StartCoroutine(ActivateButtonExplanation());
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator ActivateButtonExplanation()
    {
        buttonExplanationPanel.SetActive(true);
        textShown = true;

        yield return new WaitForSeconds(5);

        buttonExplanationPanel.SetActive(false);
    }

    public void HandleEndGameButton()
    {
        if (World.DefaultGameObjectInjectionWorld != null)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var mapEntity = entityManager.CreateEntityQuery(typeof(MapEntityComponent)).GetSingletonEntity();
            if (mapEntity != Entity.Null)
            {
                var mapEntityComponent = entityManager.GetComponentData<MapEntityComponent>(mapEntity);

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
            }
        }

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
