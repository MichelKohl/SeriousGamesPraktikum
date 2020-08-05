using System;
using UnityEngine;


[CreateAssetMenu]
[Serializable]
public class Situation : ScriptableObject
{
    public int id;
    public string description;
    public DecisionInfo[] decisions;
}

[Serializable]
public struct DecisionInfo
{
    public string description;
    public int nextSituationID;
    public DecisionCondition condition;
}


