﻿using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Achievement : ScriptableObject
{
    public string achievementName = "Achievement";
    public Sprite icon;
    [Multiline]
    public string description = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, \n" +
        "sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat,\n" +
        " sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.";
    public int reward = 0;
    public Condition[] conditions;


    public void Achieved()
    {
        Debug.Log($"trophy {achievementName} earned");
        //TODO
        // mark achievement as achieved in profile file
        // reward the play with coins
    }

    [Serializable]
    public struct Condition
    {
        public string variableName;
        public Conditional condition;
        public string value;
        public VarType varType;
    }

    public enum Conditional
    {
        IsLessThan,
        IsEqual,
        IsBiggerThan,
        Is
    }

    public enum VarType
    {
        Boolean,
        Float,
        Integer
    }
}
