using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image bar;
    public RotationConstraint rotationConstraint;

    private void Start()
    {
        // rotationConstraint.AddSource(new ConstraintSource()
        // {
        //     sourceTransform = Camera.main.transform,
        //     weight = 1
        // });
        // rotationConstraint.constraintActive = true;
    }

    public void UpdateBar(float ratio)
    {
        bar.fillAmount = ratio;
    }

    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform); // Always face the camera
    }
}