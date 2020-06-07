using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class enemy_fsm : MonoBehaviour
{
    public enum ENEMY_STATE { WANDER, CHASE, ATTACK, DEAD };
    [SerializeField]
    private ENEMY_STATE currentState;
    public ENEMY_STATE CurrentState
    {
        get
        {
            return currentState;

        }
        set
        {

            currentState = value;
            StopAllCoroutines();
            switch (currentState)
            {
                case ENEMY_STATE.WANDER:
                    StartCoroutine(EnemyWANDER());
                    break;
                case ENEMY_STATE.CHASE:
                    StartCoroutine(EnemyChase());
                    break;
                case ENEMY_STATE.ATTACK:
                    StartCoroutine(EnemyAttack());
                    break;
                case ENEMY_STATE.DEAD:
                    StartCoroutine(EnemyDEAD());
                    break;
            }

        }
    }

    private CheckMyVision checkMyVision;
    private NavMeshAgent agent = null;
    public Transform playerTransform = null;
    private Transform patrolDestination = null;
    private Health playerHealth = null;
    public float maxDamage = 10f;
    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerTransform = playerHealth.GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Dest");
        patrolDestination = destinations[Random.Range(0, destinations.Length)].GetComponent<Transform>();
        CurrentState = ENEMY_STATE.PATROL;
    }

    public IEnumerator EnemyWADER()
    {
        while (currentState == ENEMY_STATE.WANDER)
        {
            checkMyVision.sensitivity = CheckMyVision.enmSensitivity.HIGH;
            agent.isStopped = false;
            agent.SetDestination(patrolDestination.position);
            while (agent.pathPending)
            {
                yield return null;
            }

            if (checkMyVision.targetInSight)
            {
                agent.isStopped = true;
                CurrentState = ENEMY_STATE.CHASE;
                yield break;
            }
            yield return null;
        }

    }
    public IEnumerator EnemyChase()
    {
        //start with a loop
        while (currentState == ENEMY_STATE.CHASE)
        {
            //keep sensitivity low
            checkMyVision.sensitivity = CheckMyVision.enmSensitivity.LOW;
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastKnownSighting);
            while (agent.pathPending)
            {
                yield return null;
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.isStopped = true;
                if (!checkMyVision.targetInSight)
                {
                    CurrentState = ENEMY_STATE.PATROL;
                }
                else
                {

                    CurrentState = ENEMY_STATE.ATTACK;
                }
                yield break;
            }
            yield return null;
        }
        yield break;
    }
    public IEnumerator EnemyAttack()
    {
        while (currentState == ENEMY_STATE.ATTACK)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
            while (agent.pathPending)
                yield return null;
            if (agent.remainingDistance > agent.stoppingDistance)
            { 
                CurrentState = ENEMY_STATE.CHASE;
            }
            else
            {
                playerHealth.HealthPoints -= maxDamage * Time.deltaTime;
            }
            yield return null; 
        }
        yield break;
    }

    public IEnumerator EnemyDEAD()
    {
        while (currentState == ENEMY_STATE.DEAD)
        {
            
        }
        yield break;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
