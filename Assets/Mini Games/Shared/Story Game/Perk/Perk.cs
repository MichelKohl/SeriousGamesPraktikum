using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Perk : ScriptableObject
{
    [Multiline]
    public string descritption;
    public int levelRequirement = 0;
    public int skillPointCost = 0;
    public bool permanent = false;//    true    => do not remove perk from players perk list after battle. 
    public bool temporary = false;//    true    => remove perk from list after some time has passed (->duration). 
    public int duration = 0;
    public ParticleSystem buffParticles;
    public int particleSpawnPositionID = 0;
    public float particleSpawnDelay = 0;
    
    public static (Scaling[] scalings, float damageMultiplier, float critMultiplier, float statusProbability, List<Status> statuses)
        ApplyAttackPerks(List<Perk> perks, PlayerAttack playerAttack)
    {
        Scaling[] newScalings = new Scaling[] { playerAttack.STR, playerAttack.DEX, playerAttack.INT, playerAttack.FTH, playerAttack.LCK};
        float damageMultiplier = 1f;
        float critMultiplier = playerAttack.critMultiplier;
        float statusProbability = playerAttack.statusProbability;
        List<Status> statuses = new List<Status>();
        int[] addedScalings = { 0, 0, 0, 0, 0 };

        foreach (AttackPerk perk in perks.FindAll(p => p is AttackPerk))
        {
            addedScalings[0] += perk.changeSTR;
            addedScalings[1] += perk.changeDEX;
            addedScalings[2] += perk.changeINT;
            addedScalings[3] += perk.changeFTH;
            addedScalings[4] += perk.changeLCK;
            damageMultiplier += perk.damageMultiplier;
            critMultiplier += perk.critMultiplier;
            statusProbability += perk.statusProbability;
            foreach (Status s in perk.statuses) statuses.Add(s);
        }
        for (int i = 0; i < addedScalings.Length; i++)
            if (addedScalings[i] < 0)
                for (int timesDecreased = 0; timesDecreased < Mathf.Abs(addedScalings[i]); timesDecreased++)
                    newScalings[i] = ImpairScaling(newScalings[i]);
            else
                for (int timesIncreased = 0; timesIncreased < addedScalings[i]; timesIncreased++)
                    newScalings[i] = ImproveScaling(newScalings[i]);

        return (newScalings, damageMultiplier, critMultiplier, statusProbability, statuses);
    }

    public static void AddAttackPerk(Dictionary<PlayerAttack, (Scaling[] scalings, float damageMultiplier,
        float critMultiplier, float statusProbability, List<Status> statuses)> playerAttackToInfo, AttackPerk perk)
    {
        foreach (PlayerAttack key in playerAttackToInfo.Keys)
        {
            (Scaling[] scalings, float damageMultiplier, float critMultiplier,
                float statusProbability, List<Status> statuses) = playerAttackToInfo[key];
            int[] addedScalings = { perk.changeSTR, perk.changeDEX, perk.changeINT, perk.changeFTH, perk.changeLCK };
            for (int i = 0; i < scalings.Length; i++)
                if (addedScalings[i] < 0)
                    for (int timesDecreased = 0; timesDecreased < Mathf.Abs(addedScalings[i]); timesDecreased++)
                        scalings[i] = ImpairScaling(scalings[i]);
                else
                    for (int timesIncreased = 0; timesIncreased < addedScalings[i]; timesIncreased++)
                        scalings[i] = ImproveScaling(scalings[i]);
            foreach (Status s in perk.statuses) statuses.Add(s);
            playerAttackToInfo[key] = (scalings, damageMultiplier + perk.damageMultiplier, critMultiplier + perk.critMultiplier,
                statusProbability + perk.statusProbability, statuses);
        }
    }

    public static (float damageMultiplier, float statusProbability, List<Status> statuses)
        ApplyAttackPerks(List<Perk> perks, EnemyAttack enemyAttack)
    {
        float damageMultiplier = 1f;
        float statusProbability = enemyAttack.statusProbability; 
        List<Status> statuses = new List<Status>();

        foreach(AttackPerk perk in perks.FindAll(p => p is AttackPerk))
        {
            damageMultiplier += perk.damageMultiplier;
            statusProbability += perk.statusProbability;
            foreach (Status s in perk.statuses) statuses.Add(s);
        }

        return (damageMultiplier, statusProbability, statuses);
    }

    public static void AddAttackPerk(Dictionary<EnemyAttack, (float damageMultiplier,
        float critMultiplier, float statusProbability, List<Status> statuses)> enemyAttackToInfo, AttackPerk perk)
    {
        foreach(EnemyAttack key in enemyAttackToInfo.Keys)
        {
            (float damageMultiplier, float critMultiplier, float statusProbability,
                List<Status> statuses) = enemyAttackToInfo[key];
            foreach (Status s in perk.statuses) statuses.Add(s);
            enemyAttackToInfo[key] = (damageMultiplier + perk.damageMultiplier,
                critMultiplier + perk.critMultiplier, statusProbability + perk.statusProbability, statuses);
        }
    }

    private static Scaling ImproveScaling(Scaling scaling)
    {
        switch (scaling)
        {
            case Scaling.E:
                return Scaling.D;
            case Scaling.D:
                return Scaling.C;
            case Scaling.C:
                return Scaling.B;
            case Scaling.B:
                return Scaling.A;
            default:
                return Scaling.S;
        }
    }

    private static Scaling ImpairScaling(Scaling scaling)
    {
        switch (scaling)
        {
            case Scaling.C:
                return Scaling.D;
            case Scaling.B:
                return Scaling.C;
            case Scaling.A:
                return Scaling.B;
            case Scaling.S:
                return Scaling.A;
            default:
                return Scaling.E;
        }
    }
}


