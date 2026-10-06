using UnityEngine;

public class BondStretchBreak : MonoBehaviour
{
    public Transform oxygen;
    public Transform hydrogen;
    public GameObject bondVisual;

    public float breakDistance = 1.4f;

    private bool bonded = false;

    private Vector3 originalBondScale;

    void Start()
    {
        if (bondVisual != null)
        {
            originalBondScale = bondVisual.transform.localScale;
        }
    }

    void Update()
    {
        if (oxygen == null || hydrogen == null || bondVisual == null)
            return;

        // If the snap system switched the bond on,
        // we now consider this atom bonded.
        if (bondVisual.activeSelf && !bonded)
        {
            bonded = true;
        }

        if (!bonded)
            return;

        float distance = Vector3.Distance(
            oxygen.position,
            hydrogen.position
        );

        // Stretch/reposition bond while Hydrogen moves
        UpdateBondVisual();

        // Break when pulled far enough
        if (distance > breakDistance)
        {
            BreakBond();
        }
    }

    void UpdateBondVisual()
    {
        Vector3 midpoint =
            (oxygen.position + hydrogen.position) / 2f;

        bondVisual.transform.position = midpoint;

        Vector3 direction =
            hydrogen.position - oxygen.position;

        float distance = direction.magnitude;

        bondVisual.transform.up = direction.normalized;

        Vector3 scale = originalBondScale;

        // Unity Cylinder length runs along Y
        scale.y = distance / 2f;

        bondVisual.transform.localScale = scale;
    }

    void BreakBond()
    {
        bonded = false;

        bondVisual.SetActive(false);

        Debug.Log(hydrogen.name + " BOND BROKEN!");
    }
}