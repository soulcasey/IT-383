using System.Collections.Generic;
using UnityEngine;

public enum MinigameType
{
    ShootAnimal,
    // FastTargetShooting,
    MemoryShoot,
    QuickDraw
}

public enum MiniGameResult
{
    Undecided,
    Win,
    Lose
}

public abstract class Minigame : MonoBehaviour
{
    public abstract MinigameType MinigameType { get; }
    public MiniGameResult Result { get; protected set; } = MiniGameResult.Undecided;
    public List<Target> targets = new List<Target>();

    public abstract void StartMiniGame();
    public abstract void EndMiniGame();

    public void Clear()
    {
        foreach (var target in targets)
        {
            Destroy(target.gameObject);
        }
    }
}