
using UnityEngine;

public class HeatingInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform heatingZone;
    public Transform flaskHeatPoint;
    public Transform flaskTransform;
    public SimpleAtomGrab heatingFlask;

    [Header("Heating Detection")]
    public float heatingRadius = 0.30f;
    public float maxVerticalDistance = 0.25f;
    public float requiredHeatingTime = 4f;
    public float requiredTiltMovement = 70f;

    [Header("Reaction Feedback")]
    public Renderer flaskRenderer;
    public Material heatedMaterial;
    public ParticleSystem reactionParticles;

    [Header("Burner Flame")]
    public ParticleSystem burnerFlame;
    public float flameStopDelay = 1.5f;

    private float heatingTime = 0f;
    private float tiltMovement = 0f;
    private Vector3 previousUp;

    private bool wasHeating = false;
    private bool completed = false;
    private bool flameActive = false;

    void Start()
    {
        // Ensure the burner starts switched off.
        if (burnerFlame != null)
        {
            burnerFlame.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }

    void Update()
    {
        if (completed)
            return;

        if (heatingZone == null ||
            flaskHeatPoint == null ||
            flaskTransform == null ||
            heatingFlask == null)
        {
            SetFlame(false);
            return;
        }

        Vector3 offset =
            flaskHeatPoint.position - heatingZone.position;

        float horizontalDistance =
            new Vector2(offset.x, offset.z).magnitude;

        float verticalDistance =
            Mathf.Abs(offset.y);

        bool inHeatingArea =
            heatingFlask.IsGrabbed &&
            horizontalDistance <= heatingRadius &&
            verticalDistance <= maxVerticalDistance;

        // Flame responds to flask position.
        SetFlame(inHeatingArea);

        if (!inHeatingArea)
        {
            wasHeating = false;
            return;
        }

        heatingTime += Time.deltaTime;

        Vector3 currentUp = flaskTransform.up;

        if (wasHeating)
        {
            float angle =
                Vector3.Angle(previousUp, currentUp);

            // Ignore sudden simulator rotation jumps.
            if (angle >= 0.1f && angle <= 12f)
            {
                tiltMovement += angle;
            }
        }

        previousUp = currentUp;
        wasHeating = true;

        if (heatingTime >= requiredHeatingTime &&
            tiltMovement >= requiredTiltMovement)
        {
            CompleteHeating();
        }
    }

    private void SetFlame(bool shouldPlay)
    {
        if (burnerFlame == null ||
            flameActive == shouldPlay)
            return;

        flameActive = shouldPlay;

        if (shouldPlay)
        {
            burnerFlame.Play();
        }
        else
        {
            burnerFlame.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }
    }

    private void CompleteHeating()
    {
        completed = true;

        Debug.Log("HEATING COMPLETE!");

        // Change the liquid colour.
        if (flaskRenderer != null &&
            heatedMaterial != null)
        {
            flaskRenderer.material = heatedMaterial;
        }

        // Start the steam effect.
        if (reactionParticles != null)
        {
            reactionParticles.Play();
        }

        // Keep flame visible briefly after completion.
        if (burnerFlame != null)
        {
            StartCoroutine(StopFlameAfterDelay());
        }
    }

    private System.Collections.IEnumerator StopFlameAfterDelay()
    {
        yield return new WaitForSeconds(flameStopDelay);

        SetFlame(false);
    }

    private void OnDisable()
    {
        if (burnerFlame != null)
        {
            burnerFlame.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }

        flameActive = false;
    }
}
