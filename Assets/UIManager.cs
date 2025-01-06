using System;
using System.Collections;
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
    private TMP_Text healthText;

    [Space(10)]

    [Header("Mana")]
    [SerializeField]
    private Slider manaBar;
    [SerializeField]
    private TMP_Text manaText;

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
    private Image spell1Background;
    [SerializeField]
    private Image spell1CooldownVisual;
    [SerializeField]
    private Image spell2Background;
    [SerializeField]
    private Image spell2CooldownVisual;
    [SerializeField]
    private Image spell3Background;
    [SerializeField]
    private Image spell3CooldownVisual;
    [SerializeField]
    private Image spell4Background;
    [SerializeField]
    private Image spell4CooldownVisual;

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

    [Header("Spell Book")]
    [SerializeField]
    private GameObject spellBook;

    [Space(10)]

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

    [Space(10)]

    [Header("Story")]
    [SerializeField]
    private GameObject MiddleTextWarn;
    [SerializeField]
    private GameObject MiddleTextWarnTitle;
    [SerializeField]
    private GameObject MiddleTextWarnText;

    [SerializeField]
    private GameObject LoreDisplay;

    [Space(10)]
    [Header("Global")]
    [SerializeField]
    private GameObject InteractContainer;
    [SerializeField]
    private TextMeshProUGUI InteractText;

    #region private fields

    private int[] selectedSpells = new int[4];

    private List<UpgradeData> allUpgrades;

    private int currentLevel = 1;
    #endregion

    public static UIManager Instance { get; private set; }

    public void UpdateHP(int currentHP, int maxHP)
    {
        healthBar.maxValue = maxHP;
        healthBar.value = currentHP;

        healthText.text = currentHP + "/" + maxHP;
    }

    public void UpdateMana(int currentMana, int maxMana)
    {
        manaBar.maxValue = maxMana;
        manaBar.value = currentMana;

        manaText.text = currentMana + "/" + maxMana;
    }

    public void UpdateExp(int currentExp, int maxExp)
    {
        expBar.maxValue = maxExp;
        expBar.value = currentExp;
    }

    public void UpdateLevel(int level)
    {
        currentLevel++;
        levelText.text = currentLevel.ToString();
    }

    public void UpdateSpellSlot(int slot, string spellId)
    {
        switch (slot)
        {
            case 1:
                spell1Background.sprite = GetSpellIcon(int.Parse(spellId));
                selectedSpells[0] = int.Parse(spellId);
                break;
            case 2:
                spell2Background.sprite = GetSpellIcon(int.Parse(spellId));
                selectedSpells[1] = int.Parse(spellId);
                break;
            case 3:
                spell3Background.sprite = GetSpellIcon(int.Parse(spellId));
                selectedSpells[2] = int.Parse(spellId);
                break;
            case 4:
                spell4Background.sprite = GetSpellIcon(int.Parse(spellId));
                selectedSpells[3] = int.Parse(spellId);
                break;
        }
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

    public void UpdateSpellCooldown(int spellID, float elapsedTime, float cooldown)
    {
        List<Image> cooldownVisuals = new List<Image>();
        for (int i = 0; i < selectedSpells.Length; i++)
        {
            if (selectedSpells[i] == spellID)
            {
                switch (i)
                {
                    case 0:
                        cooldownVisuals.Add(spell1CooldownVisual);
                        break;
                    case 1:
                        cooldownVisuals.Add(spell2CooldownVisual);
                        break;
                    case 2:
                        cooldownVisuals.Add(spell3CooldownVisual);
                        break;
                    case 3:
                        cooldownVisuals.Add(spell4CooldownVisual);
                        break;
                    default:
                        return;
                }
            }
        }

        if (cooldownVisuals.Count == 0) return;

        foreach (Image cooldownVisual in cooldownVisuals)
        {
            if (elapsedTime >= cooldown)
            {
                cooldownVisual.fillAmount = 0;
            }
            else
            {
                cooldownVisual.fillAmount = 1 - elapsedTime / cooldown;
            }
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
        spellBook.GetComponent<SpellBookHandler>().LearnSpell(spellID);
    }

    public void ToggleSpellBook()
    {
        spellBook.GetComponent<SpellBookHandler>().ToggleSpellBook();
    }

    public void UpdatedSelectedSpellSlot(TMP_Dropdown dropDown)
    {
        SpellBookHandler spellBookHandler = spellBook.GetComponent<SpellBookHandler>();

        int slot = int.Parse(dropDown.name);
        int value = spellBookHandler.GetSpellDropdownOption(slot, dropDown.value);

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity playerEntity = entityManager.CreateEntityQuery(typeof(PlayerComponent)).GetSingletonEntity();

        DynamicBuffer<SelectedSpellsComponent> selectedSpellsBuffer = entityManager.GetBuffer<SelectedSpellsComponent>(playerEntity);
        int slot1 = slot == 1 ? value : selectedSpells[0];
        int slot2 = slot == 2 ? value : selectedSpells[1];
        int slot3 = slot == 3 ? value : selectedSpells[2];
        int slot4 = slot == 4 ? value : selectedSpells[3];

        selectedSpellsBuffer.Clear();
        selectedSpellsBuffer.Add(new SelectedSpellsComponent { SpellID = slot1 });
        selectedSpellsBuffer.Add(new SelectedSpellsComponent { SpellID = slot2 });
        selectedSpellsBuffer.Add(new SelectedSpellsComponent { SpellID = slot3 });
        selectedSpellsBuffer.Add(new SelectedSpellsComponent { SpellID = slot4 });
    }

    public void ToggleMenu()
    {
        if (levelUpCardPickerContainer.activeSelf) return;

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

    public void ShowMiddleText(int type)
    {
        if (type == 0)
        {
            LanguageManager.Instance.UpdateLocalizeStringEvent(MiddleTextWarnTitle, AvailableLocalizationTables.Lore, "POI_ENDLESS_SIEGE_TITLE");
            LanguageManager.Instance.UpdateLocalizeStringEvent(MiddleTextWarnText, AvailableLocalizationTables.Lore, "POI_ENDLESS_SIEGE_DESCRIPTION");
        }

        if (type == 1)
        {
            LanguageManager.Instance.UpdateLocalizeStringEvent(MiddleTextWarnTitle, AvailableLocalizationTables.Lore, "POI_FURY_TITLE");
            LanguageManager.Instance.UpdateLocalizeStringEvent(MiddleTextWarnText, AvailableLocalizationTables.Lore, "POI_FURY_DESCRIPTION");
        }

        StartCoroutine(ActivateShowMiddleText());
    }

    public void ShowAchievement(int achievementID)
    {
        LanguageManager.Instance.UpdateLocalizeStringEvent(MiddleTextWarnTitle, AvailableLocalizationTables.Achievements, "ACHIEVEMENT_" + achievementID + "_NAME");
        LanguageManager.Instance.UpdateLocalizeStringEvent(MiddleTextWarnText, AvailableLocalizationTables.Achievements, "ACHIEVEMENT_" + achievementID + "_DESCRIPTION");

        AchievementsManager.Instance.UnlockAchievement(achievementID);
        StartCoroutine(ActivateShowMiddleText());
    }

    public void ShowLore(WhoSpeaksLores whoSpeaks, int lore)
    {
        LoreManager loreManager = LoreDisplay.GetComponent<LoreManager>();

        loreManager.ShowLore(whoSpeaks, lore);
    }

    private IEnumerator ActivateShowMiddleText()
    {
        MiddleTextWarn.SetActive(true);

        yield return new WaitForSeconds(5);

        MiddleTextWarn.SetActive(false);
    }

    public void ToggleInteractImage(bool status)
    {
        InteractContainer.SetActive(status);

        InteractText.text = UserInputManager.Instance.GetKeyMap("Interact");
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        UpgradeDataCollection upgrades = JsonUtility.FromJson<UpgradeDataCollection>(UpgradeJSON.text);
        allUpgrades = new List<UpgradeData>(upgrades.Upgrades);
    }

    private void Start()
    {
    }
}
