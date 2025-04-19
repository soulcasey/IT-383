using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MemoryResult
{
    Unknown,
    Correct,
    Wrong,
}


public class MemoryShoot : Minigame
{
    public override MinigameType MinigameType => MinigameType.MemoryShoot;

    private List<TargetType> targetOrder = new List<TargetType>();
    public List<MemoryResult> memoryResults = new List<MemoryResult>();
    private int targetKillCount = 0;
    private Coroutine timeCoroutine;
    private int time = 0;
    private const int TARGET_COUNT = 6;
    private const int TIME_SECONDS = 40;
    private static readonly (int min, int max) X_BOUNDARY = (-4, 4);
    private static readonly (int min, int max) Z_BOUNDARY = (6, 10);
    
    public override void StartMiniGame()
    {
        while (targetOrder.Count < TARGET_COUNT)
        {
            targetOrder.Add(Logic.GetRandomEnum(exceptions: TargetType.Default));
            memoryResults.Add(MemoryResult.Unknown);
        }

        StartCoroutine(DisplayTargets(() => 
        {
            EventManager.Instance.OnTargetDeath += OnTargetDeath;
            SpawnTargets();
            timeCoroutine = StartCoroutine(TimeCoroutine());
        }));
    }

    private void SpawnTargets()
    {
        for(int i = 0; i < TARGET_COUNT; i ++)
        {
            foreach (TargetType targetType in Enum.GetValues(typeof(TargetType)))
            {
                if (targetType == TargetType.Default) continue;
                
                float randomX = UnityEngine.Random.Range(X_BOUNDARY.min, X_BOUNDARY.max);
                float randomZ = UnityEngine.Random.Range(Z_BOUNDARY.min, Z_BOUNDARY.max);

                Target target = Target.Create(targetType, new Vector3(randomX, 0, randomZ), 10);
                targets.Add(target);
                target.SetBoundary(X_BOUNDARY.min, X_BOUNDARY.max, Z_BOUNDARY.min, Z_BOUNDARY.max);
                target.status = TargetStatus.Loop;
            }
        }
    }

    private IEnumerator DisplayTargets(Action onComplete)
    {
        foreach (TargetType targetType in targetOrder)
        {
            GameManager.Instance.screen.SetScreenText(targetType.ToString(), 1);

            yield return new WaitForSeconds(2);
        }

        onComplete.Invoke();
    }

    private void DisplayScreen()
    {
        string results = string.Join(" ", memoryResults.Select(result => result switch
        {
            MemoryResult.Correct => "O",
            MemoryResult.Wrong => "X",
            _ => "?"
        }));
        string text = results + "\n" + $"Time: {time}";
        GameManager.Instance.screen.SetScreenText(text, 5);
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

    public override void EndMiniGame()
    {
        StopCoroutine(timeCoroutine);

        Result = memoryResults.All(result => result == MemoryResult.Correct) ? MiniGameResult.Win : MiniGameResult.Lose;

        foreach (var target in targets)
        {
            target.canHit = false;
            target.status = Result == MiniGameResult.Lose ? TargetStatus.Mock : TargetStatus.Idle;
        }
    }

    private void OnTargetDeath(TargetType targetType)
    {
        if (targetKillCount >= targetOrder.Count)
        {
            return;
        }

        MemoryResult result = targetOrder[targetKillCount] == targetType ? MemoryResult.Correct : MemoryResult.Wrong;
        memoryResults[targetKillCount] = result;
        targetKillCount ++;
        DisplayScreen();

        if (result == MemoryResult.Wrong || memoryResults.All(result => result == MemoryResult.Correct))
        {
            GameManager.Instance.EndCurrentGame();
        }
    }
}