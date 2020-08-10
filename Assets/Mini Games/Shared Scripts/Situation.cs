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

[CreateAssetMenu(menuName = "Situation/Navigation")]
[Serializable]
public class Navigation : Situation
{
    public Vector3 camPosition;
    public Vector3 camRotation;
    public NextPoint[] decisions;
}

[CreateAssetMenu(menuName = "Situation/Battle")]
[Serializable]
public class Battle : Situation
{

}

[CreateAssetMenu(menuName = "Situation/Dialogue")]
[Serializable]
public class Dialogue : Situation
{

}