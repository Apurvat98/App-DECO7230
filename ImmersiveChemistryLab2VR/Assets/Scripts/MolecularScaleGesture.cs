
using UnityEngine;

public class MolecularScaleGesture : MonoBehaviour
{
    [Header("Controller References")]
    public Transform leftHand;
    public Transform rightHand;

    [Header("XR References")]
    public Transform xrOrigin;
    public Transform molecularSpawn;
    public Transform molecularWorld;

    [Header("Lab Environment")]
    public GameObject labEnvironment;

    [Header("Other Laboratory Objects")]
    public GameObject[] additionalLabObjects;

    [Header("Old Gesture - Not Used")]
    public float requiredExpansion = 0.15f;

    [Header("Molecular World Placement")]
    public float molecularViewDistance = 3.5f;
    public float molecularViewHeight = 0f;
    public float molecularViewX = 0.5f;

    [Header("Portal Appearance")]
    public Material portalMaterial;
    public float portalRadius = 0.28f;
    public float innerRingRadius = 0.20f;
    public float portalLineWidth = 0.012f;
    public int ringSegments = 96;

    [Header("Circle Tracing")]
    public float radiusTolerance = 0.11f;
    public float depthTolerance = 0.25f;
    public float requiredTraceAngle = 300f;
    public float maxAnglePerFrame = 45f;
    public float allowedTimeOutsideRing = 0.6f;

    [Header("Tracing Feedback")]
    public Material progressMaterial;
    public float progressLineWidth = 0.045f;
    public float progressLineOffset = 0.025f;
    public Color progressColor = Color.white;

    private LineRenderer outerRing;
    private LineRenderer innerRing;
    private LineRenderer progressRing;

    private Vector3 portalRight;
    private Vector3 portalUp = Vector3.up;
    private Vector3 portalForward;

    private Transform tracingHand;
    private Vector2 previousDirection;
    private Vector2 startingDirection;

    private float tracedAngle;
    private float timeOutsideRing;
    private int traceDirection;

    private bool tracking;
    private bool completed;

    private Vector3 labPosition;
    private Quaternion labRotation;

    private bool labEnvironmentWasActive = true;
    private bool[] previousLabStates;

    private CharacterController xrCharacterController;

    public bool IsInMolecularWorld { get; private set; } = false;

    private void Awake()
    {
        CreatePortalVisuals();

        if (xrOrigin != null)
        {
            xrCharacterController =
                xrOrigin.GetComponent<CharacterController>();
        }
    }

    private void OnEnable()
    {
        ResetTrace();
        OrientPortal();
        UpdatePortalVisuals();
    }

    private void Update()
    {
        if (completed || IsInMolecularWorld)
            return;

        if (leftHand == null || rightHand == null)
            return;

        if (!tracking)
        {
            if (IsHandOnRing(leftHand))
            {
                StartTracing(leftHand);
            }
            else if (IsHandOnRing(rightHand))
            {
                StartTracing(rightHand);
            }

            return;
        }

        if (tracingHand == null)
        {
            ResetTrace();
            return;
        }

        if (!IsHandOnRing(tracingHand))
        {
            timeOutsideRing += Time.deltaTime;

            if (timeOutsideRing >= allowedTimeOutsideRing)
            {
                ResetTrace();
            }

            return;
        }

        timeOutsideRing = 0f;

        Vector2 direction =
            GetPortalDirection(tracingHand);

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        float angle = Vector2.SignedAngle(
            previousDirection,
            direction
        );

        // Preserve existing movement filtering.
        if (Mathf.Abs(angle) < 0.2f ||
            Mathf.Abs(angle) > maxAnglePerFrame)
        {
            previousDirection = direction;
            return;
        }

        int movementDirection =
            angle > 0f ? 1 : -1;

        if (traceDirection == 0)
        {
            traceDirection = movementDirection;
        }

        if (movementDirection == traceDirection)
        {
            tracedAngle += Mathf.Abs(angle);
        }
        else
        {
            tracedAngle = Mathf.Max(
                0f,
                tracedAngle - Mathf.Abs(angle)
            );

            if (tracedAngle == 0f)
                traceDirection = 0;
        }

        previousDirection = direction;

        UpdateProgressRing();

        if (tracedAngle >= requiredTraceAngle)
        {
            completed = true;

            Debug.Log("PORTAL TRACE COMPLETE!");

            EnterMolecularWorld();
        }
    }

    private void CreatePortalVisuals()
    {
        outerRing = CreateRing("PortalOuterRing");
        innerRing = CreateRing("PortalInnerRing");

        progressRing = CreateRing("PortalProgressRing");

        // Give the progress arc its own material.
        if (progressMaterial != null)
        {
            progressRing.sharedMaterial =
                progressMaterial;
        }

        progressRing.loop = false;
        progressRing.enabled = false;
        progressRing.startWidth = progressLineWidth;
        progressRing.endWidth = progressLineWidth;
        progressRing.startColor = progressColor;
        progressRing.endColor = progressColor;
    }

