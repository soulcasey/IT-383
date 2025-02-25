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
    Rifle,
    Launcher
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
    public float launchForce;

    private Coroutine fireCoroutine;
    private bool canShoot = true;
    private Grenade grenade;

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

    }

    private void OnGrabEnded(SelectExitEventArgs args)
    {
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
            fireCoroutine = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gunType == GunType.Launcher)
        {
            OnGunActivated(null);
        }   
    }

    private void OnGunActivated(ActivateEventArgs args)
    {
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
            case GunType.Launcher:
            {
                if (grenade != null)
                {
                    audioSource.Play();
                    grenade.Explode();
                }
                else if (canShoot == true)
                {
                    StartCoroutine(ShootCooldown());
                }
                break;
            }
            default:
            {
                if (canShoot == true)
                {
                    StartCoroutine(ShootCooldown());
                }
                break;
            }
        }
    }

    private void OnGunDeactivated(DeactivateEventArgs args)
    {
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

    private IEnumerator ShootCooldown()
    {
        canShoot = false;
        Shoot();
        yield return new WaitForSeconds(1f / fireRate);
        canShoot = true;
    }

    private void Shoot()
    {
        audioSource.Play();

        if (trailRenderer != null)
        {
            TrailRenderer trail = Instantiate(trailRenderer, muzzle.position, Quaternion.identity);
            trail.AddPosition(muzzle.position);

            if (Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hit, RANGE))
            {
                Debug.DrawLine(muzzle.position, hit.point, Color.red, 0.1f);
                Debug.Log("Hit: " + hit.collider.name);
                trail.AddPosition(hit.point);

                if (hit.collider.TryGetComponent(out IDamageable damageable) == true)
                {
                    damageable.OnHit(damage);
                }
            }
            else
            {
                Vector3 targetPosition = muzzle.position + muzzle.forward * RANGE;
                Debug.DrawLine(muzzle.position, targetPosition, Color.green, 0.1f);
                trail.AddPosition(targetPosition);
            }
        }
        else
        {
            grenade = Grenade.Create(damage, muzzle.transform);
            grenade.Launch(launchForce, muzzle.forward);
        }
    }
}
