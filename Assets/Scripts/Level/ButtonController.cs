using UnityEngine;

public class ButtonController : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private bool isCCDButton = true;
	[SerializeField] private AudioClip clickSfx;
	private GameObject laserToDestroy;
	private bool canInteract = true;

	private bool armInside;

    private void Awake()
    {
        laserToDestroy = transform.GetChild(0)?.gameObject;
    }

    private void Update()
	{
		if (armInside && Input.GetMouseButtonDown(0)) { OnButtonClicked(); }
	}

	void OnTriggerEnter2D(Collider2D p_collision)
	{
		if (!canInteract) { return; }

		if ((p_collision.CompareTag("CCDEnd") && isCCDButton) || (p_collision.CompareTag("FABRIKEnd") && !isCCDButton))
		{
			armInside = true;
		}
	}

	void OnTriggerExit2D(Collider2D p_collision)
	{
		if ((p_collision.CompareTag("CCDEnd") && isCCDButton) || (p_collision.CompareTag("FABRIKEnd") && !isCCDButton))
		{
			armInside = false;
		}
	}

	private void OnButtonClicked()
	{
		if (laserToDestroy != null)
		{
			Destroy(laserToDestroy);
			GameManager.Instance?.PlaySFX(clickSfx);
			canInteract = false;
		}
	}
}