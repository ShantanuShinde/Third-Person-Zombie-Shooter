using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    Animator animator;
    Camera mainCam;
    KeyCode currentKey = KeyCode.None;
    public bool animPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        mainCam = Camera.main;
    }

    // Update is called once per frame
    /*void Update()
    {
        if (!Input.GetKey(KeyCode.I) && !animPlaying)
        {
            // Jump
            if (animator.GetFloat("Speed") > 0.5 && Input.GetKeyDown(KeyCode.Space))
            {
                animator.Play("Jump");
            }
            // Rotate player on moving mouse along X
            if (!Dead && (animator.GetFloat("Speed") >= 0.5f || Input.GetKey(KeyCode.Mouse1)))
            {
                float h = horizontalSpeed * Input.GetAxis("Mouse X");
                transform.Rotate(0, h, 0);
            }
            if (!Input.GetKey(currentKey))
            {
                // Move player with WASD
                if (Input.GetKey(KeyCode.W))
                {

                    currentKey = KeyCode.W;
                    StartCoroutine(Front());
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    currentKey = KeyCode.A;
                    StartCoroutine(Left());
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    currentKey = KeyCode.D;
                    StartCoroutine(Right());
                }
                else if (Input.GetKey(KeyCode.S))
                {

                    currentKey = KeyCode.S;

                    StartCoroutine(Back());
                }
                else
                {
                    float forw = Mathf.Lerp(animator.GetFloat("Speed"), 0, 5 * Time.deltaTime);
                    animator.SetFloat("Speed", forw);

                    float h = Mathf.Lerp(animator.GetFloat("TurnSpeed"), 0, 10 * Time.deltaTime);
                    animator.SetFloat("TurnSpeed", h);

                }
            }

        }
    }*/
}
