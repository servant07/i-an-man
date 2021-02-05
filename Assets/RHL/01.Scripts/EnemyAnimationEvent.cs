using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour
{
    public EnemyFSM enemy;

    public void OnAttack()
    {
        enemy.OnAttack();
    }

    public void OnAttackFInished()
    {
        enemy.OnAttackFInished();
    }
}
