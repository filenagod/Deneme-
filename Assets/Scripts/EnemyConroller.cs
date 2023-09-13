using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyConroller : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] float attackRange = 2f;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 15f;
    [SerializeField] float patrolRaidus = 6f;
    [SerializeField] float patrolWaitTime = 2f;
    [SerializeField] float chaseSpeed = 4f;
    [SerializeField] float searchSpeed = 3f;
    [Header("Attack Settings")]
    [SerializeField] int damage = 2;
    [SerializeField] float attackRate = 2f;

    private bool isSearched = false;
    private bool isAttacking = false;

    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;

    enum State
    {
        Idle,
        Search,
        Chase,
        Attack

    }

    [SerializeField] private State currentState = State.Idle;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //agent.SetDestination(player.position);
        StateCheck();
        StateExecute();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,chaseRange);

        switch (currentState)
        {
            
            case State.Search:
                Gizmos.color = Color.green;
                Vector3 targetPos = new Vector3(agent.destination.x, transform.position.y, agent.destination.z);
                Gizmos.DrawLine(transform.position, targetPos);
                break;
            case State.Chase:
                Gizmos.color = Color.blue;
                Gizmos.DrawLine (transform.position, player.position);
                break;
            case State.Attack:
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, player.position);
                break;
            
        }
    }

    private void StateCheck()
    {
        float distanceToTarget = Vector3.Distance(player.position,transform.position);
        if (distanceToTarget <= chaseRange && distanceToTarget > attackRange)
        {
            currentState = State.Chase;
        }
        else if(distanceToTarget <= attackRange)
        {
            currentState = State.Attack;
        }
        else
        {
            currentState = State.Search;
        }

    }

    private void StateExecute()
    {
        switch (currentState)
        {
            case State.Idle:
                print("Idle");
                break;
            case State.Search:
                if (!isSearched && agent.remainingDistance <= 0.1f || 
                    !agent.hasPath && !isSearched)
                {
                    Vector3 agentTarget = new Vector3(agent.destination.x, transform.position.y, agent.destination.z);
                    transform.position = agentTarget;
                    agent.enabled = true;

                    Invoke("Search", patrolWaitTime);
                    animator.SetBool("Walk", false);
                    isSearched = true;
                }
                break;
            case State.Chase:
                print("Chase");
                Chase();
                break;
            case State.Attack:
                print("Attack");
                Attcak();
                break;
           
        }
    }
    private void Search()
    {
        agent.isStopped = false;
        agent.speed = searchSpeed;
        isSearched = false;
        animator.SetBool("Walk", true);
        agent.SetDestination(GetRandomPosition());
    }

    private void Attcak()
    {
        if(player == null)
        {
            return;
        }
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
        animator.SetBool("Walk", false);
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        LookTheTarget(player.position);
    }

    private void Chase()
    {
        if (player == null)
        {
            return;
        }
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        animator.SetBool("Walk", true);
        agent.SetDestination(player.position);
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackRate);
        animator.SetTrigger("Attack");
        yield return new WaitUntil(ISAttackAnimationFinished);
        isAttacking = false;
    }
    private bool ISAttackAnimationFinished()
    {
        if (!animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    private void LookTheTarget(Vector3 target)
    {
        Vector3 lookPos = new Vector3(target.x, transform.position.y,target.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos - transform.position),
            turnSpeed * Time.deltaTime);
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * patrolRaidus;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRaidus,1);
        return hit.position;
    }
    public int GetDamage()
    {
        return damage;
    }
}
