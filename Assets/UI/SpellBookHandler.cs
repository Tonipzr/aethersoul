using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellBookHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject spellBook;
    [SerializeField]
    private GameObject spellBookEntryContainer;
    [SerializeField]
    private GameObject spellBookEntryPrefab;
    [SerializeField]
    private GameObject spellBookEntryDescriptionContainer;
    [SerializeField]
    private TextAsset spellsJSON;

    [SerializeField]
    private Sprite fireSlashIcon;
    [SerializeField]
    private Sprite flameNovaIcon;
    [SerializeField]
    private Sprite gustIcon;
    [SerializeField]
    private Sprite infernoBurstIcon;
    [SerializeField]
    private Sprite meteorStormIcon;

    [SerializeField]
    private TMP_Dropdown slot1Selector;
    [SerializeField]
    private TMP_Dropdown slot2Selector;
    [SerializeField]
    private TMP_Dropdown slot3Selector;
    [SerializeField]
    private TMP_Dropdown slot4Selector;

    private List<SpellData> allSpells;
    private Dictionary<int, bool> learnedSpells;
    private Color learnedColor = new(0.1843137f, 0.7882353f, 1f);

    void Awake()
    {
        SpellDataCollection spells = JsonUtility.FromJson<SpellDataCollection>(spellsJSON.text);
        allSpells = new List<SpellData>(spells.Spells);
        allSpells = new List<SpellData>(allSpells.GetRange(0, 5));
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < allSpells.Count; i++)
        {
            GameObject spellBookEntry = Instantiate(spellBookEntryPrefab, spellBookEntryContainer.transform);

            spellBookEntry.GetComponentInChildren<Image>().sprite = GetSpellIcon(i + 1);

            SpellBookEntryTooltipHandler tooltipHandler = spellBookEntry.GetComponent<SpellBookEntryTooltipHandler>();
            tooltipHandler.spellID = i + 1;
        }

        spellBook.SetActive(false);
        learnedSpells = new Dictionary<int, bool>
        {
            { 1, false },
            { 2, false },
            { 3, false },
            { 4, false },
            { 5, false }
        };
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ToggleSpellBook()
    {
        spellBook.SetActive(!spellBook.activeSelf);

        GameObject spellBookEntry = spellBookEntryContainer.transform.GetChild(0).gameObject;
        SpellBookEntryTooltipHandler tooltipHandler = spellBookEntry.GetComponent<SpellBookEntryTooltipHandler>();
        tooltipHandler.HideAllTooltips();
    }

    public void LearnSpell(int spellID)
    {
        if (learnedSpells[spellID]) return;

        learnedSpells[spellID] = true;

        for (int i = 0; i < allSpells.Count; i++)
        {
            if (allSpells[i].SpellID == spellID && spellBookEntryContainer.transform.childCount >= i)
            {
                GameObject spellBookEntry = spellBookEntryContainer.transform.GetChild(i).gameObject;
                spellBookEntry.GetComponentInChildren<Image>().color = learnedColor;
                break;
            }
        }

        UpdateDropdowns();
    }

    private void UpdateDropdowns()
    {
        List<TMP_Dropdown> dropdowns = new List<TMP_Dropdown> { slot1Selector, slot2Selector, slot3Selector, slot4Selector };

        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            dropdown.ClearOptions();
            foreach (int spell in learnedSpells.Keys)
            {
                if (learnedSpells[spell])
                    dropdown.options.Add(new TMP_Dropdown.OptionData(LanguageManager.Instance.GetText("SPELL_" + spell + "_NAME", AvailableLocalizationTables.Spells)));
            }
        }
    }

    public int GetSpellDropdownOption(int slot, int selecPosition)
    {
        List<TMP_Dropdown> dropdowns = new List<TMP_Dropdown> { slot1Selector, slot2Selector, slot3Selector, slot4Selector };

        string spellName = dropdowns[slot - 1].options[selecPosition].text;
        return SpellIDFromName(spellName);
    }

    private Sprite GetSpellIcon(int spellID)
    {
        switch (spellID)
        {
            case 1:
                return fireSlashIcon;
            case 2:
                return flameNovaIcon;
            case 3:
                return meteorStormIcon;
            case 4:
                return infernoBurstIcon;
            case 5:
                return gustIcon;
            default:
                return null;
        }
    }

    private int SpellIDFromName(string spellName)
    {
        string spell1 = LanguageManager.Instance.GetText("SPELL_1_NAME", AvailableLocalizationTables.Spells);
        string spell2 = LanguageManager.Instance.GetText("SPELL_2_NAME", AvailableLocalizationTables.Spells);
        string spell3 = LanguageManager.Instance.GetText("SPELL_3_NAME", AvailableLocalizationTables.Spells);
        string spell4 = LanguageManager.Instance.GetText("SPELL_4_NAME", AvailableLocalizationTables.Spells);
        string spell5 = LanguageManager.Instance.GetText("SPELL_5_NAME", AvailableLocalizationTables.Spells);

        if (spellName == spell1)
            return 1;

        if (spellName == spell2)
            return 2;

        if (spellName == spell3)
            return 3;

        if (spellName == spell4)
            return 4;

        if (spellName == spell5)
            return 5;

        return -1;
    }
}
