using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

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
        Debug.Log("Settings button clicked");
    }

    public void HandleExitButton()
    {
        Debug.Log("Exiting game");

        SaveData gameSave = new SaveData(
            DreamCityStatsGameObject.FireBuff,
            DreamCityStatsGameObject.WaterBuff,
            DreamCityStatsGameObject.EarthBuff,
            DreamCityStatsGameObject.AirBuff,
            DreamCityStatsGameObject.CurrentCoins
        );

        SaveGame.Save(gameSave);

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
