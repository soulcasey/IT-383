using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class GameManager : SingletonBase<GameManager>
{
    public Minigame activeMiniGame;
    public int CurrentRound { get; private set; } = 1;

    public Tutorial tutorial;
    
    public AudioSource audioSource;

    // Screen
    public Screen screen;
    private const string NEW_ROUND_ANNOUNCEMENT_TEMPLATE = "Minigame:\n{0}";
    private const float SCREEN_DISPLAY_TIME = 4f;
    private const float START_DELAY_TIME = 0.5f;

    public void StartNewRound()
    {
        StartCoroutine(StartRoundCoroutine());
    }

    private IEnumerator StartRoundCoroutine()
    {
        MinigameType randomMinigame = activeMiniGame == null ? Logic.GetRandomEnum<MinigameType>() : Logic.GetRandomEnum(activeMiniGame.MinigameType) ;
        activeMiniGame = (Minigame)gameObject.AddComponent(Type.GetType(randomMinigame.ToString()));

        screen.SetScreenText($"Round {CurrentRound}", SCREEN_DISPLAY_TIME);

        yield return new WaitForSeconds(SCREEN_DISPLAY_TIME);

        MinigameType[] allMinigames = (MinigameType[])Enum.GetValues(typeof(MinigameType));
        float delay = 0.1f;
        int spinCycles = UnityEngine.Random.Range(7,11);
        int totalSteps = spinCycles * allMinigames.Length + Array.IndexOf(allMinigames, randomMinigame);

        for (int i = 0; i <= totalSteps; i++)
        {
            MinigameType tempType = allMinigames[i % allMinigames.Length];
            screen.SetScreenText(string.Format(NEW_ROUND_ANNOUNCEMENT_TEMPLATE, tempType), delay);
            yield return new WaitForSeconds(delay);
        }

        screen.SetScreenText(string.Format(NEW_ROUND_ANNOUNCEMENT_TEMPLATE, activeMiniGame.MinigameType), SCREEN_DISPLAY_TIME);

        yield return new WaitForSeconds(SCREEN_DISPLAY_TIME);

        activeMiniGame.StartMiniGame();
    }

    public void EndCurrentGame()
    {
        if (activeMiniGame == null) return;

        activeMiniGame.EndMiniGame(); 
        EventManager.Instance.ClearEvents();

        StartCoroutine(EndRoundCoroutine());
    }

    private IEnumerator EndRoundCoroutine()
    {     
        MiniGameResult result = activeMiniGame.Result;

        screen.SetScreenText("", 0.5f);

        yield return new WaitForSeconds(0.5f);

        switch (result)
        {
            case MiniGameResult.Win:
            {
                screen.SetScreenText("You won!", SCREEN_DISPLAY_TIME);

                yield return new WaitForSeconds(SCREEN_DISPLAY_TIME);

                activeMiniGame.Clear();

                CurrentRound ++;

                StartNewRound();

                break;
            }
            case MiniGameResult.Lose:
            case MiniGameResult.Undecided:
            {
                screen.SetScreenText("You lost...", SCREEN_DISPLAY_TIME);

                yield return new WaitForSeconds(SCREEN_DISPLAY_TIME);

                activeMiniGame.Clear();
                Destroy(activeMiniGame);
                activeMiniGame = null;

                CurrentRound = 1;

                screen.ShowTitle(true);

                break;
            }
        }
    }

    public void PlayMusic(string name)
    {
        AudioClip myClip = Resources.Load<AudioClip>(name);
        if (myClip != null)
        {
            audioSource.clip = myClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Audio clip not found: " + name);
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
