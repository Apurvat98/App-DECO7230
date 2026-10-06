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

        float leftDistance =
            Vector3.Distance(transform.position, leftController.position);

        float rightDistance =
            Vector3.Distance(transform.position, rightController.position);

        bool leftInRange = leftDistance <= grabDistance;
        bool rightInRange = rightDistance <= grabDistance;

        bool anyControllerInRange = leftInRange || rightInRange;

        // Highlight when the object can be grabbed,
        // and keep it highlighted while it is being held.
        if (anyControllerInRange || grabbed)
        {
            SetHighlight(true);
        }
        else
        {
            SetHighlight(false);
        }

        // Start grab
        if (Keyboard.current.gKey.wasPressedThisFrame &&
            !grabbed &&
            currentlyGrabbedObject == null)
        {
            if (anyControllerInRange)
            {
                if (leftInRange && rightInRange)
                {
                    activeController =
                        leftDistance <= rightDistance
                        ? leftController
                        : rightController;
                }
                else if (leftInRange)
                {
                    activeController = leftController;
                }
                else
                {
                    activeController = rightController;
                }

                grabbed = true;
                currentlyGrabbedObject = this;

                grabOffset =
                    transform.position - activeController.position;

                Debug.Log(name + " GRABBED");
            }
        }

        // Release grab
        if (Keyboard.current.gKey.wasReleasedThisFrame && grabbed)
        {
            StopGrab();

            Debug.Log(name + " RELEASED");
        }

        // Follow selected controller
        if (grabbed && activeController != null)
        {
            transform.position =
                activeController.position + grabOffset;
        }
    }

    private void SetHighlight(bool shouldHighlight)
    {
        if (objectRenderer == null || highlightMaterial == null)
            return;

        objectRenderer.material =
            shouldHighlight ? highlightMaterial : originalMaterial;
    }

    public void StopGrab()
    {
        grabbed = false;
        activeController = null;

        if (currentlyGrabbedObject == this)
        {
            currentlyGrabbedObject = null;
        }

        SetHighlight(false);
    }
}