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

public class Gun: MonoBehaviour
{
    public GunType gunType;
    public float fireRate;
    public Transform firePoint; // The point from where the bullet is shot
    public float range = 100f;
    public LayerMask hitLayers;
    public XRGrabInteractable grabInteractable;

    private void Start()
    {
        grabInteractable.selectEntered.AddListener(OnGrabStarted);
        grabInteractable.selectExited.AddListener(OnGrabEnded);
        grabInteractable.activated.AddListener(OnGunActivated);
    }

    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        Debug.Log("Grabbed");
    }

    private void OnGrabEnded(SelectExitEventArgs args)
    {
        Debug.Log("Released");
    }

    private void OnGunActivated(ActivateEventArgs args)
    {
        Debug.Log("Gun");
        Shoot();
    }

    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, range, hitLayers))
        {
            Debug.DrawLine(firePoint.position, hit.point, Color.red, 0.1f);
            Debug.Log("Hit: " + hit.collider.name);
        }
        else
        {
            Debug.DrawLine(firePoint.position, firePoint.position + firePoint.forward * range, Color.green, 0.1f);
        }
    }
}