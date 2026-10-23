
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProximityMagicRing : MonoBehaviour
{
    [Header("Controllers")]
    public Transform leftController;
    public Transform rightController;

    [Header("Proximity")]
    public float showDistance = 0.35f;

    [Header("Ring Appearance")]
    public float ringRadius = 0.14f;
    public float outerRingRadius = 0.18f;
    public int segments = 64;
    public float ringWidth = 0.004f;

    [Header("Second Ring")]
    public LineRenderer outerRing;

    [Header("Magical Symbols")]
    public Renderer[] magicRunes;

    private LineRenderer innerRing;
    private SimpleAtomGrab grabScript;
    private Transform targetObject;

    void Awake()
    {
        innerRing = GetComponent<LineRenderer>();
        grabScript = GetComponentInParent<SimpleAtomGrab>();
        targetObject = transform.parent;

        ConfigureRing(innerRing, ringRadius);

        if (outerRing != null)
            ConfigureRing(outerRing, outerRingRadius);

        SetRingsVisible(false);
    }

    void Update()
    {
        if (leftController == null ||
            rightController == null ||
            targetObject == null)
        {
            SetRingsVisible(false);
            return;
        }

        bool isGrabbed =
            grabScript != null && grabScript.IsGrabbed;

        float leftDistance = Vector3.Distance(
            targetObject.position,
            leftController.position
        );

        float rightDistance = Vector3.Distance(
            targetObject.position,
            rightController.position
        );

        bool controllerNearby =
            leftDistance <= showDistance ||
            rightDistance <= showDistance;

        SetRingsVisible(controllerNearby && !isGrabbed);
    }

    private void ConfigureRing(LineRenderer ring, float radius)
    {
        if (ring == null)
            return;

        int count = Mathf.Max(3, segments);

        ring.useWorldSpace = false;
        ring.loop = true;
        ring.positionCount = count;
        ring.startWidth = ringWidth;
        ring.endWidth = ringWidth;

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            ring.SetPosition(i, new Vector3(x, 0f, z));
        }
    }

    private void SetRingsVisible(bool visible)
    {
        if (innerRing != null)
            innerRing.enabled = visible;

        if (outerRing != null)
            outerRing.enabled = visible;

        if (magicRunes != null)
        {
            foreach (Renderer rune in magicRunes)
            {
                if (rune != null)
                    rune.enabled = visible;
            }
        }
    }

    void OnValidate()
    {
        ConfigureRing(GetComponent<LineRenderer>(), ringRadius);

        if (outerRing != null)
            ConfigureRing(outerRing, outerRingRadius);
    }
}
