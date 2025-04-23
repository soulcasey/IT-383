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
    private int time = 0;
    private const int TARGET_COUNT = 4;
    private const int TIME_SECONDS = 40;
    
    public override void StartMiniGame()
    {
        base.StartMiniGame();

        while (targetOrder.Count < TARGET_COUNT)
        {
            targetOrder.Add(Logic.GetRandomEnum<TargetType>());
            memoryResults.Add(MemoryResult.Unknown);
        }

        StartCoroutine(DisplayTargets(() => 
        {
            EventManager.Instance.OnTargetDeath += OnTargetDeath;
            SpawnTargets();
            StartCoroutine(TimeCoroutine());
        }));
    }

    private void SpawnTargets()
    {
        for(int i = 0; i < TARGET_COUNT; i ++)
        {
            foreach (TargetType targetType in Enum.GetValues(typeof(TargetType)))
            {   
                Target target = Target.Create(targetType);
                targets.Add(target);
                target.status = TargetStatus.Walk;
            }
        }
    }

    private IEnumerator DisplayTargets(Action onComplete)
    {
        GameManager.Instance.screen.SetScreenText("Remember the order of the animals, then eliminate them in that order!", 3.5f);

        yield return new WaitForSeconds(4);

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

    public override MiniGameResult GetResult()
    {
        return memoryResults.All(result => result == MemoryResult.Correct) ? MiniGameResult.Win : MiniGameResult.Lose;
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