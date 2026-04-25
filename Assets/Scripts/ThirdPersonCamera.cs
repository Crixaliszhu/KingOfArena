using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;          // 跟随目标（Player）
    public float distance = 2.5f;       // 摄像机与角色的距离
    public float height = 2f;         // 摄像机高于角色的高度
    public float mouseSensitivity = 3f;
    public float minPitch = -20f;     // 俯仰角下限
    public float maxPitch = 60f;      // 俯仰角上限

    private float yaw;   // 水平旋转角
    private float pitch; // 垂直俯仰角

    void Start()
    {
        // 初始化角度为当前摄像机朝向
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 鼠标输入控制旋转
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 计算摄像机位置：围绕目标旋转
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        Vector3 targetPos = target.position + Vector3.up * height;

        transform.position = targetPos + offset;
        transform.LookAt(targetPos);
    }
}
