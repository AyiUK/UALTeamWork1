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
        GameStart,
        ItemCollect,
        SceneChange,
        Teleport,
        TraceArea,
        HideArea
    }

    [Tooltip("Selects which signal is sent when the assigned player collider enters this trigger.")]
    public TriggerTag triggerCategory;

    [Header("Category Signals")]
    public UnityEvent OnGameStart;
    public UnityEvent onItemCollect;
    public UnityEvent onSceneChange;
    public UnityEvent onTeleport;
    public UnityEvent onTraceArea;
    public UnityEvent onHideArea;
    

    // Shared across trigger instances so one trigger can grant access to another.
    private static bool GetAuthority = false;
    // Each SceneChange trigger instance may fire once after authority is granted.
    private bool sceneChangeTriggered;
    
    private void Reset()
    {
        TriggerCollider = GetComponent<Collider>();
    }


    private void Start()
    {
        switch (triggerCategory)
        {
            case TriggerTag.GameStart:
                OnGameStart.Invoke(); 
                break;
        }
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
                Debug.Log("get the key");
                break;

            case TriggerTag.SceneChange:
                if (!GetAuthority)
                {
                    Debug.Log("you need the key to change");
                    break;
                }


                if (sceneChangeTriggered)
                {
                    break;
                }

                // Mark before invoking so re-entrant event calls cannot fire twice.
                sceneChangeTriggered = true;
                onSceneChange?.Invoke();
                
                Debug.Log("you change the scene");
                break;

                

                
            case TriggerTag.Teleport:
                onTeleport?.Invoke();
                Debug.Log("teleport");
                break;
            case TriggerTag.TraceArea:
                onTraceArea?.Invoke();
                Debug.Log("trace begin");
                break;
            case TriggerTag.HideArea:
                onHideArea?.Invoke();
                Debug.Log("hide begin");
                break;
        }

    }
}
