using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GameManager : SingletonBase<GameManager>
{
    public Minigame ActiveMinigame { get; private set; }
    public GunType ActiveGunType { get; private set; } = GunType.None;
    
    public AudioSource audioSource;

    // Screen
    public Screen screen;
    public GameObject screenCenterPosition;
    public GameObject screenSidePosition;
    private const string NEW_ROUND_ANNOUNCEMENT_TEMPLATE = "Minigame: {0}\nGun: {1}";
    private const float SCREEN_MOVE_DELAY_TIME = 4f;
    private const float START_DELAY_TIME = 0.5f;

    private void Start()
    {
        screen.startButton.onClick.AddListener(() => StartCoroutine(StartNewRound()));
    }


    private IEnumerator StartNewRound()
    {
        MinigameType randomMinigame = Logic.GetRandomEnum<MinigameType>();
        ActiveMinigame = (Minigame)gameObject.AddComponent(Type.GetType(randomMinigame.ToString()));

        // Launcher is a special gun. Do not randomly select it.
        ActiveGunType = Logic.GetRandomEnum(GunType.Launcher);
    
        screen.SetScreenText(string.Format(NEW_ROUND_ANNOUNCEMENT_TEMPLATE, ActiveMinigame.MinigameType, ActiveGunType), SCREEN_MOVE_DELAY_TIME);

        yield return new WaitForSeconds(SCREEN_MOVE_DELAY_TIME);

        bool isMoveComplete = false;

        screen.Move(screenSidePosition.transform.position, () => isMoveComplete = true);

        yield return new WaitUntil(() => isMoveComplete == true);

        yield return new WaitForSeconds(START_DELAY_TIME);

        Gun.Create(ActiveGunType);

        ActiveMinigame.StartMiniGame();
    }

    public void EndCurrentGame()
    {
        if (ActiveMinigame == null) return;

        MiniGameResult result = ActiveMinigame.Result;

        ActiveMinigame.EndMiniGame();
        Destroy(ActiveMinigame);
        ActiveMinigame = null;
        ActiveGunType = GunType.None;

        switch (result)
        {
            case MiniGameResult.Win:
            {
                screen.SetScreenText("You won!", 4);
                break;
            }
            case MiniGameResult.Lose:
            case MiniGameResult.Undecided:
            {
                screen.SetScreenText("You lost...", 4);
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