    private LineRenderer CreateRing(string ringName)
    {
        GameObject ringObject = new GameObject(ringName);
        ringObject.transform.SetParent(transform, false);

        LineRenderer ring =
            ringObject.AddComponent<LineRenderer>();

        ring.useWorldSpace = true;
        ring.loop = true;
        ring.positionCount =
            Mathf.Max(16, ringSegments);

        ring.startWidth = portalLineWidth;
        ring.endWidth = portalLineWidth;

        ring.numCornerVertices = 4;
        ring.numCapVertices = 4;

        if (portalMaterial != null)
        {
            ring.sharedMaterial = portalMaterial;
        }

        ring.startColor = Color.white;
        ring.endColor = Color.white;

        ring.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        ring.receiveShadows = false;

        return ring;
    }

    private void OrientPortal()
    {
        Vector3 toViewer;

        if (Camera.main != null)
        {
            toViewer =
                Camera.main.transform.position -
                transform.position;
        }
        else if (xrOrigin != null)
        {
            toViewer =
                xrOrigin.position -
                transform.position;
        }
        else
        {
            toViewer = Vector3.back;
        }

        toViewer.y = 0f;

        if (toViewer.sqrMagnitude < 0.001f)
            toViewer = Vector3.back;

        portalForward = toViewer.normalized;

        portalRight = Vector3.Cross(
            portalUp,
            portalForward
        ).normalized;
    }

    private void UpdatePortalVisuals()
    {
        DrawRing(outerRing, portalRadius);
        DrawRing(innerRing, innerRingRadius);
    }

    private void DrawRing(LineRenderer ring, float radius)
    {
        if (ring == null)
            return;

        int count = Mathf.Max(16, ringSegments);

        ring.positionCount = count;
        ring.loop = true;

        Vector3 center = transform.position;

        for (int i = 0; i < count; i++)
        {
            float angle =
                i * Mathf.PI * 2f / count;

            Vector3 position =
                center +
                portalRight *
                Mathf.Cos(angle) * radius +
                portalUp *
                Mathf.Sin(angle) * radius;

            ring.SetPosition(i, position);
        }
    }

    private Vector2 GetPortalDirection(Transform hand)
    {
        Vector3 offset =
            hand.position - transform.position;

        return new Vector2(
            Vector3.Dot(offset, portalRight),
            Vector3.Dot(offset, portalUp)
        );
    }

    private bool IsHandOnRing(Transform hand)
    {
        if (hand == null)
            return false;

        Vector3 offset =
            hand.position - transform.position;

        float depth =
            Mathf.Abs(
                Vector3.Dot(offset, portalForward)
            );

        Vector2 flatPosition =
            GetPortalDirection(hand);

        float radius = flatPosition.magnitude;

        return
            depth <= depthTolerance &&
            Mathf.Abs(radius - portalRadius)
                <= radiusTolerance;
    }

    private void StartTracing(Transform hand)
    {
        tracingHand = hand;

        previousDirection =
            GetPortalDirection(hand).normalized;

        startingDirection = previousDirection;

        tracedAngle = 0f;
        timeOutsideRing = 0f;
        traceDirection = 0;
        tracking = true;

        if (progressRing != null)
            progressRing.enabled = false;

        Debug.Log(
            "PORTAL TRACING STARTED: " +
            hand.name
        );
    }

    private void UpdateProgressRing()
    {
        if (progressRing == null)
            return;

        if (!tracking ||
            tracedAngle <= 0f ||
            traceDirection == 0)
        {
            progressRing.enabled = false;
            return;
        }

        float progress = Mathf.Clamp01(
            tracedAngle /
            Mathf.Max(1f, requiredTraceAngle)
        );

        int fullCount =
            Mathf.Max(16, ringSegments);

        int pointCount = Mathf.Max(
            2,
            Mathf.CeilToInt(
                fullCount * progress
            ) + 1
        );

        progressRing.enabled = true;
        progressRing.loop = false;
        progressRing.positionCount = pointCount;

        progressRing.startWidth =
            progressLineWidth;
        progressRing.endWidth =
            progressLineWidth;

        progressRing.startColor = progressColor;
        progressRing.endColor = progressColor;

        Vector3 center =
            transform.position +
            portalForward * progressLineOffset;

        float initialAngle = Mathf.Atan2(
            startingDirection.y,
            startingDirection.x
        );

        for (int i = 0; i < pointCount; i++)
        {
            float fraction =
                (float)i / (pointCount - 1);

            float angle =
                initialAngle +
                traceDirection *
                tracedAngle *
                Mathf.Deg2Rad *
                fraction;

            Vector3 position =
                center +
                portalRight *
                Mathf.Cos(angle) * portalRadius +
                portalUp *
                Mathf.Sin(angle) * portalRadius;

            progressRing.SetPosition(i, position);
        }
    }

