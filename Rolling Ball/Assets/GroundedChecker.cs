using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedChecker : MonoBehaviour
{
    [SerializeField] private string groundTag = "Ground";

    private Transform originalParent;
    [SerializeField] private Vector3 parentOffset = Vector3.zero;

    // Grounded property
    public bool Grounded { get; private set; } = false;

    public void Start()
    {
        originalParent = transform.parent;
        transform.parent = null;
    }

    public void LateUpdate()
    {
        transform.position = originalParent.position + parentOffset;
    }

    private void OnTriggerStay(Collider other)
    {
        Grounded = other.CompareTag(groundTag) ? true : false;
    }
}
