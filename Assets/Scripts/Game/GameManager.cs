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
        MinigameType randomMinigame = (MinigameType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(MinigameType)).Length);
        ActiveMinigame = (Minigame)gameObject.AddComponent(Type.GetType(randomMinigame.ToString()));

        GunType[] guns = Enum.GetValues(typeof(GunType)).Cast<GunType>().Where(gun => gun != GunType.Launcher).ToArray();
        ActiveGunType = guns[UnityEngine.Random.Range(0, guns.Length)];
    
        screen.SetScreenText(string.Format(NEW_ROUND_ANNOUNCEMENT_TEMPLATE, ActiveMinigame.MinigameType, ActiveGunType), SCREEN_MOVE_DELAY_TIME);

        yield return new WaitForSeconds(SCREEN_MOVE_DELAY_TIME);

        bool isMoveComplete = false;

        screen.Move(screenSidePosition.transform.position, () => isMoveComplete = true);

        yield return new WaitUntil(() => isMoveComplete == true);

        yield return new WaitForSeconds(START_DELAY_TIME);

        ActiveMinigame.StartMiniGame();
    }

    public void EndCurrentGame()
    {
        if (ActiveMinigame == null) return;

        Destroy(ActiveMinigame);
        ActiveMinigame = null;
    }
}
