using UnityEngine;

public class Target : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;

    public GameObject healthBarPrefab;
    private HealthBar healthBarInstance;

    private const float HEALTHBAR_Y_OFFSET = 0.8f;

    void Start()
    {
        currentHealth = maxHealth;

        Vector3 hbPosition = new Vector3(transform.position.x, transform.position.y + HEALTHBAR_Y_OFFSET, transform.position.z);
        GameObject hb = Instantiate(healthBarPrefab, hbPosition, Quaternion.identity, transform);
        healthBarInstance = hb.GetComponent<HealthBar>();
        hb.SetActive(false); // Hide initially
    }

    public void OnHit(float damage)
    {
        currentHealth -= damage;
        healthBarInstance.gameObject.SetActive(true); // Show health bar when hit
        healthBarInstance.UpdateBar(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}