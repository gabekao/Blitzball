using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    private GameController game;
    [Header("Speed Variables")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpSpeed = 10.0f;
    [SerializeField] private float slowDownFactor = 2.0f;

    [Header("Special Terrain")]
    [SerializeField] private float sandyGroundAngularDrag = 20.0f;
    [SerializeField] private float sandyGroundJumpMultiplier = 0.5f;
    [SerializeField] private float sandyGroundSpeedMultiplier = 0.725f;

    [SerializeField] private float rubberGroundJumpMultiplier = 1.5f;
    [Range(0, 1)]
    [SerializeField] private float rubberGroundBounciness = 0.5f;

    [SerializeField] private float slipperyAngularDrag = 0.0f;
    [SerializeField] private float slipperySpeedMultiplier = 1.5f;
    [SerializeField] private float slipperyJumpMultiplier = 0.75f;
    private SpecialTerrainType currentSpecialTerrain = SpecialTerrainType.None;


    [Header("Wall Jump Variables")]
    [SerializeField] private float wallJumpForceHorizontal = 5.0f;
    [SerializeField] private float wallJumpForceVertical = 5.0f;
    [SerializeField] private bool wallJumpEnabled = true;
    [SerializeField] private bool wallJumpLimited = true;
    [SerializeField] private bool wallJumpOnlyWhenMovingDownwards = false;


    [Header("Collision Variables")]
    [SerializeField] private float collisionDistanceCheck = 0.6f;
    [SerializeField] private string groundTag = "Ground";

    [Header("References")]
    private Vector3 moveVector = Vector3.zero;
    private Rigidbody rb;
    [SerializeField] private Transform cameraTransform;

    private SphereCollider collider;
    private float originalAngularDrag;


    private float wallJumpTimer = 0.5f;
    private float wallJumpTimerCurrent = 0.0f;

    private bool wallJumped = false;

    // Properties
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float JumpSpeed { get => jumpSpeed; set => jumpSpeed = value; }
    public bool WallJumpEnabled { get => wallJumpEnabled; set => wallJumpEnabled = value; }
    public bool WallJumpLimited { get => wallJumpLimited; set => wallJumpLimited = value; }

    private Vector3 previousVelocity;


    //Audio Clips Start
    private AudioSource audioSource;
    [SerializeField] private AudioClip jumpInit;
    [SerializeField] private AudioClip jumpLand;
    //Audio Clips End

    private bool wasInAir = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        previousVelocity = rb.velocity;
        originalAngularDrag = rb.angularDrag;
        if (cameraTransform == null)
        {
            cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        collider = GetComponent<SphereCollider>();
        
    }

    private void Update()
    {
        float jumpMult = 1.0f;
        switch (currentSpecialTerrain)
        {
            case SpecialTerrainType.None:
                rb.angularDrag = originalAngularDrag;
                break;

            case SpecialTerrainType.Sand:
                rb.angularDrag = sandyGroundAngularDrag;
                jumpMult = sandyGroundJumpMultiplier;
                break;

            case SpecialTerrainType.Rubber:
                jumpMult = rubberGroundJumpMultiplier;
                break;

            case SpecialTerrainType.Slippery:
                rb.angularDrag = slipperyAngularDrag;
                jumpMult = slipperyJumpMultiplier;
                break;
        }

        // For some reason, this works better over here
       
        if (CheckGrounded())
        {
            if(wasInAir){
                audioSource.PlayOneShot(jumpLand, 1.0f);
                wasInAir = false;
            }
            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpSpeed * jumpMult, rb.velocity.z);
                audioSource.PlayOneShot(jumpInit, 1.0f);
            }
        } else {
            wasInAir = true;
        }

        if (wallJumpTimerCurrent > 0)
            wallJumpTimerCurrent -= Time.deltaTime;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckIfTerrainIsSpecial();
        Quaternion cameraRotationFlattened = Quaternion.Euler(0.0f, cameraTransform.rotation.eulerAngles.y, 0.0f);

        moveVector = cameraRotationFlattened * (Input.GetAxis("Horizontal") * Vector3.right + Input.GetAxis("Vertical") * Vector3.forward);
        if (moveVector.magnitude > 1.0f)
            moveVector = moveVector.normalized;
        float modSpeed = 1.0f;
        float reverser = 1.0f;

        Vector3 rbVelocityFlat = Vector3.forward * rb.velocity.z + Vector3.right * rb.velocity.x;

        float angle = Vector3.Angle(moveVector, rbVelocityFlat);
        if (angle > 135 && angle < 225)
        {
            reverser = slowDownFactor;
        }

        switch(currentSpecialTerrain)
        {
            case SpecialTerrainType.None:
                break;

            case SpecialTerrainType.Sand:
                modSpeed = sandyGroundSpeedMultiplier;
                break;

            case SpecialTerrainType.Slippery:
                modSpeed = slipperySpeedMultiplier;
                break;
        }

        if (wallJumpTimerCurrent <= 0)
            rb.AddForce(moveVector * moveSpeed * modSpeed * reverser);


        if (wallJumpEnabled && Input.GetButtonDown("Jump"))
        {
            CheckWalljump(cameraRotationFlattened);
        }
    }

    bool CheckGrounded()
    {
        RaycastHit hit;

        Vector3 hitOffset = new Vector3(0.0f, 0.0f, collider.radius - 0.02f);

        // center hit
        Debug.DrawRay(transform.position, Vector3.down);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, collisionDistanceCheck))
        {
            if (hit.collider.CompareTag(groundTag))
            {
                wallJumped = false;

                return true;

            }
        }

        // circular hits
        for (int i = 0; i < 8; i++)
        {
            Vector3 hitOffsetRotated = Quaternion.Euler(0.0f, 360 / 8 * i, 0.0f) * hitOffset;
            //Debug.Log(hitOffsetRotated);
            Debug.DrawRay(transform.position + hitOffsetRotated, Vector3.down);
            if (Physics.Raycast(transform.position + hitOffsetRotated, Vector3.down, out hit, collisionDistanceCheck))
            {
                if (hit.collider.CompareTag(groundTag))
                {
                    wallJumped = false;
                    return true;
                }
            }
        }

        return false;
    }

    private void LateUpdate()
    {
        previousVelocity = rb.velocity;
    }

    private void CheckIfTerrainIsSpecial()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, collisionDistanceCheck))
        {
            if (hit.collider.CompareTag(groundTag))
            {
                wallJumped = false;
                SpecialTerrain checkTerrain = hit.collider.gameObject.GetComponent<SpecialTerrain>();
                if (checkTerrain != null)
                {
                    currentSpecialTerrain = checkTerrain.terrainType;
                }
                else
                {
                    currentSpecialTerrain = SpecialTerrainType.None;
                }
            }
        }
        
    }

    void CheckWalljump(Quaternion cameraRotationY)
    {
        if (wallJumpLimited && wallJumped)
            return;

        if (wallJumpOnlyWhenMovingDownwards && rb.velocity.y > 0)
            return;

        RaycastHit hit;
        Vector3 horizontalInputVector = cameraRotationY * new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 rayOffset = Vector3.forward * (collider.radius + 0.1f);
        int numRays = 10;
        
        for (int i = 0; i < numRays; i++)
        {
            Vector3 sweepRotationOffsetV = Quaternion.Euler(0.0f, -45 + ((90 / numRays) * i), 0.0f) * horizontalInputVector;
            Debug.DrawRay(transform.position, sweepRotationOffsetV);

            if (Physics.Raycast(transform.position, sweepRotationOffsetV, out hit, collider.radius + 0.1f))
            {
                if (hit.collider.CompareTag(groundTag))
                {
                    Vector3 reflection = Vector3.Reflect(horizontalInputVector * rb.velocity.magnitude, hit.normal).normalized;
                    wallJumpTimerCurrent = wallJumpTimer;

                    if (wallJumpLimited)
                        wallJumped = true;

                    // otherwise the velocity stacks and it zooms
                    rb.velocity = Vector3.zero;

                    rb.AddForce((reflection * wallJumpForceHorizontal + Vector3.up * wallJumpForceVertical), ForceMode.VelocityChange);
                    break;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag){
            case "Death":
                game.Restart();
                break;
            case "Ground":
                if (currentSpecialTerrain == SpecialTerrainType.Rubber)
            {
                rb.velocity = new Vector3(rb.velocity.x, -previousVelocity.y * rubberGroundBounciness , rb.velocity.z);
            }
                break;
            case "Bounce":
                rb.AddForce((Vector3.up * 25), ForceMode.VelocityChange);
                break;
            default:
                break;
        }
        
    }

    void AlwaysDrawWallJumpRay()
    {

    }

    void DetermineTerrainType()
    {

    }
}
