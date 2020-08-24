using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Enemy/Attack")]
[Serializable]
public class EnemyAttack : EnemyMove
{
    public float healthDamage = 0f;
    public float staminaDamage = 0f;
    public float manaDamage = 0f;

    public float hitboxDelay = 0f;
    public int hitboxID = 0;
    public bool multipleHits = false;
    public Status status = Status.None;
    public float statusProbability = 0f;
    public int timesStatusApplied = 0;

    public (float healthDamage, float staminaDamage, float manaDamage, List<Status> statuses) GetAttackInfo(int level)
    {
        float scaling = 1f + level * levelMultiplier * UnityEngine.Random.Range(0.8f, 1f);
        float statusScaling = statusProbability * scaling;
        List<Status> statuses = new List<Status>();
        if(status != Status.None)
            for (int i = 0; i < timesStatusApplied; i++)
                if (statusScaling > UnityEngine.Random.Range(0f, 1f))
                    statuses.Add(status);
        return (healthDamage * scaling, staminaDamage * scaling, manaDamage * scaling, statuses);
    }
}
