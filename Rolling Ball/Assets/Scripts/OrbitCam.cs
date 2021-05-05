using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCam : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private MenuController menuController;
    [SerializeField] private Vector3 offset = new Vector3(0.0f, 0.0f, -10.0f);

    private Vector3 offsetActual;

    [SerializeField] private float yRotation = 0.0f;
    [SerializeField] private float xRotation = 5.0f;
    [SerializeField] private bool invertCamera = false;
    [SerializeField] private float mouseLookSpeed = 5.0f;

    public Transform Target { get => target; set => target = value; }

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }


        offsetActual = offset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        yRotation += Input.GetAxis("Mouse X") * mouseLookSpeed;
        xRotation += Input.GetAxis("Mouse Y") * mouseLookSpeed;

        xRotation = Mathf.Clamp(xRotation, 0.0f, 89.0f);

        CheckIfIntersectingTheGround();

        Vector3 offsetTransformed = Quaternion.Euler(xRotation, yRotation, 0.0f) * offsetActual;
        transform.position = target.position + offsetTransformed;

        if (Physics.Raycast(target.position, (transform.position - target.position).normalized, out RaycastHit hit))
        {
           if (hit.collider.CompareTag("Wall") && hit.distance <= offset.z)
                transform.position = hit.point;
        }

        transform.LookAt(target);
    }

    void CheckIfIntersectingTheGround()
    {
        RaycastHit hit;

        //Physics.Raycast(transform.position + hitOffsetRotated, Vector3.down, out hit, collisionDistanceCheck))
        Debug.DrawLine(target.position, transform.position - target.position, Color.red);
        if (Physics.Raycast(target.position, transform.position - target.position, out hit, Vector3.Distance(target.position, transform.position) * 2))
        {
            offsetActual.z = Mathf.Max(offset.z, -Vector3.Distance(target.position, transform.position));
        }
    }
}
