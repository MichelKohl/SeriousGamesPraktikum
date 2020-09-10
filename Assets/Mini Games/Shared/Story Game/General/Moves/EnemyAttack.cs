using System.Collections.Generic;

public class EnemyAttack : EnemyMove
{
    public float healthDamage = 0f;
    public float staminaDamage = 0f;
    public float manaDamage = 0f;

    public Status status = Status.None;
    public float statusProbability = 0f;
    public int timesStatusApplied = 0;

    public bool multipleHits = false;

    public (float healthDamage, float staminaDamage, float manaDamage, List<Status> statuses, float, float ) GetAttackInfo(int level, List<Perk> perks)
    {
        (float damageMultiplier, float statusProbability,
            List<Status> statuses) = Perk.ApplyAttackPerks(perks, this);

        float scaling = 1f + level * levelMultiplier * UnityEngine.Random.Range(0.9f, 1.1f);
        float statusScaling = statusProbability * scaling;

        if(status != Status.None)
            for (int i = 0; i < timesStatusApplied; i++)
                if (statusScaling > UnityEngine.Random.Range(0f, 1f))
                    statuses.Add(status);
        scaling *= damageMultiplier;
        return (healthDamage * scaling, staminaDamage * scaling, manaDamage * scaling, statuses, 0f, 0f);
    }
}
