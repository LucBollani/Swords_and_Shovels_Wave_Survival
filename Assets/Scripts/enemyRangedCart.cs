using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyRangedCart : enemyRanged
{
    public Transform xbow1, xbow2, xbow3, xbow4;

    protected void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (canAttack())
            {
                multiShoot();
            }
        }

    }

    protected void multiShoot()
    {
        Instantiate(arrow, xbow1.position, xbow1.rotation);
        Instantiate(arrow, xbow2.position, xbow2.rotation);
        Instantiate(arrow, xbow3.position, xbow3.rotation);
        Instantiate(arrow, xbow4.position, xbow4.rotation);
    }
}
