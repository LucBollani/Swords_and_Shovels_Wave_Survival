using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnController : MonoBehaviour
{
    public GameObject enem;

    public void spawnEnemy(GameObject enem)
    {
        Instantiate(enem, transform.position, transform.rotation);
    }
}
