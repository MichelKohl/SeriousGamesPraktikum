using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Perk/Attack")]
[Serializable]
public class AttackPerk : Perk
{
    public Status[] statuses;
    public float statusProbability = 0;
    public float damageMultiplier = 0;
    public float enemyDamageMultiplier = 0;
    public float critMultiplier = 0;

    public int changeSTR = 0;
    public int changeDEX = 0;
    public int changeINT = 0;
    public int changeFTH = 0;
    public int changeLCK = 0;
}

