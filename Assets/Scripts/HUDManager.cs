using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DronController dronController;
    [SerializeField] private TextMeshProUGUI diskCountText;

    [Header("Settings")]
    [SerializeField] private string diskCountFormat = "Discos: {0}";

    private void Start()
    {
        if (dronController == null)
        {
            dronController = FindFirstObjectByType<DronController>();
        }

        if (dronController != null)
        {
            dronController.onDiskCountChanged.AddListener(UpdateDiskDisplay);
            UpdateDiskDisplay(dronController.CollectedDisks);
        }

        if (diskCountText == null)
        {
            Debug.LogWarning("HUDManager: TextMeshProUGUI no asignado en el inspector.");
        }
    }

    private void UpdateDiskDisplay(int diskCount)
    {
        if (diskCountText != null)
        {
            diskCountText.text = string.Format(diskCountFormat, diskCount);
        }
    }

    private void OnDestroy()
    {
        if (dronController != null)
        {
            dronController.onDiskCountChanged.RemoveListener(UpdateDiskDisplay);
        }
    }
}
