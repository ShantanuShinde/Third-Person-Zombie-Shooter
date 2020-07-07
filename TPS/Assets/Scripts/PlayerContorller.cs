using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class PlayerContorller : MonoBehaviour
{
    // component variables
    public Slider healthbar;
    Animator animator;
    Rigidbody rigid;
    Camera mainCam;
    Transform camRig;

    // player transform variables
    public float JumpForce = 500;
    public float horizontalSpeed = 0.1f;
    KeyCode currentKey = KeyCode.None;

    // player weapon variables
    public List<GameObject> Weapons = new List<GameObject>(3);
    public int weapInd = 3;
    Vector3 HoldingM16Position = new Vector3(0.117f, 0.292f, -0.014f);
    Vector3 HoldingM16Rotation = new Vector3(-282.855f, -7.312f, 80.66f);
    Vector3 AimingM16Position = new Vector3(0.124f, 0.301f, 0.046f);
    Vector3 AimingM16Rotation = new Vector3(-262.838f, 34.806f, 122.514f);
    Vector3 HoldingPistolPosition1 = new Vector3(0.0f, 0.13f, 0.02f);
    Vector3 HoldingPistolRotation1 = new Vector3(-90, 90, 0);
    Vector3 AimingPistolPosition1 = new Vector3(-0.0304F, 0.1516F, 0.0415F);
    Vector3 AimingPistolRotation1 = new Vector3(-102.589f, 182.26f, -92.99701f);
    Vector3 HoldingPistolRotation2 = new Vector3(-8f, -102.1f, 180f);
    Vector3 AimingPistolRotation2 = new Vector3(-2.69f, -86.551f, 164.421f);
    float fireTime;

    // player animation variables
    public bool animPlaying = false;
    public bool Dead = false;
    float defAnimspeed = 1.8f;

    int health = 100;

    public GameObject crosshair;
    PlayerAim playerAim;

    Vector2 mouseInput;

    public int MouseDamping = 2;

    PlayerStates.States previousState;

    void Start()
    {
        previousState = PlayerStates.States.Stand;
        foreach(GameObject weap in Weapons)
        {
            weap.SetActive(false);
        }
        Cursor.visible = false;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        healthbar.value = health;

        mainCam = Camera.main;

        camRig = mainCam.transform.parent.transform.parent;
        playerAim = GetComponentInChildren<PlayerAim>();
        crosshair.SetActive(false);

        mouseInput.x = Input.GetAxis("Mouse X");
        mouseInput.y = Input.GetAxis("Mouse Y");

        animator.SetFloat("AnimSpeed", defAnimspeed);
    }

    
    // Update is called once per frame
    void Update()
    {
        
        if (Time.timeScale == 1)
        {
            mouseInput.x = Mathf.Lerp(mouseInput.x, Input.GetAxis("Mouse X"), 1f / MouseDamping);
            mouseInput.y = Mathf.Lerp(mouseInput.y, Input.GetAxis("Mouse Y"), 1f / MouseDamping);
            if(weapInd != 3)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    crosshair.SetActive(true);
                }
                if (GameManager.Instance.playerStates.PlayerState == PlayerStates.States.Aim)
                {
                    //crosshair.GetComponent<Crosshair>().LookHeight(mouseInput.y * 10);
                    playerAim.SetRotation(mouseInput.y * 10);
                    float ang = playerAim.GetAngle();
                    if (ang >= -45 && ang <= 45)
                    {
                        animator.SetFloat("LookAngle", playerAim.GetAngle());
                    }
                    
                }
                else if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    crosshair.SetActive(false);
                    playerAim.ResetRotation();
                    animator.SetFloat("LookAngle", playerAim.GetAngle());
                }
            }
            
            

            // Change the player weapon on rotating scroll wheel
            if (!(Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.Mouse1)))
            {               
                if (Input.mouseScrollDelta.y > 0)
                {
                    weapInd = (weapInd + 1) % 4;
                    SwitchTo(weapInd);
                }
                else if (Input.mouseScrollDelta.y < 0)
                {
                    weapInd = ((weapInd - 1) % 4 + 4) % 4;
                    SwitchTo(weapInd);
                }
            }

            
            if (!Input.GetKey(KeyCode.I) && !animPlaying)
            {
                // Jump
                if(animator.GetFloat("Speed") > 0.5 && Input.GetKeyDown(KeyCode.Space) && !(GameManager.Instance.playerStates.PlayerState == PlayerStates.States.Aim))
                {
                    animator.Play("Jump");
                }
                // Rotate player on moving mouse along X
                if (!Dead && (animator.GetFloat("Speed") >= 0.5f || (GameManager.Instance.playerStates.PlayerState == PlayerStates.States.Aim)))
                {
                    float h = horizontalSpeed * mouseInput.x;
                    transform.Rotate(0, h, 0);
                }
                if(GameManager.Instance.playerStates.PlayerState == PlayerStates.States.Back)
                {
                    animator.SetFloat("Speed", 0);
                    animator.SetFloat("TurnSpeed", 0);
                }
                else if (!Input.GetKey(currentKey))
                {
                    
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        animator.SetFloat("AnimSpeed", 2f);
                    }
                    // Move player with WASD
                    // If player aiming
                    if ((GameManager.Instance.playerStates.PlayerState == PlayerStates.States.Aim) && weapInd != 3)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(camRig.forward, Vector3.up),10*Time.deltaTime);

                        float forw = Input.GetAxis("Vertical");
                        if (forw > 0.5f) forw = 0.5f;

                        float horz = Input.GetAxis("Horizontal");
                        if (horz > 0.5f) horz = 0.5f;

                        animator.SetFloat("Speed", forw);
                        animator.SetFloat("TurnSpeed", horz);

                    }
                    else if (Input.GetKey(KeyCode.W))
                    {
                        GameManager.Instance.playerStates.SetForward();
                        currentKey = KeyCode.W;
                        StartCoroutine(Front());
                    }
                    else if (Input.GetKey(KeyCode.A))
                    {
                        GameManager.Instance.playerStates.SetLeft();
                        currentKey = KeyCode.A;
                        StartCoroutine(Left());
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        GameManager.Instance.playerStates.SetRight();
                        currentKey = KeyCode.D;
                        StartCoroutine(Right());
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        GameManager.Instance.playerStates.SetBackward();
                        currentKey = KeyCode.S;
                        
                        StartCoroutine(Back());
                    }
                    else
                    {
                        GameManager.Instance.playerStates.SetStand();
                        float forw = Mathf.Lerp(animator.GetFloat("Speed"), 0, 5 * Time.deltaTime);
                        animator.SetFloat("Speed", forw);
                        
                        float h = Mathf.Lerp(animator.GetFloat("TurnSpeed"), 0, 10 * Time.deltaTime);
                        animator.SetFloat("TurnSpeed", h);
                        
                    }
                }
               
               
            }
            
            // Adjust weapons on aiming
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (animator.GetBool("HoldingRifle"))
                {
                    Weapons[2].transform.localPosition = AimingM16Position;
                    Weapons[2].transform.localEulerAngles = AimingM16Rotation;
                }
                else if (animator.GetBool("HoldingPistol"))
                {
                    Weapons[0].transform.localPosition = AimingPistolPosition1;
                    Weapons[0].transform.localEulerAngles = AimingPistolRotation1;
                    Weapons[1].transform.localEulerAngles = AimingPistolRotation2;
                }
                animator.SetBool("Aim", true);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {

                if (animator.GetBool("HoldingRifle"))
                {
                    Weapons[2].transform.localPosition = HoldingM16Position;
                    Weapons[2].transform.localEulerAngles = HoldingM16Rotation;
                }
                else if (animator.GetBool("HoldingPistol"))
                {
                    Weapons[0].transform.localPosition = HoldingPistolPosition1;
                    Weapons[0].transform.localEulerAngles = HoldingPistolRotation1;
                    Weapons[1].transform.localEulerAngles = HoldingPistolRotation2;
                }
                animator.SetBool("Aim", false);
            }
            if (Input.GetKey(KeyCode.Mouse1) && Input.GetKeyDown(KeyCode.Mouse0) && animator.GetBool("HoldingPistol"))
            {
                if (Weapons[0].activeSelf)
                {
                    Weapons[0].GetComponent<Weapon>().Fire1();
                }
                else if (Weapons[1].activeSelf)
                {
                    Weapons[1].GetComponent<Weapon>().Fire2();
                }
            }
            if (animator.GetBool("HoldingRifle"))
            {

                if (Input.GetKey(KeyCode.Mouse1))
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        fireTime = Time.time + 0.5f;
                        Weapons[2].GetComponent<Weapon>().Fire3();
                    }
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        if (Time.time > fireTime)
                        {
                            Weapons[2].GetComponent<Weapon>().Fire3();
                            fireTime += 0.5f;
                        }
                    }

                }
            }
        }
        
        
    }

   
    // function callod when player is hit
    public void Hit()
    {
        health -= 10;
        if (health == 0)
        {
            animator.Play("Die");
            Dead = true;
            //StartCoroutine(Die());
        }
        healthbar.value = health;
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    // function to switch player weapons
    void SwitchTo(int weaponInd)
    {
        foreach(GameObject weap in Weapons)
        {
            weap.SetActive(false);
        }
        if (weaponInd == 3)
        {
            animator.SetBool("HoldingPistol", false);
            animator.SetBool("HoldingRifle", false);
        }
        else
        {
            Weapons[weaponInd].SetActive(true);
            if(weaponInd == 0 || weaponInd == 1)
            {
                animator.SetBool("HoldingRifle", false);
                animator.SetBool("HoldingPistol", true);
            }
            else
            {
                animator.SetBool("HoldingPistol", false);
                animator.SetBool("HoldingRifle", true);
            }
        }
    }

    // function for moving player in the 4 directions
    IEnumerator Right()
    {
        int angle = Mathf.RoundToInt(Vector3.SignedAngle(mainCam.transform.forward, transform.forward, Vector3.up));
      
       if (angle >= -60 && angle <= 30)
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandQuarterTurnRight");
            yield return new WaitForSeconds(1f/animator.GetFloat("AnimSpeed"));
            float toRot = 90 - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
            animPlaying = false;
        }
     
        else if (angle > 30 && angle < 90)
        {
            float toRot = 90 - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
        }
        
        else if (angle > 90 && angle < 150)
        {
            float toRot = 90 - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
        }
        else if ((angle >= 150 || angle <= -150)|| (angle > -150 && angle < -120))
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandQuarterTurnLeft");
            yield return new WaitForSeconds(1f / animator.GetFloat("AnimSpeed"));
            float toRot = 90 - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
            animPlaying = false;
        }
        
        else if (angle >= -120 && angle <= -60)
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandHalfTurnLeft");
            yield return new WaitForSeconds(1.06f / animator.GetFloat("AnimSpeed"));
            float toRot = 90 - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
            animPlaying = false;
        }
        
        while (Input.GetKey(KeyCode.D) && GameManager.Instance.playerStates.PlayerState == PlayerStates.States.Right)
        {
            float forw = Input.GetAxis("Horizontal");
            if (forw > 0.5f && !Input.GetKey(KeyCode.LeftShift)) forw = 0.5f;
            animator.SetFloat("Speed", forw);
            float h = -Input.GetAxis("Vertical");
            if (forw <= 0.5f)
            {
                if (h > 0.5f) h = 0.5f;
                else if (h < -0.5f) h = -0.5f;
            }
            animator.SetFloat("TurnSpeed", h);

            yield return null;
        }
        currentKey = KeyCode.None;
        animator.SetFloat("AnimSpeed", defAnimspeed);
    }
    IEnumerator Left()
    {
        int angle = Mathf.RoundToInt(Vector3.SignedAngle(mainCam.transform.forward, transform.forward, Vector3.up));
        
       if (angle >= -30 && angle <= 60)
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandQuarterTurnLeft");
            yield return new WaitForSeconds(1f / animator.GetFloat("AnimSpeed"));
            float toRot = -90f - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
            animPlaying = false;
        }
       
        else if (angle >= 60 && angle <= 120)
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandHalfTurnLeft");
            yield return new WaitForSeconds(1.06f / animator.GetFloat("AnimSpeed"));
            float subAng = Mathf.Abs(transform.eulerAngles.y - yAng);
            float toRot = -90f - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
            animPlaying = false;
        }
        else if ((angle > 120 && angle < 150)|| (angle >= 150 || angle <= -150))
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandQuarterTurnRight");
            yield return new WaitForSeconds(1f / animator.GetFloat("AnimSpeed"));
            float toRot = -90f - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
            animPlaying = false;
        }
       
        else if (angle > -150 && angle < -90)
        {
            float toRot = -90f - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
        }
       
        else if (angle > -90 && angle < -30)
        {
            float toRot = -90f - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
        }
        while (Input.GetKey(KeyCode.A) && GameManager.Instance.playerStates.PlayerState == PlayerStates.States.Left)
        {
            float forw = -Input.GetAxis("Horizontal");
            if (forw > 0.5f && !Input.GetKey(KeyCode.LeftShift)) forw = 0.5f;
            animator.SetFloat("Speed", forw);
            float h = Input.GetAxis("Vertical");
            if (forw <= 0.5f)
            {
                if (h > 0.5f) h = 0.5f;
                else if (h < -0.5f) h = -0.5f;
            }
            animator.SetFloat("TurnSpeed", h);
            yield return null;
        }
        currentKey = KeyCode.None;
        animator.SetFloat("AnimSpeed", defAnimspeed);
    }
    IEnumerator Back()
    {
        int angle = Mathf.RoundToInt(Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up));
        if (angle >= -30 && angle <= 30)
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandHalfTurnLeft");
            yield return new WaitForSeconds(1.06f / animator.GetFloat("AnimSpeed"));
            
            float toRot = 180f + Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up); ;
            transform.Rotate(0, -toRot, 0);
            animPlaying = false;

        }
        else if (angle > 30 && angle < 120)
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandQuarterTurnRight");
            yield return new WaitForSeconds(1f / animator.GetFloat("AnimSpeed"));
            float subAng = Mathf.Abs(transform.eulerAngles.y - yAng);
           
            float toRot = 180f - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
            animPlaying = false;
        }
        
        else if (angle > 120 )
        {
            float toRot = 180f - Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, toRot, 0);
        }
        
        else if (angle > -180 && angle < -120)
        {
            float toRot = 180f + Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, -toRot, 0);
        }
        else if (angle >= -120 && angle <= -30)
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandQuarterTurnLeft");
            yield return new WaitForSeconds(1f / animator.GetFloat("AnimSpeed"));
            float toRot = 180f + Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, -toRot, 0);
            
            animPlaying = false;
        }
        
        
        while (Input.GetKey(KeyCode.S) && GameManager.Instance.playerStates.PlayerState == PlayerStates.States.Backward)
        {

            float forw = -Input.GetAxis("Vertical");
            if (forw > 0.5f && !Input.GetKey(KeyCode.LeftShift)) forw = 0.5f;
            animator.SetFloat("Speed", forw);
            float h = -Input.GetAxis("Horizontal");
            if (forw <= 0.5f)
            {
                if (h > 0.5f) h = 0.5f;
                else if (h < -0.5f) h = -0.5f;
            }
            animator.SetFloat("TurnSpeed", h);

            yield return null;
        }
        currentKey = KeyCode.None;
        animator.SetFloat("AnimSpeed", defAnimspeed);
    }
    IEnumerator Front()
    {
        int angle = Mathf.RoundToInt(Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up));
        if (angle < 60 && angle > 0)
        {
            float toRot = Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, -toRot, 0);
        }
        else if (angle >= 60 && angle <= 150)
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandQuarterTurnLeft");
            yield return new WaitForSeconds(1f / animator.GetFloat("AnimSpeed"));
            float toRot = Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, -toRot, 0);
            animPlaying = false;
        }
        
        else if (angle >= 150 || angle <= -150)
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandHalfTurnLeft");
            yield return new WaitForSeconds(1.06f / animator.GetFloat("AnimSpeed"));
            float toRot = Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, -toRot, 0);
            animPlaying = false;
        }
        else if (angle > -150 && angle < -60)
        {
            float yAng = transform.eulerAngles.y;
            animPlaying = true;
            animator.Play("StandQuarterTurnRight");
            yield return new WaitForSeconds(1f / animator.GetFloat("AnimSpeed"));
            float toRot = Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, -toRot, 0);
            animPlaying = false;
        }
        
        else if (angle > -60 && angle < 0)
        {
            float toRot = Vector3.SignedAngle(camRig.forward, transform.forward, Vector3.up);
            transform.Rotate(0, -toRot, 0);
        }
        while (Input.GetKey(KeyCode.W) && GameManager.Instance.playerStates.PlayerState == PlayerStates.States.Forward)
        {
            float forw = Input.GetAxis("Vertical");
            if (forw > 0.5f && !Input.GetKey(KeyCode.LeftShift)) forw = 0.5f;
            animator.SetFloat("Speed", forw);
            float h = Input.GetAxis("Horizontal");
            if (forw <= 0.5f)
            {
                if (h > 0.5f) h = 0.5f;
                else if (h < -0.5f) h = -0.5f;
            }
            animator.SetFloat("TurnSpeed", h);
            yield return null;
        }
        currentKey = KeyCode.None;
        animator.SetFloat("AnimSpeed", defAnimspeed);
    }
}
