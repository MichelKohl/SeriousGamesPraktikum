using System;
using UnityEngine;

[Serializable]
public class Situation : ScriptableObject
{
    public Vector3 camPosition;
    public Vector3 camRotation;
    [Multiline]
    public string description;
}

[Serializable]
public class DecisionInfo
{
    public string description;
    public int nextSituationID;
    public int strRequirement = 0;
    public int dexRequirement = 0;
    public int intRequirement = 0;
    public int fthRequirement = 0;
    public int lckRequirement = 0;
}

[Serializable]
public class NextPoint : DecisionInfo
{
    public double conditionDistance;
}

[Serializable]
public class DialogueOption : DecisionInfo
{
    public bool startBattle = false;
}