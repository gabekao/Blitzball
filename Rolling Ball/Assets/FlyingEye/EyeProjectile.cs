using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeProjectile : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private float speed = 10.0f;

    bool exploded = false;

    // Start is called before the first frame update
    void Start()
    {
        if (particles == null)
            particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (exploded && !particles.isPlaying)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        exploded = true;
        particles.Stop();
        GameObject go = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(go, 3);
    }
}
