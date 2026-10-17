using UnityEngine;

public class SimplePourEffect : MonoBehaviour
{
    public Transform beaker;
    public ParticleSystem pourParticles;
    public ReactionController reactionController;

    public float tiltRequired = 45f;
    public float maxPourDistance = 1.0f;

    // How long the bottle contains liquid.
    public float pourDuration = 1.5f;

    private bool isPouring = false;
    private bool bottleEmpty = false;

    private float pouredTime = 0f;

    void Start()
    {
        if (pourParticles != null)
        {
            // Make absolutely sure nothing is playing at the start.
            pourParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            pourParticles.Clear(true);
        }
    }

    void Update()
    {
        if (beaker == null || pourParticles == null)
            return;

        // Once empty, this bottle can never pour again.
        if (bottleEmpty)
        {
            if (pourParticles.isPlaying)
            {
                pourParticles.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear
                );

                pourParticles.Clear(true);
            }

            return;
        }

        float tiltAngle =
            Vector3.Angle(
                transform.up,
                Vector3.up
            );

        float distanceToBeaker =
            Vector3.Distance(
                transform.position,
                beaker.position
            );

        bool tiltedEnough =
            tiltAngle >= tiltRequired;

        bool closeEnough =
            distanceToBeaker <= maxPourDistance;

        bool canPour =
            tiltedEnough && closeEnough;

        // -----------------------------------------
        // POUR
        // -----------------------------------------

        if (canPour)
        {
            if (!isPouring)
            {
                StartPouring();
            }

            pouredTime += Time.deltaTime;

            Debug.Log(
                name +
                " POUR TIME: " +
                pouredTime.ToString("F2")
            );

            if (pouredTime >= pourDuration)
            {
                EmptyBottle();
            }
        }

        // -----------------------------------------
        // PAUSE IF BOTTLE IS PUT UPRIGHT / MOVED AWAY
        // -----------------------------------------

        else if (isPouring)
        {
            PausePouring();
        }
    }

    private void StartPouring()
    {
        isPouring = true;

        pourParticles.Play(true);

        Debug.Log(name + " STARTED POURING");
    }

    private void PausePouring()
    {
        isPouring = false;

        pourParticles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );

        pourParticles.Clear(true);

        Debug.Log(name + " PAUSED POURING");
    }

    private void EmptyBottle()
    {
        bottleEmpty = true;
        isPouring = false;

        // HARD STOP.
        pourParticles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );

        pourParticles.Clear(true);

        Debug.Log(name + " BOTTLE EMPTY");

        if (reactionController != null)
        {
            reactionController.RegisterReactant(
                gameObject.tag
            );
        }
    }
}