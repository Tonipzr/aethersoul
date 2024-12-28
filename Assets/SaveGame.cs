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

    public SaveData(
        int fireBuff,
        int waterBuff,
        int earthBuff,
        int airBuff,
        int currentCoins,
        SaveSettingsData settings
    )
    {
        FireBuff = fireBuff;
        WaterBuff = waterBuff;
        EarthBuff = earthBuff;
        AirBuff = airBuff;
        CurrentCoins = currentCoins;
        Settings = settings;
    }
}

[System.Serializable]
public class SaveSettingsData
{
    public float SpellsVolume;
    public float MusicVolume;
    public float SFXVolume;

    public float MonsterSpeed;

    public SaveSettingsData(
        float spellsVolume = 1.0f,
        float musicVolume = 1.0f,
        float sfxVolume = 1.0f,
        float monsterSpeed = 1.0f
    )
    {
        SpellsVolume = spellsVolume;
        MusicVolume = musicVolume;
        SFXVolume = sfxVolume;
        MonsterSpeed = monsterSpeed;
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
