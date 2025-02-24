using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public enum GunType
{
    Pistol,
    Sniper,
    Rifle
}

public class Gun : MonoBehaviour
{
    public GunType gunType;
    public Transform muzzle;
    public TrailRenderer trailRenderer;
    public XRGrabInteractable grabInteractable;
    public AudioSource audioSource;

    public float fireRate;
    public float damage;

    private Coroutine fireCoroutine;
    private bool canShoot = true;

    private const int RANGE = 100;

    private void Start()
    {
        grabInteractable.selectEntered.AddListener(OnGrabStarted);
        grabInteractable.selectExited.AddListener(OnGrabEnded);
        grabInteractable.activated.AddListener(OnGunActivated);
        grabInteractable.deactivated.AddListener(OnGunDeactivated);
    }

    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        Debug.Log("Grabbed");
    }

    private void OnGrabEnded(SelectExitEventArgs args)
    {
        Debug.Log("Released");
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
            fireCoroutine = null;
        }
    }

    private void OnGunActivated(ActivateEventArgs args)
    {
        Debug.Log("Press Trigger");
        
        switch (gunType)
        {
            case GunType.Rifle:
            {
                if (fireCoroutine == null)
                {
                    fireCoroutine = StartCoroutine(RapidFire());
                }
                break;
            }

            default:
            {
                if (canShoot == true)
                {
                    StartCoroutine(SingleShotCooldown());
                }
                break;
            }
        }
    }

    private void OnGunDeactivated(DeactivateEventArgs args)
    {
        Debug.Log("Release Trigger");

        if (gunType == GunType.Rifle && fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
            fireCoroutine = null;
        }
    }

    private IEnumerator RapidFire()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(1f / fireRate);
        }
    }

    private IEnumerator SingleShotCooldown()
    {
        canShoot = false;
        Shoot();
        yield return new WaitForSeconds(1f / fireRate);
        canShoot = true;
    }

    private void Shoot()
    {
        audioSource.Play();

        TrailRenderer trail = Instantiate(trailRenderer, muzzle.position, Quaternion.identity);
        trail.AddPosition(muzzle.position);

        if (Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hit, RANGE))
        {
            Debug.DrawLine(muzzle.position, hit.point, Color.red, 0.1f);
            Debug.Log("Hit: " + hit.collider.name);
            trail.AddPosition(hit.point);

            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.OnHit(damage); // Call interface method
                Debug.Log("Hit object is damageable!");
            }
        }
        else
        {
            Vector3 targetPosition = muzzle.position + muzzle.forward * RANGE;
            Debug.DrawLine(muzzle.position, targetPosition, Color.green, 0.1f);
            trail.AddPosition(targetPosition);
        }
    }
}
