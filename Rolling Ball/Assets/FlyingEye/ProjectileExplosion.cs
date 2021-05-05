using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplosion : MonoBehaviour
{
    [SerializeField] AudioClip[] explosionClips;
    private AudioSource source;

    [SerializeField] private float explosiveForce = 10.0f;
    [SerializeField] private float explosionRadius = 3.0f;
    [SerializeField] private float upwardsModifier = 1.0f;



    void Start()
    {
        source = GetComponent<AudioSource>();
        source.PlayOneShot(explosionClips[Random.Range(0, explosionClips.Length)]);

        // just make sure the player is tagged
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Rigidbody>().AddExplosionForce(explosiveForce, transform.position, explosionRadius, upwardsModifier, ForceMode.VelocityChange);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
