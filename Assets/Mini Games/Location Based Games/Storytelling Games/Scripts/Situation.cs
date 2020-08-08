using System;
using UnityEngine;


[CreateAssetMenu]
[Serializable]
public class Situation : ScriptableObject
{
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;
    [Multiline]
    public string description;
    public DecisionInfo[] decisions;
}

[Serializable]
public struct DecisionInfo
{
    public string description;
    public int nextSituationID;
    public double conditionDistance;
}


