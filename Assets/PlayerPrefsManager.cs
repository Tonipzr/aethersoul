using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    public static PlayerPrefsManager Instance { get; private set; }

    private float SpellsVolume = 1.0f;
    private float MusicVolume = 1.0f;
    private float SFXVolume = 1.0f;

    private float MonsterSpeed = 100.0f;

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

        if (gameSave != null)
        {
            SetSpellsVolume(gameSave.Settings.SpellsVolume);
            SetMusicVolume(gameSave.Settings.MusicVolume);
            SetSFXVolume(gameSave.Settings.SFXVolume);
            SetMonsterSpeed(gameSave.Settings.MonsterSpeed);
        }
    }

    public void SetSpellsVolume(float volume)
    {
        SpellsVolume = volume;
    }

    public float GetSpellsVolume()
    {
        return SpellsVolume;
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
    }

    public float GetMusicVolume()
    {
        return MusicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
    }

    public float GetSFXVolume()
    {
        return SFXVolume;
    }

    public void SetMonsterSpeed(float speed)
    {
        MonsterSpeed = speed;
    }

    public float GetMonsterSpeed()
    {
        return MonsterSpeed;
    }
}
