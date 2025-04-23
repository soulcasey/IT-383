using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public enum GunType
{
    None = -1,
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

    public int GrabCount { get; private set; }= 0;

    public static readonly Vector3 DEFAULT_SPAWN = new Vector3(0, 1, 0.5f);
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
        GrabCount ++;
    }

    private void OnGrabEnded(SelectExitEventArgs args)
    {
        GrabCount --;

        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
            fireCoroutine = null;
        }
    }

    // TestCode
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space) && gunType == GunType.Launcher)
    //     {
    //         OnGunActivated(null);
    //     }   
    // }

    private void OnGunActivated(ActivateEventArgs args)
    {
        if (IsGrabCount() == false) return;

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
                    grenade.Explode();
                }
                else if (canShoot == true)
                {
                    Launch();
                    StartCoroutine(StartCooldown());
                }
                break;
            }
            default:
            {
                if (canShoot == true)
                {
                    Shoot();
                    StartCoroutine(StartCooldown());
                }
                break;
            }
        }
    }

    private bool IsGrabCount()
    {
        return gunType == GunType.Pistol ? GrabCount == 1 : GrabCount == 2;
    }

    private void OnGunDeactivated(DeactivateEventArgs args)
    {
        if (fireCoroutine != null)
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

    private IEnumerator StartCooldown()
    {
        canShoot = false;
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

    private void Launch()
    {
        audioSource.Play();
        grenade = Grenade.Create(damage, muzzle.transform);
        grenade.Launch(launchForce, muzzle.forward);
    }

    public static List<Gun> Create(GunType gunType, Vector3? position = null)
    {
        List<Gun> guns = new List<Gun>();
        Vector3 spawnPosition = position ?? DEFAULT_SPAWN;

        switch (gunType)
        {
            case GunType.Pistol:
            {
                guns.Add(Instantiate(Resources.Load<Gun>("Prefabs/" + gunType.ToString()), spawnPosition - new Vector3(0.1f, 0, 0), Quaternion.identity));
                guns.Add(Instantiate(Resources.Load<Gun>("Prefabs/" + gunType.ToString()), spawnPosition + new Vector3(0.1f, 0, 0), Quaternion.identity));
                break;
            }
            default:
            {
                guns.Add(Instantiate(Resources.Load<Gun>("Prefabs/" + gunType.ToString()), spawnPosition, Quaternion.identity));
                break;
            }
        }

        return guns;
    }
}
