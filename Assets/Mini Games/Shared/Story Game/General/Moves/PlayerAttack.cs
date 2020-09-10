﻿using System.Collections;
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

    public (float healthDamage, float staminaDamage, float manaDamage, List<Status> statuses, float statusProbability, float luck)
        GetAttackInfo(DCPlayer player, bool preview = false)
    {
        (string _, int STR) = player.GetStat(Stat.STR);
        (string _, int DEX) = player.GetStat(Stat.DEX);
        (string _, int INT) = player.GetStat(Stat.INT);
        (string _, int FTH) = player.GetStat(Stat.FTH);
        (string _, int LCK) = player.GetStat(Stat.LCK);

        (Scaling[] scalings, float damageMultiplier, float critMultiplier,
            float statusProbability, List<Status> statuses) = Perk.ApplyAttackPerks(player.GetActivePerks(), this);
        Scaling newSTR = scalings[0];
        Scaling newDEX = scalings[1];
        Scaling newINT = scalings[2];
        Scaling newFTH = scalings[3];
        Scaling newLCK = scalings[4];

        float luck = (float)newLCK * LCK / BattleManager.maxSkillLevel;
        float scaling = (preview ? 1f : Random.Range(0.9f, 1.1f))+ ((float)newSTR * STR + (float)newDEX * DEX +
            (float)newINT * INT + (float)newFTH * FTH + (float)newLCK * LCK) /
            (BattleManager.maxSkillLevel * (float) Scaling.C * 4);
        
        if(status != Status.None)
        {
            statusProbability += luck;
            for (int i = 0; i < timesStatusApplied; i++)
                if (statusProbability > Random.Range(0f, 1f) || preview)
                    statuses.Add(status);
        }
        scaling *= damageMultiplier;
        scaling *= luck > Random.Range(0, 1f) && !preview ? critMultiplier : 1f;
        return (healthDamage * scaling, staminaDamage * scaling, manaDamage * scaling, statuses, statusProbability, luck);
    }

    public (string STR, string DEX, string INT, string FTH, string LCK, float critMultiplier) GetScalingInfo(DCPlayer player)
    {
        (Scaling[] scalings, float _, float critMultiplier, float _, List<Status> _) = Perk.ApplyAttackPerks(player.GetActivePerks(), this);
        return (ScalingToString(scalings[0]), ScalingToString(scalings[1]), ScalingToString(scalings[2]),
            ScalingToString(scalings[3]), ScalingToString(scalings[4]), critMultiplier);
    }
}
