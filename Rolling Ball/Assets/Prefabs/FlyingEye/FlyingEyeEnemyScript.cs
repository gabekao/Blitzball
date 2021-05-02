using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FlyingEyeEnemyScript : MonoBehaviour
{
    enum FlyingEyeStates
    {
        Patrol,
        Attack
    }

    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject meshObject;
    [SerializeField] private Transform gunBone;
    [SerializeField] private Transform gunProjectileSpawner;
    [SerializeField] private GameObject projectileObject;

    [Header("Radii and Threshold Variables")]
    [SerializeField] private float detectionRadius = 15.0f;
    [SerializeField] private float patrolSphereRadius = 15.0f;
    [SerializeField] private float positionThreshold = 2.0f;

    [Header("Speed Variables")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float rotateSpeed = 360.0f;

    [Header("Attack Variables")]
    [SerializeField] private float minAttackTime = 1.0f;
    [SerializeField] private float maxAttackTime = 3.0f;
    [Tooltip("Attempts to aim forward of player's movement in order to hit player more \"accurately\"")]
    [SerializeField] bool leadShot = false;

    
    

    private Vector3 homePosition;
    private Vector3 targetPosition;
    private Vector3 leadPosition;

    

    private Rigidbody playerRB;

    private FlyingEyeStates state = FlyingEyeStates.Patrol;
    private FlyingEyeStates previousState = FlyingEyeStates.Patrol;

    private CharacterController cc;
    



    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        homePosition = transform.position;
        NewTargetPosition();

        StartCoroutine(AttackTimer());

        playerRB = playerTransform.gameObject.GetComponent<Rigidbody>();
    }
    


    void Update()
    {
        // Get a new position to fly to if close to current target position
        if (CheckDistanceToTargetPosition())
        {
            NewTargetPosition();
        }


        switch(state)
        {
            case FlyingEyeStates.Attack:
                AttackState();
                break;

            case FlyingEyeStates.Patrol:
                PatrolState();
                break;
        }



        // Move
        if (Physics.Raycast(transform.position, Vector3.Normalize(targetPosition - transform.position), out RaycastHit hit, 2.0f))
        {
            NewTargetPosition();
        }
        cc.Move(Vector3.Normalize(targetPosition - transform.position) * speed * Time.deltaTime);
        
    }



    void AttackState()
    {
        
        leadPosition = playerTransform.position + playerRB.velocity;

        LookAtSmooth(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        GunLookAt();

        if (!CheckDistanceToPlayer())
        {
            ChangeState(FlyingEyeStates.Patrol);
        }
    }

    void PatrolState()
    {
        LookAtSmooth(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));

        if (CheckDistanceToPlayer())
        {
            ChangeState(FlyingEyeStates.Attack);
        }
    }



    bool CheckDistanceToTargetPosition()
    {
        return Vector3.Distance(transform.position, targetPosition) < positionThreshold;
    }



    bool CheckDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, playerTransform.position) < detectionRadius;
    }



    void LookAtSmooth(Vector3 lookTarget)
    {
        meshObject.transform.rotation = Quaternion.RotateTowards(meshObject.transform.rotation, Quaternion.LookRotation(lookTarget - transform.position), rotateSpeed * Time.deltaTime);
    }


    void GunLookAt()
    {
        if (!leadShot)
            gunBone.rotation = Quaternion.RotateTowards(gunBone.rotation, Quaternion.LookRotation(playerTransform.position - gunBone.position), rotateSpeed * Time.deltaTime);
        else
            gunBone.rotation = Quaternion.RotateTowards(gunBone.rotation, Quaternion.LookRotation(leadPosition - gunBone.position), rotateSpeed * Time.deltaTime);
    }



    void NewTargetPosition()
    {
        targetPosition = homePosition + Random.insideUnitSphere * patrolSphereRadius;
        if (Physics.Raycast(transform.position, Vector3.Normalize(targetPosition - transform.position), maxDistance: 15.0f, hitInfo: out RaycastHit hit, layerMask: ~LayerMask.GetMask("Ignore Raycast")))
        {
            if (Vector3.Distance(hit.point, homePosition) < patrolSphereRadius)
                targetPosition = hit.point - Vector3.Normalize(targetPosition - transform.position) * 2;
        }
    }



    IEnumerator AttackTimer()
    {
        while (true)
        {
            if (state == FlyingEyeStates.Attack)
                Attack();
            yield return new WaitForSeconds(Random.Range(minAttackTime, maxAttackTime));
        }
    }



    void Attack()
    {
        if (projectileObject == null)
            Debug.Log("boom");
        else
        {
            // Instantiate projectile here, make it "transform.LookAt" player, then set its speed to whatever
            GameObject go = Instantiate(projectileObject, gunProjectileSpawner.position, gunBone.rotation);
            Destroy(go, 30);
        }

    }

    private void OnDrawGizmos()
    {
        // Draw patrol radius in scene view as a green wiresphere
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(homePosition, patrolSphereRadius);

        // Draw detection radius in scene view as a red wiresphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw line to target (move) position
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPosition);
    }



    void ChangeState(FlyingEyeStates newState)
    {
        previousState = state;
        state = newState;
    }
}
