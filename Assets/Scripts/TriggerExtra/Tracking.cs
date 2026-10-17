using UnityEngine;

public class Tracking : MonoBehaviour
{
    [SerializeField] private Transform cylinder;
    [SerializeField, Min(0f)] private float surfaceOffset = 0.05f;
    [SerializeField] private float heightOffset;
    [SerializeField] private float startAngle;
    [SerializeField, Min(0f)] private float perimeterSpeed = 1f;
    [SerializeField, Range(0f, 1f)] private float travelPerimeterFraction = 1f / 3f;

    private float elapsedTime;
    private Renderer cylinderRenderer;
    [SerializeField] private Light trackLight;

    private void Start()
    {
        if (cylinder != null)
        {
            cylinderRenderer = cylinder.GetComponentInChildren<Renderer>();
        }

        if (trackLight == null)
        {
            trackLight = GetComponentInChildren<Light>(true);
        }

        if (trackLight != null)
        {
            trackLight.color = Color.white;
        }

        if (cylinder == null)
        {
            Debug.LogError("Tracking: Assign the cylinder Transform in the Inspector.", this);
            return;
        }

        elapsedTime = 0f;
        UpdatePosition();
    }

    public void OnTracking()
    {
        if (cylinder == null)
        {
            Debug.LogError("Tracking: Assign the cylinder Transform in the Inspector.", this);
            return;
        }

        if (trackLight != null)
        {
            trackLight.color = Color.red;
        }
        
    }

    private void Update()
    {
        if (cylinder == null)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float radiusX = cylinderRenderer != null
            ? cylinderRenderer.bounds.extents.x
            : Mathf.Abs(cylinder.lossyScale.x) * 0.5f;
        float radiusZ = cylinderRenderer != null
            ? cylinderRenderer.bounds.extents.z
            : Mathf.Abs(cylinder.lossyScale.z) * 0.5f;
        Vector3 center = cylinderRenderer != null
            ? cylinderRenderer.bounds.center
            : cylinder.position;

        radiusX = Mathf.Max(0.001f, radiusX + surfaceOffset);
        radiusZ = Mathf.Max(0.001f, radiusZ + surfaceOffset);

        // Follow the configured fraction of the horizontal XZ perimeter, then retrace it.
        // When X and Z radii differ, the cross-section is an ellipse.
        float perimeter = GetEllipseArcLength(0f, 360f, radiusX, radiusZ);
        float travelDistance = perimeter * Mathf.Clamp01(travelPerimeterFraction);
        float travelledDistance = travelDistance > 0f
            ? Mathf.PingPong(elapsedTime * perimeterSpeed, travelDistance)
            : 0f;
        float sweepDegrees = GetSweepForDistance(startAngle, travelledDistance, radiusX, radiusZ);
        float angle = (startAngle - sweepDegrees) * Mathf.Deg2Rad;

        Vector3 position = new Vector3(
            center.x + Mathf.Cos(angle) * radiusX,
            center.y + heightOffset,
            center.z + Mathf.Sin(angle) * radiusZ);

        // The normal of an ellipse is proportional to (cos(a)/rx, sin(a)/rz).
        Vector3 outwardNormal = new Vector3(
            Mathf.Cos(angle) / Mathf.Max(radiusX, 0.001f),
            0f,
            Mathf.Sin(angle) / Mathf.Max(radiusZ, 0.001f)).normalized;

        transform.position = position;
        transform.rotation = Quaternion.LookRotation(outwardNormal, Vector3.up);
    }

    private static float GetSweepForDistance(float startAngle, float distance, float radiusX, float radiusZ)
    {
        float lower = 0f;
        float upper = 360f;

        for (int i = 0; i < 18; i++)
        {
            float middle = (lower + upper) * 0.5f;
            if (GetEllipseArcLength(startAngle, middle, radiusX, radiusZ) < distance)
            {
                lower = middle;
            }
            else
            {
                upper = middle;
            }
        }

        return (lower + upper) * 0.5f;
    }

    private static float GetEllipseArcLength(float startAngle, float sweepDegrees, float radiusX, float radiusZ)
    {
        int steps = Mathf.Max(1, Mathf.CeilToInt(sweepDegrees));
        float stepDegrees = sweepDegrees / steps;
        float previousAngle = startAngle * Mathf.Deg2Rad;
        Vector2 previousPoint = new Vector2(
            Mathf.Cos(previousAngle) * radiusX,
            Mathf.Sin(previousAngle) * radiusZ);
        float distance = 0f;

        for (int i = 1; i <= steps; i++)
        {
            float angle = (startAngle - stepDegrees * i) * Mathf.Deg2Rad;
            Vector2 point = new Vector2(
                Mathf.Cos(angle) * radiusX,
                Mathf.Sin(angle) * radiusZ);
            distance += Vector2.Distance(previousPoint, point);
            previousPoint = point;
        }

        return distance;
    }
}
