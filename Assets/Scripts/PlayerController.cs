using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 180f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField, Min(0f)] private float jumpForwardSpeed = 4f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private float verticalVelocity;
    private Vector3 jumpForwardVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        bool isGrounded = characterController.isGrounded;
        Vector3 moveDirection;

        if (isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            Vector2 input = ReadMovementInput();

            // A/D rotate the player in place; they do not add sideways movement.
            transform.Rotate(0f, input.x * turnSpeed * Time.deltaTime, 0f, Space.Self);

            // W/S move along the player's current forward direction.
            moveDirection = transform.forward * input.y;
            moveDirection.y = 0f;
            moveDirection.Normalize();

            if (WasJumpPressed())
            {
                verticalVelocity = jumpForce;
                jumpForwardVelocity = transform.forward * jumpForwardSpeed;
                moveDirection = jumpForwardVelocity;
                isGrounded = false;
            }
        }
        else
        {
            // Ignore all WASD input while airborne and keep the takeoff direction.
            moveDirection = jumpForwardVelocity;
        }

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 velocity = isGrounded ? moveDirection * moveSpeed : jumpForwardVelocity;
        velocity.y = verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    private static bool WasJumpPressed()
    {
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
    }

    private static Vector2 ReadMovementInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return Vector2.zero;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.aKey.isPressed) horizontal -= 1f;
        if (keyboard.dKey.isPressed) horizontal += 1f;
        if (keyboard.sKey.isPressed) vertical -= 1f;
        if (keyboard.wKey.isPressed) vertical += 1f;

        return Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);
    }

}
