using UnityEngine;

public class StirInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform stirTip;
    public Transform stirCenter;
    public SimpleAtomGrab stirringTool;

    [Header("Stir Detection")]
    public float stirRadius = 0.30f;
    public float maxVerticalDistance = 0.20f;
    public float requiredRotation = 540f;

    [Header("Completion Feedback")]
    public Renderer liquidRenderer;
    public Material mixedMaterial;

    private Vector3 previousDirection;
    private float accumulatedRotation = 0f;
    private bool trackingInside = false;
    private bool completed = false;

    void Update()
    {
        if (completed)
            return;

        if (stirTip == null ||
            stirCenter == null ||
            stirringTool == null)
            return;

        // Stirring only counts while the user is actually holding the tool.
        if (!stirringTool.IsGrabbed)
        {
            trackingInside = false;
            return;
        }

        Vector3 offset =
            stirTip.position - stirCenter.position;

        float verticalDistance =
            Mathf.Abs(offset.y);

        Vector3 flatOffset =
            new Vector3(offset.x, 0f, offset.z);

        float horizontalDistance =
            flatOffset.magnitude;

        bool tipInsideBowl =
            horizontalDistance <= stirRadius &&
            horizontalDistance >= 0.03f &&
            verticalDistance <= maxVerticalDistance;

        if (!tipInsideBowl)
        {
            trackingInside = false;
            return;
        }

        Vector3 currentDirection =
            flatOffset.normalized;

        if (!trackingInside)
        {
            previousDirection = currentDirection;
            trackingInside = true;
            return;
        }

        float angle =
            Vector3.SignedAngle(
                previousDirection,
                currentDirection,
                Vector3.up
            );

        // Ignore tiny jitter.
        if (Mathf.Abs(angle) >= 1f)
        {
            accumulatedRotation += Mathf.Abs(angle);

            Debug.Log(
                "STIR PROGRESS: " +
                accumulatedRotation.ToString("F0") +
                " / " +
                requiredRotation
            );
        }

        previousDirection = currentDirection;

        if (accumulatedRotation >= requiredRotation)
        {
            CompleteMixing();
        }
    }

    private void CompleteMixing()
    {
        completed = true;

        Debug.Log("MIXING COMPLETE");

        if (liquidRenderer != null &&
            mixedMaterial != null)
        {
            liquidRenderer.material = mixedMaterial;
        }
    }
}