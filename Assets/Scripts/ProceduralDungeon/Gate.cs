using UnityEngine;
using UnityEngine.U2D.Animation;

public class Gate : MonoBehaviour
{
    public Room room;
    AudioSource audioSource;
    [SerializeField]
    AudioClip openingSound;
    [SerializeField]
    AudioClip closeSound;

    Animator animator;

    bool isOpen = false;
    public bool isLocked;

    RoomType gateType;

    [SerializeField]
    SpriteLibraryAsset defaultGate;
    [SerializeField]
    SpriteLibraryAsset bossGate;
    [SerializeField]
    SpriteLibraryAsset npcGate;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (PauseManager.Instance.IsGamePaused())
        {
            if (audioSource.isPlaying) audioSource.Pause();
        }
        else
        {
            if (!audioSource.isPlaying) audioSource.UnPause();
        }
    }

    public void Open()
    {
        if (IsOpen() || isLocked)
        {
            return;
        }

        isOpen = true;
        animator.SetTrigger("open");

        PlayFX();
    }

    public void Close(bool lockGate = false)
    {
        if (!IsOpen())
        {
            return;
        }

        PlayFX();

        isOpen = false;
        isLocked = lockGate;
        animator.SetTrigger("close");
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public void PlayFX()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(openingSound);
        }  
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    public void SetRelativeOffset(RelativePosition relativePosition)
    {
        switch (relativePosition)
        {
            case RelativePosition.Right:
                transform.position -= new Vector3(-.3f, 0, 0);
                return;
            case RelativePosition.Down:
                transform.position -= new Vector3(0, -.7f, 0);
                return;
            case RelativePosition.Left:
                transform.position -= new Vector3(.3f, 0, 0);
                return;
            default:
                transform.position -= new Vector3(0, -.3f, 0);
                return;
        }
    }

    public void SetGateType(RoomType type)
    {
        if (GameplayManager.Instance.currentDungeonType == DungeonType.Forest)
        {
            animator.speed = 0.8f;
        } else
        {
            animator.speed = 1;
        }
        if (type == RoomType.Boss)
        {
            SpriteLibrary spriteLibrary = GetComponent<SpriteLibrary>();
            spriteLibrary.spriteLibraryAsset = bossGate;
            gameObject.transform.Find("Icon").GetComponent<SpriteRenderer>().color = Color.white;
        }
        if (type == RoomType.NPC)
        {
            SpriteLibrary spriteLibrary = GetComponent<SpriteLibrary>();
            spriteLibrary.spriteLibraryAsset = npcGate;
            gameObject.transform.Find("Icon").GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        gateType = type;
    }
}
