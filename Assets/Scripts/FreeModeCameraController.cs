using UnityEngine;

/// <summary>
/// 自由模式摄像机控制器
/// - 滚轮控制距离和角度（拉近=距离缩短+角度减小，拉远=距离增大+角度增大到90度正上方）
/// - 屏幕边缘鼠标滑动围绕角色旋转
/// - WASD自由移动摄像机（受Ground边界限制）
/// - 角色移动时摄像机自动跟随保持角色居中
/// </summary>
public class FreeModeCameraController : MonoBehaviour
{
    [Header("目标")]
    public Transform target; // 角色

    [Header("距离设定")]
    public float minDistance = 5f;
    public float maxDistance = 20f;

    [Header("角度设定（X轴俯仰角）")]
    public float minPitch = 30f;  // 最小角度，能完整看到角色
    public float maxPitch = 90f;  // 最大角度，正上方

    [Header("滚轮缩放速度")]
    public float zoomSpeed = 2f;

    [Header("边缘旋转")]
    public float edgeRotateSpeed = 60f;
    public float edgeThreshold = 20f; // 屏幕边缘像素阈值

    [Header("WASD移动")]
    public float panSpeed = 10f;

    [Header("Ground边界")]
    public Transform groundTransform; // Ground对象的Transform

    // 当前参数
    private float currentDistance;
    private float currentPitch;
    private float currentYaw;
    private Vector3 cameraOffset; // WASD产生的偏移量

    // 缩放比例 0=最近 1=最远
    private float zoomT = 0.5f;

    void Start()
    {
        // 初始化为中间值
        currentDistance = Mathf.Lerp(minDistance, maxDistance, zoomT);
        currentPitch = Mathf.Lerp(minPitch, maxPitch, zoomT);
        currentYaw = 0f;
        cameraOffset = Vector3.zero;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleZoom();
        HandleEdgeRotation();
        HandlePan();
        HandleFocusTarget();
        UpdateCameraPosition();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // 滚轮向上=拉近（zoomT减小），滚轮向下=拉远（zoomT增大）
            zoomT -= scroll * zoomSpeed * 0.1f;
            zoomT = Mathf.Clamp01(zoomT);

            currentDistance = Mathf.Lerp(minDistance, maxDistance, zoomT);
            currentPitch = Mathf.Lerp(minPitch, maxPitch, zoomT);
        }
    }

    void HandleEdgeRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        float mouseX = Input.GetAxis("Mouse X");

        // 鼠标在屏幕左边缘且继续向左滑动
        if (mousePos.x <= edgeThreshold && mouseX < 0)
        {
            currentYaw -= edgeRotateSpeed * Time.deltaTime;
        }
        // 鼠标在屏幕右边缘且继续向右滑动
        else if (mousePos.x >= Screen.width - edgeThreshold && mouseX > 0)
        {
            currentYaw += edgeRotateSpeed * Time.deltaTime;
        }
    }

    void HandlePan()
    {
        float h = 0f, v = 0f;
        if (Input.GetKey(KeyCode.A)) h = -1f;
        if (Input.GetKey(KeyCode.D)) h = 1f;
        if (Input.GetKey(KeyCode.W)) v = 1f;
        if (Input.GetKey(KeyCode.S)) v = -1f;

        if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f) return;

        // 基于当前yaw角计算水平面上的移动方向（不受俯仰角影响）
        Vector3 forward = Quaternion.Euler(0, currentYaw, 0) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0, currentYaw, 0) * Vector3.right;

        Vector3 moveDir = (right * h + forward * v).normalized;
        Vector3 newOffset = cameraOffset + moveDir * panSpeed * Time.deltaTime;

        // 检查Ground边界限制
        if (groundTransform != null && Camera.main != null)
        {
            Vector3 groundCenter = groundTransform.position;
            float groundHalfX = groundTransform.localScale.x * 5f;
            float groundHalfZ = groundTransform.localScale.z * 5f;

            // 摄像机看向的地面中心点
            Vector3 lookCenter = target.position + newOffset;

            // 计算当前视野在地面上覆盖的半径（基于距离和角度的近似值）
            Camera cam = Camera.main;
            float verticalFovRad = cam.fieldOfView * Mathf.Deg2Rad;
            float horizontalFovRad = verticalFovRad * cam.aspect;
            // 摄像机到地面的水平距离近似
            float heightAboveGround = currentDistance * Mathf.Sin(currentPitch * Mathf.Deg2Rad);
            float viewHalfZ = heightAboveGround * Mathf.Tan(verticalFovRad * 0.5f);
            float viewHalfX = heightAboveGround * Mathf.Tan(horizontalFovRad * 0.5f);

            // 限制：Ground边界不能越过屏幕中心
            // 即 lookCenter 到 Ground 边界的距离必须 >= 视野半径的一半
            // 换言之：lookCenter.x 不能让左边界到达屏幕中心
            float maxOffsetX = groundHalfX - viewHalfX * 0.5f;
            float maxOffsetZ = groundHalfZ - viewHalfZ * 0.5f;

            // 确保最大偏移不为负（视野比Ground还大时不限制）
            if (maxOffsetX < 0) maxOffsetX = groundHalfX;
            if (maxOffsetZ < 0) maxOffsetZ = groundHalfZ;

            // lookCenter相对于Ground中心的偏移
            float relX = lookCenter.x - groundCenter.x;
            float relZ = lookCenter.z - groundCenter.z;

            // 限制偏移范围
            if (relX < -maxOffsetX || relX > maxOffsetX ||
                relZ < -maxOffsetZ || relZ > maxOffsetZ)
            {
                // 只限制超出的方向
                if (h < 0 && relX < -maxOffsetX) return;
                if (h > 0 && relX > maxOffsetX) return;
                if (v > 0 && relZ > maxOffsetZ) return;
                if (v < 0 && relZ < -maxOffsetZ) return;
            }
        }

        cameraOffset = newOffset;
    }

    void HandleFocusTarget()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            cameraOffset = Vector3.zero;
        }
    }

    Vector3 CalculateCameraPosition(Vector3 offset)
    {
        Vector3 pivotPos = target.position + offset;
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 dir = rotation * Vector3.back;
        return pivotPos + dir * currentDistance;
    }

    Quaternion CalculateCameraRotation()
    {
        return Quaternion.Euler(currentPitch, currentYaw, 0);
    }

    void UpdateCameraPosition()
    {
        Vector3 pivotPos = target.position + cameraOffset;
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 dir = rotation * Vector3.back;

        transform.position = pivotPos + dir * currentDistance;
        transform.rotation = rotation;
    }

    /// <summary>
    /// 当角色移动时，重置偏移使角色回到屏幕中心
    /// </summary>
    public void ResetOffsetToTarget()
    {
        cameraOffset = Vector3.Lerp(cameraOffset, Vector3.zero, 5f * Time.deltaTime);
    }
}
