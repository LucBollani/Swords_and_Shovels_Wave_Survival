using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyRanged : enemy
{
    public GameObject arrow;

    protected override void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player") {
            transform.LookAt(other.transform);
            if(canAttack())
            {
                shoot(other.bounds.center);
            }
        }
        
    }

    protected virtual void shoot(Vector3 playerPos)
    {
        anim.SetTrigger("Attack_1");

        Quaternion shootRotation = Quaternion.LookRotation(playerPos, Vector3.up);
        GameObject ins = Instantiate(arrow, col.bounds.center, shootRotation);
        ins.transform.LookAt(playerPos);
        Debug.Log("Shot:" + ins);
    }
}
