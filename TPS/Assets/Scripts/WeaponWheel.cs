using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponWheel : MonoBehaviour
{
    public GameObject Wheel;
    public GameObject weap1;
    public GameObject weap2;
    private Animator animator;
    public GameObject weap3;
    public GameObject weap4;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Wheel.SetActive(false);
        weap1.SetActive(false);
        weap2.SetActive(false);
        weap3.SetActive(false);
       /* weap4.SetActive(false);*/
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 1)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Wheel.SetActive(true);
                Cursor.visible = true;
            }
            if (Input.GetKeyUp(KeyCode.I))
            {
                Wheel.SetActive(false);
                Cursor.visible = false;
            }
        }
        
    }

    public void Weapon1()
    {
        print("Here");
        animator.SetBool("HoldingPistol", true);
        animator.SetBool("HoldingRifle", false);
        weap1.SetActive(true);
        weap2.SetActive(false);
        weap3.SetActive(false);
        GetComponent<PlayerContorller>().weapInd = 0;
        //weap4.SetActive(false);
    }
    public void Weapon2()
    {
        animator.SetBool("HoldingPistol", true);
        animator.SetBool("HoldingRifle", false);
        weap2.SetActive(true);
        weap1.SetActive(false);
        weap3.SetActive(false);
        GetComponent<PlayerContorller>().weapInd = 1;
        //weap4.SetActive(false);
    }
    public void Weapon3()
    {
        animator.SetBool("HoldingRifle", true);
        animator.SetBool("HoldingPistol", false);
        weap3.SetActive(true);
        weap2.SetActive(false);
        weap1.SetActive(false);
        GetComponent<PlayerContorller>().weapInd = 2;
        // weap4.SetActive(false);
    }
    /*
    public void Weapon4()
    {
        weap4.SetActive(true);
        weap2.SetActive(false);
        weap3.SetActive(false);
        weap1.SetActive(false);
    }*/
}
