using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCam : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0.0f, 0.0f, -10.0f);

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
    }

    // Update is called once per frame
    void LateUpdate()
    {
        yRotation += Input.GetAxis("Mouse X") * mouseLookSpeed;
        xRotation += Input.GetAxis("Mouse Y") * mouseLookSpeed;

        xRotation = Mathf.Clamp(xRotation, 0.0f, 89.0f);

        Cursor.lockState = CursorLockMode.Locked;


        Vector3 offsetTransformed = Quaternion.Euler(xRotation, yRotation, 0.0f) * offset;
        transform.position = target.position + offsetTransformed;



        transform.LookAt(target);
    }
}
