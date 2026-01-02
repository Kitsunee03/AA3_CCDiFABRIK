using UnityEngine;
using UnityEngine.Events;

public class DiskController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isCCDButton = true;
    [SerializeField] private AudioClip pickUpSfx;

    [HideInInspector] public UnityEvent onCollected;

    private bool armInside;
    private DronController dronInside;
    private bool collected;

    private void Update()
    {
        if (collected) return;
        if (armInside && Input.GetMouseButtonDown(0)) { Collect(); }
    }

    void OnTriggerEnter2D(Collider2D p_collision)
    {
        if (p_collision.CompareTag("CCDEnd") && isCCDButton) // CCD
        {
            armInside = true;
            dronInside = p_collision.GetComponentInParent<DronController>();
        }
        else if (p_collision.CompareTag("FABRIKEnd") && !isCCDButton) // FABRIK
        {
            armInside = true;
            dronInside = p_collision.GetComponentInParent<DronController>();
        }
    }

    void OnTriggerExit2D(Collider2D p_collision)
    {
        if (p_collision.CompareTag("CCDEnd") && isCCDButton) // CCD
        {
            armInside = false;
            dronInside = null;
        }
        else if (p_collision.CompareTag("FABRIKEnd") && !isCCDButton) // FABRIK
        {
            armInside = false;
            dronInside = null;
        }
    }

    private void Collect()
    {
        if (collected) { return; }

        collected = true;
        dronInside?.AddDisk();
        onCollected?.Invoke();

        GameManager.Instance?.PlaySFX(pickUpSfx);
        Destroy(gameObject);
    }
}