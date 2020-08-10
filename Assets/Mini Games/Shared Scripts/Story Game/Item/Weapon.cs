using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName ="Item/Weapon")]
[Serializable]
public class Weapon : Item
{
    public GameObject prefab;
    private int damage = 0;
    private float criticalModifier = 1f;
    private bool bleed = false;
    private bool poison = false;
    private bool stun = false;
    private bool healPoison = false;

    public float GetDamage(int strength, int dexterity, int intelligence, int faith, int luck, int accuracy)
    {
        int scalingFactor = 0;
        scalingFactor += strength *     (int) strengthScaling;
        scalingFactor += dexterity *    (int) dexterityScaling;
        scalingFactor += intelligence * (int) intelligenceScaling;
        scalingFactor += faith *        (int) faithScaling;
        scalingFactor += luck *         (int) luckScaling;
        bool isCritical = IsCritical(luck);
        return IsAccurate(accuracy) && !isCritical ? (float) damage * scalingFactor * (isCritical ? criticalModifier : 1f) : 0;
    }

    public bool HasStatus()
    {
        return bleed || poison || stun || healPoison;
    }

    public void ApplyStatus(Fighter target, int luck, bool bleedOverride = false, bool poisonOverride = false, bool stunOverride = false)
    {
        if (IsCritical(luck) && (bleed || bleedOverride))   target.currentStatus.Add(Status.Bleed);
        if (IsCritical(luck) && (poison || poisonOverride)) target.currentStatus.Add(Status.Poison);
        if (IsCritical(luck) && (stun || stunOverride))     target.currentStatus.Add(Status.Stun);
        if (healPoison) target.HealPoison();
    }
}
