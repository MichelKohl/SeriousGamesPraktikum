﻿using UnityEngine;
using System;


[CreateAssetMenu(menuName ="Player/Starter Class")]
[Serializable]
public class StarterClass : ScriptableObject
{
    public int level = 1;
    public DCPlayer model;
    // character stats
    public int strength = 1;    // -> one and two handed weapons & stamina regen
    public int dexterity = 1;   // -> one handed weapons (especially daggers) & initiative
    public int intelligence = 1;// -> effectiveness of magic spells & mana regen
    public int faith = 1;       // -> effectiveness of faith spells & mana regen
    public int luck = 1;        // -> chance of critical hits & effects (stun, bleed, poison etc.) and chance of evading an attack

    // all attacks
    public bool[] unlocked;

    public DCPlayer Init(Transform parent, Vector3 position, Quaternion rotation)
    {
        return Instantiate(model, position, rotation, parent).Init(this);
    }
}