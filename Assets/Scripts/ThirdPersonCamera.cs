using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 2.5f;
    public float height = 2f;
    public float mouseSensitivity = 3f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    private float yaw;
    private float pitch;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 暂停时不处理鼠标旋转
        if (PauseMenuManager.IsPaused) return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        Vector3 targetPos = target.position + Vector3.up * height;

        transform.position = targetPos + offset;
        transform.LookAt(targetPos);
    }
}
