using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int FireBuff;
    public int WaterBuff;
    public int EarthBuff;
    public int AirBuff;
    public int CurrentCoins;

    public SaveSettingsData Settings;

    public SaveAchievements Achievements;

    public SaveStats Stats;

    public SaveData(
        int fireBuff,
        int waterBuff,
        int earthBuff,
        int airBuff,
        int currentCoins,
        SaveSettingsData settings,
        SaveAchievements achievements,
        SaveStats stats
    )
    {
        FireBuff = fireBuff;
        WaterBuff = waterBuff;
        EarthBuff = earthBuff;
        AirBuff = airBuff;
        CurrentCoins = currentCoins;
        Settings = settings;
        Achievements = achievements;
        Stats = stats;
    }
}

[System.Serializable]
public class SaveSettingsData
{
    public float SpellsVolume;
    public float MusicVolume;
    public float SFXVolume;

    public float MonsterSpeed;

    public string Language;

    public SaveSettingsData(
        float spellsVolume = 1.0f,
        float musicVolume = 1.0f,
        float sfxVolume = 1.0f,
        float monsterSpeed = 1.0f,
        string language = "es"
    )
    {
        SpellsVolume = spellsVolume;
        MusicVolume = musicVolume;
        SFXVolume = sfxVolume;
        MonsterSpeed = monsterSpeed;
        Language = language;
    }
}

[System.Serializable]
public class SaveAchievements
{
    public int[] UnlockedAchievements;

    public SaveAchievements(int[] achievements = null)
    {
        UnlockedAchievements = achievements;
    }
}

[System.Serializable]
public class SaveStats
{
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

    public SaveStats(
        int totalSpellsUsed = 0,
        int totalGoldCollected = 0,
        int totalGoldUsed = 0,
        int totalEnemiesKilled = 0,
        int totalEnemiesKilledNoDamage = 0,
        float totalTraveledDistance = 0,
        int totalPOIsVisited = 0,
        int totalPOIsCleared = 0,
        int totalBuffsCollected = 0,
        int totalCheckpointsReached = 0,
        int totalLevelsUp = 0,
        int totalSpellsUnlocked = 0
    )
    {
        TotalSpellsUsed = totalSpellsUsed;
        TotalGoldCollected = totalGoldCollected;
        TotalGoldUsed = totalGoldUsed;
        TotalEnemiesKilled = totalEnemiesKilled;
        TotalEnemiesKilledNoDamage = totalEnemiesKilledNoDamage;
        TotalTraveledDistance = totalTraveledDistance;
        TotalPOIsVisited = totalPOIsVisited;
        TotalPOIsCleared = totalPOIsCleared;
        TotalBuffsCollected = totalBuffsCollected;
        TotalCheckpointsReached = totalCheckpointsReached;
        TotalLevelsUp = totalLevelsUp;
        TotalSpellsUnlocked = totalSpellsUnlocked;
    }
}

public static class SaveGame
{
    private static string saveFilePath = Application.persistentDataPath + "/save.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Juego guardado en: " + saveFilePath);
    }

    public static SaveData Load()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Juego cargado desde: " + saveFilePath);
            return data;
        }
        else
        {
            Debug.LogWarning("No se encontró un archivo de guardado en: " + saveFilePath);
            return null;
        }
    }
}
