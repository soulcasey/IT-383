using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject explosionEffect;
    public Rigidbody rb;

    public float explosionRadius = 5f;

    private float damage = 0;
    public float lifeTime = 5f; // Auto-destroy after some time
    private const string PREFAB_DIRECTORY = "Prefabs/Grenade";

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    public static Grenade Create(float damage, Transform transform)
    {
        Grenade grenade = Instantiate(Resources.Load<Grenade>(PREFAB_DIRECTORY), transform.position, transform.rotation);
        grenade.damage = damage;
        return grenade;
    }

    public void Launch(float force, Vector3 direction)
    {
        rb.AddForce(direction * force);
    }

    public void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            IDamageable damageable = nearbyObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.OnHit(damage);
            }
        }

        Destroy(gameObject); // Destroy bomb after explosion
    }
}