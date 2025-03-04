using System.Collections;
using UnityEngine;

public class Bullet: MonoBehaviour
{
    public TrailRenderer trailRenderer;

    private void Start()
    {
        StartCoroutine(RemoveBullet());
    }

    private IEnumerator RemoveBullet()
    {
        float duration = trailRenderer.time;

        yield return new WaitForSeconds(duration);

        Destroy(gameObject);
    }

}