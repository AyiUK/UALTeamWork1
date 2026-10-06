using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class KillZone : MonoBehaviour
{
    [SerializeField] private Transform playerModel;
    [SerializeField, Min(0f)] private float secondsBeforeRespawn = 0f;
    [Tooltip("When assigned, only this collider is used to detect the player entering and leaving the KillZone.")]
    public Collider playerColliderToDetect;
    [Tooltip("Log trigger entry, exit, timer resets, and death to the Console.")]
    public bool debugMode;

    private readonly HashSet<Collider> playerCollidersInside = new HashSet<Collider>();
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private float timeInside;
    private CharacterController characterController;

    private void Awake()
    {
        Collider zoneCollider = GetComponent<Collider>();
        if (zoneCollider is MeshCollider meshCollider && !meshCollider.convex)
        {
            // Non-convex MeshColliders cannot be used as trigger colliders with a Rigidbody.
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = gameObject.AddComponent<BoxCollider>();
            }

            if (meshCollider.sharedMesh != null)
            {
                boxCollider.center = meshCollider.sharedMesh.bounds.center;
                Vector3 meshSize = meshCollider.sharedMesh.bounds.size;
                meshSize.y = Mathf.Max(meshSize.y, 0.2f);
                boxCollider.size = meshSize;
            }

            meshCollider.enabled = false;
            zoneCollider = boxCollider;
        }

        zoneCollider.isTrigger = true;

        Rigidbody zoneRigidbody = GetComponent<Rigidbody>();
        zoneRigidbody.isKinematic = true;
        zoneRigidbody.useGravity = false;

        if (playerModel == null)
        {
            GameObject playerObject = GameObject.Find("PlayerModel");
            if (playerObject != null)
            {
                playerModel = playerObject.transform;
            }
        }

        if (playerModel != null)
        {
            characterController = playerModel.GetComponentInParent<CharacterController>();
            if (characterController != null)
            {
                playerModel = characterController.transform;
            }

            spawnPosition = playerModel.position;
            spawnRotation = playerModel.rotation;
        }
        else
        {
            Debug.LogError("KillZone: Assign PlayerModel in the Inspector or name the player object PlayerModel.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerCollider(other))
        {
            bool wasAdded = playerCollidersInside.Add(other);
            if (wasAdded && debugMode)
            {
                Debug.Log($"KillZone: detected {other.name} entering. Timer started.", this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerCollidersInside.Remove(other))
        {
            if (debugMode)
            {
                Debug.Log($"KillZone: detected {other.name} leaving after {timeInside:F2} seconds.", this);
            }

            if (playerCollidersInside.Count == 0)
            {
                timeInside = 0f;
                if (debugMode)
                {
                    Debug.Log("KillZone: no detected player colliders remain inside. Timer reset.", this);
                }
            }
        }
    }

    private void Update()
    {
        if (playerModel == null || playerCollidersInside.Count == 0)
        {
            return;
        }

        timeInside += Time.deltaTime;
        if (timeInside >= secondsBeforeRespawn)
        {
            KillPlayer();
        }
    }

    private bool IsPlayerCollider(Collider other)
    {
        if (playerColliderToDetect != null)
        {
            return other == playerColliderToDetect;
        }

        Transform otherTransform = other.transform;
        return otherTransform == playerModel || otherTransform.IsChildOf(playerModel);
    }

    private void KillPlayer()
    {
        if (debugMode)
        {
            Debug.Log($"KillZone: player reached {timeInside:F2} seconds and died.", playerModel);
        }
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        playerCollidersInside.Clear();
        timeInside = 0f;

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        playerModel.SetPositionAndRotation(spawnPosition, spawnRotation);

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }
}
