using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnParent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform player;
    [SerializeField] float verticalOffset;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate() 
    {
    

      transform.position = player.position + Vector3.up * verticalOffset;
    }
}
