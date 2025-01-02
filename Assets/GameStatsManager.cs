using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get; private set; }

    public int TotalSpellsUsed;
    public int TotalGoldCollected;
    public int TotalGoldUsed;
    public int TotalEnemiesKilled;
    public int TotalEnemiesKilledNoDamage;
    public float TotalTraveledDistance;
    public int TotalPOIsVisited;
    public int TotalPOIsCleared;
    public int TotalBuffsCollected;
    public int TotalCheckpointsReached;
    public int TotalLevelsUp;
    public int TotalSpellsUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SaveData gameSave = SaveGame.Load();

        if (gameSave != null && gameSave.Stats != null)
        {
            SetStat(GameStatType.TotalSpellsUsed, gameSave.Stats.TotalSpellsUsed);
            SetStat(GameStatType.TotalGoldCollected, gameSave.Stats.TotalGoldCollected);
            SetStat(GameStatType.TotalGoldUsed, gameSave.Stats.TotalGoldUsed);
            SetStat(GameStatType.TotalEnemiesKilled, gameSave.Stats.TotalEnemiesKilled);
            SetStat(GameStatType.TotalEnemiesKilledNoDamage, gameSave.Stats.TotalEnemiesKilledNoDamage);
            SetStat(GameStatType.TotalTraveledDistance, gameSave.Stats.TotalTraveledDistance);
            SetStat(GameStatType.TotalPOIsVisited, gameSave.Stats.TotalPOIsVisited);
            SetStat(GameStatType.TotalPOIsCleared, gameSave.Stats.TotalPOIsCleared);
            SetStat(GameStatType.TotalBuffsCollected, gameSave.Stats.TotalBuffsCollected);
            SetStat(GameStatType.TotalCheckpointsReached, gameSave.Stats.TotalCheckpointsReached);
            SetStat(GameStatType.TotalLevelsUp, gameSave.Stats.TotalLevelsUp);
            SetStat(GameStatType.TotalSpellsUnlocked, gameSave.Stats.TotalSpellsUnlocked);
        }
    }

    public void SetStat(GameStatType type, float value)
    {
        switch (type)
        {
            case GameStatType.TotalSpellsUsed:
                TotalSpellsUsed = (int)value;
                break;
            case GameStatType.TotalGoldCollected:
                TotalGoldCollected = (int)value;
                break;
            case GameStatType.TotalGoldUsed:
                TotalGoldUsed = (int)value;
                break;
            case GameStatType.TotalEnemiesKilled:
                TotalEnemiesKilled = (int)value;
                break;
            case GameStatType.TotalEnemiesKilledNoDamage:
                TotalEnemiesKilledNoDamage = (int)value;
                break;
            case GameStatType.TotalTraveledDistance:
                TotalTraveledDistance = value;
                break;
            case GameStatType.TotalPOIsVisited:
                TotalPOIsVisited = (int)value;
                break;
            case GameStatType.TotalPOIsCleared:
                TotalPOIsCleared = (int)value;
                break;
            case GameStatType.TotalBuffsCollected:
                TotalBuffsCollected = (int)value;
                break;
            case GameStatType.TotalCheckpointsReached:
                TotalCheckpointsReached = (int)value;
                break;
            case GameStatType.TotalLevelsUp:
                TotalLevelsUp = (int)value;
                break;
            case GameStatType.TotalSpellsUnlocked:
                TotalSpellsUnlocked = (int)value;
                break;
        }
    }
}

public enum GameStatType
{
    TotalSpellsUsed,
    TotalGoldCollected,
    TotalGoldUsed,
    TotalEnemiesKilled,
    TotalEnemiesKilledNoDamage,
    TotalTraveledDistance,
    TotalPOIsVisited,
    TotalPOIsCleared,
    TotalBuffsCollected,
    TotalCheckpointsReached,
    TotalLevelsUp,
    TotalSpellsUnlocked
}
