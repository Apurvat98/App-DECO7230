using UnityEngine;
using TMPro;
using System.Collections;

public class MoleculeAssemblyManager : MonoBehaviour
{
    public GameObject bondLeft;
    public GameObject bondRight;

    public GameObject feedbackObject;
    public TMP_Text feedbackText;

    private Coroutine hideRoutine;

    public void RefreshFeedback()
    {
        bool leftFormed =
            bondLeft != null && bondLeft.activeSelf;

        bool rightFormed =
            bondRight != null && bondRight.activeSelf;

        if (leftFormed && rightFormed)
        {
            ShowCompleteMessage();
        }
        else
        {
            ShowTemporaryBondMessage();
        }
    }

    private void ShowTemporaryBondMessage()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }

        feedbackObject.SetActive(true);
        feedbackText.text = "BOND FORMED";

        hideRoutine = StartCoroutine(
            HideAfterDelay(1.5f)
        );
    }

    private void ShowCompleteMessage()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }

        feedbackObject.SetActive(true);

        feedbackText.text =
            "H2O COMPLETE\nWater molecule formed";
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        feedbackObject.SetActive(false);
        hideRoutine = null;
    }
}