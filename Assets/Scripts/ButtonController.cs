using UnityEngine;

public class ButtonController : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private bool isCCDButton = true;
	[SerializeField] private GameObject laserToDestroy;
	[SerializeField] private AudioSource clickSfx;

	private bool armInside;

	private void Update()
	{
		if (armInside && Input.GetMouseButtonDown(0)) { OnButtonClicked(); }
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if ((collision.CompareTag("CCDEnd") && isCCDButton) ||
			(collision.CompareTag("FABRIKEnd") && !isCCDButton))
		{
			armInside = true;
		}
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if ((collision.CompareTag("CCDEnd") && isCCDButton) ||
			(collision.CompareTag("FABRIKEnd") && !isCCDButton))
		{ armInside = false; }
	}

	private void OnButtonClicked()
	{
		if (laserToDestroy != null)
		{
			Destroy(laserToDestroy);
		}
		if (clickSfx != null) { clickSfx.Play(); }
		// Opcional: destruir el botón tras pulsar
		// Destroy(gameObject);
	}
}
