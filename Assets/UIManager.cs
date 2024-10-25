using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private Image healthFill;

    [SerializeField]
    private Slider manaBar;
    [SerializeField]
    private Image manaFIll;

    [SerializeField]
    private Slider expBar;
    [SerializeField]
    private Image expFill;

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
