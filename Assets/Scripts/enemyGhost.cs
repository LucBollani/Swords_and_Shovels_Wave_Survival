using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class enemyGhost : MonoBehaviour
{
    public int health = 10;
    public float attackRate = 1;
    public int attackDamage = 2;
    public HealthBar healthBar;
    public GameObject floatingText;
    public Transform floatTransf;

    protected Transform target;
    protected Animator anim;
    protected SphereCollider attackZone;

    protected bool jumping = false;
    protected float attackTimer = 0;
    protected float invulnerablityTime = 0f;
    public bool isWeirdo = false;

    protected Vector3 oldPos, newPos;

    protected AudioSource hit;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        attackZone = GetComponent<SphereCollider>();
        if (FindObjectOfType<PlayerController>())
        {
            target = FindObjectOfType<PlayerController>().gameObject.transform;
        }
        healthBar.setMaxHealth(health);

        oldPos = transform.position;
        newPos = transform.position;

        hit = GetComponent<AudioSource>();

        if (Random.Range(0, 10) >= 7)
        {
            Debug.Log("WIRDO");
            isWeirdo = true;
            Destroy(gameObject, 20);
        }
    }

    public void applyDamage(int damage)
    {
        if (Time.time > invulnerablityTime + 0.3f)
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

    protected void checkAlive()
    {
        if (health <= 0)
        {
            target.GetComponent<PlayerController>().hasteTick();
            Destroy(gameObject, 1.5f);
            anim.SetBool("Die", true);
            target = null;
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

    protected void Update()
    {
        if (target)
        {
            if (isWeirdo)
            {
                transform.Translate(target.position * Time.deltaTime);
                transform.LookAt(target.position);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime);
            }
            updateAnimation();
            
        }
        else
        {
            updateAnimation();
        }
    }

    protected void updateAnimation()
    {
        anim.SetFloat("moveSpeed", calculateVelocity() * 100);
    }

    protected void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && canAttack())
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
        if (isWeirdo) target.GetComponent<PlayerController>().applyDamage(1);
        else target.GetComponent<PlayerController>().applyDamage(attackDamage);
    }


}
