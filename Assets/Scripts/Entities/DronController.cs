using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class DronController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb2d;

    [Header("Arms")]
    [SerializeField] private CCD arm1;
    [SerializeField] private FABRIK arm2;

    [Header("Target")]
    private MyVector2 targetPosition;
    private Camera mainCamera;
    private Mouse mouse;
    private bool isTargetFixed = false;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    private Keyboard kb;
    private MyVector2 inputDirection = MyVector2.zero;

    [Header("Collectibles")]
    public UnityEvent<string> onTaskChanged;
    private int collectedDisks = 0;
    private int totalDisks = 7;

    private void Awake()
    {
        kb = Keyboard.current;
        mouse = Mouse.current;
        rb2d = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        onTaskChanged?.Invoke("Disks: " + collectedDisks.ToString() + "/" + totalDisks.ToString());
    }

    private void Update()
    {
        HandleInput();
        HandleArmToggle();
        HandleMouseTarget();
    }

    private void FixedUpdate() { HandleMovement(); }

    private void HandleInput()
    {
        if (kb == null) { kb = Keyboard.current; }
        if (kb == null) { return; }

        // get input direction
        inputDirection = MyVector2.zero;
        if (kb.wKey.isPressed) { inputDirection += MyVector2.up; }
        if (kb.sKey.isPressed) { inputDirection += MyVector2.down; }
        if (kb.aKey.isPressed) { inputDirection += MyVector2.left; }
        if (kb.dKey.isPressed) { inputDirection += MyVector2.right; }

        // normalize to prevent faster diagonal movement
        if (inputDirection.magnitude > 0) { inputDirection = inputDirection.normalized; }
    }

    private void HandleMouseTarget()
    {
        if (mouse == null) { mouse = Mouse.current; }
        if (mouse == null || mainCamera == null) { return; }

        // free target on left click
        if (mouse.leftButton.wasPressedThisFrame) { isTargetFixed = false; }

        // set target on right click
        if (mouse.rightButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPos = mouse.position.ReadValue();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new(mouseScreenPos.x, mouseScreenPos.y, 0));
            targetPosition = new(worldPos.x, worldPos.y);

            isTargetFixed = true;
        }

        // if target is not fixed, update it to mouse position
        if (!isTargetFixed)
        {
            Vector2 mouseScreenPos = mouse.position.ReadValue();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new(mouseScreenPos.x, mouseScreenPos.y, 0));
            targetPosition = new(worldPos.x, worldPos.y);
        }

        // Actualizar el target en los brazos
        if (arm1 != null && arm1.gameObject.activeSelf) { arm1.SetTarget(targetPosition); }
        if (arm2 != null && arm2.gameObject.activeSelf) { arm2.SetTarget(targetPosition); }
    }

    private void HandleMovement()
    {
        // smooth movement with velocity
        MyVector2 targetVelocity = inputDirection * movementSpeed;
        rb2d.linearVelocity = targetVelocity;
    }

    private void HandleArmToggle()
    {
        if (kb == null) { return; }

        // CCD toggle
        if (kb.digit1Key.wasPressedThisFrame)
        {
            if (arm1 != null) { arm1.gameObject.SetActive(!arm1.gameObject.activeSelf); }
        }

        // FABRIK toggle
        if (kb.digit2Key.wasPressedThisFrame)
        {
            if (arm2 != null) { arm2.gameObject.SetActive(!arm2.gameObject.activeSelf); }
        }
    }

    public void AddDisk()
    {
        collectedDisks++;
        onTaskChanged?.Invoke("Disks: " + collectedDisks.ToString() + "/" + totalDisks.ToString());
    }

    public void SetArmActiveStates(bool isCCDActive, bool isFABRIKActive)
    {
        if (arm1 != null) { arm1.gameObject.SetActive(isCCDActive); }
        if (arm2 != null) { arm2.gameObject.SetActive(isFABRIKActive); }
    }

    #region UI
    public bool IsCCDActive() => arm1 != null && arm1.gameObject.activeSelf;
    public bool IsFABRIKActive() => arm2 != null && arm2.gameObject.activeSelf;
    public string ActiveAlgorithm()
    {
        if (arm1.gameObject.activeSelf && arm2.gameObject.activeSelf) { return "CCD & FABRIK"; }
        if (arm1.gameObject.activeSelf) { return "CCD"; }
        if (arm2.gameObject.activeSelf) { return "FABRIK"; }

        return "None";
    }

    public string CCDIterationsThisFrame()
    {
        int iterations = 0;
        if (arm1.gameObject.activeSelf) { iterations += arm1.IterationsThisFrame; }
        return iterations.ToString();
    }
    public string FABRIKIterationsThisFrame()
    {
        int iterations = 0;
        if (arm2.gameObject.activeSelf) { iterations += arm2.IterationsThisFrame; }
        return iterations.ToString();
    }

    public string CCDDistanceToTarget()
    {
        if (arm1.gameObject.activeSelf)
        {
            return arm1.GetDistanceToTarget().ToString("F2");
        }
        return "-1";
    }
    public string FABRIKDistanceToTarget()
    {
        if (arm2.gameObject.activeSelf)
        {
            return arm2.GetDistanceToTarget().ToString("F2");
        }
        return "-1";
    }
    #endregion
}