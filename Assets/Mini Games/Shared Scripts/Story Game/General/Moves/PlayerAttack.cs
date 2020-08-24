using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerMove
{
    public float healthDamage = 0f;
    public float staminaDamage = 0f;
    public float manaDamage = 0f;
    public Status status = Status.None;
    public int timesStatusApplied = 0;
    public float statusProbability = 0f;
    public bool multipleHits = false;

    public (float healthDamage, float staminaDamage, float manaDamage, List<Status> statuses) GetAttackInfo(int STR, int DEX, int INT, int FTH, int LCK)
    {
        float luck = ((float)this.LCK * LCK) / (BattleManager.maxSkillLevel * (float)Scaling.C);
        float scaling = Random.Range(0.8f, 1f + luck) + ((float)this.STR * STR + (float)this.DEX * DEX +
            (float)this.INT * INT + (float)this.FTH * FTH + (float)this.LCK * LCK) /
            (BattleManager.maxSkillLevel * (float) Scaling.C * 4);
        
        List<Status> statuses = new List<Status>();
        if(status != Status.None)
        {
            float statusProbability = (this.statusProbability + (float)this.LCK * LCK) / (BattleManager.maxSkillLevel * (float)Scaling.A);
            for (int i = 0; i < timesStatusApplied; i++)
                if (statusProbability > Random.Range(0f, 1f))
                    statuses.Add(status);
        }
        return (healthDamage * scaling, staminaDamage * scaling, manaDamage * scaling, statuses);
    }
}
