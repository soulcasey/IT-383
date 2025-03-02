using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour
{
    public Rigidbody rb;

    public float explosionRadius = 2.5f;

    private Coroutine autoCoroutine;
    private float damage = 0;
    private const string PREFAB_DIRECTORY = "Prefabs/Grenade";
    private const float AUTO_DESTROY_DELAY = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Gun _) == true)
        {
            return;
        }
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
        Debug.Log("Launch");
        rb.AddForce(direction * force);
        autoCoroutine = StartCoroutine(AutoExplode());
    }

    private IEnumerator AutoExplode()
    {
        yield return new WaitForSeconds(AUTO_DESTROY_DELAY);
        Explode();
    }

    public void Explode()
    {
        Debug.Log("Explode");

        if (autoCoroutine != null)
        {
            StopCoroutine(autoCoroutine);
        }

        Explosion.Create(transform);

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