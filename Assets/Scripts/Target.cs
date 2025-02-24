using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;

    private HealthBar healthBar;
    private Outline outline;
    private Coroutine outlineCoroutine;

    private const float HEALTHBAR_Y_OFFSET = 0.8f;

    private void Start()
    {
        currentHealth = maxHealth;

        Vector3 hbPosition = new Vector3(transform.position.x, transform.position.y + HEALTHBAR_Y_OFFSET, transform.position.z);
        HealthBar hb = Instantiate(Resources.Load<HealthBar>($"Prefabs/HealthBar"), hbPosition, Quaternion.identity, transform);
        healthBar = hb.GetComponent<HealthBar>();
        healthBar.gameObject.SetActive(false);

        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }
        
        outline.enabled = false;
        outline.OutlineWidth = 10;
        outline.OutlineColor = Color.red;
    }

    public void OnHit(float damage)
    {
        currentHealth -= damage;
        healthBar.gameObject.SetActive(true);
        healthBar.UpdateBar(currentHealth / maxHealth);

        if (outlineCoroutine != null)
        {
            StopCoroutine(outlineCoroutine);
        }
        outlineCoroutine = StartCoroutine(ShowOutline(0.5f));

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ShowOutline(float duration)
    {
        outline.enabled = true;
        yield return new WaitForSeconds(duration);
        outline.enabled = false;
    }

    // Test
    private void OnMouseDown()
    {
        OnHit(10);
    }
}