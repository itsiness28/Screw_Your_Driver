using UnityEngine;

public class MouseCameraP1 : MonoBehaviour
{
    public Transform target;

    public float sensitivity = 3f;
    public float distance = 6f;
    public float height = 3f;

    float yaw;

    void LateUpdate()
    {
        if (target == null) return;

        yaw += Input.GetAxisRaw("Mouse X") * sensitivity;

        Vector3 offset = Quaternion.Euler(0f, yaw, 0f) * new Vector3(0f, 0f, -distance);
        transform.position = target.position + Vector3.up * height + offset;

        transform.LookAt(target.position + Vector3.up * height);
    }
}
