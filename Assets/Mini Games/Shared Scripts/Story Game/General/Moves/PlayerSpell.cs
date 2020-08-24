using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Player/Spell")]
[Serializable]
public class PlayerSpell : PlayerAttack
{
    public SpellType type;
    public SpellProjectile projectile;
    public float delay;
}


