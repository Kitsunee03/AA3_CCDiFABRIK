using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class DronController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb2d;

    [Header("Arms")]
    [SerializeField] private GameObject arm1;
    [SerializeField] private GameObject arm2;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    private Keyboard kb;

    private void Awake()
    {
        kb = Keyboard.current;
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovement();
        HandleArmToggle();
    }

    private void HandleMovement()
    {
        if (kb == null) { kb = Keyboard.current; }
        if (kb == null) { return; }

        // get input direction
        MyVector2 inputDirection = MyVector2.zero;
        if (kb.wKey.isPressed) { inputDirection += MyVector2.up; }
        if (kb.sKey.isPressed) { inputDirection += MyVector2.down; }
        if (kb.aKey.isPressed) { inputDirection += MyVector2.left; }
        if (kb.dKey.isPressed) { inputDirection += MyVector2.right; }

        // normalize to prevent faster diagonal movement
        if (inputDirection.magnitude > 0) { inputDirection = inputDirection.normalized; }

        // calculate new position
        MyVector2 currentPosition = new(transform.position.x, transform.position.y);
        MyVector2 movement = inputDirection * movementSpeed * Time.deltaTime;
        MyVector2 newPosition = currentPosition + movement;

        // apply new position
        transform.position = newPosition;
    }

    private void HandleArmToggle()
    {
        if (kb == null) { return; }

        // CCD toggle
        if (kb.digit1Key.wasPressedThisFrame)
        {
            if (arm1 != null) { arm1.SetActive(!arm1.activeSelf); }
        }

        // FABRIK toggle
        if (kb.digit2Key.wasPressedThisFrame)
        {
            if (arm2 != null) { arm2.SetActive(!arm2.activeSelf); }
        }
    }

    #region Accessors
    public MyVector2 Position { get { return new MyVector2(transform.position.x, transform.position.y); } }
    public Rigidbody2D Rb2D { get { return rb2d; } }
    #endregion
}