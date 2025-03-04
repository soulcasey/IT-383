using UnityEngine;

public enum MinigameType
{
    ShootAnimal,
    FastTargetShooting,
    MemoryShoot,
    QuickDrawDuel
}

public abstract class Minigame : MonoBehaviour
{
    public abstract void Start();
    public abstract void End();
}