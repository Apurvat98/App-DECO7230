using UnityEngine;

public class SimplePourEffect : MonoBehaviour
{
    public Transform beaker;
    public ParticleSystem pourParticles;

    public float tiltRequired = 45f;
    public float maxPourDistance = 1.0f;

    private bool pouring = false;

    void Start()
    {
        if (pourParticles != null)
        {
            pourParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }

    void Update()
    {
        if (beaker == null || pourParticles == null)
            return;

        float tiltAngle =
            Vector3.Angle(transform.up, Vector3.up);

        float distanceToBeaker =
            Vector3.Distance(transform.position, beaker.position);

        bool shouldPour =
    distanceToBeaker <= maxPourDistance;

        if (shouldPour && !pouring)
        {
            pouring = true;
            pourParticles.Play();
        }
        else if (!shouldPour && pouring)
        {
            pouring = false;
            pourParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }
    }
}