using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Add text fade with tween
// TODO: Add event to check on target shoot
public enum DrawType
{
    Wait,
    WaitFake,
    Shoot,
}

public class QuickDraw : Minigame
{
    public override MinigameType MinigameType => MinigameType.QuickDraw;

    private Queue<DrawType> drawQueue = new Queue<DrawType>();

    private Coroutine waitCoroutine;
    
    private const string ANNOUNCEMENT_WAIT = "Wait for it...";
    private const string ANNOUNCEMENT_WAIT_FAKE = "WAITFORIT!!";
    private const string ANNOUNCEMENT_SHOOT = "SHOOT!!";
    private const string COWBOY_MUSIC = "Sound/CowboyMusic";
    private const float COUNTDOWN_TIME = 0.5f;
    private const float TEXT_DISPLAY_TIME = 1f;
    private const float WAIT_FAKE_CHANCE = 30;

    private readonly Vector3 TARGET_SPAWN_POSITION = new Vector3(0, 0.482f, 5.532f);
    private readonly (int min, int max) WAIT_COUNT = new (5, 10);  
    private readonly (float min, float max) GAP_TIME = new (0.5f, 3f); 
    private static readonly (int min, int max) X_BOUNDARY = (-4, 4);
    private static readonly (int min, int max) Z_BOUNDARY = (6, 10);
    
    public override void StartMiniGame()
    {
        drawQueue.Clear();

        drawQueue.Enqueue(DrawType.Wait);

        // min-2 and max-1 because first and last are fixed
        int waitCount = UnityEngine.Random.Range(WAIT_COUNT.min - 2, WAIT_COUNT.max - 1);

        for (int i = 0; i < waitCount; i ++)
        {
            drawQueue.Enqueue(UnityEngine.Random.Range(0, 100) >= WAIT_FAKE_CHANCE ? DrawType.Wait : DrawType.WaitFake);
        }

        drawQueue.Enqueue(DrawType.Shoot);

        Target target = Target.Create(TargetType.Dog, TARGET_SPAWN_POSITION, 1);
        target.SetBoundary(X_BOUNDARY.min, X_BOUNDARY.max, Z_BOUNDARY.min, Z_BOUNDARY.max);
        target.status = TargetStatus.Loop;

        targets.Add(target);

        EventManager.Instance.OnTargetDeath += OnTargetDeath;

        GameManager.Instance.PlayMusic(COWBOY_MUSIC);

        waitCoroutine = StartCoroutine(WaitCoroutine());
    }

    public override void EndMiniGame()
    {
        GameManager.Instance.StopMusic();
        Result = drawQueue.Peek() == DrawType.Shoot ? MiniGameResult.Win : MiniGameResult.Lose;

        foreach (var target in targets)
        {
            target.canHit = false;
            if (Result == MiniGameResult.Lose && target.status != TargetStatus.Death)
            {
                target.status = TargetStatus.Mock;
            }
        }
    }

    private IEnumerator WaitCoroutine()
    {
        while (drawQueue.Count > 0)
        {
            DrawType currentDrawType = drawQueue.Peek();

            string displayText = currentDrawType switch
            {
                DrawType.Wait => ANNOUNCEMENT_WAIT,
                DrawType.WaitFake => ANNOUNCEMENT_WAIT_FAKE,
                DrawType.Shoot => ANNOUNCEMENT_SHOOT,
                _ => string.Empty
            };

            GameManager.Instance.screen.SetScreenText(displayText, TEXT_DISPLAY_TIME);

            float gapTime = currentDrawType == DrawType.Shoot ? COUNTDOWN_TIME : TEXT_DISPLAY_TIME + UnityEngine.Random.Range(GAP_TIME.min, GAP_TIME.max);

            yield return new WaitForSeconds(gapTime);

            drawQueue.Dequeue();
        }

        GameManager.Instance.EndCurrentGame();
    }

    private void OnTargetDeath(TargetType targetType)
    {
        StopCoroutine(waitCoroutine);

        GameManager.Instance.EndCurrentGame();
    }
}