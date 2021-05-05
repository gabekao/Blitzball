using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] public GameObject[] spawns;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float speed = 5;
    private Vector3[] nextPos;
    private Vector3 moveVector = Vector3.zero;
    private Rigidbody rb;
    private float direction = 1.0f;
    private int j = 0;
    //private Vector3 nextPos;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nextPos = new Vector3[spawns.Length];
        for (int i = 0; i < spawns.Length; i++){
            nextPos[i] = spawns[i].transform.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(rb.position, nextPos[j]) <= 5){
            rb.velocity = new Vector3(0, 0, 0);
            rb.angularVelocity = new Vector3(0, 0, 0);
            Debug.Log("AY");
            transform.position = nextPos[j+1];
            direction *= -1;
            j = (j + 2) % 4;
            Debug.Log(j);
        }
        moveVector = (direction * Vector3.right);
        rb.AddForce(moveVector * speed);

    }
}
