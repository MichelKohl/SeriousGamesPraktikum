using System;
using UnityEngine;

[Serializable]
public class Situation : ScriptableObject
{
    [Multiline]
    public string description;
}

[Serializable]
public class DecisionInfo
{
    public string description;
    public int nextSituationID;
}

[Serializable]
public class NextPoint : DecisionInfo
{
    public double conditionDistance;
}