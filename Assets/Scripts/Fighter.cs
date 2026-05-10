using UnityEngine;
using System;

public enum FighterTeam
{
    Player,
    Enemy
}

public class Fighter : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    public int attackDamage = 20;
    public float attackRange = 2f;
    public FighterTeam team = FighterTeam.Player;

    [HideInInspector] public bool isBlocking = false;
    [HideInInspector] public bool isAttacking = false;

    public event Action<int, int> OnHPChanged;
    public event Action OnDeath;

    void Start()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// 对指定目标造成伤害（保留旧接口，兼容现有调用）
    /// </summary>
    public void TryDealDamage(Fighter target)
    {
        if (target == null || target.currentHP <= 0) return;

        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (dist > attackRange) return;
        if (target.isBlocking) return;

        target.TakeDamage(attackDamage);
    }

    /// <summary>
    /// 自动检测攻击范围内的敌对目标并造成伤害（支持多敌人）
    /// </summary>
    public void DealDamageInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var hit in hits)
        {
            Fighter target = hit.GetComponent<Fighter>();
            if (target == null) continue;
            if (target == this) continue;
            if (target.team == this.team) continue; // 同阵营不伤害
            if (target.currentHP <= 0) continue;
            if (target.isBlocking) continue;

            target.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        OnHPChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}
