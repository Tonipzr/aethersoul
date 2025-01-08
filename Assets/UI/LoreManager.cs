using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoreManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Container;
    [SerializeField]
    private GameObject ImageBoss;
    [SerializeField]
    private GameObject ImagePlayer;
    [SerializeField]
    private TMP_Text TextLore;

    private List<int> shownLore = new List<int>();

    // Start is called before the first frame update
    void Awake()
    {
        Container.SetActive(false);
        shownLore = new List<int>();
    }

    public void ShowLore(WhoSpeaksLores whoSpeaks, int lore)
    {
        if (shownLore.Contains(lore)) return;

        LanguageManager.Instance.UpdateLocalizeStringEvent(TextLore.gameObject, AvailableLocalizationTables.Lore, "LORE_MESSAGE_" + lore);

        switch (whoSpeaks)
        {
            case WhoSpeaksLores.Boss:
                ImageBoss.SetActive(true);
                ImagePlayer.SetActive(false);
                break;
            case WhoSpeaksLores.Player:
                ImageBoss.SetActive(false);
                ImagePlayer.SetActive(true);
                break;
        }

        shownLore.Add(lore);

        StartCoroutine(ActivateShowLore());
    }

    private IEnumerator ActivateShowLore()
    {
        Container.SetActive(true);

        yield return new WaitForSeconds(10);

        Container.SetActive(false);
    }
}

public enum WhoSpeaksLores
{
    Boss,
    Player
}