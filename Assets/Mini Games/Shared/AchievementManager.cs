using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] protected string gameName = "Location Based Game";
    [SerializeField] protected Achievement[] achievements;
    [SerializeField] private float achievUpdateFreq = 1f;
    [SerializeField] private GameObject achievementPopUp;

    protected Dictionary<string, int> observableInts = new Dictionary<string, int>();
    protected Dictionary<string, float> observableFloats = new Dictionary<string, float>();
    protected Dictionary<string, bool> observableBools = new Dictionary<string, bool>();

    private float updateCounter;
    private Dictionary<string, bool> isAchieved;

    protected virtual void Start()
    {
        // initializing variables
        updateCounter = 0;
        InitializeObservables();
        // initializing achievements information
        Dictionary<string, (TrophyType trophyType, string description, int reward)> info =
            new Dictionary<string, (TrophyType trophyType, string description, int reward)>();
        foreach (Achievement achievement in achievements)
            info[achievement.achievementName] = (achievement.trophyType, achievement.description, achievement.reward);
        GameManager.INSTANCE.profile.SetAchievementsInfo(gameName, info);
        // initializing wether trophies were already earned
        isAchieved = GameManager.INSTANCE.profile.GetAchieved(gameName);
        if(isAchieved == null) {
            isAchieved = new Dictionary<string, bool>();
            foreach (Achievement achievement in achievements)
                isAchieved[achievement.achievementName] = false;
            GameManager.INSTANCE.profile.SetAchievements(gameName, isAchieved);
        }
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        // updating depending on frequency
        if (updateCounter < achievUpdateFreq)
        {
            updateCounter += Time.deltaTime;
            return;
        }
        updateCounter = 0;
        // go through each achievement...
        foreach (Achievement achievement in achievements)
        {
            if (!isAchieved.ContainsKey(achievement.achievementName))
            {
                isAchieved[achievement.achievementName] = false;
                GameManager.INSTANCE.profile.AddAchievement(gameName, achievement.achievementName);
            }
            // ... if trophy already earned move on...
            if (isAchieved[achievement.achievementName]) continue;
            // ... else: check achievement conditions and...
            bool achieved = true;
            foreach (Condition condition in achievement.conditions)
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
                                Debug.Log($"condition ({condition.variableName}) of achievement ({achievement.achievementName}) is inconsistent.");
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
                                Debug.Log($"condition ({condition.variableName}) of achievement ({achievement.achievementName}) is inconsistent.");
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
                                Debug.Log($"condition ({condition.variableName}) of achievement ({achievement.achievementName}) is inconsistent.");
                                achieved = false;
                                break;
                        }
                        break;
                }
                if (!achieved) break;// early break
            }// ... if all condition are met -> pop up and achievement & save that a trophy was earned.
            isAchieved[achievement.achievementName] = achieved;
            if (achieved) AchievementPopUp(achievement);
        }
    }

    public virtual void InitializeObservables()
    {

    }

    public Achievement[] GetAllAchievements()
    {
        return achievements;
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

    private void AchievementPopUp(Achievement achievement)
    {
        Debug.Log($"Trophy: {achievement.achievementName} earned.");
        GameManager.INSTANCE.profile.SetAchieved(gameName, achievement.achievementName);
        //TODO
        // also create a game manager prefab.
    }
}
