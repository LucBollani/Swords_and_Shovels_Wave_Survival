using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowController : MonoBehaviour
{
    public float lifeTime = 1f;
    public float speed = 10f;
    public int arrowDamage = 2;
    public string hitTag;
    public string selfTag;
    public bool improved;
    public int hits = 3;

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(0, 0, 90));
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != selfTag && !other.isTrigger)
        {
            if (other.tag == hitTag)
            {
                if (hitTag == "Enemy")
                {
                    other.GetComponent<enemy>().applyDamage(arrowDamage);
                }
                else
                {
                    other.GetComponent<PlayerController>().applyDamage(arrowDamage);
                }
            }
            else if (other.tag == "statue")
            {
                other.GetComponent<ghostSpawner>().applyDamage(4);
                Destroy(gameObject);
            }
            else Destroy(gameObject);

            if (improved)
            {
                hits--;
                if (hits == 0) Destroy(gameObject);
            }
            else Destroy(gameObject);
        }
    }
}
