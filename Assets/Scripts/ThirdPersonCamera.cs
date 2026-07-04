using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float followSpeed = 10f;
    public float rotateSpeed = 5f;

    float mouseX;

    void Start()
    {

    }
    void LateUpdate()
    {
        mouseX += Input.GetAxis("Mouse X") * rotateSpeed;

        Quaternion rotation = Quaternion.Euler(0, mouseX, 0);

        Vector3 position = target.position - (rotation * Vector3.forward * distance);
        position.y += height;

        transform.position = Vector3.Lerp(transform.position, position, followSpeed * Time.deltaTime);

        transform.LookAt(target);
    }
}