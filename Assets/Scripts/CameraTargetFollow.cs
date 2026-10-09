using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private Camera aimCamera;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField, Range(0f, 89f)] private float maxYawFromPlayer = 89f;
    [SerializeField, Tooltip("Width and height of the mouse turn area as a fraction of the camera view.")]
    private Vector2 aimAreaSize = new Vector2(0.6f, 0.6f);

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
        transform.position = followPosition;
        ClampCurrentYawToPlayer();

        if (aimCamera == null) return;

        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mousePosition = mouse.position.ReadValue();
        float cameraDepth = Vector3.Dot(followPosition - aimCamera.transform.position, aimCamera.transform.forward);
        if (cameraDepth > 0f)
        {
            Vector3 cursorWorld = aimCamera.ScreenToWorldPoint(
                new Vector3(aimCamera.pixelWidth * 0.5f, mousePosition.y, cameraDepth));
            Vector3 screenCenterWorld = aimCamera.ScreenToWorldPoint(
                new Vector3(aimCamera.pixelWidth * 0.5f, aimCamera.pixelHeight * 0.5f, cameraDepth));
            followPosition.z += cursorWorld.z - screenCenterWorld.z;
            transform.position = followPosition;
        }

        if (mouse.delta.ReadValue().sqrMagnitude <= 0f) return;
        if (!IsMouseInsideAimArea(mousePosition)) return;

        Ray mouseRay = aimCamera.ScreenPointToRay(mousePosition);
        Plane aimPlane = new Plane(Vector3.up, transform.position);
        if (!aimPlane.Raycast(mouseRay, out float distance)) return;

        Vector3 direction = Vector3.ProjectOnPlane(
            mouseRay.GetPoint(distance) - transform.position,
            Vector3.up);
        if (direction.sqrMagnitude < 0.0001f) return;

        Vector3 limitedDirection = ClampYawToPlayer(direction);
        if (limitedDirection.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(limitedDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime);
    }

    private bool IsMouseInsideAimArea(Vector2 screenPosition)
    {
        Rect cameraRect = aimCamera.pixelRect;
        if (cameraRect.width <= 0f || cameraRect.height <= 0f) return false;

        Vector2 viewportPosition = new Vector2(
            (screenPosition.x - cameraRect.x) / cameraRect.width,
            (screenPosition.y - cameraRect.y) / cameraRect.height);

        float halfWidth = Mathf.Clamp01(aimAreaSize.x) * 0.5f;
        float halfHeight = Mathf.Clamp01(aimAreaSize.y) * 0.5f;
        return Mathf.Abs(viewportPosition.x - 0.5f) <= halfWidth
            && Mathf.Abs(viewportPosition.y - 0.5f) <= halfHeight;
    }

    private void ClampCurrentYawToPlayer()
    {
        Vector3 targetForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        Vector3 limitedDirection = ClampYawToPlayer(targetForward);
        if (limitedDirection.sqrMagnitude < 0.0001f
            || Vector3.Angle(targetForward, limitedDirection) < 0.01f) return;

        transform.rotation = Quaternion.LookRotation(limitedDirection, Vector3.up);
    }

    private Vector3 ClampYawToPlayer(Vector3 direction)
    {
        Vector3 playerForward = Vector3.ProjectOnPlane(player.forward, Vector3.up);
        if (playerForward.sqrMagnitude < 0.0001f || direction.sqrMagnitude < 0.0001f)
        {
            return Vector3.zero;
        }

        playerForward.Normalize();
        direction.Normalize();
        float yawFromPlayer = Vector3.SignedAngle(playerForward, direction, Vector3.up);
        yawFromPlayer = Mathf.Clamp(yawFromPlayer, -maxYawFromPlayer, maxYawFromPlayer);
        return Quaternion.AngleAxis(yawFromPlayer, Vector3.up) * playerForward;
    }
}
