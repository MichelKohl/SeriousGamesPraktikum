using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Player/Spell")]
[Serializable]
public class PlayerSpell : PlayerMove
{
    public SpellType type;
    public SpellProjectile projectile;
    public float delay;
}


