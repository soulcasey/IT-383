using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

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