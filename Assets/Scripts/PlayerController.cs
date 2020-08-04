using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 30;
    private int health = 30;
    public float speed = 1f;
    public float jumpForce = 10;
    public float fallMultiplier = 1.5f;
    public float smoothTurn = 1f;
    public float velAnimScale = 100f;
    public float runSpeedMod = 1.3f;
    public GameObject bulletPrefab;
    public GameObject improvedBullet;
    public float bulletForce = 10f;
    public HealthBar healthBar;
    public cooldownBar cdBar;
    public int swordDamage = 5;
    public float swordRange = 1f;
    public float strikeCD = 0.3f;
    public float shootCD = 1f;
    public float ghostCD = 2f;
    public float slowDuration = 1f;
    public float slowIntensity;
    private float slowTimer;

    public GameObject floatingText;
    public Transform floatTransf;

    public Incapacitate inc;

    private Vector3 oldPos, newPos;
    private float velocity;

    private Rigidbody rb;
    private CapsuleCollider col;
    private Animator anim;

    public GameObject bowBack;
    public GameObject bowHand;
    public GameObject swordHand;
    public GameObject swordBack;

    public slashEnabler slash;

    private float cooldownWaitTime = 0;

    private enum Stance { MELEE, RANGED };
    private Stance stance;
    public bool dead;

    private enum Unlockable { BOW, HASTE, IMPROVE, SHIFT, LIFEREGEN };
    private bool[] unlocks = new bool[13];

    public float hasteRefreshTime;
    public float hasteDuration;
    private float hasteKills;
    public float hasteSpeed;
    private float hasteTimer;
    private bool hasteOn;

    private AudioSource hit;
    public AudioSource death;

    public SkinnedMeshRenderer orinalMesh;
    public SkinnedMeshRenderer ghostMesh;
    private bool isGhost = false;

    private float lifeRegenTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        inc = GetComponentInChildren<Incapacitate>();

        bowHand.GetComponent<MeshRenderer>().enabled = false;
        bowBack.GetComponent<MeshRenderer>().enabled = false;
        swordBack.GetComponent<MeshRenderer>().enabled = false;
        stance = Stance.MELEE;

        oldPos = transform.position;
        newPos = transform.position;

        health = maxHealth;
        healthBar.setMaxHealth(health);

        slowTimer = Time.time;
        hasteTimer = Time.time;
        hasteKills = 0;
        hasteOn = false;
        gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
        hit = GetComponent<AudioSource>();

        ghostMesh.enabled = false;

        lifeRegenTimer = Time.time;

        dead = false;
    }


    void FixedUpdate()
    {
        if (dead) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        calculateVelocity();
        movePlayer(moveX, moveZ);
    }

    private void Update()
    {
        if (dead) return;

        
        if (cdBar.slider.value >= cdBar.slider.maxValue || isGhost)
        {
            if (Input.GetKey(KeyCode.LeftShift) && unlocks[(int)Unlockable.SHIFT])
            {
                if (!isGhost)
                {
                    //become ghost
                    isGhost = true;
                    cdBar.setMaxCDValue(ghostCD);
                    cooldownWaitTime = ghostCD;
                    ghostTrigger();
                }
                else if (isGhost)
                {
                    cooldownWaitTime -= Time.deltaTime;
                    cdBar.setCDValue(cooldownWaitTime);
                    if (cooldownWaitTime <= 0)
                    {
                        isGhost = false;
                        ghostTrigger();
                    }
                }
            }
            else if (isGhost)
            {
                isGhost = false;
                ghostTrigger();
            }
            else if (Input.GetKey(KeyCode.Mouse1) && unlocks[(int)Unlockable.BOW])
            {
                if(stance != Stance.RANGED)
                {
                    stance = Stance.RANGED;
                    swapWeapons();
                }
                shoot();
                if (hasteOn) cdBar.setMaxCDValue(shootCD / hasteSpeed);
                else cdBar.setMaxCDValue(shootCD);
                cdBar.setCDValue(0);
                cooldownWaitTime = 0;
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                if (stance != Stance.MELEE)
                {
                    stance = Stance.MELEE;
                    swapWeapons();
                }
                strike();
                if (hasteOn) cdBar.setMaxCDValue(strikeCD / hasteSpeed);
                else cdBar.setMaxCDValue(strikeCD);
                cdBar.setCDValue(0);
                cooldownWaitTime = 0;
            }
        }
        else
        {
            cooldownWaitTime += Time.deltaTime;
            cdBar.setCDValue(cooldownWaitTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump();
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        if (unlocks[(int)Unlockable.LIFEREGEN] && Time.time > lifeRegenTimer)
        {
            heal(1);
            lifeRegenTimer = Time.time + 0.1f;
        }

        anim.SetFloat("Speed",  velocity);
    }

    void ghostTrigger()
    {
        ghostMesh.enabled = !ghostMesh.enabled;
        orinalMesh.enabled = !orinalMesh.enabled;
        GetComponent<CapsuleCollider>().enabled = !GetComponent<CapsuleCollider>().enabled;
        GetComponent<BoxCollider>().enabled = !GetComponent<BoxCollider>().enabled;
        GetComponent<Rigidbody>().isKinematic = !GetComponent<Rigidbody>().isKinematic;
    }

    bool isGrounded()
    {
        RaycastHit hit;

        Physics.SphereCast(col.bounds.center, col.radius * 0.9f, Vector3.down, out hit, col.height/2);

        if (hit.collider)
        {
            return true;
        }

        return false;
    }

    void calculateVelocity()
    {
        newPos = transform.position;
        velocity = (newPos - oldPos).magnitude * velAnimScale;
        oldPos = newPos;
    }

    void movePlayer(float moveX, float moveZ)
    {
        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized * speed * Time.deltaTime;

        if (isSlowed()) movement /= slowIntensity;
        if (hasteOn)
        {
            checkHaste();
            movement *= hasteSpeed;
        }

        rb.MovePosition(transform.position + movement);

        if(moveX != 0 || moveZ != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveX, 0f, moveZ), Vector3.up);
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTurn * Time.deltaTime);
            rb.MoveRotation(newRotation);
        }
    }

    bool isSlowed()
    {
        return slowTimer > Time.time;
    }

    public void triggerSlow()
    {
        slowTimer = Time.time + slowDuration;
    }

    private void jump()
    {
        if (isGrounded())
        {
            anim.SetTrigger("Airborne");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void shoot()
    {
        Vector3 shootPosition = getShootPos();

        Quaternion shootRotation = Quaternion.LookRotation(shootPosition, Vector3.up);

        Instantiate(bulletPrefab, col.bounds.center, shootRotation);

        anim.SetTrigger("Shoot");
    }

    private void strike()
    {
        Vector3 shootPosition = getShootPos();
        slash.setOrientationToAttack(Quaternion.LookRotation(shootPosition, Vector3.up));
        anim.SetTrigger("Strike");
    }

    public void applyDamage(int damage)
    {
        if (isGhost) return;
        health -= damage;
        if (health < 0) health = 0;
        if (floatingText != null) showFloatingText(damage);
        healthBar.setHealth(health);
        hit.Play();
        checkAlive();
    }

    public void heal(int amount)
    {
        if (health + amount > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += amount;
        }
        healthBar.setHealth(health);
    }

    void showFloatingText(int dmg)
    {
        var go = Instantiate(floatingText, transform.position, Quaternion.identity, floatTransf);
        go.GetComponentInChildren<TextMeshProUGUI>().text = dmg.ToString();
    }

    void checkAlive()
    {
        if (health <= 0)
        {
            dead = true;
            AudioSource d = Instantiate(death);
            Destroy(d, 1);

            inc.ActivateRagdoll();
        }
    }

    private void swapWeapons()
    {
        bowHand.GetComponent<MeshRenderer>().enabled = !bowHand.GetComponent<MeshRenderer>().enabled;
        bowBack.GetComponent<MeshRenderer>().enabled = !bowBack.GetComponent<MeshRenderer>().enabled;
        swordHand.GetComponent<MeshRenderer>().enabled = !swordHand.GetComponent<MeshRenderer>().enabled;
        swordBack.GetComponent<MeshRenderer>().enabled = !swordBack.GetComponent<MeshRenderer>().enabled;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        Vector3 shootPosition = getShootPos();

        anim.SetLookAtWeight(1, 0.3f, 1, 1, 1);
        anim.SetLookAtPosition(shootPosition);
    }

    private Vector3 getShootPos()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 shootPosition = (mousePos - playerPos);
        shootPosition.z = shootPosition.y;
        shootPosition.y = col.bounds.center.y;

        return shootPosition;
    }

    public void hasteTick()
    {
        if (unlocks[(int)Unlockable.HASTE] == false) return;
        if (Time.time < hasteTimer)
        {
            hasteKills++;
            //Debug.Log("Refresh hasteCounter: " + hasteKills);
            hasteTimer = Time.time + hasteRefreshTime;
            if (hasteKills >= 5)
            {
                hasteTrigger();
            }
        }
        else
        {
            hasteKills = 1;
            //Debug.Log("Start hasteCounter: " + hasteKills);
            hasteTimer = Time.time + hasteRefreshTime;
        }
    }

    private void hasteTrigger()
    {
        hasteTimer = Time.time + hasteDuration;
        hasteOn = true;
        gameObject.GetComponentInChildren<TrailRenderer>().enabled = true;
    }

    private void checkHaste()
    {
        if (Time.time > hasteTimer)
        {
            //Debug.Log("Haste Ended");
            hasteOn = false;
            hasteKills = 0;
            gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
        }
    }

    public void unlockSkill(int skillIndex)
    {
        Unlockable skill = (Unlockable)skillIndex;
        unlocks[skillIndex] = true;

        if (skill == Unlockable.BOW)
        {
            bowBack.GetComponent<MeshRenderer>().enabled = true;
        }
        else if (skill == Unlockable.IMPROVE)
        {
            slash.GetComponentInChildren<slash>().slow = true;
            bulletPrefab = improvedBullet;
            strikeCD /= 2f;
        }
    }

    public void revive()
    {
        transform.position = new Vector3(-6, 1, 1);
        inc.DeactivateRagdoll();
        dead = false;
    }
}
