using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    // Start is called before the first frame update
   // public GameObject spark;
    public GameObject bullet;
    public GameObject muzzle;
    public ParticleSystem muzzleSpark;
    public Transform playerHand;
    public Transform crossHair;

    private void Awake()
    {
        transform.SetParent(playerHand);
    }
    public void Fire1()
    {
       // muzzle.transform.LookAt(crossHair);
        GameObject bulletInstance = Instantiate(bullet);
        bulletInstance.transform.position = muzzle.transform.position;
        bulletInstance.transform.eulerAngles = muzzle.transform.eulerAngles;
        GetComponent<AudioSource>().Play();
    }

    public void Fire2()
    {
        //muzzle.transform.LookAt(crossHair);
        GameObject bulletInstance = Instantiate(bullet);
        bulletInstance.transform.position = muzzle.transform.position;
        bulletInstance.transform.eulerAngles = muzzle.transform.eulerAngles;
        muzzleSpark.Play();
        GetComponent<AudioSource>().Play();
        
    }
    public void Fire3()
    {
        //muzzle.transform.LookAt(crossHair);
        GameObject bulletInstance = Instantiate(bullet);
        bulletInstance.transform.position = muzzle.transform.position;
        bulletInstance.transform.eulerAngles = muzzle.transform.eulerAngles;
        bulletInstance.transform.localScale = muzzle.transform.localScale;
        GetComponent<AudioSource>().Play();
        muzzleSpark.Play();
    }
}
