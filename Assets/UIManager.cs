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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
