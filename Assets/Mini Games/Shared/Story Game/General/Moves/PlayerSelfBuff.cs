using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Player/Self Buff")]
[Serializable]
public class PlayerSelfBuff : PlayerMove
{
    public Perk buff;

    public int EnhanceDuration(DCPlayer player, int duration)
    {
        (string _, int strength) = player.GetStat(Stat.STR);
        (string _, int dexterity) = player.GetStat(Stat.DEX);
        (string _, int intelligence) = player.GetStat(Stat.INT);
        (string _, int faith) = player.GetStat(Stat.FTH);
        (string _, int luck) = player.GetStat(Stat.LCK);

        return Mathf.RoundToInt(((float) STR * (float) DEX * (float) INT * (float) FTH * (float) LCK
            * strength * dexterity * intelligence * faith * luck) / Mathf.Pow(BattleManager.maxSkillLevel * (float) Scaling.S, 2) *
            duration);
    }
}


