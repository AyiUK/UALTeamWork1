using UnityEngine;
using UnityEngine.Events;

public class PlayerOnTrigger : MonoBehaviour
{
    //[Tooltip("The trigger collider on this GameObject. Make sure Is Trigger is enabled.")]
    public Collider TriggerCollider;
   // [Tooltip("Only this collider will activate the trigger.")]
    public Collider PlayerCollider;

    public enum TriggerTag
    {
        ItemCollect,
        SceneChange,
        Teleport,
        TraceArea,
        HideArea
    }

    [Tooltip("Selects which signal is sent when the assigned player collider enters this trigger.")]
    public TriggerTag triggerCategory;

    [Header("Category Signals")]
    public UnityEvent onItemCollect;
    public UnityEvent onSceneChange;
    public UnityEvent onTeleport;
    public UnityEvent onTraceArea;
    public UnityEvent onHideArea;

    // Shared across trigger instances so one trigger can grant access to another.
    private static bool GetAuthority = false;

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
            Debug.Log($"Trigger on {name} entered by {other.name}; category: {triggerCategory}", this);
            EmitSelectedCategorySignal();
        }
    }

    private void EmitSelectedCategorySignal()
    {
        switch (triggerCategory)
        {
            case TriggerTag.ItemCollect:
                onItemCollect?.Invoke();
                
                GetAuthority=true;
                Debug.Log("获得权力了");
                break;

            case TriggerTag.SceneChange:
                if (!GetAuthority)
                {
                    Debug.Log("尚未获得改变场景的权限");
                    break;
                }

                onSceneChange?.Invoke();
                Debug.Log("门打开，灯变色");
                break;

                

                
            case TriggerTag.Teleport:
                onTeleport?.Invoke();
                Debug.Log("可以传送了");
                break;
            case TriggerTag.TraceArea:
                onTraceArea?.Invoke();
                Debug.Log("可以被追踪了");
                break;
            case TriggerTag.HideArea:
                onHideArea?.Invoke();
                Debug.Log("可以躲藏了");
                break;
        }

    }
}
