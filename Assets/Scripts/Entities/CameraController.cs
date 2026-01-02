using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.05f;

    [Header("Zoom Settings")]
    [SerializeField] private Vector2 zoomLimits = new(2f, 20f);
    [SerializeField] private float zoomSpeed = 20f;

    private Camera cam;
    private Mouse mouse;
    private float targetZoom;

    private void Awake()
    {
        TryGetComponent(out cam);
        mouse = Mouse.current;

        if (cam != null) { targetZoom = cam.orthographicSize; }
    }

    private void LateUpdate()
    {
        if (target == null) { return; }

        HandleZoom();
        HandlePosition();
    }

    private void HandleZoom()
    {
        if (mouse == null) { mouse = Mouse.current; }
        if (mouse == null) { return; }

        float scrollInput = mouse.scroll.ReadValue().y;

        if (scrollInput != 0)
        {
            targetZoom -= scrollInput * zoomSpeed * 0.01f;
            targetZoom = Mathf.Clamp(targetZoom, zoomLimits.x, zoomLimits.y);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, smoothSpeed);
    }

    private void HandlePosition()
    {
        MyVector2 currentPos = new(transform.position.x, transform.position.y);
        MyVector2 targetPos = new(target.position.x, target.position.y);

        MyVector2 smoothedPosition = MyVector2.Lerp(currentPos, targetPos, smoothSpeed);

        transform.position = new(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}