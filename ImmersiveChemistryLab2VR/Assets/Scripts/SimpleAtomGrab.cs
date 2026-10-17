using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleAtomGrab : MonoBehaviour
{
    public Transform leftController;
    public Transform rightController;

    public float grabDistance = 0.8f;

    public Material highlightMaterial;

    // Turn this on for bottles so they rotate with your wrist.
    public bool followControllerRotation = true;

    private bool grabbed = false;

    private Transform activeController;

    private Renderer objectRenderer;
    private Material originalMaterial;

    // Position and rotation offsets recorded when grabbing.
    private Vector3 localGrabOffset;
    private Quaternion rotationOffset;

    private static SimpleAtomGrab currentlyGrabbedObject;

    private bool previousLeftGrip = false;
    private bool previousRightGrip = false;

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
        // KEYBOARD INPUT
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
            if (leftGripPressed && leftInRange)
            {
                BeginGrab(
                    leftController,
                    GrabSource.LeftController
                );
            }
            else if (rightGripPressed && rightInRange)
            {
                BeginGrab(
                    rightController,
                    GrabSource.RightController
                );
            }
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
            }
            else if (
                grabSource == GrabSource.LeftController &&
                leftGripReleased
            )
            {
                StopGrab();
            }
            else if (
                grabSource == GrabSource.RightController &&
                rightGripReleased
            )
            {
                StopGrab();
            }
        }

        // ----------------------------------------------------
        // FOLLOW CONTROLLER
        // ----------------------------------------------------

        if (grabbed && activeController != null)
        {
            // Follow controller position.
            transform.position =
                activeController.TransformPoint(
                    localGrabOffset
                );

            // Follow controller rotation.
            if (followControllerRotation)
            {
                transform.rotation =
                    activeController.rotation *
                    rotationOffset;
            }
        }

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

        // Store object's position relative to controller.
        localGrabOffset =
            activeController.InverseTransformPoint(
                transform.position
            );

        // Store object's rotation relative to controller.
        rotationOffset =
            Quaternion.Inverse(
                activeController.rotation
            ) * transform.rotation;

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

        Debug.Log(name + " RELEASED");
    }
}