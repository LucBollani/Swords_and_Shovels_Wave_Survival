using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    public int health = 10;
    public float attackRate = 1;
    public int attackDamage = 2;
    public HealthBar healthBar;
    public GameObject floatingText;
    public Transform floatTransf;

    protected Transform target;
    protected NavMeshAgent agent;
    protected Animator anim;
    protected SphereCollider attackZone;
    protected float moveSpeed;
    protected float slowTime;
    protected enum speedState { NORMAL, SLOW }
    protected speedState spdstate = speedState.NORMAL;

    protected bool jumping = false;
    protected float attackTimer = 0;
    protected float invulnerablityTime = 0f;
    protected BoxCollider col;

    protected Vector3 oldPos, newPos;

    protected AudioSource hit;

    public virtual void Start()
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
    }

    public void applyDamage(int damage)
    {
        if (Time.time > invulnerablityTime + 0.1f)
        {
            health -= damage;
            if (floatingText != null) showFloatingText(damage);
            healthBar.setHealth(health);
            checkAlive();
            invulnerablityTime = Time.time;
            hit.Play();
        }
    }

    protected float calculateVelocity()
    {
        newPos = transform.position;
        float velocity = (newPos - oldPos).magnitude;
        oldPos = newPos;
        //Debug.Log("Calculated Velocity: " + velocity);
        return velocity;
    }

    protected virtual void checkAlive()
    {
        if(health <= 0)
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
        }
    }

    protected void showFloatingText(int dmg)
    {
        var go = Instantiate(floatingText, transform.position, Quaternion.identity, floatTransf);
        go.GetComponentInChildren<TextMeshProUGUI>().text = dmg.ToString();
    }

    protected virtual void Update()
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
            else if(Time.time > slowTime && spdstate == speedState.SLOW)
            {
                agent.speed = moveSpeed;
                spdstate = speedState.NORMAL;
            }
        }
        else
        {
            agent.speed = 0f;
            updateAnimation();
        }
    }

    protected void updateAnimation()
    {
        anim.SetFloat("moveSpeed", calculateVelocity() * 100);
        if (agent.isOnOffMeshLink && !jumping)
        {
            anim.SetBool("jumping", true);
            jumping = true;
        }
        else if (!agent.isOnOffMeshLink)
        {
            anim.SetBool("jumping", false);
            jumping = false;
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && canAttack())
        {
            Attack(other.gameObject);
        }
    }

    protected bool canAttack()
    {
        if (Time.time > attackTimer + attackRate)
        {
            attackTimer = Time.time;
            return true;
        }
        return false;
    }

    protected virtual void Attack(GameObject target)
    {
        if (target.GetComponent<PlayerController>().dead) return;
        anim.SetTrigger("Attack_1");
        target.GetComponent<PlayerController>().applyDamage(attackDamage);
    }

    public void slow()
    {
        slowTime = Time.time + 1f;
    }

}
