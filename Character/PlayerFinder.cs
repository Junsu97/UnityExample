using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFinder : MonoBehaviour
{
    private CircleCollider2D _circleCollider;

    public List<InGameCharacterMover> targets = new List<InGameCharacterMover>();

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    public void SetKillRange(float range)
    {
        _circleCollider.radius = range;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var player = col.GetComponent<InGameCharacterMover>();
        if (player && player.playerType == EPlayerType.Crew)
        {
            if (!targets.Contains(player))
            {
                targets.Add(player);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<InGameCharacterMover>();
        if (player && player.playerType == EPlayerType.Crew)
        {
            if (targets.Contains(player))
            {
                targets.Remove(player);
            }
        }
    }

    public InGameCharacterMover GetFirstTarget()// 가장 가까이있는 타겟을 반환하는 코드
    {
        float dis = float.MaxValue;
        InGameCharacterMover closeTarget = null;
        foreach (var target in targets)
        {
            float newDis = Vector3.Distance(transform.position, target.transform.position);
            if (newDis < dis)
            {
                dis = newDis;
                closeTarget = target;
            }
        }

        targets.Remove(closeTarget);
        return closeTarget;
    }
}
