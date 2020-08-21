﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Enemy/Spell")]
[Serializable]
public class EnemySpell : EnemyAttack
{
    public SpellType type;
    public SpellProjectile projectile;
    public float delay;
}
