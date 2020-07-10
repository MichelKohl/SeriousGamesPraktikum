using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Achievement;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] protected string gameName = "Location Based Game";
    [SerializeField] protected Achievement[] achievements;
    [SerializeField] private float achievUpdateFreq = 1f;

    protected Dictionary<string, int> observableInts = new Dictionary<string, int>();
    protected Dictionary<string, float> observableFloats = new Dictionary<string, float>();
    protected Dictionary<string, bool> observableBools = new Dictionary<string, bool>();
    protected GameManager gameManager;

    private float updateCounter;
    private bool[] achieved;

    protected virtual void Start()
    {
        updateCounter = 0;

        try
        {
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            achieved = gameManager.profile.GetAchievements(gameName).Achieved;
        }
        catch (Exception) { }
        if(achievements == null)
            achieved = Enumerable.Repeat(false, achievements.Length).ToArray();

        InitializeObservables();
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        if (updateCounter < achievUpdateFreq)
        {
            updateCounter += Time.deltaTime;
            return;
        }
        updateCounter = 0;
        
        for (int i = 0; i < achievements.Length; i++)
        {
            if (this.achieved[i]) continue;
            bool achieved = true;
            foreach (Condition condition in achievements[i].conditions)
            {
                switch (condition.varType)
                {
                    case VarType.Integer:
                        if (!observableInts.ContainsKey(condition.variableName))
                            Debug.Log($"{condition.variableName} is not an observable integer variable.");
                        switch (condition.condition)
                        {
                            case Conditional.IsLessThan:
                                achieved = observableInts[condition.variableName] < int.Parse(condition.value);
                                break;
                            case Conditional.IsBiggerThan:
                                achieved = observableInts[condition.variableName] > int.Parse(condition.value);
                                break;
                            case Conditional.IsEqual:
                                achieved = observableInts[condition.variableName] == int.Parse(condition.value);
                                break;
                            default:
                                Debug.Log($"condition ({condition.variableName}) of achievement ({achievements[i].achievementName}) is inconsistent.");
                                achieved = false;
                                break;
                        }
                        break;
                    case VarType.Float:
                        if (!observableFloats.ContainsKey(condition.variableName))
                            Debug.Log($"{condition.variableName} is not an observable float variable.");
                        switch (condition.condition)
                        {
                            case Conditional.IsLessThan:
                                achieved = observableFloats[condition.variableName] < float.Parse(condition.value);
                                break;
                            case Conditional.IsBiggerThan:
                                achieved = observableFloats[condition.variableName] > float.Parse(condition.value);
                                break;
                            case Conditional.IsEqual:
                                achieved = observableFloats[condition.variableName] == float.Parse(condition.value);
                                break;
                            default:
                                Debug.Log($"condition ({condition.variableName}) of achievement ({achievements[i].achievementName}) is inconsistent.");
                                achieved = false;
                                break;
                        }
                        break;
                    case VarType.Boolean:
                        if (!observableBools.ContainsKey(condition.variableName))
                            Debug.Log($"{condition.variableName} is not an observable boolean variable.");
                        switch (condition.condition)
                        {
                            case Conditional.Is:
                                achieved = observableBools[condition.variableName] == bool.Parse(condition.value);
                                break;
                            default:
                                Debug.Log($"condition ({condition.variableName}) of achievement ({achievements[i].achievementName}) is inconsistent.");
                                achieved = false;
                                break;
                        }
                        break;
                }
                if (!achieved) break;
            }
            this.achieved[i] = achieved;
            if (achieved)
            {
                if (gameManager != null)
                    gameManager.profile.SetAchieved(gameName, i);
                AchievementPopUp();
            }
        }
    }

    public virtual void InitializeObservables()
    {

    }

    public List<string> GetAllObservablesNames()
    {
        List<string> list = new List<string>();
        foreach (string key in observableInts.Keys) list.Add($"{key}|integer");
        foreach (string key in observableFloats.Keys) list.Add($"{key}|float");
        foreach (string key in observableBools.Keys) list.Add($"{key}|boolean");
        return list;
    }

    protected int ObservableInt(string name)
    {
        return observableInts[name];
    }

    protected void ObservableInt(string name, int value)
    {
        observableInts[name] = value;
    }

    protected float ObservableFloat(string name)
    {
        return observableFloats[name];
    }

    protected void ObservableFloat(string name, float value)
    {
        observableFloats[name] = value;
    }

    protected bool ObservableBool(string name)
    {
        return observableBools[name];
    }

    protected void ObservableBool(string name, bool value)
    {
        observableBools[name] = value;
    }

    private void AchievementPopUp()
    {
        //TODO
        // also create a game manager prefab.
    }
}
