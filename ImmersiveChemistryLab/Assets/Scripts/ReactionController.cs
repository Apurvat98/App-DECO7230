using UnityEngine;

public class ReactionController : MonoBehaviour
{
    private bool hasReactantA = false;
    private bool hasReactantB = false;
    private bool reactionStarted = false;

    public GameObject reactionEffect;

    public Renderer reactionBeakerRenderer;
    public Material reactionActiveMaterial;

    public GameObject molecularFocus;

    // Called by SimplePourEffect after the user
    // physically tilts and pours a reactant.
    public void RegisterReactant(string reactantTag)
    {
        if (reactionStarted)
            return;

        if (reactantTag == "ReactantA")
        {
            hasReactantA = true;

            Debug.Log(
                "Reactant A poured into beaker!"
            );
        }

        if (reactantTag == "ReactantB")
        {
            hasReactantB = true;

            Debug.Log(
                "Reactant B poured into beaker!"
            );
        }

        if (hasReactantA &&
            hasReactantB &&
            !reactionStarted)
        {
            StartReaction();
        }
    }

    private void StartReaction()
    {
        reactionStarted = true;

        Debug.Log("REACTION STARTED!");

        // ----------------------------------------
        // REACTION PARTICLES
        // ----------------------------------------

        if (reactionEffect != null)
        {
            reactionEffect.SetActive(true);

            ParticleSystem particles =
                reactionEffect.GetComponent<ParticleSystem>();

            if (particles != null)
            {
                particles.Play();
            }
        }

        // ----------------------------------------
        // CHANGE BEAKER MATERIAL
        // ----------------------------------------

        if (reactionBeakerRenderer != null &&
            reactionActiveMaterial != null)
        {
            reactionBeakerRenderer.material =
                reactionActiveMaterial;
        }

        // ----------------------------------------
        // ENABLE MOLECULAR TRANSITION
        // ----------------------------------------

        if (molecularFocus != null)
        {
            molecularFocus.SetActive(true);

            MolecularScaleGesture scaleGesture =
                molecularFocus.GetComponent<
                    MolecularScaleGesture
                >();

            if (scaleGesture != null)
            {
                scaleGesture.BeginGesture();
            }
        }
    }
}