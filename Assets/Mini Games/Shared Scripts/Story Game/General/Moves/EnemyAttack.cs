using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Enemy/Attack")]
[Serializable]
public class EnemyAttack : EnemyMove
{
    public float damage = 0f;
    public float hitboxDelay = 0f;
    public int hitboxID = 0;
    public bool multipleHits = false;
}
