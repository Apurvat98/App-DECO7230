using UnityEngine;

public class ExpansionCueAnimation : MonoBehaviour
{
    public Transform leftArrow;
    public Transform rightArrow;

    public float moveDistance = 0.35f;
    public float speed = 2f;

    private Vector3 leftStart;
    private Vector3 rightStart;

    void Start()
    {
        if (leftArrow != null)
            leftStart = leftArrow.localPosition;

        if (rightArrow != null)
            rightStart = rightArrow.localPosition;
    }

    void Update()
    {
        float movement = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float offset = movement * moveDistance;

        if (leftArrow != null)
            leftArrow.localPosition = leftStart + Vector3.left * offset;

        if (rightArrow != null)
            rightArrow.localPosition = rightStart + Vector3.right * offset;
    }
}