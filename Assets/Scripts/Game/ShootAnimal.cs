using System;
using System.Collections;
using UnityEngine;

// TODO: Set spawn boundary
public class ShootAnimal : Minigame
{
    public override MinigameType MinigameType => MinigameType.ShootAnimal;
    public TargetType CurrentTargetType { get; private set; } = TargetType.Default;
    public int Score { get; private set; } = 0;

    private Coroutine timeCoroutine;
    private Coroutine spawnLoopCoroutine;


    private const string ANNOUNCEMENT_TEMPLATE = "Target the {0}!";
    private const int TIME_SECONDS = 60;
    private static readonly (int min, int max) SPAWN_INTERVAL_TIME = (1, 4);
    private const int SCORE_MINIMUM = 30;

    private (int min, int max) X_BOUDNARY;
    private (int min, int max) Z_BOUNDARY;
    
    public override void StartMiniGame()
    {
        CurrentTargetType = Logic.GetRandomEnum(TargetType.Default);

        GameManager.Instance.screen.SetScreenText(string.Format(ANNOUNCEMENT_TEMPLATE, CurrentTargetType), 5);

        EventManager.Instance.OnTargetDeath += OnTargetDeath;

        timeCoroutine = StartCoroutine(TimeCoroutine());
        spawnLoopCoroutine = StartCoroutine(SpawnLoopCoroutine());
    }

    private IEnumerator TimeCoroutine()
    {
        yield return new WaitForSeconds(TIME_SECONDS);

        EndMiniGame();
    }

    private IEnumerator SpawnLoopCoroutine()
    {
        while(true)
        {
            float randomInterval = UnityEngine.Random.Range(SPAWN_INTERVAL_TIME.min, SPAWN_INTERVAL_TIME.max);
            TargetType randomTargetType = Logic.GetRandomEnum<TargetType>();

            yield return new WaitForSeconds(randomInterval);

            // Target.Create(randomTargetType, )
        }
    }

    private void OnTargetDeath(TargetType targetType)
    {
        if (targetType == CurrentTargetType)
        {
            Debug.Log("Target killed! +1");
            Score ++;
        }
        else
        {
            Debug.Log("Wrong target... -1");
            Score --;
        }
    }

    public override void EndMiniGame()
    {
        StopCoroutine(spawnLoopCoroutine);
        StopCoroutine(timeCoroutine);
        
        EventManager.Instance.ClearEvents();
    }
}