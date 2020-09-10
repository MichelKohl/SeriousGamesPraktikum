using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Melee")]
[Serializable]
public class EnemyMelee : EnemyAttack
{
    public float hitboxDelay = 0f;
    public int hitboxID = 0;
}
