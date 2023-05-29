using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class PocketRadialSlot : MonoBehaviour
{
	public Pocket pocket;
    public Color hoverColor;
    public Color baseColor;
    public Color selectedColor;
    public SpriteRenderer pocketSpriteRenderer;
	public UiAnimator pocketImage;
	public UiSpriteAnimationData emptyAnimation;
	public bool playerIsUsing;
	private Image background;
	public GameObject pocketClonePrefab;
	public Material defaultMaterial;

	private Image _pocketMenuImage;

	private float _defaultSpeed;

	private void Awake()
	{
		background = GetComponent<Image>();
		background.color = baseColor;
		pocketClonePrefab.GetComponent<Animator>().keepAnimatorControllerStateOnDisable = true;
		_defaultSpeed = pocketClonePrefab.GetComponent<Animator>().speed;
	}

	public void Setup(Pocket pocket)
	{
		defaultMaterial = pocket.GetComponent<SpriteRenderer>().material;
		if (_pocketMenuImage == null)
			_pocketMenuImage = transform.GetChild(0).GetComponentInChildren<Image>();
		
		this.pocket = pocket;
		pocketClonePrefab.GetComponent<SpriteRenderer>().material = defaultMaterial;
		if (pocket.pocketType == PetType.Egg)
		{
			pocketClonePrefab.GetComponent<SpriteLibrary>().spriteLibraryAsset = pocket.GetComponent<SpriteLibrary>().spriteLibraryAsset;
			pocketClonePrefab.GetComponent<Animator>().SetInteger("state", 0);
			pocketImage.gameObject.SetActive(false);
		}
		else if (pocket.pocketType == PetType.Default)
		{
			pocketClonePrefab.GetComponent<Animator>().SetInteger("state", 1);
			pocketClonePrefab.GetComponent<SpriteLibrary>().spriteLibraryAsset = pocket.GetComponent<SpriteLibrary>().spriteLibraryAsset;
			pocketClonePrefab.GetComponent<SpriteRenderer>().flipY = pocket.flipY;
			pocketImage.gameObject.SetActive(false);
		}

		if (pocket.GetHealth().GetCurrentHealth() > 0f)
        {
			pocketClonePrefab.GetComponent<SpriteRenderer>().material = defaultMaterial;
			pocketClonePrefab.GetComponent<Animator>().enabled = true;
			pocketClonePrefab.GetComponent<Animator>().speed = _defaultSpeed;
		} else {
			pocketClonePrefab.GetComponent<SpriteRenderer>().material = pocket.greyscaleMaterial;
			//pocketClonePrefab.GetComponent<Animator>().enabled = false;
			pocketClonePrefab.GetComponent<Animator>().speed = 0f;
		}

		playerIsUsing = pocket.IsActive();
		background.color = playerIsUsing ? selectedColor : baseColor;
	}

	public void SetEmpty()
	{
		pocket = null;
		pocketImage.animationData = emptyAnimation;
	}

	public void Select()
	{
		background.color = hoverColor;
	}

	public void Deselect()
	{
		if (playerIsUsing)
		{
			background.color = selectedColor;
		}
		else
		{
			background.color = baseColor;
		}
	}
}
