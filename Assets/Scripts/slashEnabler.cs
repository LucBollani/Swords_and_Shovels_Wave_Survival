using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slashEnabler : MonoBehaviour
{
    private BoxCollider col;
    private TrailRenderer tr;
    private Animator anim;
    private Transform player;
    private Quaternion orientation;
    private bool preAttack = false;
    private bool preAttack2 = false;
    private bool attack = false;
    private bool attack2 = false;
    public Transform dad;
    private AudioSource swingSound;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponentInChildren<BoxCollider>();
        tr = GetComponentInChildren<TrailRenderer>();
        col.enabled = false;
        tr.enabled = false;
        swingSound = GetComponent<AudioSource>();

        if (FindObjectOfType<PlayerController>())
        {
            player = FindObjectOfType<PlayerController>().gameObject.transform;
        }

        orientation = player.rotation;
    }

    private void Update()
    {
        dad.position = player.position;
        if (attack)
        {
            //Debug.Log("Attacking");
            anim.SetTrigger("Slash");
            swingSound.Play();
            attack = false;
        }
        /*else if (attack2)
        {
            anim.SetTrigger("Slash2");
            swingSound.Play();
            attack2 = false;
        }*/
    }

    public void toggleCollider()
    {
        col.enabled = !col.enabled;
        tr.enabled = !tr.enabled;
    }

    public void setOrientationToAttack(Quaternion ori)
    {
        orientation = ori;
        //if (improved) preAttack2 = true;
        preAttack = true;
    }


    void LateUpdate()
    {
        if (preAttack)
        {
            preAttack = false;
            dad.rotation = orientation;
            attack = true;
        }
        /*else if (preAttack2)
        {
            preAttack2 = false;
            dad.rotation = orientation;
            attack2 = true;
        }*/
    }
}
