using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Animator animator;
    public Fighter fighter;
    public Fighter playerFighter;
    public float attackCooldown = 2f;
    public float blockChance = 0.3f;
    public float blockDuration = 1.5f;

    private NavMeshAgent agent;
    private float attackTimer = 0f;
    private float blockTimer = 0f;
    private bool isDead = false;

    private enum State { Chase, Attack, Block }
    private State currentState = State.Chase;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        fighter.OnDeath += OnDeath;
    }

    void Update()
    {
        if (isDead || playerFighter == null || playerFighter.currentHP <= 0) return;

        float dist = Vector3.Distance(transform.position, playerFighter.transform.position);
        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Chase:
                Chase(dist);
                break;
            case State.Attack:
                Attack();
                break;
            case State.Block:
                Block();
                break;
        }

        // 更新动画速度参数
        float spd = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speed", spd);
    }

    void Chase(float dist)
    {
        agent.isStopped = false;
        agent.SetDestination(playerFighter.transform.position);

        // 进入攻击范围
        if (dist <= fighter.attackRange && attackTimer <= 0f)
        {
            // 随机决定格挡还是攻击
            if (Random.value < blockChance)
            {
                currentState = State.Block;
                blockTimer = blockDuration;
                agent.isStopped = true;
                fighter.isBlocking = true;
                animator.SetBool("block", true);
            }
            else
            {
                currentState = State.Attack;
                agent.isStopped = true;
                FaceTarget();
                fighter.isAttacking = true;
                animator.SetTrigger("slashUp");
            }
        }
    }

    void Attack()
    {
        // 等待攻击动画完成（通过 OnAttackEnd 回调）
    }

    void Block()
    {
        blockTimer -= Time.deltaTime;
        if (blockTimer <= 0f)
        {
            fighter.isBlocking = false;
            animator.SetBool("block", false);
            currentState = State.Chase;
        }
    }

    void FaceTarget()
    {
        Vector3 dir = (playerFighter.transform.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    // 在攻击动画的伤害帧通过 Animation Event 调用
    public void OnDealDamage()
    {
        fighter.DealDamageInRange();
    }

    // 在 SlashDown 动画结束时通过 Animation Event 调用
    public void OnAttackEnd()
    {
        fighter.isAttacking = false;
        attackTimer = attackCooldown;
        currentState = State.Chase;
    }

    // SlashUp 播完自动触发 SlashDown
    public void OnSlashUpEnd()
    {
        animator.SetTrigger("slashDown");
    }

    void OnDeath()
    {
        isDead = true;
        agent.isStopped = true;
        // 如果有死亡动画参数则播放，否则直接隐藏
        // animator.SetTrigger("die");
        gameObject.SetActive(false);
    }
}
