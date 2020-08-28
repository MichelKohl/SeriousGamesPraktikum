using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Perk/Attribute")]
[Serializable]
public class AttributePerk : Perk
{
    public int addSTR = 0;
    public int addDEX = 0;
    public int addINT = 0;
    public int addFTH = 0;
    public int addLCK = 0;
    public float addHealthRegen = 0;
    public float addStaminaRegen = 0;
    public float addManaRegen = 0;
    public float maxHealthMultiplier = 1f;
    public float maxStaminaMultiplier = 1f;
    public float maxManaMultiplier = 1f;
    public float initiativeMultiplier = 1f;
    public float accuracyMultiplier = 1f;
}
