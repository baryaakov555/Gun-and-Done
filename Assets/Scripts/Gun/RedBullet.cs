using UnityEngine;

public class RedBullet : MonoBehaviour
{
    private Rigidbody rb;

    public float explosionRadius = 5.0f;
    public float explosionForce = 500.0f;
    public float upwardsModifier = 2.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        DestroyBullet();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Explode();
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        Vector3 explosionPosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();

            if (hitRb != null)
            {
                // Apply explosion force
                hitRb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject, 5f);
    }
}
