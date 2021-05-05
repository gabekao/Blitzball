using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderRobotScript : MonoBehaviour
{
    enum SpiderRobotState
    {
        Idle,
        Patrol,
        Attack,
        ReturnHome
    }

    [SerializeField] private Transform home;
    [SerializeField] private Transform target;
    [SerializeField] private float patrolRadius = 10.0f;
    public float PatrolRadius
    {
        get { return patrolRadius; }
        private set { patrolRadius = value; }
    }

    [SerializeField] private float maxDistanceFromHomeWhileAttacking = 10.0f;
    [SerializeField] private float sightRadius = 10.0f;
    [SerializeField] private float positionThreshold = 1.0f;

    [SerializeField] private float minimumTimeIdlePatrol = 1.0f;
    [SerializeField] private float maximumTimeIdlePatrol = 1.0f;
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float attackSpeed = 7.0f;

    private Vector3 targetLocation;

    private SpiderRobotState state = SpiderRobotState.Idle;
    private SpiderRobotState previousState = SpiderRobotState.Idle;
    private NavMeshAgent nav;
    private float distanceFromHome = 0;
    [SerializeField] private Animator anim;

    private LayerMask navAreaMask = -1; // ???

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        StartCoroutine(IdlePatrolSwitchTimer());

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        nav.speed = patrolSpeed;
    }


    // Update is called once per frame
    void Update()
    {
        distanceFromHome = Vector3.Distance(transform.position, home.position);

        switch(state)
        {
            case SpiderRobotState.Idle:
                Idle();
                break;

            case SpiderRobotState.Patrol:
                Patrol();
                break;

            case SpiderRobotState.Attack:
                Attack();
                break;

            case SpiderRobotState.ReturnHome:
                ReturnHome();
                break;
        }

        anim.SetFloat("speed", nav.velocity.magnitude);
    }


    void Idle()
    {
        if (!nav.isStopped)
            nav.isStopped = true;

        CheckForPlayer();
    }


    void Patrol()
    {
        if (nav.isStopped)
            nav.isStopped = false;

        if (Vector3.Distance(transform.position, nav.destination) < positionThreshold)
        {
            nav.SetDestination(GetNewPatrolPosition());
        }

        CheckForPlayer();
    }


    void Attack()
    {
        if (nav.isStopped)
            nav.isStopped = false;
        nav.speed = attackSpeed;
        anim.speed = 2;

        nav.SetDestination(target.position);

        if (!CheckForPlayer() || distanceFromHome > maxDistanceFromHomeWhileAttacking)
        {
            nav.speed = patrolSpeed;
            anim.speed = 1;
            ChangeState(SpiderRobotState.ReturnHome);
        }

    }


    void ReturnHome()
    {
        if (nav.isStopped)
            nav.isStopped = false;

        nav.SetDestination(home.position);
        if (Vector3.Distance(transform.position, home.position) < positionThreshold)
        {
            int choose = Random.Range(0, 2);
            switch (choose)
            {
                case 0:
                    ChangeState(SpiderRobotState.Idle);
                    break;
                case 1:
                    ChangeState(SpiderRobotState.Patrol);
                    break;
            }
        }
    }


    void ChangeState(SpiderRobotState newState)
    {
        previousState = state;
        state = newState;
    }


    Vector3 GetNewPatrolPosition()
    {
        Vector3 randomDirection = home.position + Random.insideUnitSphere * patrolRadius;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, navAreaMask);

        return hit.position;
    }
    

    bool CheckForPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) < sightRadius)
        {
            ChangeState(SpiderRobotState.Attack);
            return true;
        }
        return false;
    }



    IEnumerator IdlePatrolSwitchTimer()
    {
        while (true)
        {
            switch (state)
            {
                case SpiderRobotState.Idle:
                    ChangeState(SpiderRobotState.Patrol);
                    break;

                case SpiderRobotState.Patrol:
                    ChangeState(SpiderRobotState.Idle);
                    break;
            }


            yield return new WaitForSeconds(Random.Range(minimumTimeIdlePatrol, maximumTimeIdlePatrol));
        }
    }
}
