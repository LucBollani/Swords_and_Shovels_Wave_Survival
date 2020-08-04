using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteorController : MonoBehaviour
{
    public float lifeTime = 5f;
    public float speed = 10f;
    public int damage = 10;
    public GameObject explosion;
    public GameObject fire;

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
        Debug.Log(other.tag);
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().applyDamage(damage);
            GameObject exp = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(exp, 0.5f);
        }
        else if (other.tag == "Ground")
        {
            // instantiate fire on ground
            GameObject exp = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(exp, 0.5f);
            GameObject fi = Instantiate(fire, transform.position, Quaternion.identity);
            Destroy(fi, 5f);
        }
        //instantiate explosion
        Destroy(gameObject);
    }
}
