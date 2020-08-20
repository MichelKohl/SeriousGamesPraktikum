using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Attack/Player Attack")]
[Serializable]
public class PlayerAttack : Move
{
    public int levelRequirement = 1;
    public Scaling STR = Scaling.F;
    public Scaling DEX = Scaling.F;
    public Scaling INT = Scaling.F;
    public Scaling FTH = Scaling.F;
    public Scaling LCK = Scaling.F;
}
