using UnityEngine;

public class ItemRotation : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        float newY = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPosition + new Vector3(0, newY, 0);
    }
}
