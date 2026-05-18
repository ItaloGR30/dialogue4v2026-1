using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public enum ControlMode { Force, Velocity }

    [Header("Movement")]
    public ControlMode controlMode = ControlMode.Force;
    [Tooltip("Movement strength when using Force mode, or target speed when using Velocity mode.")]
    public float moveSpeed = 5f;
    [Tooltip("Maximum horizontal speed applied when using Force mode.")]
    public float maxVelocity = 7f;
    [Tooltip("When true, movement is relative to the main camera's forward/right.")]
    public bool useCameraRelative = true;
    [Tooltip("Allow or block player movement at runtime.")]
    public bool allowMovement = true;

    Rigidbody rb;
    PlayerInput playerInput;
    InputAction moveAction;
    Vector2 moveInput = Vector2.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null && playerInput.actions != null)
        {
            moveAction = playerInput.actions.FindAction("Move", true);
        }

        if (moveAction != null)
        {
            moveAction.performed += OnMovePerformed;
            moveAction.canceled += OnMoveCanceled;
        }
        else
        {
            Debug.LogWarning("PlayerController: 'Move' action not found on PlayerInput.actions. Make sure PlayerInput is using the InputSystem_Actions asset and the Player map.");
        }
    }

    void OnDestroy()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMovePerformed;
            moveAction.canceled -= OnMoveCanceled;
        }
    }

    void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (!allowMovement || rb == null)
            return;

        Vector3 desiredMove = new Vector3(moveInput.x, 0f, moveInput.y);

        if (useCameraRelative && Camera.main != null)
        {
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0f;
            camRight.Normalize();
            desiredMove = camRight * moveInput.x + camForward * moveInput.y;
        }

        if (desiredMove.sqrMagnitude > 1f)
            desiredMove.Normalize();

        if (controlMode == ControlMode.Force)
        {
            rb.AddForce(desiredMove * moveSpeed, ForceMode.Force);

            // Clamp horizontal velocity
            Vector3 v = rb.linearVelocity;
            Vector3 horizontal = new Vector3(v.x, 0f, v.z);
            float hMag = horizontal.magnitude;
            if (hMag > maxVelocity)
            {
                horizontal = horizontal.normalized * maxVelocity;
                rb.linearVelocity = new Vector3(horizontal.x, v.y, horizontal.z);
            }
        }
        else // Velocity mode: directly set horizontal velocity (keeps vertical velocity intact)
        {
            Vector3 target = desiredMove * moveSpeed;
            Vector3 current = rb.linearVelocity;
            rb.linearVelocity = new Vector3(target.x, current.y, target.z);
        }
    }

    // Public API
    public void EnableMovement() => allowMovement = true;
    public void DisableMovement()
    {
        allowMovement = false;
        if (rb != null)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    public void ResetVelocity()
    {
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        if (moveSpeed < 0f) moveSpeed = 0f;
        if (maxVelocity < 0f) maxVelocity = 0f;
    }
    #endif

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            InteractOM.Interact();
        }
    }
}

