using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class Item : ScriptableObject
{
    public string itemName;
    public Image icon;
    [Multiline]
    public string description; 
    protected Scaling strengthScaling;
    protected Scaling dexterityScaling;
    protected Scaling intelligenceScaling;
    protected Scaling faithScaling;
    protected Scaling luckScaling;

    public bool IsCritical(int luck, float critModifier = 1f)
    {
        return UnityEngine.Random.Range(0, 1f) <= luck * (int)luckScaling * 0.1f * critModifier;
    }

    protected bool IsAccurate(int accuracy)
    {
        return UnityEngine.Random.Range(0, 1f) <= accuracy;
    }
}