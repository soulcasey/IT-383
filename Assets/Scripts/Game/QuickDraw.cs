using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private const string ANNOUNCEMENT_WAIT = "Wait for it...";
    private const string ANNOUNCEMENT_WAIT_FAKE = "WAITFORIT!!";
    private const string ANNOUNCEMENT_SHOOT = "SHOOT!!";
    private const string COWBOY_MUSIC = "Sound/CowboyMusic";
    private const float COUNTDOWN_TIME = 1f;
    private const float TEXT_DISPLAY_TIME = 1f;
    private const float WAIT_FAKE_CHANCE = 30;

    private readonly (int min, int max) WAIT_COUNT = new (5, 10);  
    private readonly (float min, float max) GAP_TIME = new (0.5f, 3f); 

    public override void StartMiniGame()
    {
        base.StartMiniGame();

        drawQueue.Clear();

        drawQueue.Enqueue(DrawType.Wait);

        // min-2 and max-1 because first and last are fixed
        int waitCount = UnityEngine.Random.Range(WAIT_COUNT.min - 2, WAIT_COUNT.max - 1);

        for (int i = 0; i < waitCount; i ++)
        {
            drawQueue.Enqueue(UnityEngine.Random.Range(0, 100) >= WAIT_FAKE_CHANCE ? DrawType.Wait : DrawType.WaitFake);
        }

        drawQueue.Enqueue(DrawType.Shoot);

        Vector3 spawnPosition =  spawnPosition = Camera.main.transform.forward.normalized * 7f;
        spawnPosition.y = 0f;
        Target target = Target.Create(Logic.GetRandomEnum<TargetType>(), position: spawnPosition, maxHealth: 1);
        target.status = TargetStatus.Walk;

        targets.Add(target);

        EventManager.Instance.OnTargetDeath += OnTargetDeath;

        GameManager.Instance.PlayMusic(COWBOY_MUSIC);

        StartCoroutine(WaitCoroutine());
    }

    public override MiniGameResult GetResult()
    {
        return drawQueue.TryPeek(out DrawType drawtype) == true && drawtype == DrawType.Shoot ? MiniGameResult.Win : MiniGameResult.Lose;
    }

    private IEnumerator WaitCoroutine()
    {
        GameManager.Instance.screen.SetScreenText("Wait for the moment... don't shoot yet", 4f);

        yield return new WaitForSeconds(4.5f);

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

            if (currentDrawType == DrawType.Shoot)
            {
                targets.First().status = TargetStatus.Roll;

                float timer = COUNTDOWN_TIME;
                while (timer > 0f)
                {
                    GameManager.Instance.screen.SetScreenText($"{displayText}\n{timer:F2}s", 0.1f);
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                GameManager.Instance.screen.SetScreenText(displayText, TEXT_DISPLAY_TIME);

                float gapTime = TEXT_DISPLAY_TIME + UnityEngine.Random.Range(GAP_TIME.min, GAP_TIME.max);
                yield return new WaitForSeconds(gapTime);
            }

            drawQueue.Dequeue();
        }

        GameManager.Instance.EndCurrentGame();
    }

    private void OnTargetDeath(TargetType targetType)
    {
        GameManager.Instance.EndCurrentGame();
    }
}