using UnityEngine;

public class SpellBookEntryTooltipHandler : MonoBehaviour
{
    public int spellID;

    public GameObject tooltipContainer;
    public TMPro.TMP_Text spellName;
    public TMPro.TMP_Text spellDescription;
    public TMPro.TMP_Text spellLore;

    public void ShowTooltip()
    {
        DisableBrotherTooltip();

        tooltipContainer.SetActive(true);
        LanguageManager.Instance.UpdateLocalizeStringEvent(spellName.gameObject, AvailableLocalizationTables.Spells, "SPELL_" + spellID + "_NAME");
        LanguageManager.Instance.UpdateLocalizeStringEvent(spellDescription.gameObject, AvailableLocalizationTables.Spells, "SPELL_" + spellID + "_DESCRIPTION_ESCARLINA");
        LanguageManager.Instance.UpdateLocalizeStringEvent(spellLore.gameObject, AvailableLocalizationTables.Spells, "SPELL_" + spellID + "_LORE_ESCARLINA");
    }

    public void HideAllTooltips()
    {
        foreach (Transform child in transform.parent)
        {
            child.GetComponent<SpellBookEntryTooltipHandler>().HideTooltip();
        }
    }

    public void HideTooltip()
    {
        tooltipContainer.SetActive(false);
    }

    private void DisableBrotherTooltip()
    {
        foreach (Transform child in transform.parent)
        {
            if (child != transform)
            {
                child.GetComponent<SpellBookEntryTooltipHandler>().HideTooltip();
            }
        }
    }
}
