
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GazeTracker : UdonSharpBehaviour
{
    [Tooltip("Array of GazeTrackCollider behaviors to track")]
    public GazeTrackCollider[] gazeTrackColliders;

    [Tooltip("Update rate in seconds")]
    public float updateRate = 2f;

    [Tooltip("Material using the TimeSeriesShader")]
    public Material timeSeriesShaderMaterial;

    [Tooltip("CustomRenderTexture for time series visualization")]
    public CustomRenderTexture customRenderTexture;

    void Start()
    {
        if (gazeTrackColliders == null || gazeTrackColliders.Length != 6)
        {
            Debug.LogError("GazeTracker requires exactly 6 GazeTrackCollider behaviors");
            return;
        }

        if (timeSeriesShaderMaterial == null)
        {
            Debug.LogError("GazeTracker requires a material using the TimeSeriesShader");
            return;
        }

        if (customRenderTexture != null)
        {
            customRenderTexture.updatePeriod = updateRate;
        }
        else
        {
            Debug.LogWarning("CustomRenderTexture is not assigned in GazeTracker");
        }


        // Start the update loop
        SendCustomEventDelayedSeconds(nameof(UpdateGaze), updateRate);
    }

    /// <summary>
    /// Updates the gaze tracking and plots the results
    /// </summary>
    public void UpdateGaze()
    {
        // Check gaze for each collider
        for (int i = 0; i < gazeTrackColliders.Length; i++)
        {
            if (gazeTrackColliders[i].CheckGaze())
            {
                break; // Early loop break if gaze is detected
            }
        }

        // Reset counters and update shader values
        for (int i = 0; i < gazeTrackColliders.Length; i++)
        {
            int gazeCount = gazeTrackColliders[i].ResetGazeCounter();
            timeSeriesShaderMaterial.SetFloat($"_Value{i + 1}", gazeCount);
        }

        // Schedule the next update
        SendCustomEventDelayedSeconds(nameof(UpdateGaze), updateRate);
    }
}
