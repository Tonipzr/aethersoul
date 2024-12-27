using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DreamCityUIHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject interactImage;

    [SerializeField]
    private GameObject buffUI;

    [SerializeField]
    private GameObject buffTitle;
    [SerializeField]
    private GameObject buffDescription;
    [SerializeField]
    private GameObject currentBuffLevel;
    [SerializeField]
    private GameObject buffCost;
    [SerializeField]
    private Button increaseBuffButton;

    [SerializeField]
    private GameObject audioUpgradeEffect;

    public static DreamCityUIHandler Instance { get; private set; }

    #region FireVars
    private static string fireName = "Emberis";
    private static string fireDescription = "Diosa del fuego, encarna pasión, renacimiento y poder ardiente.";
    #endregion

    #region WaterVars
    private static string waterName = "Mareira";
    private static string waterDescription = "Diosa del agua, símbolo de fluidez, mareas y vida oceánica.";
    #endregion

    #region EarthVars
    private static string earthName = "Sylvara";
    private static string earthDescription = "Diosa de la tierra, protectora de bosques y fauna.";
    #endregion

    #region AirVars
    private static string airName = "Brisalia";
    private static string airDescription = "Diosa del viento, portadora de libertad, calma e inspiración.";
    #endregion

    #region SharedVars
    public bool BoughtBuff = false;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ToggleInteractImage(bool status)
    {
        interactImage.SetActive(status);
    }

    public void ToggleBuffUI(bool status, string statueType)
    {
        SetBuffUI(statueType);
        buffUI.SetActive(status);
    }

    private void HandleIncreaseButton(string statueType)
    {
        int nextLevelPrice = int.MaxValue;
        switch (statueType)
        {
            case "FireStatue":
                nextLevelPrice = DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.FireBuff + 1);
                break;
            case "WaterStatue":
                nextLevelPrice = DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.WaterBuff + 1);
                break;
            case "EarthStatue":
                nextLevelPrice = DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.EarthBuff + 1);
                break;
            case "WindStatue":
                nextLevelPrice = DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.AirBuff + 1);
                break;
        }

        if (DreamCityStatsGameObject.CurrentCoins >= nextLevelPrice)
        {
            increaseBuffButton.interactable = true;
        }
        else
        {
            increaseBuffButton.interactable = false;
        }

        increaseBuffButton.onClick.RemoveAllListeners();

        increaseBuffButton.onClick.AddListener(() =>
        {
            switch (statueType)
            {
                case "FireStatue":
                    DreamCityStatsGameObject.IncreaseFireBuff();
                    break;
                case "WaterStatue":
                    DreamCityStatsGameObject.IncreaseWaterBuff();
                    break;
                case "EarthStatue":
                    DreamCityStatsGameObject.IncreaseEarthBuff();
                    break;
                case "WindStatue":
                    DreamCityStatsGameObject.IncreaseAirBuff();
                    break;
            }

            DreamCityStatsGameObject.DecreaseCoins(nextLevelPrice);
            SetBuffUI(statueType);
            BoughtBuff = true;

            if (DreamCityStatsGameObject.CurrentCoins < nextLevelPrice)
            {
                increaseBuffButton.interactable = false;
            }
        });
    }

    private void SetBuffUI(string statueType)
    {
        switch (statueType)
        {
            case "FireStatue":
                buffTitle.GetComponent<TMPro.TextMeshProUGUI>().text = fireName;
                buffDescription.GetComponent<TMPro.TextMeshProUGUI>().text = fireDescription;
                currentBuffLevel.GetComponent<TMPro.TextMeshProUGUI>().text = "Level " + DreamCityStatsGameObject.FireBuff;
                buffCost.GetComponent<TMPro.TextMeshProUGUI>().text = "Price " + (DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.FireBuff + 1)).ToString();
                break;
            case "WaterStatue":
                buffTitle.GetComponent<TMPro.TextMeshProUGUI>().text = waterName;
                buffDescription.GetComponent<TMPro.TextMeshProUGUI>().text = waterDescription;
                currentBuffLevel.GetComponent<TMPro.TextMeshProUGUI>().text = "Level " + DreamCityStatsGameObject.WaterBuff;
                buffCost.GetComponent<TMPro.TextMeshProUGUI>().text = "Price " + (DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.WaterBuff + 1)).ToString();
                break;
            case "EarthStatue":
                buffTitle.GetComponent<TMPro.TextMeshProUGUI>().text = earthName;
                buffDescription.GetComponent<TMPro.TextMeshProUGUI>().text = earthDescription;
                currentBuffLevel.GetComponent<TMPro.TextMeshProUGUI>().text = "Level " + DreamCityStatsGameObject.EarthBuff;
                buffCost.GetComponent<TMPro.TextMeshProUGUI>().text = "Price " + (DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.EarthBuff + 1)).ToString();
                break;
            case "WindStatue":
                buffTitle.GetComponent<TMPro.TextMeshProUGUI>().text = airName;
                buffDescription.GetComponent<TMPro.TextMeshProUGUI>().text = airDescription;
                currentBuffLevel.GetComponent<TMPro.TextMeshProUGUI>().text = "Level " + DreamCityStatsGameObject.AirBuff;
                buffCost.GetComponent<TMPro.TextMeshProUGUI>().text = "Price " + (DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.AirBuff + 1)).ToString();
                break;
        }
        HandleIncreaseButton(statueType);
    }

    public void CheckPointInteract()
    {
        SceneToLoadGameObject.FromSceneToScene("DreamCityScene", "MainMenuScene");

        SceneManager.LoadScene("LoadingScene");
    }

    public void PlayAudioUpgradeEffect()
    {
        audioUpgradeEffect.GetComponent<AudioSource>().Play();
    }
}
