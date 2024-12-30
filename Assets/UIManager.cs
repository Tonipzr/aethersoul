using System;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
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

    [Space(10)]

    [Header("Coins")]
    [SerializeField]
    private TextMeshProUGUI coinsText;
    private static int MaxCoinsNumber = 99999;

    [Space(10)]

    [Header("Upgrades")]
    [SerializeField]
    private TextAsset UpgradeJSON;
    [SerializeField]
    private GameObject levelUpCardPickerContainer;
    [SerializeField]
    private GameObject Card1;
    [SerializeField]
    private GameObject Card2;
    [SerializeField]
    private GameObject Card3;
    [SerializeField]
    private Sprite cardUpgradeImage;

    [Space(10)]

    [Header("MainMenu")]
    [SerializeField]
    private GameObject mainMenuContainer;
    [SerializeField]
    private GameObject SettingsMenuContainer;

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

    [Header("Objectives")]
    [SerializeField]
    private GameObject objectivesContainer;
    [SerializeField]
    private GameObject objective1;
    [SerializeField]
    private GameObject objective2;
    [SerializeField]
    private GameObject objective3;

    private Dictionary<int, GameObject> objectivesRefs = new();

    #region private fields

    private int spellBookEntryPerRow = 10;
    private List<SpellData> allSpells;
    private List<SpellData> searchedSpells;
    private int[] learnedSpells;
    private int[] selectedSpells = new int[4];
    private Color learnedColor = new(0.1843137f, 0.7882353f, 1f);

    private List<UpgradeData> allUpgrades;

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

    public void ToggleMenu()
    {
        mainMenuContainer.SetActive(!mainMenuContainer.activeSelf);

        if (mainMenuContainer.activeSelf)
        {
            SettingsMenuContainer.SetActive(false);
        }
        else
        {
            SettingsMenuContainer.SetActive(mainMenuContainer.activeSelf);
        }

        ToggleGamePause();
    }

    public void ToggleCardPicker()
    {
        levelUpCardPickerContainer.SetActive(!levelUpCardPickerContainer.activeSelf);

        ToggleGamePause();

        if (levelUpCardPickerContainer.activeSelf)
        {
            List<UpgradeData> randomUpgrades = selectRandomUpgrades();

            BuildCard(Card1, randomUpgrades[0]);
            BuildCard(Card2, randomUpgrades[1]);
            BuildCard(Card3, randomUpgrades[2]);
        }
    }

    private void ToggleGamePause()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity mapEntity = entityManager.CreateEntityQuery(typeof(MapEntityComponent)).GetSingletonEntity();

        if (entityManager.HasComponent<MapEntityGameStateComponent>(mapEntity))
        {
            MapEntityGameStateComponent gameState = entityManager.GetComponentData<MapEntityGameStateComponent>(mapEntity);
            gameState.IsPaused = !gameState.IsPaused;

            entityManager.SetComponentData(mapEntity, gameState);

            if (gameState.IsPaused)
            {
                AudioManager.Instance.PlayAudio(AudioType.Pause);
            }
            else
            {
                AudioManager.Instance.PlayAudio(AudioType.UnPause);
            }
        }
    }

    private void BuildCard(GameObject card, UpgradeData upgrade)
    {
        TextMeshProUGUI cardTextComponents = card.GetComponentInChildren<TextMeshProUGUI>();
        Image[] cardImage = card.GetComponentsInChildren<Image>();
        cardTextComponents.text = getUpgradeDescription(upgrade);
        cardImage[1].sprite = cardUpgradeImage;

        EventTrigger eventTrigger = card.GetComponent<EventTrigger>();

        EventTrigger.Entry pointerClickEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };

        pointerClickEntry.callback.AddListener((data) =>
        {
            OnPointerClickHandler((PointerEventData)data, upgrade);
        });

        eventTrigger.triggers.Add(pointerClickEntry);
    }

    private void OnPointerClickHandler(PointerEventData data, UpgradeData upgrade)
    {
        RemoveClickListener(Card1);
        RemoveClickListener(Card2);
        RemoveClickListener(Card3);

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity playerEntity = entityManager.CreateEntityQuery(typeof(PlayerComponent)).GetSingletonEntity();

        if (
            upgrade.Type == "HealthRestore" ||
            upgrade.Type == "ManaRestore" ||
            upgrade.Type == "UnlockSpell"
        )
        {
            if (upgrade.Type == "UnlockSpell")
            {
                entityManager.AddComponentData(playerEntity, new SpellLearnComponent { SpellID = (int)upgrade.UpgradePerLevel });
                AudioManager.Instance.PlayAudio(AudioType.Buff);
            }

            if (upgrade.Type == "HealthRestore")
            {
                HealthComponent healthComponent = entityManager.GetComponentData<HealthComponent>(playerEntity);

                entityManager.AddComponentData(playerEntity, new HealthRestoreComponent { HealAmount = healthComponent.MaxHealth });
                AudioManager.Instance.PlayAudio(AudioType.Heal);
            }

            if (upgrade.Type == "ManaRestore")
            {
                ManaComponent manaComponent = entityManager.GetComponentData<ManaComponent>(playerEntity);

                entityManager.AddComponentData(playerEntity, new ManaRestoreComponent { RestoreAmount = manaComponent.MaxMana });
                AudioManager.Instance.PlayAudio(AudioType.Heal);
            }
        }
        else
        {
            Enum.TryParse(upgrade.Type, out UpgradeType upgradeType);

            var buffer = entityManager.GetBuffer<ActiveUpgradesComponent>(playerEntity);

            float upgradeLevel = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].UpgradeID == upgrade.UpgradeID)
                {
                    upgradeLevel = buffer[i].Value;
                    buffer.RemoveAt(i);
                    break;
                }
            }

            buffer.Add(new ActiveUpgradesComponent { UpgradeID = upgrade.UpgradeID, Type = upgradeType, Value = upgrade.UpgradePerLevel + upgradeLevel });
            AudioManager.Instance.PlayAudio(AudioType.Buff);
        }

        ToggleCardPicker();
    }

    private void RemoveClickListener(GameObject card)
    {
        EventTrigger eventTrigger = card.GetComponent<EventTrigger>();

        eventTrigger.triggers.Clear();
    }

    private string getUpgradeDescription(UpgradeData upgrade)
    {
        return LanguageManager.Instance.GetText("UPGRADE_" + upgrade.UpgradeID + "_DESCRIPTION", AvailableLocalizationTables.Upgrades, upgrade.UpgradePerLevel.ToString());
    }

    private List<UpgradeData> selectRandomUpgrades()
    {
        List<UpgradeData> result = new();

        List<int> usedIndexes = new();
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, allUpgrades.Count);

            while (usedIndexes.IndexOf(randomIndex) != -1)
            {
                randomIndex = UnityEngine.Random.Range(0, allUpgrades.Count);
            }

            result.Add(allUpgrades[randomIndex]);

            usedIndexes.Add(randomIndex);
        }

        return result;
    }

    public void AddObjectives(int[] objectiveID)
    {
        if (objectiveID.Length != 3) return;

        for (int i = 0; i < objectiveID.Length; i++)
        {
            GameObject objective = null;

            switch (i)
            {
                case 0:
                    objective = objective1;
                    break;
                case 1:
                    objective = objective2;
                    break;
                case 2:
                    objective = objective3;
                    break;
            }

            objective.GetComponentInChildren<LocalizeStringEvent>().StringReference = new LocalizedString { TableReference = AvailableLocalizationTables.Objectives.ToString(), TableEntryReference = "OBJECTIVE_" + objectiveID[i] + "_DESCRIPTION" };
            objectivesRefs.Add(objectiveID[i], objective);
        }
    }

    public void CompleteObjective(int objectiveID)
    {
        if (objectivesRefs.ContainsKey(objectiveID))
        {
            GameObject objective = objectivesRefs[objectiveID];
            objective.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        }
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
        UpgradeDataCollection upgrades = JsonUtility.FromJson<UpgradeDataCollection>(UpgradeJSON.text);
        allSpells = new List<SpellData>(spells.Spells);
        allUpgrades = new List<UpgradeData>(upgrades.Upgrades);
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
