using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneHandler : MonoBehaviour
{
    [SerializeField]
    private Slider loadingSlider;

    // Start is called before the first frame update
    void Start()
    {
        LoadScene(SceneToLoadGameObject.GetSceneToLoad());
    }

    private void LoadScene(string sceneName)
    {
        StartCoroutine(LoadMainSceneAsync(sceneName));

        if (SceneToLoadGameObject.GetPreviousScene() == "MainScene")
        {
            ResetECS();
        }
    }

    private IEnumerator LoadMainSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            loadingSlider.value = Mathf.Clamp01(operation.progress / 0.9f);

            if (operation.progress >= 0.9f)
            {
                loadingSlider.value = 1f;
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void ResetECS()
    {
        var defaultWorld = World.DefaultGameObjectInjectionWorld;
        defaultWorld.EntityManager.CompleteAllTrackedJobs();

        foreach (var system in defaultWorld.Systems)
        {
            system.Enabled = false;
        }

        defaultWorld.Dispose();

        DefaultWorldInitialization.Initialize("Default World", false);
        if (!ScriptBehaviourUpdateOrder.IsWorldInCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld))
        {
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld);
        }
    }
}
