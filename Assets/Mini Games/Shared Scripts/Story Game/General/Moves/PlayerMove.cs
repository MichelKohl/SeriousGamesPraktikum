using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Move
{
    public int levelRequirement = 1;
    public int skillPointCost = 1;
    public Scaling STR = Scaling.E;
    public Scaling DEX = Scaling.E;
    public Scaling INT = Scaling.E;
    public Scaling FTH = Scaling.E;
    public Scaling LCK = Scaling.E;
    public float probOfBlock = 0f;

    public string GetStat(Stat stat)
    {
        switch (stat)
        {
            case Stat.STR:
                return ScalingToString(STR);
            case Stat.DEX:
                return ScalingToString(DEX);
            case Stat.INT:
                return ScalingToString(INT);
            case Stat.FTH:
                return ScalingToString(FTH);
            case Stat.LCK:
                return ScalingToString(LCK);
            default:
                return "";
        }
    }

    public string ScalingToString(Scaling scaling)
    {
        switch (scaling)
        {
            case Scaling.A:
                return "A";
            case Scaling.B:
                return "B";
            case Scaling.C:
                return "C";
            case Scaling.D:
                return "D";
            case Scaling.E:
                return "E";
            default:
                return "S";
        }
    }
}
