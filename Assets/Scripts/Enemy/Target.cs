using System.Collections;
using Unity.Mathematics;
using UnityEngine;


public enum TargetType
{
    Default,
    Dog,
    Sparrow
}

public enum TargetMovement
{
    Idle,
    Loop,
}

public class Target : MonoBehaviour, IDamageable
{
    public TargetType targetType;
    public float maxHealth = 100f;
    public float speed = 1f;
    [SerializeField]
    private Animator animator;

    [Header("Movement")]
    public TargetMovement movementType = TargetMovement.Idle;
    public Vector3 targetPosition;
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    private float currentHealth;

    private HealthBar healthBar;
    private Outline outline;

    private Coroutine outlineCoroutine;

    private const string PREFAB_DIRECTORY = "Prefabs/";
    private const float DISTANCE_THREADHOLD = 0.2f;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar = HealthBar.Create(transform);
        healthBar.gameObject.SetActive(false);

        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        if (targetPosition == null)
        {
            targetPosition = transform.position;
        }
        
        outline.enabled = false;
        outline.OutlineWidth = 10;
        outline.OutlineColor = Color.red;
    }

    private void Update()
    {
        switch (movementType)
        {
            case TargetMovement.Idle:
                animator.Play("Idle_A");
                break;

            case TargetMovement.Loop:
                animator.Play("Run");

                if (Vector3.Distance(transform.position, targetPosition) < DISTANCE_THREADHOLD)
                {
                    float randomX = UnityEngine.Random.Range(minX, maxX);
                    float randomZ = UnityEngine.Random.Range(minZ, maxZ);
                    targetPosition = new Vector3(randomX, transform.position.y, randomZ);
                }

                transform.rotation = Quaternion.LookRotation((targetPosition - transform.position).normalized);
                Vector3 move = speed * Time.deltaTime * transform.forward;

                transform.position += move;
                break;
        }
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
            Death();
        }
    }

    private IEnumerator ShowOutline(float duration)
    {
        outline.enabled = true;
        yield return new WaitForSeconds(duration);
        outline.enabled = false;
    }

    private void Death()
    {
        EventManager.Instance.TargetDeath(targetType);
        Destroy(gameObject);
    }

    public static Target Create(TargetType targetType, Vector3 position, float maxHealth = 100)
    {
        string path = PREFAB_DIRECTORY + targetType.ToString();
        Target target = Resources.Load<Target>(path);

        if (target == null)
        {
            Debug.LogError($"Prefab not found at path: {path}");
            return null;
        }

        target.targetType = targetType;

        target.maxHealth = maxHealth;

        target.targetPosition = position;

        return Instantiate(target, position, Quaternion.identity);
    }
    
    public void SetBoundary(int minX, int maxX, int minZ, int maxZ)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minZ = minZ;
        this.maxZ = maxZ;
    }
}
