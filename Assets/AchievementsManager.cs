using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager Instance { get; private set; }

    private List<int> unlocked = new List<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UnlockAchievement(int id)
    {
        if (!unlocked.Contains(id))
        {
            unlocked.Add(id);
        }
    }

    public bool IsAchievementUnlocked(int id)
    {
        return unlocked.Contains(id);
    }

    public List<int> GetUnlockedAchievements()
    {
        return unlocked;
    }
}