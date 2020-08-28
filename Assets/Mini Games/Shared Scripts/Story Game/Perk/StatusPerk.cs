using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Perk/Status")]
[Serializable]
public class StatusPerk : Perk
{
    public Status status = Status.None;
    public float resistance = 0f;
    public float damageMultiplier = 1f;
}
