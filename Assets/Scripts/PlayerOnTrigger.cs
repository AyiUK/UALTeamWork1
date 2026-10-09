using UnityEngine;

public class PlayerOnTrigger : MonoBehaviour
{
    [Tooltip("The trigger collider on this GameObject. Make sure Is Trigger is enabled.")]
    public Collider TriggerCollider;

    [Tooltip("Only this collider will activate the trigger.")]
    public Collider PlayerCollider;

    private void Reset()
    {
        TriggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TriggerCollider == null || PlayerCollider == null)
        {
            return;
        }

        if (!TriggerCollider.isTrigger)
        {
            Debug.LogError("PlayerOnTrigger: TriggerCollider must have Is Trigger enabled.", this);
            return;
        }

        if (other == PlayerCollider)
        {
            ColliderIsTriggered();
        }
    }

    /// <summary>
    /// Called when the assigned PlayerCollider enters the assigned TriggerCollider.
    /// Override this method or add the desired response here.
    /// </summary>
    public virtual void ColliderIsTriggered()
    {
        Debug.Log("tirgger被触发了", this);
    }
}
