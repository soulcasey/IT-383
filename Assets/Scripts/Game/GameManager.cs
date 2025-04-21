using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class GameManager : SingletonBase<GameManager>
{
    public Minigame ActiveMinigame { get; private set; }
    public GunType ActiveGunType { get; private set; } = GunType.None;

    public int CurrentRound { get; private set; } = 1;
    
    public AudioSource audioSource;

    public List<Gun> guns = new List<Gun>();

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
        MinigameType randomMinigame = ActiveMinigame == null ? Logic.GetRandomEnum<MinigameType>() : Logic.GetRandomEnum(ActiveMinigame.MinigameType) ;
        ActiveMinigame = (Minigame)gameObject.AddComponent(Type.GetType(randomMinigame.ToString()));

        // Launcher is a special gun. Do not randomly select it.
        ActiveGunType = Logic.GetRandomEnum(GunType.Launcher, GunType.None);

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

        screen.SetScreenText(string.Format(NEW_ROUND_ANNOUNCEMENT_TEMPLATE, ActiveMinigame.MinigameType), SCREEN_DISPLAY_TIME);

        yield return new WaitForSeconds(SCREEN_DISPLAY_TIME);

        bool isMoveComplete = false;

        screen.Move(true, () => isMoveComplete = true);

        yield return new WaitUntil(() => isMoveComplete == true);

        yield return new WaitForSeconds(START_DELAY_TIME);

        guns = Gun.Create(ActiveGunType);

        ActiveMinigame.StartMiniGame();
    }

    public void EndCurrentGame()
    {
        if (ActiveMinigame == null) return;

        ActiveMinigame.EndMiniGame(); 
        EventManager.Instance.ClearEvents();

        StartCoroutine(EndRoundCoroutine());
    }

    private IEnumerator EndRoundCoroutine()
    {     
        MiniGameResult result = ActiveMinigame.Result;

        yield return new WaitForSeconds(2);

        screen.SetScreenText("", 100);

        bool isMoveComplete = false;

        screen.Move(false, () => isMoveComplete = true);

        yield return new WaitUntil(() => isMoveComplete == true);

        switch (result)
        {
            case MiniGameResult.Win:
            {
                screen.SetScreenText("You won!", SCREEN_DISPLAY_TIME);

                yield return new WaitForSeconds(SCREEN_DISPLAY_TIME);

                foreach (Gun gun in guns)
                {
                    Destroy(gun.gameObject);
                }
                guns.Clear();
                ActiveMinigame.Clear();

                CurrentRound ++;

                StartNewRound();

                break;
            }
            case MiniGameResult.Lose:
            case MiniGameResult.Undecided:
            {
                screen.SetScreenText("You lost...", SCREEN_DISPLAY_TIME);

                yield return new WaitForSeconds(SCREEN_DISPLAY_TIME);

                foreach (Gun gun in guns)
                {
                    Destroy(gun.gameObject);
                }
                guns.Clear();
                ActiveMinigame.Clear();
                Destroy(ActiveMinigame);
                ActiveMinigame = null;
                ActiveGunType = GunType.None;

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
