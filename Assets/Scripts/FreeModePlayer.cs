using UnityEngine;

/// <summary>
/// 自由模式下的玩家控制器
/// - 鼠标左键点击地面移动角色到目标位置
/// - 匀速移动，移动时播放行走动画
/// - 移动时摄像机自动跟随保持角色居中
/// </summary>
public class FreeModePlayer : MonoBehaviour
{
    public float moveSpeed = 4f;
    public Animator animator;
    public FreeModeCameraController cameraController;
    public LayerMask groundLayer; // 用于射线检测地面

    private CharacterController controller;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float gravity = -9.8f;
    private float verticalSpeed = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleClickToMove();
        MoveToTarget();
    }

    void HandleClickToMove()
    {
        // 鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 射线检测地面
            if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
            {
                targetPosition = hit.point;
                targetPosition.y = transform.position.y; // 保持同一高度
                isMoving = true;

                // 朝向目标
                Vector3 dir = (targetPosition - transform.position).normalized;
                if (dir.magnitude > 0.01f)
                {
                    transform.rotation = Quaternion.LookRotation(dir);
                }
            }
        }
    }

    void MoveToTarget()
    {
        if (!isMoving)
        {
            if (animator) animator.SetFloat("speed", 0f);
            return;
        }

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        float distance = direction.magnitude;

        // 到达目标位置
        if (distance < 0.1f)
        {
            isMoving = false;
            if (animator) animator.SetFloat("speed", 0f);
            return;
        }

        // 匀速移动
        Vector3 moveDir = direction.normalized;

        // 平滑转向
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);

        // 重力
        if (controller.isGrounded)
            verticalSpeed = -0.5f;
        else
            verticalSpeed += gravity * Time.deltaTime;

        Vector3 movement = moveDir * moveSpeed * Time.deltaTime;
        movement.y = verticalSpeed * Time.deltaTime;
        controller.Move(movement);

        if (animator) animator.SetFloat("speed", moveSpeed);

        // 角色移动时，摄像机自动跟随使角色居中
        if (cameraController != null)
        {
            cameraController.ResetOffsetToTarget();
        }
    }
}
