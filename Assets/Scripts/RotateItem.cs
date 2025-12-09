using UnityEngine;

public class RotateItem : MonoBehaviour
{
    public Vector3 rotateDirection = Vector3.up;
    public float rotateSpeed = 50.0f;
    private void Update()
    {
        RotateObject();
    }

    private void RotateObject()
    {
        gameObject.transform.Rotate(rotateDirection, Time.deltaTime * rotateSpeed);
    }
}
