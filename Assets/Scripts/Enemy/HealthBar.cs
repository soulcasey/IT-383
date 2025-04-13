using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image bar;
    private const float HEALTHBAR_Y_OFFSET = 1f;
    private const string PREFAB_DIRECTORY = "Prefabs/HealthBar";

    public static HealthBar Create(Transform transform)
    {
        Vector3 hbPosition = new Vector3(transform.position.x, transform.position.y + HEALTHBAR_Y_OFFSET, transform.position.z);
        return Instantiate(Resources.Load<HealthBar>(PREFAB_DIRECTORY), hbPosition, Quaternion.identity, transform);
    }

    public void UpdateBar(float ratio)
    {
        bar.fillAmount = ratio;
    }

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform); // Always face the camera
    }
}