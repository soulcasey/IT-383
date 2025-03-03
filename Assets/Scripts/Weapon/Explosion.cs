using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject explosionObject;

    private const string PREFAB_DIRECTORY = "Prefabs/Explosion";
    private const float DURATION = 0.4f;
    

    public static void Create(Transform transform)
    {
        Explosion explosion = Instantiate(Resources.Load<Explosion>(PREFAB_DIRECTORY), transform.position, transform.rotation);
        explosion.StartCoroutine(explosion.Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(DURATION);

        explosionObject.SetActive(false);

        yield return new WaitForSeconds(audioSource.clip.length - DURATION);

        Destroy(gameObject);
    }
}