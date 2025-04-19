using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Set spawn boundary
public class ShootAnimal : Minigame
{
    public override MinigameType MinigameType => MinigameType.ShootAnimal;
    public TargetType CurrentTargetType { get; private set; } = TargetType.Default;
    public int Score { get; private set; } = 0;
    private int time = 0;

    private Coroutine timeCoroutine;
    private Coroutine spawnLoopCoroutine;


    private const string ANNOUNCEMENT_TEMPLATE = "Target the {0}!";
    private const int TIME_SECONDS = 40;
    private static readonly (float min, float max) SPAWN_INTERVAL_TIME = (0.2f, 1.5f);
    private const int SCORE_MINIMUM = 10;

    private static readonly (int min, int max) X_BOUNDARY = (-4, 4);
    private static readonly (int min, int max) Z_BOUNDARY = (6, 10);
    
    public override void StartMiniGame()
    {
        CurrentTargetType = Logic.GetRandomEnum(TargetType.Default);

        EventManager.Instance.OnTargetDeath += OnTargetDeath;

        timeCoroutine = StartCoroutine(TimeCoroutine());
        spawnLoopCoroutine = StartCoroutine(SpawnLoopCoroutine());
    }

    private IEnumerator TimeCoroutine()
    {
        for (time = TIME_SECONDS; time > 0; time--)
        {
            DisplayScreen();
            yield return new WaitForSeconds(1);
        }

        GameManager.Instance.EndCurrentGame();
    }

    private IEnumerator SpawnLoopCoroutine()
    {
        while(true)
        {
            float randomInterval = UnityEngine.Random.Range(SPAWN_INTERVAL_TIME.min, SPAWN_INTERVAL_TIME.max);
            TargetType randomTargetType = Logic.GetRandomEnum(TargetType.Default);
            float randomX = UnityEngine.Random.Range(X_BOUNDARY.min, X_BOUNDARY.max);
            float randomZ = UnityEngine.Random.Range(Z_BOUNDARY.min, Z_BOUNDARY.max);
            

            yield return new WaitForSeconds(randomInterval);

            Target target = Target.Create(randomTargetType, new Vector3(randomX, 0, randomZ), 10);
            targets.Add(target);
            target.SetBoundary(X_BOUNDARY.min, X_BOUNDARY.max, Z_BOUNDARY.min, Z_BOUNDARY.max);
            target.status = TargetStatus.Loop;
        }
    }

    private void DisplayScreen()
    {
        string text = string.Format($"{ANNOUNCEMENT_TEMPLATE}\n", CurrentTargetType) + "\n" + $"Time: {time}" + "\n" + $"Score: {Score}";
        GameManager.Instance.screen.SetScreenText(text, 5);
    }

    private void OnTargetDeath(TargetType targetType)
    {
        Debug.LogWarning(targetType + " " + CurrentTargetType);    

        if (targetType == CurrentTargetType)
        {
            Debug.Log("Target killed! +1");
            Score ++;
        }
        else
        {
            Debug.Log("Wrong target... -1");
            if (Score > 0) Score --;
        }

        DisplayScreen();
    }

    public override void EndMiniGame()
    {
        StopCoroutine(spawnLoopCoroutine);
        StopCoroutine(timeCoroutine);

        Result = Score >= SCORE_MINIMUM ? MiniGameResult.Win : MiniGameResult.Lose;

        foreach (var target in targets)
        {
            target.canHit = false;
            target.status = Result == MiniGameResult.Lose ? TargetStatus.Mock : TargetStatus.Idle;
        }
    }
}