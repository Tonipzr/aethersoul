using UnityEngine;

public class DreamCityStatsGameObject : MonoBehaviour
{
    public static int FireBuff = 0;
    public static int WaterBuff = 0;
    public static int EarthBuff = 0;
    public static int AirBuff = 0;

    public static int IncreaseCostPerLevel = 5;

    public static int CurrentCoins = 0;

    private static DreamCityStatsGameObject instance = null;

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

    public static void IncreaseFireBuff()
    {
        FireBuff++;
    }

    public static void IncreaseWaterBuff()
    {
        WaterBuff++;
    }

    public static void IncreaseEarthBuff()
    {
        EarthBuff++;
    }

    public static void IncreaseAirBuff()
    {
        AirBuff++;
    }

    public static void IncreaseCoins(int amount)
    {
        CurrentCoins += amount;
    }

    public static void DecreaseCoins(int amount)
    {
        CurrentCoins -= amount;
    }
}
