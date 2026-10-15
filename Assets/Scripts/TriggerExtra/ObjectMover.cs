using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] Vector3 startPos, endPos;
    Vector3 offsetPos;
    [SerializeField] float duration;
    [SerializeField] float WaitTime;
    void Start()
    {
        offsetPos = transform.localPosition;
    }

    public void OpenDoor()
    {
        StartCoroutine(DoorOpenCoroutine());
    }
    public void CloseDoor()
    {
        transform.localPosition = offsetPos + startPos;
    }

    IEnumerator DoorOpenCoroutine()
    {
        // Wait before starting the door's movement animation.
        yield return new WaitForSeconds(WaitTime);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Vector3 newPos = Vector3.Lerp(startPos, endPos, easeOutQuart(t));
            transform.localPosition = offsetPos + newPos;
            yield return null;
        }
    }

    float easeOutQuart(float x) => 1 - Mathf.Pow(1 - x, 4);
}
