using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuAchievementsHandler : MonoBehaviour
{
    [SerializeField]
    private TextAsset AchievementsJSON;

    [SerializeField]
    private GameObject AchievementsContainer;

    [SerializeField]
    private GameObject AchievementPrefab;

    private List<int> AlreadyLoadedAchievements = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadAchievements()
    {
        AchievementDataCollection achievements = JsonUtility.FromJson<AchievementDataCollection>(AchievementsJSON.text);
        List<int> achievementsList = new List<int>();
        SaveData gameSave = SaveGame.Load();

        if (gameSave != null && gameSave.Achievements != null && gameSave.Achievements.UnlockedAchievements != null)
        {
            foreach (int achievement in gameSave.Achievements.UnlockedAchievements)
            {
                achievementsList.Add(achievement);
            }
        }
        else
        {
            Debug.Log("No hay logros guardados");
        }

        foreach (AchievementData achievement in achievements.Achievements)
        {
            if (AlreadyLoadedAchievements.Contains(achievement.AchievementID)) continue;

            GameObject achievementObject = Instantiate(AchievementPrefab, AchievementsContainer.transform);

            Transform title = achievementObject.transform.Find("Title");
            Transform description = achievementObject.transform.Find("Description");

            title.GetComponent<TextMeshProUGUI>().text = LanguageManager.Instance.GetText("ACHIEVEMENT_" + achievement.AchievementID + "_NAME", AvailableLocalizationTables.Achievements);
            description.GetComponent<TextMeshProUGUI>().text = LanguageManager.Instance.GetText("ACHIEVEMENT_" + achievement.AchievementID + "_DESCRIPTION", AvailableLocalizationTables.Achievements);

            if (achievementsList.Contains(achievement.AchievementID))
            {
                Transform check = achievementObject.transform.Find("Check");
                check.gameObject.SetActive(true);
            }

            AlreadyLoadedAchievements.Add(achievement.AchievementID);
        }
    }
}
