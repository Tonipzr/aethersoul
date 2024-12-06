using UnityEngine;

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

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
    Death
}
