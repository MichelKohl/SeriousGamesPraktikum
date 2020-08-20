using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Attack/Player Spell")]
[Serializable]
public class PlayerSpell : Move
{
    public SpellType type;
    public SpellProjectile projectile;
    public ParticleSpawnPosition projectileSpawnPosition;
    public float delay;
}


