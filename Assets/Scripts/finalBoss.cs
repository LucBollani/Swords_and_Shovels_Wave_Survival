using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class finalBoss : enemy
{
    private float fireBoltTimer;
    private float meteorTimer;
    public GameObject fireBall;
    public GameObject fireBallSky;
    public Transform fireballOrigin;
    public Transform fireballSkyOrigin;

    public override void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        attackZone = GetComponent<SphereCollider>();
        if (FindObjectOfType<PlayerController>())
        {
            target = FindObjectOfType<PlayerController>().gameObject.transform;
        }
        healthBar.setMaxHealth(health);

        oldPos = transform.position;
        newPos = transform.position;
        moveSpeed = agent.speed;
        slowTime = Time.time;

        col = GetComponent<BoxCollider>();
        hit = GetComponent<AudioSource>();

        fireBoltTimer = Time.time + 1f;
        meteorTimer = Time.time + 1f;
    }

    protected override void Update()
    {
        if (target)
        {
            agent.SetDestination(target.position);
            updateAnimation();
            if (Time.time < slowTime && spdstate == speedState.NORMAL)
            {
                Debug.Log("slow");
                agent.speed = moveSpeed / 1.5f;
                spdstate = speedState.SLOW;
            }
            else if (Time.time > slowTime && spdstate == speedState.SLOW)
            {
                agent.speed = moveSpeed;
                spdstate = speedState.NORMAL;
            }
            if (Time.time > fireBoltTimer)
            {
                if (Vector3.Distance(transform.position, target.position) < 10)
                {
                    fireBoltTimer = Time.time + 0.5f;
                    shotgunClose(target.position);
                }
                else if (Vector3.Distance(transform.position, target.position) > 10 && Vector3.Distance(transform.position, target.position) < 20)
                {
                    fireBoltTimer = Time.time + 1.5f;
                    shotgunFar(target.position);
                }
            }
            if (Time.time > meteorTimer)
            {
                meteorTimer = Time.time + 0.5f;
                skyShoot(target.position);
            }
        }
        else
        {
            agent.speed = 0f;
            updateAnimation();
        }
    }

    protected virtual void shoot(Vector3 playerPos)
    {
        //anim.SetTrigger("Attack_1");

        playerPos += Vector3.up * 0.8f;
        Quaternion shootRotation = Quaternion.LookRotation(playerPos, Vector3.up);
        GameObject ins = Instantiate(fireBall, fireballOrigin.position, shootRotation);
        ins.transform.LookAt(playerPos);
    }

    protected virtual void shotgunClose(Vector3 playerPos)
    {
        anim.SetTrigger("Attack_2");
        playerPos += Vector3.up * 0.8f;
        fireballOrigin.LookAt(playerPos);
        Quaternion shootRotation = fireballOrigin.rotation;
        shootRotation *= Quaternion.Euler(0, -60, 0);
        for (int i = 0; i < 4; i++)
        {
            Instantiate(fireBall, fireballOrigin.position, shootRotation);
            shootRotation *= Quaternion.Euler(0, 40, 0);
        }
        
    }

    protected virtual void shotgunFar(Vector3 playerPos)
    {
        anim.SetTrigger("Attack_2");
        playerPos += Vector3.up * 0.8f;
        fireballOrigin.LookAt(playerPos);
        Quaternion shootRotation = fireballOrigin.rotation;
        shootRotation *= Quaternion.Euler(0, -20, 0);
        for (int i = 0; i < 10; i++)
        {
            Instantiate(fireBall, fireballOrigin.position, shootRotation);
            shootRotation *= Quaternion.Euler(0, 5, 0);
        }

    }

    protected virtual void skyShoot(Vector3 playerPos)
    {
        Vector3 origin = playerPos + new Vector3(Random.Range(-10, 10), 10, Random.Range(-10, 10));
        GameObject ins = Instantiate(fireBallSky, origin, Quaternion.identity * Quaternion.Euler(90, 0, 0));
        Debug.Log("Shot:" + ins);
    }

    protected override void checkAlive()
    {
        if (health <= 0)
        {
            target.GetComponent<PlayerController>().hasteTick();
            Destroy(gameObject, 1.5f);
            anim.SetBool("Die", true);
            target = null;
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().AddForce(Vector3.up * 3000);
            foreach (Collider c in GetComponents<Collider>()) c.enabled = false;
            foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;
            GameObject statue = GameObject.FindGameObjectWithTag("statue");
            Debug.Log("THIS IS :" + statue);
            if (statue != null) Destroy(statue);
        }
    }
}
