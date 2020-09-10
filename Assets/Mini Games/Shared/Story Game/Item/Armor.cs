using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Item/Armor")]
[Serializable]
public class Armor : Item
{
    public ArmorType type;
    private int resistance;
    public float initiativeModifier;
    public float healthModifier;
    public float staminaModifier;
    public float staminaRegenModifier;
    public float manaModifier;
    public float manaRegenModifier;

    public float GetDamageResistance(int strength, int dexterity, int intelligence, int faith, int luck)
    {
        int scalingFactor = 0;
        scalingFactor += strength *     (int) strengthScaling;
        scalingFactor += dexterity *    (int) dexterityScaling;
        scalingFactor += intelligence * (int) intelligenceScaling;
        scalingFactor += faith *        (int) faithScaling;
        scalingFactor += luck *         (int) luckScaling;
        return (float) resistance * scalingFactor;
    }

    public bool AttackEvaded(int luck)
    {
        return IsCritical(luck);
    }
}

public enum ArmorType
{
    Head,
    Arms,
    Body,
    Legs,
    Ring
}
