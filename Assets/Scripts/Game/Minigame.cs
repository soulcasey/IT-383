using System.Collections.Generic;
using UnityEngine;

public enum MinigameType
{
    ShootAnimal,
    MemoryShoot,
    QuickDraw
}

public enum MiniGameResult
{
    Win,
    Lose
}

public abstract class Minigame : MonoBehaviour
{
    public abstract MinigameType MinigameType { get; }
    public List<Target> targets = new List<Target>();
    public List<Gun> guns = new List<Gun>();

    public virtual void StartMiniGame()
    {
        GunType gunType = Logic.GetRandomEnum(GunType.Launcher, GunType.None);
        guns.AddRange(Gun.Create(gunType));
    }

    public virtual void EndMiniGame()
    {
        GameManager.Instance.StopMusic();
        StopAllCoroutines();

        foreach (var target in targets)
        {
            if (target.status == TargetStatus.Death)
            {
                continue;
            }
            target.canHit = false;
            target.status = GetResult() == MiniGameResult.Lose ? TargetStatus.Mock : TargetStatus.Idle;
        }
    }

    public abstract MiniGameResult GetResult();

    public void Clear()
    {
        foreach (var target in targets)
        {
            Destroy(target.gameObject);
        }

        foreach (Gun gun in guns)
        {
            Destroy(gun.gameObject);
        }
    }
}