
using UnityEngine;

public class PortalVisualAnimation : MonoBehaviour
{
    [Header("Portal Rings")]
    public LineRenderer outerRing;
    public LineRenderer innerRing;

    [Header("Pulse Animation")]
    public float pulseSpeed = 2f;
    public float minWidth = 0.009f;
    public float maxWidth = 0.016f;

    [Header("Golden Colours")]
    public Color dimColor = new Color(0.85f, 0.58f, 0.18f, 1f);
    public Color brightColor = new Color(1f, 0.95f, 0.65f, 1f);

    private void OnEnable()
    {
        FindRings();
    }

    private void Start()
    {
        FindRings();
    }

    private void FindRings()
    {
        // Rings are created at runtime by MolecularScaleGesture.
        Transform outer = transform.Find("PortalOuterRing");
        Transform inner = transform.Find("PortalInnerRing");

        if (outer != null)
            outerRing = outer.GetComponent<LineRenderer>();

        if (inner != null)
            innerRing = inner.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (outerRing == null || innerRing == null)
        {
            FindRings();

            if (outerRing == null || innerRing == null)
                return;
        }

        float pulse =
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        float width = Mathf.Lerp(
            minWidth,
            maxWidth,
            pulse
        );

        Color colour = Color.Lerp(
            dimColor,
            brightColor,
            pulse
        );

        // Animate only visual properties.
        // Do not change ring positions or portal detection.
        outerRing.startWidth = width;
        outerRing.endWidth = width;
        outerRing.startColor = colour;
        outerRing.endColor = colour;

        innerRing.startWidth = width * 0.75f;
        innerRing.endWidth = width * 0.75f;
        innerRing.startColor = colour;
        innerRing.endColor = colour;
    }
}
