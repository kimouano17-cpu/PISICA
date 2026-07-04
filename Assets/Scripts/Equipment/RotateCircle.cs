using UnityEngine;

public class RotateCircle : MonoBehaviour
{
    public float rotateSpeed = 40f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}