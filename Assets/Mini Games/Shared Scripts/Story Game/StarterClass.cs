using UnityEngine;
using System;


[CreateAssetMenu]
[Serializable]
public class StarterClass : ScriptableObject
{
    // class name and starter level
    public string className;
    public int level;
    // character stats
    public int strength;    // -> one and two handed weapons & stamina regen
    public int dexterity;   // -> one handed weapons (especially daggers) & initiative
    public int intelligence;// -> effectiveness of magic spells & mana regen
    public int faith;       // -> effectiveness of faith spells & mana regen
    public int luck;        // -> chance of critical hits & effects (stun, bleed, poison etc.) and chance of evading an attack
    //starter equipment
    public Weapon[] weapons;        // at most two
    public Armor[] equipment;       // at most five
    public Consumable[] consumables;// no limited amounts 
}
