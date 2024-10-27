using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HP")]
    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private Image healthFill;

    [Space(10)]

    [Header("Mana")]
    [SerializeField]
    private Slider manaBar;
    [SerializeField]
    private Image manaFIll;

    [Space(10)]

    [Header("Experience")]
    [SerializeField]
    private Slider expBar;
    [SerializeField]
    private Image expFill;

    [Space(10)]

    [Header("Spells")]
    [SerializeField]
    private TextMeshProUGUI spell1Id;
    [SerializeField]
    private TextMeshProUGUI spell2Id;
    [SerializeField]
    private TextMeshProUGUI spell3Id;
    [SerializeField]
    private TextMeshProUGUI spell4Id;

    [Header("Spell Book")]
    [SerializeField]
    private GameObject spellBook;
    [SerializeField]
    private GameObject spellBookEntryPrefab;
    [SerializeField]
    private TextAsset spellsJSON;


    #region private fields

    private int spellBookEntryPerRow = 10;
    private List<SpellData> allSpells;
    private int[] learnedSpells;
    private Color learnedColor = new(0.1843137f, 0.7882353f, 1f);

    #endregion

    public static UIManager Instance { get; private set; }

    public void UpdateHP(int currentHP, int maxHP)
    {
        healthBar.maxValue = maxHP;
        healthBar.value = currentHP;
    }

    public void UpdateMana(int currentMana, int maxMana)
    {
        manaBar.maxValue = maxMana;
        manaBar.value = currentMana;
    }

    public void UpdateExp(int currentExp, int maxExp)
    {
        expBar.maxValue = maxExp;
        expBar.value = currentExp;
    }

    public void UpdateSpellSlot(int slot, string spellId)
    {
        switch (slot)
        {
            case 1:
                spell1Id.text = spellId;
                break;
            case 2:
                spell2Id.text = spellId;
                break;
            case 3:
                spell3Id.text = spellId;
                break;
            case 4:
                spell4Id.text = spellId;
                break;
        }
    }

    public void LearnSpell(int spellID)
    {
        if (learnedSpells[spellID] == 1) return;

        learnedSpells[spellID] = 1;

        for (int i = 0; i < allSpells.Count; i++)
        {
            if (allSpells[i].SpellID == spellID)
            {
                GameObject spellBookEntry = spellBook.transform.GetChild(i + 3).gameObject;
                spellBookEntry.GetComponent<Button>().interactable = false;
                spellBookEntry.GetComponentInChildren<Image>().color = learnedColor;
                break;
            }
        }
    }

    public void ToggleSpellBook()
    {
        spellBook.SetActive(!spellBook.activeSelf);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SpellDataCollection spells = JsonUtility.FromJson<SpellDataCollection>(spellsJSON.text);
        allSpells = new List<SpellData>(spells.Spells);
    }

    private void Start()
    {
        for (int i = 0; i < allSpells.Count; i++)
        {
            GameObject spellBookEntry = Instantiate(spellBookEntryPrefab, spellBook.transform);
            spellBookEntry.GetComponentInChildren<TextMeshProUGUI>().text = allSpells[i].SpellID.ToString();

            if (i % spellBookEntryPerRow == 0)
            {
                spellBookEntry.transform.localPosition = new Vector3(-275, 100 - i / spellBookEntryPerRow * 60, 0);
            }
            else
            {
                spellBookEntry.transform.localPosition = new Vector3(-275 + i % spellBookEntryPerRow * 60, 100 - i / spellBookEntryPerRow * 60, 0);
            }

            spellBookEntry.GetComponent<Button>().interactable = false;
        }

        spellBook.SetActive(false);
        learnedSpells = new int[allSpells.Count];
    }
}
