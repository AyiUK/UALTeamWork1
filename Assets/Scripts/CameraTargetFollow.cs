using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private Camera aimCamera;
    [SerializeField, Min(0.01f)] private float turnSmoothTime = 0.25f;
    [SerializeField, Min(1f)] private float maxTurnSpeed = 90f;
    [SerializeField, Range(0f, 89f)] private float maxYawFromPlayer = 89f;

    private float targetYawOffset;
    private float yawSmoothVelocity;
    private bool hasInitializedYaw;

    private void Awake()
    {
        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 followPosition = player.position + offset;
        Vector3 playerForward = Vector3.ProjectOnPlane(player.forward, Vector3.up);
        if (playerForward.sqrMagnitude < 0.0001f) return;
        playerForward.Normalize();

        if (!hasInitializedYaw)
        {
            Vector3 initialForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            targetYawOffset = initialForward.sqrMagnitude > 0.0001f
                ? Vector3.SignedAngle(playerForward, initialForward, Vector3.up)
                : 0f;
            targetYawOffset = Mathf.Clamp(targetYawOffset, -maxYawFromPlayer, maxYawFromPlayer);
            hasInitializedYaw = true;
        }

        Mouse mouse = Mouse.current;
        if (aimCamera != null && mouse != null)
        {
            Vector2 mousePosition = mouse.position.ReadValue();
            Rect cameraRect = aimCamera.pixelRect;
            float cameraDepth = Vector3.Dot(
                followPosition - aimCamera.transform.position,
                aimCamera.transform.forward);
            if (cameraDepth > 0f && cameraRect.width > 0f && cameraRect.height > 0f)
            {
                Vector3 cursorWorld = aimCamera.ScreenToWorldPoint(
                    new Vector3(cameraRect.center.x, mousePosition.y, cameraDepth));
                Vector3 screenCenterWorld = aimCamera.ScreenToWorldPoint(
                    new Vector3(cameraRect.center.x, cameraRect.center.y, cameraDepth));
                followPosition.z += cursorWorld.z - screenCenterWorld.z;
            }

            if (mouse.delta.ReadValue().sqrMagnitude > 0f && cameraRect.width > 0f)
            {
                float viewportX = (mousePosition.x - cameraRect.x) / cameraRect.width;
                targetYawOffset = (Mathf.Clamp01(viewportX) - 0.5f) * 2f * maxYawFromPlayer;
            }
        }

        transform.position = followPosition;

        Vector3 currentForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        float currentYawOffset = currentForward.sqrMagnitude > 0.0001f
            ? Vector3.SignedAngle(playerForward, currentForward, Vector3.up)
            : targetYawOffset;
        currentYawOffset = Mathf.Clamp(currentYawOffset, -maxYawFromPlayer, maxYawFromPlayer);

        float smoothTime = Mathf.Max(0.01f, turnSmoothTime);
        float smoothedYawOffset = Mathf.SmoothDampAngle(
            currentYawOffset,
            targetYawOffset,
            ref yawSmoothVelocity,
            smoothTime,
            maxTurnSpeed,
            Time.deltaTime);
        smoothedYawOffset = Mathf.Clamp(smoothedYawOffset, -maxYawFromPlayer, maxYawFromPlayer);

        Vector3 smoothedDirection = Quaternion.AngleAxis(smoothedYawOffset, Vector3.up) * playerForward;
        transform.rotation = Quaternion.LookRotation(smoothedDirection, Vector3.up);
    }

}
