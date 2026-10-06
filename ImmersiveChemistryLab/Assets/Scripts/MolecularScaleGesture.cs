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

    private bool tracking = false;
    private bool completed = false;

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
    if (!tracking || completed) return;

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
        // Hide focus sphere
        gameObject.SetActive(false);

        if (molecularWorld != null)
        {
            molecularWorld.gameObject.SetActive(true);

            // Keep molecular world stable relative to XR rig
            molecularWorld.SetParent(xrOrigin);
            molecularWorld.localPosition = new Vector3(0f, 0f, 2f);
            molecularWorld.localRotation = Quaternion.identity;
        }

        // Move to molecular area
        xrOrigin.position = molecularSpawn.position;
        xrOrigin.rotation = molecularSpawn.rotation;

        Debug.Log("Entered molecular world");
    }
}