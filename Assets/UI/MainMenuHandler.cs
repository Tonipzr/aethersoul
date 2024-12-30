using TMPro;
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
            )
        );

        SaveGame.Save(gameSave);

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
