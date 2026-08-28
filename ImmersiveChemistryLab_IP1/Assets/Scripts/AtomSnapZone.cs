using UnityEngine;

public class AtomSnapZone : MonoBehaviour
{
    public Transform snapPoint;
    public GameObject bondVisual;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hydrogen"))
            return;

        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            
            rb.isKinematic = true;
        }

        other.transform.position = snapPoint.position;
        other.transform.rotation = snapPoint.rotation;

        SimpleAtomGrab grab = other.GetComponent<SimpleAtomGrab>();

        if (grab != null)
        {
            grab.StopGrab();
        }

        if (bondVisual != null)
        {
            bondVisual.SetActive(true);
        }

        Debug.Log("Hydrogen snapped into bond position!");
    }
}