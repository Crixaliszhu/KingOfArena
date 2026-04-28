using UnityEngine;

/// <summary>
/// 自由模式下的玩家控制器，只有移动功能，无战斗
/// </summary>
public class FreeModePlayer : MonoBehaviour
{
    public float maxSpeed = 4f;
    public Animator animator;
    public Transform cameraTransform;

    private CharacterController controller;
    private float speed = 0f;
    private float acceleration = 2f;
    private float gravity = -9.8f;
    private float verticalSpeed = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * z + camRight * x;

        if (moveDir.magnitude > 0.01f)
        {
            speed = Mathf.MoveTowards(speed, maxSpeed, acceleration * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDir),
                10f * Time.deltaTime
            );
        }
        else
        {
            speed = 0;
        }

        if (controller.isGrounded)
            verticalSpeed = -0.5f;
        else
            verticalSpeed += gravity * Time.deltaTime;

        Vector3 movement = moveDir.normalized * speed * Time.deltaTime;
        movement.y = verticalSpeed * Time.deltaTime;
        controller.Move(movement);

        if (animator) animator.SetFloat("speed", speed);
    }
}
