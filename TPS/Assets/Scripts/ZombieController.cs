using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;
    public GameObject player;

    public float turnSmoothing = 15f;          
    public float speedDampTime = 0.1f;
    private const float stopDistanceProportion = 0.1f;


    bool dead = false;
    private int health = 100;

    private PlayerContorller pc;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent.updateRotation = false;
        agent.SetDestination(player.transform.position);
        agent.isStopped = true;
        pc = player.GetComponent<PlayerContorller>();
    }

    
    // Update is called once per frame
    void Update()
    {
        
        agent.SetDestination(player.transform.position);
        
        if (agent.pathPending)
            return;
        bool runWalk = animator.GetBool("RunWalk");
        if (pc.Dead)
        {
            animator.SetBool("PlayerDead", true);
            agent.isStopped = true;
            animator.SetBool("PlayerClose", false);
        }
        else if (!dead)
        {

            if (agent.remainingDistance <= agent.stoppingDistance * stopDistanceProportion)
            {
                agent.isStopped = true;
                animator.SetBool("PlayerClose", true);
                Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);
              
            }
           
            else if (agent.remainingDistance < 6.0f)
            {
                
                Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                float proportionalDistance = 1f - agent.remainingDistance / agent.stoppingDistance;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);
                if (runWalk)
                {
                    animator.SetBool("RunWalk", false);
                }
                
                //speed = Mathf.Lerp(0f, 0.5f, proportionalDistance);
                agent.isStopped = false;
                animator.SetBool("PlayerClose", false);
            }
            else if (agent.remainingDistance > 6.0f)
            {
                
                Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSmoothing * Time.deltaTime);
                float proportionalDistance = 1f - agent.remainingDistance / agent.stoppingDistance;
                if (!runWalk)
                {
                    animator.SetBool("RunWalk", true);
                }
                
                //speed = Mathf.Lerp(0.5f, 1f, proportionalDistance);
                agent.isStopped = false;
                animator.SetBool("PlayerClose", false);
            }

        }
        
        //animator.SetFloat("Speed", speed);
    }

    public void Hit()
    {
        health -= 20;
        if(health == 0)
        {
            dead = true;
            animator.Play("Die");
            StartCoroutine(Die());
            
        }
    }

    IEnumerator Die()
    {
        GameManager.Instance.waveSpawner.currentEnemyNum--;
        
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
