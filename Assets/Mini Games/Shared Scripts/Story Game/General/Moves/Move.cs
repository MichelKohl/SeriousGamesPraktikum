using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : ScriptableObject

{
    public string animationName;

    public float healthCost = 0f;
    public float staminaCost = 0f;
    public float manaCost = 0f;

    public float healthBenfit = 0f;
    public float staminaBenefit = 0f;
    public float manaBenefit = 0f;

    public ParticleSystem[] particleEffects;
    public float[] particleDelay;
    public ParticleSpawnPosition[] particleSpawnPositions;
}

public enum Scaling : int
{
    S = 13,
    A = 8,
    B = 5,
    C = 3,
    D = 2,
    E = 1,
    F = 0
}

public enum Status
{
    Poison,
    Stun,
    Bleed,
    HealPoison,
    None
}

public enum ParticleSpawnPosition
{
    None,
    LeftHand,
    RightHand,
    LeftFoot,
    RightFoot,
    Chest,
    Weapon
}

public enum SpellType
{
    Single,
    Multiple,
    Areal,
    SelfBuff,
    Heal
}
