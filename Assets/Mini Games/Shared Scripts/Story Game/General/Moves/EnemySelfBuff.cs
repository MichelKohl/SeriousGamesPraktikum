using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Enemy/Self Buff")]
[Serializable]
public class EnemySelfBuff : EnemyMove
{
    public Consumable consumable;
}
