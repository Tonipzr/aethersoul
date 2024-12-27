using UnityEngine;

public class SceneToLoadGameObject : MonoBehaviour
{
    private static string SceneToLoad = "MainMenuScene";
    private static string PreviousScene = "MainMenuScene";
    private static SceneToLoadGameObject instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void SetSceneToLoad(string sceneName)
    {
        SceneToLoad = sceneName;
    }

    public static string GetSceneToLoad()
    {
        return SceneToLoad;
    }

    public static void SetPreviousScene(string sceneName)
    {
        PreviousScene = sceneName;
    }

    public static string GetPreviousScene()
    {
        return PreviousScene;
    }

    public static void FromSceneToScene(string sceneName, string SceneToLoad)
    {
        SetPreviousScene(sceneName);
        SetSceneToLoad(SceneToLoad);
    }
}
