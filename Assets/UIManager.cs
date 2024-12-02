using System.Collections.Generic;
using TMPro;
using Unity.Entities;
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
    [SerializeField]
    private TextMeshProUGUI levelText;

    [Header("Coins")]
    [SerializeField]
    private TextMeshProUGUI coinsText;
    private static int MaxCoinsNumber = 99999;

    [Space(10)]

    [Header("Spells")]
    [SerializeField]
    private TextMeshProUGUI spell1Id;
    [SerializeField]
    private Image spell1CooldownVisual;
    [SerializeField]
    private TextMeshProUGUI spell2Id;
    [SerializeField]
    private Image spell2CooldownVisual;
    [SerializeField]
    private TextMeshProUGUI spell3Id;
    [SerializeField]
    private Image spell3CooldownVisual;
    [SerializeField]
    private TextMeshProUGUI spell4Id;
    [SerializeField]
    private Image spell4CooldownVisual;

    [Header("Spell Book")]
    [SerializeField]
    private GameObject spellBook;
    [SerializeField]
    private GameObject spellBookEntryPrefab;
    [SerializeField]
    private TextAsset spellsJSON;
    [SerializeField]
    private TMP_InputField spellBookSearchInput;
    [SerializeField]
    private GameObject spellBookLearnDebugButtonContainer;


    #region private fields

    private int spellBookEntryPerRow = 10;
    private List<SpellData> allSpells;
    private List<SpellData> searchedSpells;
    private int[] learnedSpells;
    private int[] selectedSpells = new int[4];
    private Color learnedColor = new(0.1843137f, 0.7882353f, 1f);

    private int currentLevel = 1;
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

    public void UpdateExp(int currentExp, int maxExp, bool levelUp = false)
    {
        expBar.maxValue = maxExp;
        expBar.value = currentExp;

        if (levelUp)
        {
            currentLevel++;
            levelText.text = currentLevel.ToString();
        }
    }

    public void UpdateSpellSlot(int slot, string spellId)
    {
        switch (slot)
        {
            case 1:
                spell1Id.text = spellId;
                selectedSpells[0] = int.Parse(spellId);
                break;
            case 2:
                spell2Id.text = spellId;
                selectedSpells[1] = int.Parse(spellId);
                break;
            case 3:
                spell3Id.text = spellId;
                selectedSpells[2] = int.Parse(spellId);
                break;
            case 4:
                spell4Id.text = spellId;
                selectedSpells[3] = int.Parse(spellId);
                break;
        }
    }

    public void UpdateSpellCooldown(int spellID, float elapsedTime, float cooldown)
    {
        Image cooldownVisual = null;

        for (int i = 0; i < selectedSpells.Length; i++)
        {
            if (selectedSpells[i] == spellID)
            {
                switch (i)
                {
                    case 0:
                        cooldownVisual = spell1CooldownVisual;
                        break;
                    case 1:
                        cooldownVisual = spell2CooldownVisual;
                        break;
                    case 2:
                        cooldownVisual = spell3CooldownVisual;
                        break;
                    case 3:
                        cooldownVisual = spell4CooldownVisual;
                        break;
                    default:
                        return;
                }

                break;
            }
        }

        if (cooldownVisual == null) return;

        if (elapsedTime >= cooldown)
        {
            cooldownVisual.fillAmount = 0;
        }
        else
        {
            cooldownVisual.fillAmount = 1 - elapsedTime / cooldown;
        }
    }

    public void UpdateCoins(int coins)
    {
        if (coins > MaxCoinsNumber)
        {
            coinsText.text = "+" + MaxCoinsNumber.ToString();
        }
        else
        {
            coinsText.text = coins.ToString();
        }
    }

    public void LearnSpell(int spellID)
    {
        if (learnedSpells[spellID - 1] == 1) return;

        learnedSpells[spellID - 1] = 1;

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

        spellBookSearchInput.onValueChanged.AddListener(OnSpellSearchValueChanged);

        // Debug button
        spellBookLearnDebugButtonContainer.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => debugLearnSpell(1));
        spellBookLearnDebugButtonContainer.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => debugLearnSpell(2));
        spellBookLearnDebugButtonContainer.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => debugLearnSpell(3));
        spellBookLearnDebugButtonContainer.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => debugLearnSpell(4));
        spellBookLearnDebugButtonContainer.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => debugLearnSpell(5));
    }

    private void debugLearnSpell(int SpellID)
    {
        EntityManager entityMananger = World.DefaultGameObjectInjectionWorld.EntityManager;

        Entity playerEntity = entityMananger.CreateEntityQuery(typeof(PlayerComponent)).GetSingletonEntity();

        entityMananger.AddComponentData(playerEntity, new SpellLearnComponent { SpellID = SpellID });
    }

    private void OnSpellSearchValueChanged(string input)
    {
        List<SpellData> filteredSpells = searchSpellByName(input);

        for (int i = 0; i < allSpells.Count; i++)
        {
            if (filteredSpells.IndexOf(allSpells[i]) != -1)
            {
                GameObject spellBookEntry = spellBook.transform.GetChild(i + 3).gameObject;
                spellBookEntry.SetActive(true);
            }
            else
            {
                GameObject spellBookEntry = spellBook.transform.GetChild(i + 3).gameObject;
                spellBookEntry.SetActive(false);
            }
        }
    }

    private List<SpellData> searchSpellByName(string name)
    {
        List<SpellData> result = new();

        foreach (var spell in allSpells)
        {
            if (spell.Name.ToLower().Contains(name.ToLower()))
            {
                result.Add(spell);
            }
        }

        return result;
    }
}
