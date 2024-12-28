using Unity.Entities;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

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

    public string GetText(string key, AvailableLocalizationTables table = AvailableLocalizationTables.UI, string arg = "")
    {
        var stringTable = LocalizationSettings.StringDatabase.GetTableAsync(table.ToString());

        if (stringTable.IsDone && stringTable.Result != null)
        {
            var entry = stringTable.Result.GetEntry(key);
            if (entry != null)
            {
                if (table == AvailableLocalizationTables.Upgrades || table == AvailableLocalizationTables.UI)
                {
                    return entry.GetLocalizedString(new object[] { arg });
                }

                return entry.GetLocalizedString();
            }
        }

        Debug.LogWarning($"Key '{key}' on table '{table}' not found.");
        return key;
    }
}

public enum AvailableLocalizationTables
{
    Spells,
    UI,
    Upgrades,
    DreamCity
}