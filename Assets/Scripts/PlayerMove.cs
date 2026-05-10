using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 0;
    public Animator animator;
    public Transform cameraTransform;
    public Fighter fighter;
    public Fighter enemyFighter;

    private CharacterController controller;
    private float maxSpeed = 2;
    private float addSpeed = 1f;
    private bool isAttacking = false;
    private float gravity = -9.8f;
    private float verticalSpeed = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (fighter.currentHP <= 0) return;

        // 鼠标左键按下 → 抬剑
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            fighter.isAttacking = true;
            animator.SetTrigger("slashUp");
        }

        // 鼠标左键释放 → 砍下
        if (Input.GetMouseButtonUp(0) && isAttacking)
        {
            animator.SetTrigger("slashDown");
        }

        // 鼠标右键按下 → 格挡
        if (Input.GetMouseButtonDown(1))
        {
            fighter.isBlocking = true;
            animator.SetBool("block", true);
        }

        // 鼠标右键释放 → 取消格挡
        if (Input.GetMouseButtonUp(1))
        {
            fighter.isBlocking = false;
            animator.SetBool("block", false);
        }

        getMove();
    }

    // 在 SlashDown 攻击动画的伤害帧通过 Animation Event 调用
    public void OnDealDamage()
    {
        fighter.DealDamageInRange();
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
        fighter.isAttacking = false;
    }

    // 防止共用动画 clip 上的 Event 报错
    public void OnSlashUpEnd() { }

    void getMove()
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
            float realSpeed = speed + addSpeed * Time.deltaTime;
            if (realSpeed > maxSpeed) realSpeed = maxSpeed;
            speed = realSpeed;

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

        animator.SetFloat("speed", speed);
    }
}
