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

    public abstract void StartMiniGame();
    public abstract void EndMiniGame();
}