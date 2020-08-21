using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Player/Attack")]
[Serializable]
public class PlayerAttack : PlayerMove
{
    public float damage = 0f;
    public int levelRequirement = 1;
    public int hitboxID = 0;
    public bool multipleHits = false;
}
