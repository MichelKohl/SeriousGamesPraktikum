using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Player/Self Buff")]
[Serializable]
public class PlayerSelfBuff : PlayerMove
{
    public Perk buff;
}


