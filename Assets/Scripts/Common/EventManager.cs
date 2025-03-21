using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class EventManager : SingletonBase<EventManager>
{
    public event Action<TargetType> OnTargetDeath;

    public void TargetDeath(TargetType targetType)
    {
        OnTargetDeath?.Invoke(targetType);
    }

    public void ClearEvents()
    {
        OnTargetDeath = null;
    }
}