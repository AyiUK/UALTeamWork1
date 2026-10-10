using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 180f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private float verticalVelocity;
    private float previousAimAngle;
    private float pendingAimAngle;
    private bool hasPreviousAimAngle;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {
        Keyboard keyboard = Keyboard.current;
        Vector2 input = ReadMovementInput(keyboard);
        bool turnAround = keyboard != null && keyboard.sKey.wasPressedThisFrame;

        if (cameraTarget != null)
        {
            // Use changes in the target's angle relative to the player. This avoids
            // chasing a world-space heading that can move when the player turns.
            Vector3 localAimDirection = transform.InverseTransformDirection(cameraTarget.forward);
            localAimDirection.y = 0f;
            if (localAimDirection.sqrMagnitude > 0.0001f)
            {
                float currentAimAngle = Mathf.Atan2(localAimDirection.x, localAimDirection.z) * Mathf.Rad2Deg;
                if (hasPreviousAimAngle)
                {
                    pendingAimAngle += Mathf.DeltaAngle(previousAimAngle, currentAimAngle);
                    float maxTurnThisFrame = turnSpeed * Time.deltaTime;
                    float turnStep = Mathf.Clamp(pendingAimAngle, -maxTurnThisFrame, maxTurnThisFrame);
                    transform.Rotate(0f, turnStep, 0f, Space.Self);
                    pendingAimAngle -= turnStep;
                }

                previousAimAngle = currentAimAngle;
                hasPreviousAimAngle = true;
            }
        }

        if (turnAround)
        {
            transform.Rotate(0f, 180f, 0f, Space.Self);
            pendingAimAngle = 0f;
            hasPreviousAimAngle = false;
        }

        float strafeTurnInput = 0f;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed) strafeTurnInput -= 1f;
            if (keyboard.dKey.isPressed) strafeTurnInput += 1f;
        }
        transform.Rotate(0f, strafeTurnInput * turnSpeed * Time.deltaTime, 0f, Space.Self);

        // W moves forward; A/D strafe and turn; S turns around.
        Vector3 moveDirection = transform.forward * input.y + transform.right * input.x;
        moveDirection.y = 0f;
        moveDirection.Normalize();

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    private static Vector2 ReadMovementInput(Keyboard keyboard)
    {
        if (keyboard == null)
        {
            return Vector2.zero;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.aKey.isPressed) horizontal -= 1f;
        if (keyboard.dKey.isPressed) horizontal += 1f;
        if (keyboard.wKey.isPressed) vertical += 1f;

        return Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);
    }

}
