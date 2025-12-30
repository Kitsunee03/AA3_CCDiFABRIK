using UnityEngine;
using UnityEngine.Events;

public class DiskController : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string armEndTag = "ArmEnd"; // Tag del extremo del brazo que puede recoger discos
    [SerializeField] private AudioSource pickUpSfx; // Opcional: sonido de recogida
    [Header("Eventos")]
    public UnityEvent onCollected; // Enlazar aquí la suma en UI o GameManager

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
        if (collision.CompareTag(armEndTag))
        {
            armInside = true;
            dronInside = collision.GetComponentInParent<DronController>();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(armEndTag))
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
