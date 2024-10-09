using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GazeTrackCollider : UdonSharpBehaviour
{
    [Tooltip("Maximum distance for gaze detection")]
    public float maxGazeDistance = 4f;

    [Tooltip("Counter for players that gazed at the object since last check")]
    public int gazeCounter = 0;

    private BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("GazeTrackCollider requires a BoxCollider component");
        }
    }
    /// <summary>
    /// Checks if the local player is looking at the front Z face of the BoxCollider.
    /// </summary>
    /// <returns>whether player's gaze hit this collider (for early loop breaking)</returns>
    public bool CheckGaze()
    {
        if (boxCollider == null) return false;

        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null) return false;

        // Get the player's head position and forward direction
        Vector3 playerPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        Vector3 playerForward = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation * Vector3.forward;

        // Perform a raycast from the player's head position in the direction they're looking
        RaycastHit hit;
        if (Physics.Raycast(playerPosition, playerForward, out hit, maxGazeDistance))
        {
            // Check if the ray hit this collider
            if (hit.collider == boxCollider)
            {
                // Calculate the hit normal and the collider's forward direction
                Vector3 hitNormal = hit.normal;
                Vector3 colliderForward = transform.forward;

                // Assuming the box collider is thin in the local Z extent,
                // we can be more lenient with the dot product check.
                // This allows for oblique viewing angles while still
                // rejecting cases where the player is looking at the back of the collider.
                if (Vector3.Dot(hitNormal, -colliderForward) > 0.1f)
                {
                    // increment local counter; vrchat docs are still ambiguous whether NetworkEventTarget.All includes local player.
                    gazeCounter++;
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "NetworkIncrementGazeCounter");
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Network event to increment gaze counter on all clients
    /// </summary>
    public void NetworkIncrementGazeCounter()
    {
        gazeCounter++;
    }

    /// <summary>
    /// Resets the gaze counter to zero and returns the previous value
    /// </summary>
    /// <returns>The value of the gaze counter before resetting</returns>
    public int ResetGazeCounter()
    {
        int previousCount = gazeCounter;
        gazeCounter = 0;
        return previousCount;
    }

}
