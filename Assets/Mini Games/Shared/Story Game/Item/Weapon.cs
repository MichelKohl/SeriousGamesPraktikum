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

    public float GetDamage(int strength, int dexterity, int intelligence, int faith, int luck, int accuracy, float attackModifier)
    {
        int scalingFactor = 0;
        scalingFactor += strength *     (int) strengthScaling;
        scalingFactor += dexterity *    (int) dexterityScaling;
        scalingFactor += intelligence * (int) intelligenceScaling;
        scalingFactor += faith *        (int) faithScaling;
        scalingFactor += luck *         (int) luckScaling;
        bool isCritical = IsCritical(luck);
        return IsAccurate(accuracy) && !isCritical ? damage * attackModifier * scalingFactor * (isCritical ? criticalModifier : 1f) : 0;
    }
}
