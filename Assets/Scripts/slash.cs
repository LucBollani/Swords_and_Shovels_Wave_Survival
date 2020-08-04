using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slash : MonoBehaviour
{
    public bool slow = false;

     private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && !other.isTrigger)
        {
            Debug.Log(slow);
            if (slow)
            {
                other.GetComponent<enemy>().slow();
                other.GetComponent<enemy>().applyDamage(8);
            }
            else other.GetComponent<enemy>().applyDamage(5);
        }
        else if (other.tag == "Ghost")
        {
            if (other.GetComponent<enemyGhost>().isWeirdo)
            {
                other.GetComponent<enemyGhost>().applyDamage(5);
            }
            else other.GetComponent<enemyGhost>().applyDamage(3);
        }
        else if (other.tag == "statue")
        {
            other.GetComponent<ghostSpawner>().applyDamage(10);
        }
    }
}
