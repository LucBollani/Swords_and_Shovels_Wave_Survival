using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyGreen : enemy
{
    protected override void Attack(GameObject target)
    {
        anim.SetTrigger("Attack_1");
        PlayerController player = target.GetComponent<PlayerController>();

        player.triggerSlow();
        player.applyDamage(attackDamage);
    }
}
