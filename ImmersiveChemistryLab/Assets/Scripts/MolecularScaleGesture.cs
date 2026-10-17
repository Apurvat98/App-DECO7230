using UnityEngine;

public class MolecularScaleGesture : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public Transform xrOrigin;
    public Transform molecularSpawn;
    public Transform molecularWorld;

    public float requiredExpansion = 0.15f;

    private Vector3 leftStartPosition;
    private Vector3 rightStartPosition;

    private Vector3 labPosition;
    private Quaternion labRotation;

    private bool tracking = false;
    private bool completed = false;

    public bool IsInMolecularWorld { get; private set; } = false;

    private void OnEnable()
    {
        if (leftHand != null && rightHand != null)
        {
            BeginGesture();
        }
    }

    public void BeginGesture()
    {
        leftStartPosition = leftHand.position;
        rightStartPosition = rightHand.position;

        tracking = true;
        completed = false;

        Debug.Log("Scale gesture started");
    }

    void Update()
    {
        if (!tracking || completed)
            return;

        float currentDistance =
            Vector3.Distance(leftHand.position, rightHand.position);

        float startDistance =
            Vector3.Distance(leftStartPosition, rightStartPosition);

        float expansionAmount =
            currentDistance - startDistance;

        Debug.Log("EXPANSION: " + expansionAmount);

        if (expansionAmount >= requiredExpansion)
        {
            completed = true;
            EnterMolecularWorld();
        }
    }

    private void EnterMolecularWorld()
    {
        // Save where the user was standing in the lab
        // so we can return to exactly the same place later.
        labPosition = xrOrigin.position;
        labRotation = xrOrigin.rotation;

        IsInMolecularWorld = true;

        if (molecularWorld != null)
        {
            molecularWorld.gameObject.SetActive(true);

            // Keep the molecular scene positioned relative to the XR rig.
            molecularWorld.SetParent(xrOrigin);

            molecularWorld.localPosition =
                new Vector3(0f, 0f, 2f);

            molecularWorld.localRotation =
                Quaternion.identity;
        }

        // Move XR rig to the molecular area.
        xrOrigin.position = molecularSpawn.position;
        xrOrigin.rotation = molecularSpawn.rotation;

        Debug.Log("Entered molecular world");

        // Hide MolecularFocus / transition cue.
        // The separate MolecularModeControls script remains active.
        gameObject.SetActive(false);
    }

    public void ExitMolecularWorld()
    {
        if (!IsInMolecularWorld)
            return;

        // Hide molecular environment.
        if (molecularWorld != null)
        {
            molecularWorld.gameObject.SetActive(false);
        }

        // Return the player to their previous laboratory position.
        xrOrigin.position = labPosition;
        xrOrigin.rotation = labRotation;

        IsInMolecularWorld = false;

        Debug.Log("Returned to laboratory");

        // Reactivate MolecularFocus so the user can
        // enter molecular mode again if they want.
        gameObject.SetActive(true);
    }
}