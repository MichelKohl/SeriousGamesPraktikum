using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Attack/Enemy Attack")]
[Serializable]
public class EnemyAttack : Move
{
    public float levelMultiplier = 1f;// changes damage depending on level
    public float hitboxDelay = 0f;
}
