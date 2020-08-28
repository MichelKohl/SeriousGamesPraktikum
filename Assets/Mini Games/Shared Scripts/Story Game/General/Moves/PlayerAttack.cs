using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerMove
{
    public float healthDamage = 0f;
    public float staminaDamage = 0f;
    public float manaDamage = 0f;
    public float critMultiplier = 1.2f;
    public Status status = Status.None;
    public int timesStatusApplied = 0;
    public float statusProbability = 0f;
    public bool multipleHits = false;

    public (float healthDamage, float staminaDamage, float manaDamage, List<Status> statuses) GetAttackInfo(int STR, int DEX, int INT, int FTH, int LCK, List<Perk> perks)
    {
        (Scaling[] scalings, float damageMultiplier, float critMultiplier,
            float statusProbability, List<Status> statuses) = Perk.ApplyAttackPerks(perks, this);
        Scaling newSTR = scalings[0];
        Scaling newDEX = scalings[1];
        Scaling newINT = scalings[2];
        Scaling newFTH = scalings[3];
        Scaling newLCK = scalings[4];

        float luck = (float)newLCK * LCK / (BattleManager.maxSkillLevel / (float)Scaling.C * (float)Scaling.D);
        float scaling = Random.Range(0.9f, 1.1f) + ((float)newSTR * STR + (float)newDEX * DEX +
            (float)newINT * INT + (float)newFTH * FTH + (float)newLCK * LCK) /
            (BattleManager.maxSkillLevel * (float) Scaling.C * 4);
        
        if(status != Status.None)
        {
            statusProbability += luck;
            for (int i = 0; i < timesStatusApplied; i++)
                if (statusProbability > Random.Range(0f, 1f))
                    statuses.Add(status);
        }
        scaling *= damageMultiplier;
        scaling *= luck > Random.Range(0, 1f) ? critMultiplier : 1f;
        return (healthDamage * scaling, staminaDamage * scaling, manaDamage * scaling, statuses);
    }
}
