using UnityEngine;
using System;

public class Fighter : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    public int attackDamage = 20;
    public float attackRange = 2f;

    [HideInInspector] public bool isBlocking = false;
    [HideInInspector] public bool isAttacking = false;

    public event Action<int, int> OnHPChanged; // currentHP, maxHP
    public event Action OnDeath;

    void Start()
    {
        currentHP = maxHP;
    }

    // 尝试对目标造成伤害，在攻击动画的关键帧调用
    public void TryDealDamage(Fighter target)
    {
        if (target == null || target.currentHP <= 0) return;

        float dist = Vector3.Distance(transform.position, target.transform.position);

        // 超出攻击范围，不造成伤害
        if (dist > attackRange) return;

        // 对方正在防御，不造成伤害
        if (target.isBlocking) return;

        target.TakeDamage(attackDamage);
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
