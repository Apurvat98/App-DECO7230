using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleAtomGrab : MonoBehaviour
{
    public Transform controller;
    public float grabDistance = 0.8f;

    private bool grabbed = false;
    private Vector3 grabOffset;

    public bool IsGrabbed
    {
        get { return grabbed; }
    }

    void Update()
    {
        if (controller == null)
            return;

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            float distance = Vector3.Distance(
                transform.position,
                controller.position
            );

            if (distance <= grabDistance)
            {
                grabbed = true;
                grabOffset = transform.position - controller.position;

                Debug.Log(name + " GRABBED");
            }
        }

        if (Keyboard.current.gKey.wasReleasedThisFrame && grabbed)
        {
            grabbed = false;
            Debug.Log(name + " RELEASED");
        }

        if (grabbed)
        {
            transform.position = controller.position + grabOffset;
        }
    }

    public void StopGrab()
    {
        grabbed = false;
    }
}