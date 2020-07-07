using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float bulletSpeed = 100.0f;
    public Vector3 forward;
    // Start is called before the first frame update
    GameObject player;

   
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(transform.forward * bulletSpeed * Time.deltaTime, Space.World);
        if (Vector3.Distance(player.transform.position, transform.position) > 250.0f)
        {
            Destroy(gameObject);
        }

    }

    void OnCollisionEnter(Collision other)
    {
        
        if(other.gameObject.tag == "Zombie")
        {
            other.gameObject.GetComponent<ZombieController>().Hit();
        }
        Destroy(gameObject);
    }
}
