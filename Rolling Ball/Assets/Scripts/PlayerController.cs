using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    //Checkpoint position handling variables:
    [SerializeField] private GameController game;

    [SerializeField] private float moveSpeed = 8.0f;
    [SerializeField] private float jumpSpeed = 8.0f;
    [SerializeField] private float wallJumpForceHorizontal = 8.0f;
    [SerializeField] private float wallJumpForceVertical = 8.0f;
    

    private Vector3 moveVector = Vector3.zero;
    private Rigidbody rb;
    [SerializeField] private Transform cameraTransform;

    private SphereCollider collider;
    [SerializeField] private float collisionDistanceCheck = 0.6f;
    [SerializeField] private string groundTag = "Ground";
    [SerializeField] private string deathTag = "Death";

    [SerializeField] private bool wallJumpEnabled = true;
    [SerializeField] private bool wallJumpLimited = false;
    private float wallJumpTimer = 0.5f;
    private float wallJumpTimerCurrent = 0.0f;

    private bool wallJumped = false;
    private bool saved = false;

    // Properties
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float JumpSpeed { get => jumpSpeed; set => jumpSpeed = value; }
    public bool WallJumpEnabled { get => wallJumpEnabled; set => wallJumpEnabled = value; }
    public bool WallJumpLimited { get => wallJumpLimited; set => wallJumpLimited = value; }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cameraTransform == null)
        {
            cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        collider = GetComponent<SphereCollider>();

        //Checkpoint position handling:
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        transform.position = game.lastCheckpointPos;
        
    }

    private void Update()
    {
        CheckGrounded();
        // For some reason, this works better over here
        if (Input.GetButtonDown("Jump"))
        {
            if (CheckGrounded())
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            }
        }

        if (wallJumpTimerCurrent > 0)
            wallJumpTimerCurrent -= Time.deltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion cameraRotationFlattened = Quaternion.Euler(0.0f, cameraTransform.rotation.eulerAngles.y, 0.0f);

        moveVector = cameraRotationFlattened * (Input.GetAxis("Horizontal") * Vector3.right + Input.GetAxis("Vertical") * Vector3.forward);

        if (wallJumpTimerCurrent <= 0)
            rb.AddForce(moveVector * moveSpeed);

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
            if (hit.collider.CompareTag(deathTag))
            {
                rb.velocity = Vector3.zero;
                this.transform.position = game.lastCheckpointPos;
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
                if (hit.collider.CompareTag(deathTag))
                {
                rb.velocity = Vector3.zero;
                this.transform.position = game.lastCheckpointPos;
                }
            }
        }

        return false;
    }

    void CheckWalljump(Quaternion cameraRotationY)
    {
        if (wallJumpLimited && wallJumped)
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

    private void OnTriggerEnter(Collider other){
        switch(other.gameObject.tag){
            case "Death":
                game.Restart();
                break;
            default:
                break;
        }
    }
}
