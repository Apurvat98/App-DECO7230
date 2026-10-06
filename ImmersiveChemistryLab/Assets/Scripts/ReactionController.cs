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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ReactantA"))
        {
            hasReactantA = true;
            Debug.Log("Reactant A detected!");
        }

        if (other.CompareTag("ReactantB"))
        {
            hasReactantB = true;
            Debug.Log("Reactant B detected!");
        }

        if (hasReactantA && hasReactantB && !reactionStarted)
        {
            StartReaction();
        }
    }

    private void StartReaction()
{
    reactionStarted = true;

    Debug.Log("REACTION STARTED!");

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

    if (reactionBeakerRenderer != null && reactionActiveMaterial != null)
    {
        reactionBeakerRenderer.material = reactionActiveMaterial;
    }

    if (molecularFocus != null)
{
    molecularFocus.SetActive(true);

    MolecularScaleGesture scaleGesture =
        molecularFocus.GetComponent<MolecularScaleGesture>();

    if (scaleGesture != null)
    {
        scaleGesture.BeginGesture();
    }
}
}
}