using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hides renderers between the camera and player, then restores their original state.
/// Attach this component to the camera or another object that stays active with it.
/// </summary>
public class CameraColliderTracking : MonoBehaviour
{
    [SerializeField] private Camera trackingCamera;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask obstacleLayers = Physics.DefaultRaycastLayers;

    private readonly Dictionary<Renderer, bool> originalRendererStates = new Dictionary<Renderer, bool>();
    private readonly HashSet<Renderer> currentObstructions = new HashSet<Renderer>();
    private readonly List<Renderer> restoreBuffer = new List<Renderer>();

    private void Awake()
    {
        if (trackingCamera == null)
        {
            trackingCamera = GetComponent<Camera>();
            if (trackingCamera == null) trackingCamera = Camera.main;
        }

        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null) player = playerController.transform;
        }
    }

    private void LateUpdate()
    {
        if (trackingCamera == null || player == null)
        {
            RestoreAll();
            return;
        }

        Vector3 origin = trackingCamera.transform.position;
        Vector3 toPlayer = player.position - origin;
        float distance = toPlayer.magnitude;
        if (distance <= Mathf.Epsilon)
        {
            RestoreAll();
            return;
        }

        currentObstructions.Clear();
        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            toPlayer / distance,
            distance,
            obstacleLayers,
            QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            Transform hitTransform = hit.collider.transform;
            if (hitTransform == player || hitTransform.IsChildOf(player))
            {
                break;
            }

            Renderer[] renderers = hitTransform.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;
                currentObstructions.Add(renderer);
                if (!originalRendererStates.ContainsKey(renderer))
                {
                    originalRendererStates.Add(renderer, renderer.enabled);
                    renderer.enabled = false;
                }
            }
        }

        restoreBuffer.Clear();
        foreach (KeyValuePair<Renderer, bool> entry in originalRendererStates)
        {
            if (entry.Key == null || !currentObstructions.Contains(entry.Key))
            {
                if (entry.Key != null) entry.Key.enabled = entry.Value;
                restoreBuffer.Add(entry.Key);
            }
        }

        foreach (Renderer renderer in restoreBuffer)
        {
            originalRendererStates.Remove(renderer);
        }
    }

    private void OnDisable()
    {
        RestoreAll();
    }

    private void RestoreAll()
    {
        foreach (KeyValuePair<Renderer, bool> entry in originalRendererStates)
        {
            if (entry.Key != null) entry.Key.enabled = entry.Value;
        }

        originalRendererStates.Clear();
        currentObstructions.Clear();
        restoreBuffer.Clear();
    }
}
