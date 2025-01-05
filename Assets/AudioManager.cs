using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private GameObject HealSound;
    [SerializeField]
    private GameObject ClawSound;
    [SerializeField]
    private GameObject StepGrassSound;
    [SerializeField]
    private GameObject FireSpellSound;
    [SerializeField]
    private GameObject StepRockSound;
    [SerializeField]
    private GameObject BuffSound;
    [SerializeField]
    private GameObject WindSpellSound;
    [SerializeField]
    private GameObject PauseSound;
    [SerializeField]
    private GameObject UnPauseSound;
    [SerializeField]
    private GameObject DeathSound;
    [SerializeField]
    private GameObject UpgradeEffect;
    [SerializeField]
    private GameObject HoverSound;
    [SerializeField]
    private GameObject ConfirmSound;

    [SerializeField]
    private AudioMixer AudioMixer;

    [SerializeField]
    private AudioSource BGM_InGame;
    [SerializeField]
    private AudioSource BGM_MainMenu;
    [SerializeField]
    private AudioSource BGM_DreamCity;

    public static AudioManager Instance { get; private set; }

    private bool initialized;

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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            if (PlayerPrefsManager.Instance != null)
            {
                SetVolumeToChannel("SpellsVolume", PlayerPrefsManager.Instance.GetSpellsVolume());
                SetVolumeToChannel("MusicVolume", PlayerPrefsManager.Instance.GetMusicVolume());
                SetVolumeToChannel("SFXVolume", PlayerPrefsManager.Instance.GetSFXVolume());
                initialized = true;
            }
        }
    }

    public void SetVolumeToChannel(string channel, float volume)
    {
        AudioMixer.SetFloat(channel, Mathf.Log10(volume) * 20);
    }

    public void PlayBGM(string scene)
    {
        BGM_InGame.Stop();
        BGM_MainMenu.Stop();
        BGM_DreamCity.Stop();

        if (scene == "MainScene")
        {
            BGM_InGame.Play();
        }

        if (scene == "MainMenuScene")
        {
            BGM_MainMenu.Play();
        }

        if (scene == "DreamCityScene")
        {
            BGM_DreamCity.Play();
        }
    }

    public void PlayAudio(AudioType audioType)
    {
        switch (audioType)
        {
            case AudioType.Heal:
                GetAudioSource(HealSound).Play();
                break;
            case AudioType.Claw:
                GetAudioSource(ClawSound).Play();
                break;
            case AudioType.StepGrass:
                GetAudioSource(StepGrassSound).Play();
                break;
            case AudioType.FireSpell:
                GetAudioSource(FireSpellSound).Play();
                break;
            case AudioType.StepRock:
                GetAudioSource(StepRockSound).Play();
                break;
            case AudioType.Buff:
                GetAudioSource(BuffSound).Play();
                break;
            case AudioType.WindSpell:
                GetAudioSource(WindSpellSound).Play();
                break;
            case AudioType.Pause:
                GetAudioSource(PauseSound).Play();
                break;
            case AudioType.UnPause:
                GetAudioSource(UnPauseSound).Play();
                break;
            case AudioType.Death:
                GetAudioSource(DeathSound).Play();
                break;
            case AudioType.UpgradeEffect:
                GetAudioSource(UpgradeEffect).Play();
                break;
            case AudioType.Hover:
                GetAudioSource(HoverSound).Play();
                break;
            case AudioType.Confirm:
                GetAudioSource(ConfirmSound).Play();
                break;
        }
    }

    private AudioSource GetAudioSource(GameObject audioObject)
    {
        return audioObject.GetComponent<AudioSource>();
    }
}

public enum AudioType
{
    Heal,
    Claw,
    StepGrass,
    FireSpell,
    StepRock,
    Buff,
    WindSpell,
    Pause,
    UnPause,
    Death,
    UpgradeEffect,
    Hover,
    Confirm
}
