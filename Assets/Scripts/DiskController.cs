using UnityEngine;
using UnityEngine.Events;

public class DiskController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isCCDButton = true;
    [SerializeField] private AudioSource pickUpSfx;
    [Header("Events")]
    public UnityEvent onCollected; // Link score increment in UI or GameManager here

    private bool armInside;
    private DronController dronInside;
    private bool collected;

    private void Update()
    {
        if (collected) return;
        if (armInside && Input.GetMouseButtonDown(0)) { Collect(); }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CCDEnd") && isCCDButton) // CCD
        {
            armInside = true;
            dronInside = collision.GetComponentInParent<DronController>();
        }
        else if (collision.CompareTag("FABRIKEnd") && !isCCDButton) // FABRIK
        {
            armInside = true;
            dronInside = collision.GetComponentInParent<DronController>();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CCDEnd") && isCCDButton) // CCD
        {
            armInside = false;
            dronInside = null;
        }
        else if (collision.CompareTag("FABRIKEnd") && !isCCDButton) // FABRIK
        {
            armInside = false;
            dronInside = null;
        }
    }

    private void Collect()
    {
        if (collected)
            return;

        collected = true;
        dronInside?.AddDisk();
        onCollected?.Invoke();

        if (pickUpSfx != null)
        {
            pickUpSfx.Play();
            Destroy(gameObject, pickUpSfx.clip.length); // Deja sonar el audio antes de destruir
        }
        else { Destroy(gameObject); }
    }
}
