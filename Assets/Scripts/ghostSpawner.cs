using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ghostSpawner : MonoBehaviour
{
    public int health = 50;
    public HealthBar healthBar;
    public GameObject floatingText;
    public Transform floatTransf;
    private Transform target;
    public GameObject ghost;
    public float spawnPeriod = 1;
    private float spawnTimer;

    protected BoxCollider col;
    protected AudioSource hit;

    public void Start()
    {
        col = GetComponent<BoxCollider>();
        hit = GetComponent<AudioSource>();
        healthBar.setMaxHealth(health);
        spawnTimer = Time.time;
        target = FindObjectOfType<PlayerController>().gameObject.transform;
    }

    private void Update()
    {
        if (Time.time > spawnTimer)
        {
            Instantiate(ghost, transform.position, transform.rotation);
            spawnTimer = Time.time + spawnPeriod;
        }
    }

    protected void checkAlive()
    {
        if (health <= 0)
        {
            for (int i = 0; i < 5; i++) target.GetComponent<PlayerController>().hasteTick();
            Destroy(gameObject, 1.5f);
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().AddForce(Vector3.up * 3000);
            col.enabled = false;
        }
    }

    protected void showFloatingText(int dmg)
    {
        var go = Instantiate(floatingText, transform.position, Quaternion.identity, floatTransf);
        go.GetComponentInChildren<TextMeshProUGUI>().text = dmg.ToString();
    }

    public void applyDamage(int damage)
    {
        {
            health -= damage;
            if (floatingText != null) showFloatingText(damage);
            healthBar.setHealth(health);
            checkAlive();
            hit.Play();
        }
    }
}
