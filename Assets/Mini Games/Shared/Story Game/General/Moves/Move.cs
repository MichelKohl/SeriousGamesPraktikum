using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : ScriptableObject

{
    public string animationName;

    public float healthCost = 0f;
    public float staminaCost = 0f;
    public float manaCost = 0f;

    public ParticleSystem[] particleEffects;
    public float[] particleDelay;
    public float[] particleDuration;
    public int[] particleSpawnPositionID;
    public bool[] releaseOnInit;


    
}

public enum Scaling : int
{
    S = 13,
    A = 8,
    B = 5,
    C = 3,
    D = 2,
    E = 1
}

public enum Status
{
    Poison,
    Stun,
    Bleed,
    Burn,
    HealPoison,
    None
}

public enum SpellType
{
    Single,
    Multiple,
    Areal
}
