using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GameManager : SingletonBase<GameManager>
{
    public Minigame ActiveMinigame { get; private set; }
    public GunType ActiveGunType { get; private set; }

    public Button startButton;
    
    // Textscreen
    public TextMeshProUGUI screenText;
    private Coroutine screenTextCoroutine;
    private const string NEW_ROUND_ANNOUNCEMENT_TEMPLATE = "Minigame: {0}!\nGun: {1}";

    private void Start()
    {
        startButton.onClick.AddListener(() => StartNewRound());
    }


    public void StartNewRound()
    {
        MinigameType randomMinigame = (MinigameType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(MinigameType)).Length);
        ActiveMinigame = (Minigame)gameObject.AddComponent(Type.GetType(randomMinigame.ToString()));

        GunType[] guns = Enum.GetValues(typeof(GunType)).Cast<GunType>().Where(gun => gun != GunType.Launcher).ToArray();
        ActiveGunType = guns[UnityEngine.Random.Range(0, guns.Length)];
    
        SetScreenText(string.Format(NEW_ROUND_ANNOUNCEMENT_TEMPLATE, ActiveMinigame, ActiveGunType), 4);
    }

    public void EndCurrentGame()
    {
        if (ActiveMinigame == null) return;

        Destroy(ActiveMinigame);
        ActiveMinigame = null;
    }

    public void SetScreenText(string text, int time)
    {
        if (screenTextCoroutine != null)
        {
            StopCoroutine(screenTextCoroutine);
        }

        screenTextCoroutine = StartCoroutine(ScreenTextCoroutine(text, time));
    }

    private IEnumerator ScreenTextCoroutine(string text, int time)
    {
        screenText.text = text;
        yield return new WaitForSeconds(time);
        screenText.text = string.Empty;
    }
}
