using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        HideArea,
        Counter
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
    public UnityEvent onCounter;
    public UnityEvent onPlayerEnter;

    [Tooltip("Delay in seconds before the GameStart trigger fires.")]
    [Min(0f)]
    public float WaitTime = 0f;

    [Tooltip("Number of successful SceneChange triggers required to fire this Counter trigger.")]
    [Min(1)]
    public int sceneChangesRequired = 14;
    

    // Shared across trigger instances so one trigger can grant access to another.
    private static bool GetAuthority = false;
    private static int sceneChangeCount;
    private static bool traceAreaTriggered;
    // Each SceneChange trigger instance may fire once after authority is granted.
    private bool sceneChangeTriggered;
    private bool counterTriggered;

    private void OnEnable()
    {
        TriggerCollider.isTrigger = true;
        SceneManager.sceneLoaded += ResetTraceAreaForNewScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ResetTraceAreaForNewScene;
    }

    private static void ResetTraceAreaForNewScene(Scene scene, LoadSceneMode mode)
    {
        traceAreaTriggered = false;
    }
    
    private void Reset()
    {
        TriggerCollider = GetComponent<Collider>();
    }


    private void Start()
    {
        switch (triggerCategory)
        {
            case TriggerTag.GameStart:
                StartCoroutine(TriggerGameStartAfterWait());
                break;
        }
    }

    private System.Collections.IEnumerator TriggerGameStartAfterWait()
    {
        yield return new WaitForSeconds(WaitTime);
        OnGameStart?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TriggerCollider == null || PlayerCollider == null)
        {
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
        onPlayerEnter.Invoke();
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
                sceneChangeCount++;
                
                Debug.Log("you change the scene");
                break;


            case TriggerTag.TraceArea:
                if (traceAreaTriggered)
                {
                    break;
                }

                traceAreaTriggered = true;
                onTraceArea?.Invoke();
                Debug.Log("trace begin");
                break;


            case TriggerTag.Teleport:
                onTeleport?.Invoke();
                Debug.Log("teleport");
                sceneChangeCount = 0;

                break;


            case TriggerTag.HideArea:
                onHideArea?.Invoke();
                Debug.Log("hide begin");
                break;           
        }

    }

    private void Update()
    {
        switch (triggerCategory)
        {
            case TriggerTag.Counter:

                if (!counterTriggered && sceneChangeCount >= Mathf.Max(1, sceneChangesRequired))
                {
                    counterTriggered = true;
                    onCounter?.Invoke();
                    Debug.Log("elevator open");
                }

                break;
        }
    }
}
