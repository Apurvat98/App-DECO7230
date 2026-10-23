using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleAtomGrab : MonoBehaviour
{
    public Transform leftController;
    public Transform rightController;

    public float grabDistance = 0.8f;

    public Material highlightMaterial;

    // Bottles rotate with the controller/wrist.
    public bool followControllerRotation = true;

    private bool grabbed = false;
    private Transform activeController;

    private Renderer objectRenderer;
    private Material originalMaterial;

    private Vector3 localGrabOffset;
    private Quaternion rotationOffset;

    // Only one SimpleAtomGrab object can be held at once.
    private static SimpleAtomGrab currentlyGrabbedObject;

    // Prevent every SimpleAtomGrab instance from processing
    // the same keyboard G press during the same frame.
    private static int keyboardProcessedFrame = -1;

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
        // DISTANCE CHECK FOR THIS OBJECT
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
        // QUEST / XR CONTROLLER INPUT
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
        // REAL / SIMULATED XR GRIP
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
        }

        // ----------------------------------------------------
        // KEYBOARD G FALLBACK
        //
        // IMPORTANT:
        // Instead of every bottle independently reacting to G,
        // search ALL SimpleAtomGrab objects and grab only the
        // closest valid object/controller pair.
        // ----------------------------------------------------

        if (
            keyboardGrabPressed &&
            currentlyGrabbedObject == null &&
            keyboardProcessedFrame != Time.frameCount
        )
        {
            keyboardProcessedFrame = Time.frameCount;
            TryKeyboardGrabClosestObject();
        }

        // ----------------------------------------------------
        // RELEASE
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
        // FOLLOW ACTIVE CONTROLLER
        // ----------------------------------------------------

        if (grabbed && activeController != null)
        {
            transform.position =
                activeController.TransformPoint(
                    localGrabOffset
                );

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

    // --------------------------------------------------------
    // GLOBAL KEYBOARD GRAB SELECTION
    // --------------------------------------------------------

    private static void TryKeyboardGrabClosestObject()
    {
        SimpleAtomGrab[] allGrabbableObjects =
            FindObjectsByType<SimpleAtomGrab>(
                FindObjectsSortMode.None
            );

        SimpleAtomGrab bestObject = null;
        Transform bestController = null;

        float bestDistance = Mathf.Infinity;

        foreach (SimpleAtomGrab candidate in allGrabbableObjects)
        {
            if (
                candidate == null ||
                !candidate.isActiveAndEnabled ||
                candidate.leftController == null ||
                candidate.rightController == null
            )
            {
                continue;
            }

            float leftDistance =
                Vector3.Distance(
                    candidate.transform.position,
                    candidate.leftController.position
                );

            if (
                leftDistance <= candidate.grabDistance &&
                leftDistance < bestDistance
            )
            {
                bestDistance = leftDistance;
                bestObject = candidate;
                bestController = candidate.leftController;
            }

            float rightDistance =
                Vector3.Distance(
                    candidate.transform.position,
                    candidate.rightController.position
                );

            if (
                rightDistance <= candidate.grabDistance &&
                rightDistance < bestDistance
            )
            {
                bestDistance = rightDistance;
                bestObject = candidate;
                bestController = candidate.rightController;
            }
        }

        if (
            bestObject != null &&
            bestController != null
        )
        {
            bestObject.BeginGrab(
                bestController,
                GrabSource.Keyboard
            );

            Debug.Log(
                "Keyboard G selected " +
                bestObject.name +
                " at distance " +
                bestDistance.ToString("F3")
            );
        }
        else
        {
            Debug.Log(
                "Keyboard G: no grabbable object is close enough."
            );
        }
    }

    // --------------------------------------------------------
    // BEGIN GRAB
    // --------------------------------------------------------

    private void BeginGrab(
        Transform controller,
        GrabSource source
    )
    {
        activeController = controller;

        grabbed = true;
        currentlyGrabbedObject = this;

        grabSource = source;

        localGrabOffset =
            activeController.InverseTransformPoint(
                transform.position
            );

        rotationOffset =
            Quaternion.Inverse(
                activeController.rotation
            ) * transform.rotation;

        Debug.Log(
            name + " GRABBED using " + source
        );
    }

    // --------------------------------------------------------
    // HIGHLIGHT
    // --------------------------------------------------------

    private void SetHighlight(bool shouldHighlight)
    {
        if (
            objectRenderer == null ||
            highlightMaterial == null
        )
        {
            return;
        }

        objectRenderer.material =
            shouldHighlight
            ? highlightMaterial
            : originalMaterial;
    }

    // --------------------------------------------------------
    // RELEASE
    // --------------------------------------------------------

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