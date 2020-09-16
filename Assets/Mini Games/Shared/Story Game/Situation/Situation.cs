using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Situation : ScriptableObject
{
    public Vector3 camPosition;
    public Vector3 camRotation;
    public float spotlightRange = 20f;
    public bool flushDeadEnemies = false;
    public bool important = false;
    [Multiline]
    public string description;
    public int[] activateGameObject;
    public int[] deactivateGameObject;
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

    public PlotPoint[] blocks;
    public PlotPoint[] enablers;

    public bool IsBlocked(List<PlotPoint> playerPath)
    {
        try
        {
            foreach (PlotPoint block in blocks)
                if (playerPath.Contains(block))
                    return true;
            foreach (PlotPoint enabler in enablers)
                if (!playerPath.Contains(enabler))
                    return true;
        }
        catch (NullReferenceException) { }
        return false;
    }
}

[Serializable]
public struct PlotPoint
{
    public int chapter;
    public int situation;
}

[Serializable]
public class NextPoint : DecisionInfo
{
    public bool changeChapter = false;
    public int nextChapter;
    public int startSituation;
    public double conditionDistance;
}

[Serializable]
public class DialogueOption : DecisionInfo
{
    public bool startBattle = false;
}