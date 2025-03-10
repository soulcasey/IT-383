using System;
using System.Collections;
using UnityEngine;

public enum TargetType
{
    Dog,
    Cat,
    Bird
}

public class ShootAnimal : Minigame
{
    public override MinigameType MinigameType => MinigameType.ShootAnimal;
    public TargetType CurrentTargetType { get; private set; }

    private const string ANNOUNCEMENT_TEMPLATE = "Target the {0}!";
    
    public override void Start()
    {
        SetRandomTargetType();
    }

    private void SetRandomTargetType()
    {
        CurrentTargetType = (TargetType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TargetType)).Length);
        GameManager.Instance.SetScreenText(string.Format(ANNOUNCEMENT_TEMPLATE, CurrentTargetType), 5);
    }

    public override void End()
    {
        throw new NotImplementedException();
    }
}