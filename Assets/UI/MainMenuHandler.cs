using TMPro;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject SettingsPanel;
    [SerializeField]
    private GameObject KeybindsPanel;
    [SerializeField]
    private GameObject SoundsPanel;
    [SerializeField]
    private GameObject DificultyPanel;

    [SerializeField]
    private GameObject AchievementsPanel;
    [SerializeField]
    private GameObject AchievementsContainer;

    [SerializeField]
    private Slider SpellsVolumeSlider;
    [SerializeField]
    private Slider MusicVolumeSlider;
    [SerializeField]
    private Slider SFXVolumeSlider;

    [SerializeField]
    private Slider MonsterSpeedSlider;

    [SerializeField]
    private TMP_Dropdown LanguageDropdown;


    private enum CurrentSettingsLocation
    {
        None,
        Main,
        Keybinds,
        Sound,
        Difficulty
    }

    private CurrentSettingsLocation currentSettingsLocation = CurrentSettingsLocation.None;

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentSettingsLocation(CurrentSettingsLocation.None);

        SpellsVolumeSlider.value = PlayerPrefsManager.Instance.GetSpellsVolume();
        MusicVolumeSlider.value = PlayerPrefsManager.Instance.GetMusicVolume();
        SFXVolumeSlider.value = PlayerPrefsManager.Instance.GetSFXVolume();

        MonsterSpeedSlider.value = PlayerPrefsManager.Instance.GetMonsterSpeed();

        if (PlayerPrefsManager.Instance.GetLanguage() == "es")
        {
            LanguageDropdown.value = 0;
        }
        else if (PlayerPrefsManager.Instance.GetLanguage() == "en")
        {
            LanguageDropdown.value = 1;
        }
        else if (PlayerPrefsManager.Instance.GetLanguage() == "ca")
        {
            LanguageDropdown.value = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandlePlayButton()
    {
        Debug.Log("Play button clicked");

        SceneToLoadGameObject.FromSceneToScene("MainMenuScene", "MainScene");

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity mapEntity = entityManager.CreateEntityQuery(typeof(MapEntityComponent)).GetSingletonEntity();

        if (entityManager.HasComponent<MapEntityGameStateComponent>(mapEntity))
        {
            MapEntityGameStateComponent gameState = entityManager.GetComponentData<MapEntityGameStateComponent>(mapEntity);
            gameState.GameStarted = true;
            gameState.GamePhase = GamePhase.Phase1;

            entityManager.SetComponentData(mapEntity, gameState);
        }

        SceneManager.LoadScene("LoadingScene");
    }

    public void HandleDreamCityButton()
    {
        Debug.Log("Dream City button clicked");

        SceneToLoadGameObject.FromSceneToScene("MainMenuScene", "DreamCityScene");

        SceneManager.LoadScene("LoadingScene");
    }

    public void HandleSettingsButton()
    {
        SetCurrentSettingsLocation(CurrentSettingsLocation.Main);
    }

    public void HandleKeybindsButton()
    {
        SetCurrentSettingsLocation(CurrentSettingsLocation.Keybinds);
    }

    public void HandleSoundButton()
    {
        SetCurrentSettingsLocation(CurrentSettingsLocation.Sound);
    }

    public void HandleDifficultyButton()
    {
        SetCurrentSettingsLocation(CurrentSettingsLocation.Difficulty);
    }

    public void HandleBackSettingsButton(bool global = false)
    {
        if (global) SetCurrentSettingsLocation(CurrentSettingsLocation.None);
        else SetCurrentSettingsLocation(CurrentSettingsLocation.Main);
    }

    public void HandleLanguageDropdown()
    {
        Debug.Log("Language dropdown value: " + LanguageDropdown.options[LanguageDropdown.value].text);

        string lang = "es";
        if (LanguageDropdown.options[LanguageDropdown.value].text == "Español")
        {
            lang = "es";
        }
        else if (LanguageDropdown.options[LanguageDropdown.value].text == "English")
        {
            lang = "en";
        }
        else if (LanguageDropdown.options[LanguageDropdown.value].text == "Català")
        {
            lang = "ca";
        }

        PlayerPrefsManager.Instance.SetLanguage(lang);
        LanguageManager.Instance.SetLanguage(LanguageDropdown.options[LanguageDropdown.value].text);
    }

    public void SaveConfiguration()
    {
        Debug.Log("Configuration saved");

        if (currentSettingsLocation == CurrentSettingsLocation.Sound)
        {
            PlayerPrefsManager.Instance.SetSpellsVolume(SpellsVolumeSlider.value);
            PlayerPrefsManager.Instance.SetMusicVolume(MusicVolumeSlider.value);
            PlayerPrefsManager.Instance.SetSFXVolume(SFXVolumeSlider.value);

            AudioManager.Instance.SetVolumeToChannel("SpellsVolume", SpellsVolumeSlider.value);
            AudioManager.Instance.SetVolumeToChannel("MusicVolume", MusicVolumeSlider.value);
            AudioManager.Instance.SetVolumeToChannel("SFXVolume", SFXVolumeSlider.value);
        }

        if (currentSettingsLocation == CurrentSettingsLocation.Difficulty)
        {
            PlayerPrefsManager.Instance.SetMonsterSpeed(MonsterSpeedSlider.value);
        }
    }

    private void SetCurrentSettingsLocation(CurrentSettingsLocation location)
    {
        currentSettingsLocation = location;

        switch (location)
        {
            case CurrentSettingsLocation.Main:
                SettingsPanel.SetActive(true);
                KeybindsPanel.SetActive(false);
                SoundsPanel.SetActive(false);
                DificultyPanel.SetActive(false);
                break;
            case CurrentSettingsLocation.Keybinds:
                SettingsPanel.SetActive(true);
                KeybindsPanel.SetActive(true);
                SoundsPanel.SetActive(false);
                DificultyPanel.SetActive(false);
                break;
            case CurrentSettingsLocation.Sound:
                SettingsPanel.SetActive(true);
                KeybindsPanel.SetActive(false);
                SoundsPanel.SetActive(true);
                DificultyPanel.SetActive(false);
                break;
            case CurrentSettingsLocation.Difficulty:
                SettingsPanel.SetActive(true);
                KeybindsPanel.SetActive(false);
                SoundsPanel.SetActive(false);
                DificultyPanel.SetActive(true);
                break;
            default:
                SettingsPanel.SetActive(false);
                KeybindsPanel.SetActive(false);
                SoundsPanel.SetActive(false);
                DificultyPanel.SetActive(false);
                break;
        }
    }

    public void HandleAchievementsButton()
    {
        Debug.Log("Achievements button clicked");

        AchievementsPanel.SetActive(!AchievementsPanel.activeSelf);

        if (AchievementsPanel.activeSelf)
        {
            AchievementsContainer.GetComponent<MainMenuAchievementsHandler>().LoadAchievements();
        }
    }

    public void HandleButtonHover()
    {
        AudioManager.Instance.PlayAudio(AudioType.Hover);
    }

    public void HandleButtonConfirm()
    {
        AudioManager.Instance.PlayAudio(AudioType.Confirm);
    }

    public void HandleExitButton()
    {
        Debug.Log("Exiting game");

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

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
