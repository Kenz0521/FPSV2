using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    enum EnemyState
    {
        Wander,
        Pursue,
        Attack,
        Recovery
    }

    EnemyState currentState;

    NavMeshAgent agent;
    Rigidbody rb;
    Transform player;

    // assignment variables
    [SerializeField] float wanderRange = 10f;
    [SerializeField] float playerSightRange = 12f;
    [SerializeField] float playerAttackRange = 3f;
    [SerializeField] float recoveryTime = 2f;

    Vector3 startingLocation;

    float currentStateElapsed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        rb = GetComponent<Rigidbody>();

        player = FindObjectOfType<FPSController>().transform;

        startingLocation = transform.position;

        ChangeState(EnemyState.Wander);
    }

    void Update()
    {
        currentStateElapsed += Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.Wander:
                UpdateWander();
                break;

            case EnemyState.Pursue:
                UpdatePursue();
                break;

            case EnemyState.Attack:
                UpdateAttack();
                break;

            case EnemyState.Recovery:
                UpdateRecovery();
                break;
        }
    }

    
    // STATE CHANGES
    

    void ChangeState(EnemyState newState)
    {
        ExitState(currentState);

        currentState = newState;

        currentStateElapsed = 0;

        EnterState(newState);
    }

    void EnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Wander:

                WanderToNewPoint();

                break;

            case EnemyState.Attack:

                AttackPlayer();

                break;
        }
    }

    void ExitState(EnemyState state)
    {

    }

    
    // WANDER
    

    void UpdateWander()
    {
        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        // see player
        if (distanceToPlayer <= playerSightRange)
        {
            ChangeState(EnemyState.Pursue);
            return;
        }

        // reached destination
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            WanderToNewPoint();
        }
    }

    void WanderToNewPoint()
    {
        Vector3 randomPoint =
            startingLocation +
            Random.insideUnitSphere * wanderRange;

        randomPoint.y = transform.position.y;

        agent.SetDestination(randomPoint);
    }

    
    // PURSUE
    

    void UpdatePursue()
    {
        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        agent.SetDestination(player.position);

        // lost player
        if (distanceToPlayer > playerSightRange)
        {
            ChangeState(EnemyState.Wander);
            return;
        }

        // close enough to attack
        if (distanceToPlayer <= playerAttackRange)
        {
            ChangeState(EnemyState.Attack);
        }
    }

    
    // ATTACK
    

    void UpdateAttack()
    {

    }

    void AttackPlayer()
    {
        agent.ResetPath();

        Vector3 direction =
            (player.position - transform.position).normalized;

        rb.AddForce(direction * 15f, ForceMode.Impulse);

        ChangeState(EnemyState.Recovery);
    }

    
    // RECOVERY
    

    void UpdateRecovery()
    {
        if (currentStateElapsed >= recoveryTime)
        {
            ChangeState(EnemyState.Pursue);
        }
    }


    // COLLISION DAMAGE


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.GetComponentInParent<FPSController>() != null)
        {
            Debug.Log("Player Hit!");
        }
    }
}
