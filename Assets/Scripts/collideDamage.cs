using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collideDamage : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().applyDamage(damage);
        }
    }
}
