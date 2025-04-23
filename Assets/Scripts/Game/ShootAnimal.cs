using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// TODO: Set spawn boundary
public class ShootAnimal : Minigame
{
    public override MinigameType MinigameType => MinigameType.ShootAnimal;
    public TargetType CurrentTargetType { get; private set; }
    public int Score { get; private set; } = 0;
    public int Life { get; private set; } = 4;
    private int time = 0;

    private const int TIME_SECONDS = 30;
    private const float SPAWN_INTERVAL = 0.2f;
    private const int TARGET_COUNT_MAIN = 8;
    private const int TARGET_COUNT_OTHER = 12;


    public override void StartMiniGame()
    {   
        base.StartMiniGame();
        CurrentTargetType = Logic.GetRandomEnum<TargetType>();

        StartCoroutine(DisplayTargets(() => 
        {
            EventManager.Instance.OnTargetDeath += OnTargetDeath;
            StartCoroutine(TimeCoroutine());
            StartCoroutine(SpawnLoopCoroutine());
        }));
    }

    private IEnumerator DisplayTargets(Action onComplete)
    {
        GameManager.Instance.screen.SetScreenText($"Eliminate all {CurrentTargetType} without hitting others!", 3.5f);

        yield return new WaitForSeconds(4);

        onComplete.Invoke();
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
        List<TargetType> targetTypes = new List<TargetType>();

        for (int i = 0; i < TARGET_COUNT_MAIN; i ++)
        {
            targetTypes.Add(CurrentTargetType);
        }

        for (int i = 0; i < TARGET_COUNT_OTHER; i ++)
        {
            targetTypes.Add(Logic.GetRandomEnum(exceptions: CurrentTargetType));
        }

        Logic.Shuffle(targetTypes);

        foreach (TargetType targetType in targetTypes)
        {
            yield return new WaitForSeconds(SPAWN_INTERVAL);

            Target target = Target.Create(targetType);
            targets.Add(target);
            target.status = TargetStatus.Walk;
        }
    }

    private void DisplayScreen()
    {
        string text = $"Time: {time}\nScore: {Score}/{TARGET_COUNT_MAIN}\nLife: {Life}";
        GameManager.Instance.screen.SetScreenText(text, 5);
    }

    private void OnTargetDeath(TargetType targetType)
    {
        Debug.LogWarning(targetType + " " + CurrentTargetType);    

        if (targetType == CurrentTargetType)
        {
            Score ++;
            if (Score >= TARGET_COUNT_MAIN)
            {
                GameManager.Instance.EndCurrentGame();
            }
        }
        else
        {
            Life --;
            if (Life <= 0)
            {
                GameManager.Instance.EndCurrentGame();
            }
        }

        DisplayScreen();
    }

    public override void EndMiniGame()
    {
        StopAllCoroutines();

        Result = Score == TARGET_COUNT_MAIN ? MiniGameResult.Win : MiniGameResult.Lose;

        foreach (var target in targets)
        {
            target.canHit = false;
            target.status = Result == MiniGameResult.Lose ? TargetStatus.Mock : TargetStatus.Idle;
        }
    }
}