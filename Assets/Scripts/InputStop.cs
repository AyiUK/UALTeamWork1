using UnityEngine;

public class InputStop : MonoBehaviour
{
    private static int activeBlockerCount;

    /// <summary>True while at least one enabled InputStop component is active.</summary>
    public static bool IsMovementBlocked => activeBlockerCount > 0;

    private void OnEnable()
    {
        activeBlockerCount++;
    }

    private void OnDisable()
    {
        activeBlockerCount = Mathf.Max(0, activeBlockerCount - 1);
    }
}
