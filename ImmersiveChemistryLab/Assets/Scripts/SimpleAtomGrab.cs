using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleAtomGrab : MonoBehaviour
{
    public Transform leftController;
    public Transform rightController;

    public float grabDistance = 0.8f;

    public Material highlightMaterial;

    private bool grabbed = false;
    private Vector3 grabOffset;

    private Transform activeController;

    private Renderer objectRenderer;
    private Material originalMaterial;

    // Only one object can be grabbed at a time
    private static SimpleAtomGrab currentlyGrabbedObject;

    // Previous Quest grip states.
    // These let us detect the exact frame a grip is pressed/released.
    private bool previousLeftGrip = false;
    private bool previousRightGrip = false;

    // Records what input originally started the grab.
    private enum GrabSource
    {
        None,
        Keyboard,
        LeftController,
        RightController
    }

    private GrabSource grabSource = GrabSource.None;

    public bool IsGrabbed
    {
        get { return grabbed; }
    }

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInChildren<Renderer>();
        }

        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }
    }

    void Update()
    {
        // Do nothing if controller transforms have not been assigned.
        if (leftController == null || rightController == null)
            return;

        // ----------------------------------------------------
        // DISTANCE CHECK
        // ----------------------------------------------------

        float leftDistance =
            Vector3.Distance(
                transform.position,
                leftController.position
            );

        float rightDistance =
            Vector3.Distance(
                transform.position,
                rightController.position
            );

        bool leftInRange =
            leftDistance <= grabDistance;

        bool rightInRange =
            rightDistance <= grabDistance;

        bool anyControllerInRange =
            leftInRange || rightInRange;

        // ----------------------------------------------------
        // QUEST CONTROLLER INPUT
        // ----------------------------------------------------

        UnityEngine.XR.InputDevice leftDevice =
            UnityEngine.XR.InputDevices.GetDeviceAtXRNode(
                UnityEngine.XR.XRNode.LeftHand
            );

        UnityEngine.XR.InputDevice rightDevice =
            UnityEngine.XR.InputDevices.GetDeviceAtXRNode(
                UnityEngine.XR.XRNode.RightHand
            );

        bool leftGrip = false;
        bool rightGrip = false;

        if (leftDevice.isValid)
        {
            leftDevice.TryGetFeatureValue(
                UnityEngine.XR.CommonUsages.gripButton,
                out leftGrip
            );
        }

        if (rightDevice.isValid)
        {
            rightDevice.TryGetFeatureValue(
                UnityEngine.XR.CommonUsages.gripButton,
                out rightGrip
            );
        }

        bool leftGripPressed =
            leftGrip && !previousLeftGrip;

        bool rightGripPressed =
            rightGrip && !previousRightGrip;

        bool leftGripReleased =
            !leftGrip && previousLeftGrip;

        bool rightGripReleased =
            !rightGrip && previousRightGrip;

        // ----------------------------------------------------
        // KEYBOARD INPUT FOR UNITY SIMULATOR
        // ----------------------------------------------------

        bool keyboardGrabPressed =
            Keyboard.current != null &&
            Keyboard.current.gKey.wasPressedThisFrame;

        bool keyboardGrabReleased =
            Keyboard.current != null &&
            Keyboard.current.gKey.wasReleasedThisFrame;

        // ----------------------------------------------------
        // HIGHLIGHT
        // ----------------------------------------------------

        if (anyControllerInRange || grabbed)
        {
            SetHighlight(true);
        }
        else
        {
            SetHighlight(false);
        }

        // ----------------------------------------------------
        // START GRAB
        // ----------------------------------------------------

        if (!grabbed && currentlyGrabbedObject == null)
        {
            // Physical Quest LEFT grip
            if (leftGripPressed && leftInRange)
            {
                BeginGrab(
                    leftController,
                    GrabSource.LeftController
                );
            }

            // Physical Quest RIGHT grip
            else if (rightGripPressed && rightInRange)
            {
                BeginGrab(
                    rightController,
                    GrabSource.RightController
                );
            }

            // Unity Editor / XR Simulator keyboard G
            else if (
                keyboardGrabPressed &&
                anyControllerInRange
            )
            {
                Transform chosenController;

                if (leftInRange && rightInRange)
                {
                    chosenController =
                        leftDistance <= rightDistance
                        ? leftController
                        : rightController;
                }
                else if (leftInRange)
                {
                    chosenController = leftController;
                }
                else
                {
                    chosenController = rightController;
                }

                BeginGrab(
                    chosenController,
                    GrabSource.Keyboard
                );
            }
        }

        // ----------------------------------------------------
        // RELEASE GRAB
        // ----------------------------------------------------

        if (grabbed)
        {
            if (
                grabSource == GrabSource.Keyboard &&
                keyboardGrabReleased
            )
            {
                StopGrab();
                Debug.Log(name + " RELEASED");
            }
            else if (
                grabSource == GrabSource.LeftController &&
                leftGripReleased
            )
            {
                StopGrab();
                Debug.Log(name + " RELEASED");
            }
            else if (
                grabSource == GrabSource.RightController &&
                rightGripReleased
            )
            {
                StopGrab();
                Debug.Log(name + " RELEASED");
            }
        }

        // ----------------------------------------------------
        // FOLLOW ACTIVE CONTROLLER
        // ----------------------------------------------------

        if (grabbed && activeController != null)
        {
            transform.position =
                activeController.position + grabOffset;
        }

        // Store current grip states for the next frame.
        previousLeftGrip = leftGrip;
        previousRightGrip = rightGrip;
    }

    private void BeginGrab(
        Transform controller,
        GrabSource source
    )
    {
        activeController = controller;

        grabbed = true;
        currentlyGrabbedObject = this;

        grabSource = source;

        grabOffset =
            transform.position -
            activeController.position;

        Debug.Log(
            name + " GRABBED using " + source
        );
    }

    private void SetHighlight(bool shouldHighlight)
    {
        if (
            objectRenderer == null ||
            highlightMaterial == null
        )
            return;

        objectRenderer.material =
            shouldHighlight
            ? highlightMaterial
            : originalMaterial;
    }

    public void StopGrab()
    {
        grabbed = false;
        activeController = null;
        grabSource = GrabSource.None;

        if (currentlyGrabbedObject == this)
        {
            currentlyGrabbedObject = null;
        }

        SetHighlight(false);
    }
}