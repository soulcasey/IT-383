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
    public Transform muzzle;
    public float range = 100f;
    public TrailRenderer trace;
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
        trace.Clear();
        trace.AddPosition(muzzle.transform.position);

        if (Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hit, range))
        {
            Debug.DrawLine(muzzle.position, hit.point, Color.red, 0.1f);
            Debug.Log("Hit: " + hit.collider.name);
            trace.AddPosition(hit.point);
        }
        else
        {
            Vector3 targetPosition = muzzle.position + muzzle.forward * range;
            Debug.DrawLine(muzzle.position, muzzle.position + muzzle.forward * range, Color.green, 0.1f);
            trace.AddPosition(transform.position = targetPosition);
        }
    }

    // Test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Shoot();
        }
    }
}