using UnityEngine;

public enum MinigameType
{
    ShootAnimal,
    // FastTargetShooting,
    // MemoryShoot,
    QuickDraw
}

public abstract class Minigame : MonoBehaviour
{
    public abstract MinigameType MinigameType { get; }
    public abstract void StartMiniGame();
    public abstract void EndMiniGame();
}