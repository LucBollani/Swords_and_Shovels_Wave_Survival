using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorFire : MonoBehaviour
{
    private float timer;

    private void Awake()
    {
        timer = Time.time;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && Time.time > timer)
        {
            timer = Time.time + 0.08f;
            other.GetComponent<PlayerController>().applyDamage(2);
        }
    }
}
