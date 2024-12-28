using TMPro;
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
    private TextMeshProUGUI interactText;

    public static DreamCityUIHandler Instance { get; private set; }

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

        // Debug.Log(UserInputManager.Instance.GetKeyMap("Interact"));

        interactText.text = UserInputManager.Instance.GetKeyMap("Interact");
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
                buffTitle.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("GODS_FIRE_NAME", AvailableLocalizationTables.DreamCity);
                buffDescription.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("GODS_FIRE_DESCRIPTION", AvailableLocalizationTables.DreamCity);
                currentBuffLevel.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("LEVEL_TEXT", AvailableLocalizationTables.UI, DreamCityStatsGameObject.FireBuff.ToString());
                buffCost.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("PRICE_TEXT", AvailableLocalizationTables.UI, (DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.FireBuff + 1)).ToString());
                break;
            case "WaterStatue":
                buffTitle.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("GODS_WATER_NAME", AvailableLocalizationTables.DreamCity); ;
                buffDescription.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("GODS_WATER_DESCRIPTION", AvailableLocalizationTables.DreamCity);
                currentBuffLevel.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("LEVEL_TEXT", AvailableLocalizationTables.UI, DreamCityStatsGameObject.WaterBuff.ToString());
                buffCost.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("PRICE_TEXT", AvailableLocalizationTables.UI, (DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.WaterBuff + 1)).ToString());
                break;
            case "EarthStatue":
                buffTitle.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("GODS_EARTH_NAME", AvailableLocalizationTables.DreamCity); ;
                buffDescription.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("GODS_EARTH_DESCRIPTION", AvailableLocalizationTables.DreamCity);
                currentBuffLevel.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("LEVEL_TEXT", AvailableLocalizationTables.UI, DreamCityStatsGameObject.EarthBuff.ToString());
                buffCost.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("PRICE_TEXT", AvailableLocalizationTables.UI, (DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.EarthBuff + 1)).ToString());
                break;
            case "WindStatue":
                buffTitle.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("GODS_AIR_NAME", AvailableLocalizationTables.DreamCity); ;
                buffDescription.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("GODS_AIR_DESCRIPTION", AvailableLocalizationTables.DreamCity);
                currentBuffLevel.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("LEVEL_TEXT", AvailableLocalizationTables.UI, DreamCityStatsGameObject.AirBuff.ToString());
                buffCost.GetComponent<TMPro.TextMeshProUGUI>().text = LanguageManager.Instance.GetText("PRICE_TEXT", AvailableLocalizationTables.UI, (DreamCityStatsGameObject.IncreaseCostPerLevel * (DreamCityStatsGameObject.AirBuff + 1)).ToString());
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
        AudioManager.Instance.PlayAudio(AudioType.UpgradeEffect);
    }
}
