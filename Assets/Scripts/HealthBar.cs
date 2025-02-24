using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image bar;

    public void UpdateBar(float ratio)
    {
        bar.fillAmount = ratio;
    }

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform); // Always face the camera
    }
}