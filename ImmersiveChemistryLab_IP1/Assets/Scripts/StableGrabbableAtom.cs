using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StableGrabbableAtom : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isGrabbed = false;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (!isGrabbed)
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }
    }

    public void GrabStarted()
    {
        isGrabbed = true;
    }

    public void GrabEnded()
    {
        isGrabbed = false;
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
}