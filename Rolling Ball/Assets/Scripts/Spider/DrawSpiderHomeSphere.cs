using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSpiderHomeSphere : MonoBehaviour
{
    // this is to draw the radius of the sphere in the scene view for convenience
    [SerializeField] private SpiderRobotScript srs;
    private float gizmoRadius;

    private void FixedUpdate()
    {
        if (srs !=null)
            gizmoRadius = srs.PatrolRadius;
    }

    private void OnDrawGizmos()
    {
        if (srs != null)
            Gizmos.color = new Color(0xFF, 0xFF, 0x88);
            Gizmos.DrawWireSphere(transform.position, gizmoRadius);
    }
}
