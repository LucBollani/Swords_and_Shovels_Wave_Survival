using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Incapacitate : MonoBehaviour
{
    public Transform player;
    private Animator anim;
    private Rigidbody charRigidbody;
    private CapsuleCollider capsuleCollider;
    private BoxCollider boxCollider;

    private Rigidbody[] rigidbodies;
    private Collider[] colliders;


	// Use this for initialization
	void Awake ()
    {
        anim = player.GetComponent<Animator>();
        charRigidbody = player.GetComponent<Rigidbody>();
        capsuleCollider = player.GetComponent<CapsuleCollider>();
        boxCollider = player.GetComponent<BoxCollider>();

        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        SetCollidersEnabled(false);
        SetRigidBodiesKinematic(true);
	}
	
	private void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = enabled;
        }
    }

    private void SetRigidBodiesKinematic(bool kinematic)
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = kinematic;
        }
    }

    public void ActivateRagdoll()
    {
        capsuleCollider.enabled = false;
        boxCollider.enabled = false;
        charRigidbody.isKinematic = true;
        anim.enabled = false;

        SetCollidersEnabled(true);
        SetRigidBodiesKinematic(false);
    }

    public void DeactivateRagdoll()
    {
        capsuleCollider.enabled = true;
        boxCollider.enabled = true;
        charRigidbody.isKinematic = false;
        anim.enabled = true;

        SetCollidersEnabled(false);
        SetRigidBodiesKinematic(true);
    }
}
