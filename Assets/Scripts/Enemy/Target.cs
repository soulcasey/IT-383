using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;


public enum TargetType
{
    Dog,
    Sparrow,
    Snake,
    Fish,
    Monkey,
}

public enum TargetStatus
{
    Idle,
    Walk,
    Roll,
    Death,
    Mock,
}

public class Target : MonoBehaviour, IDamageable
{
    public TargetType targetType;
    public float maxHealth = 100f;

    public bool canHit = true;
    private Animator animator;

    [Header("Movement")]
    public TargetStatus status = TargetStatus.Idle;
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

    private static readonly (int min, int max) BOUNDARY = (-9, 9);

    private void Start()
    {
        animator = GetComponent<Animator>();

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
        switch (status)
        {
            case TargetStatus.Idle:
                animator.Play("Idle_A");
                break;
            case TargetStatus.Walk:
                animator.Play("Run");
                WalkLoop(1);
                break;
            case TargetStatus.Roll:
                animator.Play("Roll");
                WalkLoop(2);
                break;
            case TargetStatus.Mock:
                animator.Play("Spin");
                break;
            case TargetStatus.Death:
                animator.Play("Death");
                break;
        }
    }

    private void WalkLoop(float speed)
    {
        if (Vector3.Distance(transform.position, targetPosition) < DISTANCE_THREADHOLD)
        {
            targetPosition = GetRandomPosition();
        }

        transform.rotation = Quaternion.LookRotation((targetPosition - transform.position).normalized);
        Vector3 move = speed * Time.deltaTime * transform.forward;

        transform.position += move;
    }

    public void OnHit(float damage)
    {
        if (canHit == false) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

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
        canHit = false;
        status = TargetStatus.Death;
        EventManager.Instance.TargetDeath(targetType);
        StartCoroutine(PlayDeathAnimation());
    }

    private IEnumerator PlayDeathAnimation()
    {
        yield return new WaitForSeconds(0.8f);
        gameObject.SetActive(false);
    }


    public static Target Create(TargetType targetType, Vector3? position = null, float maxHealth = 10)
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

        target.targetPosition = position != null ? (Vector3)position : target.GetRandomPosition();

        return Instantiate(target, target.targetPosition, Quaternion.identity);
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            UnityEngine.Random.Range(BOUNDARY.min, BOUNDARY.max + 1),
            0,
            UnityEngine.Random.Range(BOUNDARY.min, BOUNDARY.max + 1));
    }
}
