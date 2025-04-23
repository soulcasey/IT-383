using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// TODO: Set spawn boundary
public class Tutorial : MonoBehaviour
{
    public List<Target> targets = new List<Target>();
    public List<Gun> guns = new List<Gun>();

    public void StartTutorial()
    {   
        GameManager.Instance.screen.ShowTitle(false);

        foreach (TargetType targetType in Enum.GetValues(typeof(TargetType)))
        { 
                Target target = Target.Create(targetType);
                target.canHit = false;
                targets.Add(target);
                target.status = TargetStatus.Walk;
            }

        StartCoroutine(DisplayTutorial());
    }

    private IEnumerator DisplayTutorial()
    {
        GameManager.Instance.screen.SetScreenText("Grab the dual pistol with each hand.", 999f);
        List<Gun> pistols = Gun.Create(GunType.Pistol);
        guns.AddRange(pistols);
        yield return new WaitUntil(() => pistols.All(pistol => pistol.GrabCount == 1));
        GameManager.Instance.screen.SetScreenText("Shoot a target to eliminate it.", 999f);
        foreach (Target target in targets)
        {
            target.canHit = true;
        }
        yield return new WaitUntil(() => targets.Count(target => target.status == TargetStatus.Death) == 1);
        foreach (Target target in targets)
        {
            target.canHit = false;
        }

        GameManager.Instance.screen.SetScreenText("Grab the sniper handle with right hand, then the handguard with left hand.", 999f);
        Gun sniper = Gun.Create(GunType.Sniper).First();
        guns.Add(sniper);
        yield return new WaitUntil(() => sniper.GrabCount == 2);
        GameManager.Instance.screen.SetScreenText("Shoot a target to eliminate it.", 999f);
        foreach (Target target in targets)
        {
            target.canHit = true;
        }
        yield return new WaitUntil(() => targets.Count(target => target.status == TargetStatus.Death) == 2);
        foreach (Target target in targets)
        {
            target.canHit = false;
        }

        GameManager.Instance.screen.SetScreenText("Grab the rifle handle with right hand, then the handguard with left hand.", 999f);
        Gun rifle = Gun.Create(GunType.Rifle).First();
        guns.Add(rifle);
        yield return new WaitUntil(() => rifle.GrabCount == 2);
        GameManager.Instance.screen.SetScreenText("Shoot a target to eliminate it.", 999f);
        foreach (Target target in targets)
        {
            target.canHit = true;
        }
        yield return new WaitUntil(() => targets.Count(target => target.status == TargetStatus.Death) == 3);

        GameManager.Instance.screen.SetScreenText("Now eliminate all animals. Grab any gun.", 999f);
        yield return new WaitUntil(() => targets.All(target => target.status == TargetStatus.Death));

        EndTutorial();
    }


    public void EndTutorial()
    {
        EventManager.Instance.ClearEvents();

        foreach (var target in targets)
        {
            Destroy(target.gameObject);
        }
        targets.Clear();

        foreach (Gun gun in guns)
        {
            Destroy(gun.gameObject);
        }
        guns.Clear();

        StartCoroutine(EndTutorialCoroutine());
    }

    private IEnumerator EndTutorialCoroutine()
    {
        GameManager.Instance.screen.SetScreenText("Tutorial Complete!", 2.5f);
        yield return new WaitForSeconds(3);

        GameManager.Instance.screen.ShowTitle(true);
    }
}