    private void ResetTrace()
    {
        tracingHand = null;
        tracedAngle = 0f;
        timeOutsideRing = 0f;
        traceDirection = 0;
        tracking = false;

        if (progressRing != null)
            progressRing.enabled = false;
    }

    public void BeginGesture()
    {
        if (!IsInMolecularWorld)
            ResetTrace();
    }

    private bool IsProtectedObject(GameObject obj)
    {
        if (obj == null)
            return true;

        Transform candidate = obj.transform;

        return ContainsOrIs(candidate, xrOrigin) ||
               ContainsOrIs(candidate, molecularWorld) ||
               ContainsOrIs(candidate, molecularSpawn) ||
               ContainsOrIs(candidate, transform);
    }

    private bool ContainsOrIs(
        Transform candidate,
        Transform protectedTransform)
    {
        if (candidate == null ||
            protectedTransform == null)
            return false;

        return candidate == protectedTransform ||
               protectedTransform.IsChildOf(candidate);
    }

    private void HideAdditionalLabObjects()
    {
        if (additionalLabObjects == null)
            return;

        previousLabStates =
            new bool[additionalLabObjects.Length];

        for (int i = 0;
             i < additionalLabObjects.Length;
             i++)
        {
            GameObject obj =
                additionalLabObjects[i];

            if (obj == null ||
                IsProtectedObject(obj))
                continue;

            previousLabStates[i] =
                obj.activeSelf;

            obj.SetActive(false);
        }
    }

    private void RestoreAdditionalLabObjects()
    {
        if (additionalLabObjects == null ||
            previousLabStates == null)
            return;

        int count = Mathf.Min(
            additionalLabObjects.Length,
            previousLabStates.Length
        );

        for (int i = 0; i < count; i++)
        {
            GameObject obj =
                additionalLabObjects[i];

            if (obj == null ||
                IsProtectedObject(obj))
                continue;

            obj.SetActive(previousLabStates[i]);
        }
    }

    private void MoveXRTo(
        Vector3 position,
        Quaternion rotation)
    {
        if (xrOrigin == null)
            return;

        if (xrCharacterController == null)
        {
            xrCharacterController =
                xrOrigin.GetComponent<CharacterController>();
        }

        bool wasEnabled =
            xrCharacterController != null &&
            xrCharacterController.enabled;

        if (wasEnabled)
            xrCharacterController.enabled = false;

        xrOrigin.SetPositionAndRotation(
            position,
            rotation
        );

        if (wasEnabled)
            xrCharacterController.enabled = true;
    }

    public void EnterMolecularWorld()
    {
        if (IsInMolecularWorld)
            return;

        if (xrOrigin == null ||
            molecularSpawn == null ||
            molecularWorld == null)
        {
            Debug.LogWarning(
                "MolecularScaleGesture: Missing XR reference."
            );

            completed = false;
            return;
        }

        labPosition = xrOrigin.position;
        labRotation = xrOrigin.rotation;

        IsInMolecularWorld = true;
        completed = true;

        // Hide the imported laboratory.
        if (labEnvironment != null)
        {
            labEnvironmentWasActive =
                labEnvironment.activeSelf;

            if (!IsProtectedObject(labEnvironment))
            {
                labEnvironment.SetActive(false);
            }
        }

        // Hide separate laboratory objects.
        HideAdditionalLabObjects();

        // Move XR Origin without collision interference.
        MoveXRTo(
            molecularSpawn.position,
            molecularSpawn.rotation
        );

        // Show and position the molecular environment.
        molecularWorld.gameObject.SetActive(true);
        molecularWorld.SetParent(xrOrigin);

        molecularWorld.localPosition = new Vector3(
            molecularViewX,
            molecularViewHeight,
            molecularViewDistance
        );

        molecularWorld.localRotation =
            Quaternion.identity;

        Debug.Log(
            "Entered molecular world. View distance: " +
            molecularViewDistance
        );

        gameObject.SetActive(false);
    }

    public void ExitMolecularWorld()
    {
        if (!IsInMolecularWorld)
            return;

        // Hide molecular objects.
        if (molecularWorld != null)
        {
            molecularWorld.gameObject.SetActive(false);
        }

        // Restore the saved XR Origin position.
        MoveXRTo(
            labPosition,
            labRotation
        );

        // Restore imported laboratory.
        if (labEnvironment != null &&
            !IsProtectedObject(labEnvironment))
        {
            labEnvironment.SetActive(
                labEnvironmentWasActive
            );
        }

        // Restore separate laboratory objects.
        RestoreAdditionalLabObjects();

        IsInMolecularWorld = false;
        completed = true;

        ResetTrace();

        Debug.Log("Returned to laboratory");
    }
}
