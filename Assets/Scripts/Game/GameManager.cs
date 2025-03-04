using System;
using System.Collections;
using UnityEngine;
using System.Linq;

public class GameManager : SingletonBase<GameManager>
{
    private Minigame activeMinigame;
    private GunType activeGunType;

    private void Start()
    {
        
    }


    public void StartNewRound(MinigameType minigameType, GunType gunType)
    {
        activeMinigame = (Minigame)gameObject.AddComponent(Type.GetType(minigameType.ToString()));
        activeGunType = gunType;
    }


    public (MinigameType minigameType, GunType gunType) GetRandomGameData()
    {
        MinigameType randomMinigame = (MinigameType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(MinigameType)).Length);

        GunType[] guns = Enum.GetValues(typeof(GunType)).Cast<GunType>().Where(gun => gun != GunType.Launcher).ToArray();
        GunType randomGun = guns[UnityEngine.Random.Range(0, guns.Length)];

        return (randomMinigame, randomGun);
    }

    public void EndCurrentGame()
    {
        if (activeMinigame == null) return;

        Destroy(activeMinigame);
        activeMinigame = null;
    }
}
